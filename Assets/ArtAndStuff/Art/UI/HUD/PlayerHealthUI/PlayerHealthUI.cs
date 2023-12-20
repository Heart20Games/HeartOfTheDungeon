using UnityEngine;
using TMPro;
using Body;
using UnityEngine.Assertions;

public class PlayerHealthUI : BaseMonoBehaviour
{
    public float startingHealth = 20f;
    public float previousHealth = 20f;
    [SerializeField] private GameObject healthFill;
    [SerializeField] private GameObject healthPipTextParent;
    [SerializeField] private TextMeshPro healthNumber;
    [SerializeField] private Animator healthAnimator;
    [SerializeField] private TextMeshPro healthTextPip;
    [SerializeField] private Transform heathPipTextTransform;
    private Animator healthPipTextAnimator;
    private Modified<int> maxHealth;
    private Modified<int> currentHealth;
    private Character character;
    private bool maxHealthConnected = true;
    private bool currentHealthConnected = true;
    private bool initialized = false;
    private bool waitingForInitialization = false;

    // Start is called before the first frame update
    void Start()
    {
        healthFill = GameObject.Find("PlayerHealthFill");
        healthNumber = this.transform.GetComponentInChildren<TMPro.TextMeshPro>();
        healthAnimator = GetComponent<Animator>();
        initialized = true;

        SetUpHealhPipText();
    }

    private void Update()
    {
        if (!maxHealthConnected && character != null)
            ConnectMaxHealth(character.maxHealth);
        if (!currentHealthConnected && character != null)
            ConnectCurrentHealth(character.currentHealth);
        if (initialized && waitingForInitialization)
        {
            waitingForInitialization = false;
            UpdateHealth();
        }
    }

    private void SetUpHealhPipText()
    {
        healthTextPip.gameObject.SetActive(false);

        healthPipTextAnimator = healthTextPip.GetComponent<Animator>();
    }

    private void ShowHealthPipText(bool isDamage, float value, Color color)
    {
        if (healthTextPip == null) return;

        healthTextPip.gameObject.SetActive(true);

        healthPipTextAnimator.Play("FadeIn", -1, 0);

        if (isDamage)
        {
            healthTextPip.text = "-" + value;
        }
        else
        {
            healthTextPip.text = "+" + value;
        }

        healthTextPip.color = color;
    }

    // Connections
    public void ConnectCharacter(Character character)
    {
        if (character !=  null)
        {
            this.character = character;
            ConnectMaxHealth(character.maxHealth, true);
            ConnectCurrentHealth(character.currentHealth, true);
        }
    }

    public void InitializeHealth()
    {
        if (initialized)
            UpdateHealth();
        else
            waitingForInitialization = true;
    }

    public void ConnectMaxHealth(Modified<int> health, bool ensureConnection=false)
    {
        maxHealthConnected = !ensureConnection;
        if (health != null)
        {
            health.Subscribe(ModifyMaxHealth);
            InitializeHealth();
            maxHealthConnected = true;
        }
    }

    public void ConnectCurrentHealth(Modified<int> health, bool ensureConnection=false)
    {
        currentHealthConnected = !ensureConnection;
        if (health != null)
        {
            health.Subscribe(ModifyCurrentHealth);
            InitializeHealth();
            currentHealthConnected = true;
        }
    }

    // Modifiers
    public void ModifyMaxHealth(int finalHealth)
    {
        UpdateHealth(currentHealth != null ? currentHealth.Value : previousHealth, finalHealth);
    }

    public void ModifyCurrentHealth(int finalHealth)
    {
        UpdateHealth(finalHealth);
    }
  
    // Update Health
    public void UpdateHealth()
    {
        float current = currentHealth != null ? currentHealth.Value : previousHealth;
        UpdateHealth(current);
    }

    public void UpdateHealth(float currentHealth)
    {
        float total = maxHealth != null ? maxHealth.Value : startingHealth;
        UpdateHealth(currentHealth, total);
    }

    public void UpdateHealth(float currentHealth, float totalHealth)
    {
        Assert.IsNotNull(healthAnimator);
        Assert.IsNotNull(healthFill);
        Assert.IsNotNull(healthNumber);

        float healthDifference = Mathf.Abs(currentHealth - previousHealth);

        if(currentHealth < previousHealth)
        {
            healthAnimator.SetTrigger("Health Down");
            if(currentHealth != totalHealth)
               ShowHealthPipText(true, healthDifference, Color.red);
        }
        else if(currentHealth > previousHealth)
        {
            healthAnimator.SetTrigger("Health Up");
            ShowHealthPipText(false, healthDifference, Color.green);
        }
        else if(totalHealth > startingHealth)
        {
            healthAnimator.SetTrigger("Health Max Up");
        }
        else
        {
            return;
        }

        previousHealth = currentHealth;
        startingHealth = totalHealth;

        float fillPosition = (currentHealth / startingHealth);
        healthFill.transform.localPosition = new Vector3 (0, Mathf.Lerp(-220f, 0f, fillPosition), 0);
        healthNumber.text = currentHealth.ToString();
    }
}

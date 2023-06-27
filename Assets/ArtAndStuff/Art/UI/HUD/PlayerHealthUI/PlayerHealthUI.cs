using UnityEngine;
using TMPro;
using Body;
using UnityEngine.Assertions;

public class PlayerHealthUI : BaseMonoBehaviour
{
    public float startingHealth = 20f;
    public float previousHealth = 20f;
    [SerializeField] private GameObject healthFill;
    [SerializeField] private TextMeshPro healthNumber;
    [SerializeField] private Animator healthAnimator;
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

    public void ConnectMaxHealth(Modified<int> health, bool ensureConnection=false)
    {
        maxHealthConnected = !ensureConnection;
        if (health != null)
        {
            maxHealth?.UnSubscribe(ModifyMaxHealth);
            maxHealth = health;
            maxHealth.Subscribe(ModifyMaxHealth);
            maxHealthConnected = true;
            if (initialized)
                UpdateHealth();
            else
                waitingForInitialization = true;
        }
    }

    public void ConnectCurrentHealth(Modified<int> health, bool ensureConnection=false)
    {
        currentHealthConnected = !ensureConnection;
        if (health != null)
        {
            currentHealth?.UnSubscribe(ModifyCurrentHealth);
            currentHealth = health;
            currentHealth.Subscribe(ModifyCurrentHealth);
            currentHealthConnected = true;
            if (initialized)
                UpdateHealth();
            else
                waitingForInitialization = true;
        }
    }

    // Modifiers
    public int ModifyMaxHealth(int oldHealth, int newHealth)
    {
        UpdateHealth(currentHealth.Value, newHealth);
        return newHealth;
    }

    public int ModifyCurrentHealth(int oldHealth, int newHealth)
    {
        UpdateHealth(newHealth, maxHealth.Value);
        return newHealth;
    }
  
    // Update Health
    public void UpdateHealth()
    {
        float total = maxHealth != null ? maxHealth.Value : startingHealth;
        float current = currentHealth != null ? currentHealth.Value : previousHealth;
        UpdateHealth(current, total);
    }

    public void UpdateHealth(float currentHealth, float totalHealth)
    {
        Assert.IsNotNull(healthAnimator);
        Assert.IsNotNull(healthFill);
        Assert.IsNotNull(healthNumber);

        if(currentHealth < previousHealth)
        {
            healthAnimator.SetTrigger("Health Down");
        }
        else if(currentHealth > previousHealth)
        {
            healthAnimator.SetTrigger("Health Up");
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

using UnityEngine;
using TMPro;
using Body;
using UnityEngine.Assertions;
using Modifiers;
using UnityEngine.Events;
using MyBox;

public class PlayerHealthUI : BaseMonoBehaviour
{
    public float startingHealth = 20f;
    public float previousHealth = 20f;
    [SerializeField] private GameObject healthFill;
    [SerializeField] private TextMeshPro healthNumber;
    [SerializeField] private Animator healthAnimator;
    private Character character;
    private ModField<int> Health => character != null ? character.Health : null;
    private bool healthConnected = false;
    private bool initialized = false;
    private bool waitingForInitialization = false;

    public UnityEvent<float> onHealthChanged;

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
        if (!healthConnected && Health != null) ConnectHealth();
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
        }
    }

    public void ConnectHealth()
    {
        if (Health != null)
        {
            Health.Subscribe(ModifyCurrentHealth, ModifyMaxHealth);
            healthConnected = true;
            InitializeHealth();
        }
    }

    public void InitializeHealth()
    {
        if (initialized)
            UpdateHealth();
        else
            waitingForInitialization = true;
    }

    // Modifiers
    public void ModifyMaxHealth(int finalHealth)
    {
        UpdateHealth(Health != null ? Health.current.Value : previousHealth, finalHealth);
    }

    public void ModifyCurrentHealth(int finalHealth)
    {
        UpdateHealth(finalHealth);
    }
  
    // Update Health
    public void UpdateHealth()
    {
        float current = Health != null ? Health.current.Value : previousHealth;
        UpdateHealth(current);
    }

    public void UpdateHealth(float currentHealth)
    {
        float total = Health != null ? Health.max.Value : startingHealth;
        UpdateHealth(currentHealth, total);
    }

    public void UpdateHealth(float currentHealth, float totalHealth)
    {
        Assert.IsNotNull(healthAnimator);
        Assert.IsNotNull(healthFill);
        Assert.IsNotNull(healthNumber);

        float healthDifference = Mathf.Abs(currentHealth - previousHealth);
        
        if (currentHealth != previousHealth)
            onHealthChanged.Invoke(currentHealth);

        if(currentHealth < previousHealth)
            healthAnimator.SetTrigger("Health Down");
        else if(currentHealth > previousHealth)
            healthAnimator.SetTrigger("Health Up");
        else if(totalHealth > startingHealth)
            healthAnimator.SetTrigger("Health Max Up");
        else return;

        previousHealth = currentHealth;
        startingHealth = totalHealth;

        float fillPosition = (currentHealth / startingHealth);
        healthFill.transform.localPosition = new Vector3 (0, Mathf.Lerp(-220f, 0f, fillPosition), 0);
        healthNumber.text = currentHealth.ToString();
    }

    // Testing

    [ButtonMethod]
    public void TestHealthDrop()
    {
        ModifyCurrentHealth(Health.current.Value - 1);
    }

    [ButtonMethod]
    public void TestHealthUp()
    {
        ModifyCurrentHealth(Health.current.Value + 1);
    }
}

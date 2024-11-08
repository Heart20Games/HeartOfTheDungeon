using Articy.Unity;
using UnityEngine;

public class GameConditionsManager : MonoBehaviour
{
    public static GameConditionsManager instance = null;

    public int playerHitByBeam = 0;
    public int slimeWizardChargingBeam = 0;
    public int slimeReinforcements = 0;
    public int slimeKilled = 0;
    public int slimeWizardShield = 0;
    public int lowHealth = 0;
    public int environmentalDamage = 0;

    private ArticyReference articyReference;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void CalloutCondition(int condition, ArticyRef articyRef, float lineNumber)
    {
        articyReference = GetComponent<ArticyReference>();
        //if (condition <= 7)
        //{
            articyReference.reference = articyRef;
            Debug.Log(articyRef, this);
            DialogueManager.Instance.StartDialogue(articyReference.GetObject<ArticyObject>(), lineNumber);
            condition++;
            Debug.Log(condition, this);
        //}  
    } 
}

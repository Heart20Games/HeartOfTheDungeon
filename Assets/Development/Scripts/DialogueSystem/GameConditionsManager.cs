using Articy.Unity;
using System.Collections.Generic;
using UnityEngine;

public class GameConditionsManager : MonoBehaviour
{
    public static GameConditionsManager instance = null;
    public Dictionary<string,int> conditions = new Dictionary<string,int>();
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
        articyReference = GetComponent<ArticyReference>();

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public static int GetGameCondition(string condition)
    {
        if (instance.conditions.TryGetValue(condition, out int result))
            return result;
        return 0;
    }

    public static void IncrementGameCondition(string condition)
    {
        if (instance.conditions.TryGetValue(condition, out int _))
        {
            instance.conditions[condition] += 1;
        }
        else
        {
            instance.conditions[condition] = 1;
        }
    }

    public static void CalloutCondition(string condition, ArticyRef articyRef, float lineNumber)
    {
        if (DialogueManager.Instance == null) return;

        instance.articyReference.reference = articyRef;
        Debug.Log(articyRef, instance);
        DialogueManager.Instance.StartDialogue(instance.articyReference.GetObject<ArticyObject>(), lineNumber);
        IncrementGameCondition(condition);
        Debug.Log(condition, instance);
    } 
}

using Articy.Unity;
using UnityEngine;

public class CalloutReferences : MonoBehaviour
{
    [SerializeField] public ArticyRef DBO1;
    [SerializeField] public ArticyRef DBO2;
    [SerializeField] public ArticyRef DBO3;
    [SerializeField] public ArticyRef DBR1;
    [SerializeField] public ArticyRef DBR2;
    [SerializeField] public ArticyRef DBR3;
    [SerializeField] public ArticyRef DBB1;
    [SerializeField] public ArticyRef DBB2;
    [SerializeField] public ArticyRef DBB3;
    [SerializeField] public ArticyRef CDBO1;
    [SerializeField] public ArticyRef CDBO2;
    [SerializeField] public ArticyRef CDBO3;
    [SerializeField] public ArticyRef CDBR1;
    [SerializeField] public ArticyRef CDBR2;
    [SerializeField] public ArticyRef CDBR3;
    [SerializeField] public ArticyRef CDBB1;
    [SerializeField] public ArticyRef CDBB2;
    [SerializeField] public ArticyRef CDBB3;

    public static CalloutReferences instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}

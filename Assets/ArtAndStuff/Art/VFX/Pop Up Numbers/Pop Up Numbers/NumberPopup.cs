using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberPopup : BaseMonoBehaviour
{
    public Transform serverParent;
    public Transform serverTarget;
    [Space]
    public DigitServer negativePrefab;
    public DigitServer neutralPrefab;
    public DigitServer positivePrefab;
    [ReadOnly] public int lastNumber = 0;
    [Space]
    [SerializeField] private int testNumber;

    private bool shouldShowEffectOnInitialization;

    private Coroutine shouldShowEffectRoutine;

    public enum SignRule { AsGiven, AsPositive, AsNegative, ZeroPositive, ZeroNegative }
    public SignRule signRule = SignRule.AsGiven;

    [Space]
    public bool debug = false;

    private void Awake()
    {
        if (serverParent == null)
        {
            serverParent = transform;
        }
    }

    public void PopupChange(int newNumber)
    {
        PopupNumber(newNumber - lastNumber);
        lastNumber = newNumber;
    }

    public void PopupNumber(int number)
    {
        if (isActiveAndEnabled)
        {
            if(shouldShowEffectOnInitialization)
            {
                Print($"Popping up {number}.", debug);

                DigitServer serverPrefab;
                switch (signRule)
                {
                    case SignRule.AsPositive:
                        serverPrefab = positivePrefab; break;
                    case SignRule.AsNegative:
                        serverPrefab = negativePrefab; break;
                    case SignRule.ZeroPositive:
                        serverPrefab = number < 0 ? negativePrefab : positivePrefab; break;
                    case SignRule.ZeroNegative:
                        serverPrefab = number > 0 ? positivePrefab : negativePrefab; break;
                    default: // AsGiven
                        serverPrefab = number == 0 ? neutralPrefab : number < 0 ? negativePrefab : positivePrefab; break;
                }

                DigitServer server = Instantiate(serverPrefab, serverParent, true);
                server.gameObject.SetActive(false);
                server.transform.position = serverTarget.position;

                server.ServeNumber(number);
                server.gameObject.SetActive(true);

                StartCoroutine(DestroyOldPopup(server));
            }
            else
            {
                if(shouldShowEffectRoutine == null)
                {
                    shouldShowEffectRoutine = StartCoroutine(SetShouldShowEffect());
                }
            }
        }
    }

    [ButtonMethod]
    public void TestPopup()
    {
        PopupNumber(testNumber);
    }

    public IEnumerator DestroyOldPopup(DigitServer server)
    {
        yield return new WaitForSeconds(5);
        Destroy(server.gameObject);
    }

    public IEnumerator SetShouldShowEffect()
    {
        yield return new WaitForSeconds(1);
        shouldShowEffectOnInitialization = true;

        shouldShowEffectRoutine = null;
    }
}

using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberPopup : MonoBehaviour
{
    public Transform serverParent;
    [Space]
    public DigitServer negativePrefab;
    public DigitServer neutralPrefab;
    public DigitServer positivePrefab;
    [ReadOnly] public int lastNumber = 0;
    [Space]
    [SerializeField] private int testNumber;

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
        DigitServer serverPrefab = number < 0 ? negativePrefab : number == 0 ? neutralPrefab : positivePrefab;

        DigitServer server = Instantiate(serverPrefab, serverParent, true);
        server.gameObject.SetActive(false);
        server.transform.position = transform.position;
        server.ServeNumber(number);
        server.gameObject.SetActive(true);
        StartCoroutine(DestroyOldPopup(server));
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
}

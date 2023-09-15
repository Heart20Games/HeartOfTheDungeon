using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pips : BaseMonoBehaviour
{
    [SerializeField] private List<GameObject> pips = new();
    private readonly List<Animator> pipAnimator = new();
    [SerializeField] private GameObject pipPrefab;
    [SerializeField] private Transform pipCanvas;
    [SerializeField] private string filledProperty = "IsFilled";

    private void FixedUpdate()
    {
        transform.TrueLookAt(Camera.main.transform.position);
    }

    public void SetFilled(int filled)
    {
        for (int i = 0; i < filled; i++)
        {
            pipAnimator[i].SetBool("IsFilled", true);
        }
        for (int i = filled; i < pips.Count; i++)
        {
            pipAnimator[i].SetBool("IsFilled", false);
        }
    }

    protected void ClearPips()
    {
        foreach (GameObject pip in pips)
        {
            Destroy(pip);
        }
        pips.Clear();
        pipAnimator.Clear();
    }

    private void AddPips(int number)
    {
        for (int i = 0; i < number; i++)
        {
            GameObject pip = Instantiate(pipPrefab, pipCanvas);
            pips.Add(pip);
            pipAnimator.Add(pip.GetComponent<Animator>());
        }
    }

    private void RemovePips(int number)
    {
        pips.RemoveRange(pips.Count - number, number);
        pipAnimator.RemoveRange(pipAnimator.Count - number, number);
    }

    public void SetPipCount(int total)
    {
        ClearPips();
        if (pips.Count > total)
            RemovePips(pips.Count - total);
        else if (pips.Count < total)
            AddPips(total - pips.Count);
    }


}

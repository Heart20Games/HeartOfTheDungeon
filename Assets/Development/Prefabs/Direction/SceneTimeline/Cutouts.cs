using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutouts : MonoBehaviour
{
    public GameObject[] cutouts;

    private void Start()
    {
        EnableCutouts(enabled);
    }

    private void OnEnable()
    {
        EnableCutouts(true);
    }

    private void OnDisable()
    {
        EnableCutouts(false);
    }

    public void EnableCutouts(bool enable)
    {
        foreach (GameObject cutout in cutouts)
        {
            cutout.SetActive(enable);
        }
    }
}

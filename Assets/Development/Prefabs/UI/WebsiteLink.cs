using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OpenSite : MonoBehaviour
{
    public string url;
    public void OpenWebsite()
    {
        Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSfSPbKlx8zzyA7LdvqVQynpwPnBz8hEQ1Y1UduKDbNiN-79mw/viewform?usp=sf_link");
    }
}
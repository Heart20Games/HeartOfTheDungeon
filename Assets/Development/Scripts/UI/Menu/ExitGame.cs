using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitGame : MonoBehaviour
{
  //Button myButton;

  //private void Start()
  //{
  //  //myButton = GetComponent<Button>();
  //  //myButton.onClick.AddListener(ExitFunction);
  //}

  public void ExitFunction()
  {
     Application.Quit();
  }
}
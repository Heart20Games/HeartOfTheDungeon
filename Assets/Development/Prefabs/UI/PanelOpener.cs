using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelOpener : MonoBehaviour
{
	public GameObject Panel;
	public EventSystem eventSystem;
	public GameObject previouslySelected;
	public GameObject firstSelected;
	public bool alwaysOpenToFirst = false;
	private GameObject returnTo;

	public void OpenPanel()
	{
		if (Panel !=null)
		{
			bool isActive = Panel.activeSelf;
			Panel.SetActive(!isActive);
			if (Panel !=null) 
			{
			if (!isActive)
			{ 
				// In Open Panel method, when opening:
previouslySelected = eventSystem.currentSelectedGameObject;
eventSystem.SetSelectedGameObject((alwaysOpenToFirst || returnTo == null) ? firstSelected : returnTo);

			} else 
			{
// In Open Panel method, when closing:
returnTo = eventSystem.currentSelectedGameObject;
eventSystem.SetSelectedGameObject(previouslySelected);
previouslySelected = null;
			}
			}
		}
	}
}




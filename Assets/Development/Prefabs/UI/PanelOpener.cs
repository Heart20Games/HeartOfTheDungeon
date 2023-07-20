using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;

public class PanelOpener : MonoBehaviour
{
	public GameObject Panel;
	public EventSystem eventSystem;
	public GameObject previouslySelected;
	public GameObject firstSelected;
	public InputActionReference cancelAction;
	public bool alwaysOpenToFirst = false;

    private GameObject returnTo;
	private bool active = false;

    public void Awake()
    {
        if (cancelAction.action != null)
			if (!cancelAction.action.enabled)
				cancelAction.action.Enable();
    }

    public void Update()
    {
        if (active && cancelAction.action != null && cancelAction.action.triggered)
		{
			OpenPanel();
		}
    }

    public void OpenPanel()
	{
		if (Panel !=null)
		{
			active = !Panel.activeSelf;
			Panel.SetActive(active);
			if (Panel !=null) 
			{
				if (active)
				{
					// In Open Panel method, when opening:
					previouslySelected = eventSystem.currentSelectedGameObject;
					eventSystem.SetSelectedGameObject((alwaysOpenToFirst || returnTo == null) ? firstSelected : returnTo);
			    }
                else 
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




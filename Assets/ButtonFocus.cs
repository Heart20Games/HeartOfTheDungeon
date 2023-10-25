using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonFocus : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject button;

    [SerializeField] private bool focusOnEnable = false;

    private void OnEnable()
    {
        if (focusOnEnable && eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(button);
        }
    }
}

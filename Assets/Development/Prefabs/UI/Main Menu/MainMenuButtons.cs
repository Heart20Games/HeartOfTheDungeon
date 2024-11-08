using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HotD.UI
{
    public class MainMenuButtons : BaseMonoBehaviour
    {
        [SerializeField] private string hoverKeyword = "Hover";
        [SerializeField][ReadOnly] private GameObject[] hoverIndicators;
        [SerializeField][ReadOnly] private Button[] buttons;
        GameObject lastSelectedGameObject = null;

        private void Awake()
        {
            buttons = GetComponentsInChildren<Button>();
            hoverIndicators = new GameObject[buttons.Length];
            for (int i = 0; i < buttons.Length; i++)
            {
                Transform parent = buttons[i].transform.parent;
                for (int j = 0; j < parent.childCount; j++)
                {
                    Transform child = parent.GetChild(j);
                    if (child.name.Contains(hoverKeyword))
                    {
                        hoverIndicators[i] = child.gameObject;
                        break;
                    }
                }
            }
        }

        private void Start()
        {
            EventSystem system = EventSystem.current;
            if (system == null) return;

            if (system.firstSelectedGameObject == null)
            {
                Button button = GetFirstActiveButton();
                system.SetSelectedGameObject(button.gameObject);
                ButtonSelected(button);
            }
            else
            {
                SelectHover(-1);
            }

            lastSelectedGameObject = system.firstSelectedGameObject;
        }

        private void Update()
        {
            EventSystem system = EventSystem.current;
            if (system == null) return;

            if (lastSelectedGameObject != system.currentSelectedGameObject)
            {
                if (system.currentSelectedGameObject != null)
                {
                    ButtonSelected(system.currentSelectedGameObject.GetComponent<Button>());
                }

                lastSelectedGameObject = system.currentSelectedGameObject;
            }
        }

        public Button GetFirstActiveButton()
        {
            foreach (Button button in buttons)
            {
                if (button != null && button.IsActive())
                    return button;
            }
            return null;
        }

        public void ButtonSelected(Button button)
        {
            if (button == null) return;

            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i] == button)
                {
                    SelectHover(i);
                    return;
                }
            }
        }

        public void SelectHover(int index)
        {
            for (int i = 0; i < hoverIndicators.Length; i++)
            {
                hoverIndicators[i].SetActive(index == i);
            }
        }
    }
}
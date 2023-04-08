using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static GameController;
using static Selectable;

public class GameControl : MonoBehaviour
{
    public GameMode mode = GameMode.Character;
    public List<SelectType> selectableTypes;
    [HideInInspector] public GameController controller;
    public UnityEvent onSelectorConfirmed;

    private void Awake()
    {
        controller = FindObjectOfType<GameController>();
        controller.selector.onConfirm.AddListener(OnSelectorConfirmed);
    }

    public void OnSelectorConfirmed()
    {
        onSelectorConfirmed.Invoke();
    }

    public void UseSelector()
    {
        SetMode(GameMode.Selection);
    }

    public void UseCharacter()
    {
        SetMode(GameMode.Character);
    }

    public void SetMode(GameMode mode)
    {
        controller.Mode = mode;
    }

    public void SetTimeScale(float timeScale)
    {
        controller.TimeScale = timeScale;
    }
}

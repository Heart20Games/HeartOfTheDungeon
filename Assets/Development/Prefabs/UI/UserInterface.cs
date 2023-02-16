using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UserInterface : MonoBehaviour
{
    public Character controlledCharacter;

    public void SetCharacter(Character character)
    {
        controlledCharacter = character;
    }

    //public Button testButton;
    //public void ClickButton(Button btn)
    //{
    //    Button button = btn;
    //    using (var e = new NavigationSubmitEvent() { target = button })
    //        button.SendEvent(e);
    //}
}

using Body;
using TMPro;
using UnityEngine;

public class TargetCharacterPanel : BaseMonoBehaviour
{
    [ReadOnly] [SerializeField] private Character character;
    public Character Character { get => character; set => SetCharacter(value); }
    public Portraits portraits;
    public SpriteRenderer portrait;
    public TMP_Text text;

    public void SetCharacter(Character character)
    {
        this.character = character;
        if (character != null)
        {
            // Find the portrait for the character.
            if (portraits.bank.TryGetValue(character.characterUIElements.characterName, out var emotions))
            {
                // Choose an emmotion, and set the image of the character in the UI.
                PortraitImage portrait = emotions["neutral"];
                this.portrait.sprite = portrait.image;
            }

            // Set the name of the character in the UI.
            text.text = character.characterUIElements.characterName;
        }
    }
}

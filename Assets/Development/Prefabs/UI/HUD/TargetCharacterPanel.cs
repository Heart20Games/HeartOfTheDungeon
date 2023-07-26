using Body;
using UnityEngine;

public class TargetCharacterPanel : BaseMonoBehaviour
{
    [ReadOnly] [SerializeField] private Character character;
    public Character Character { get => character; set => SetCharacter(value); }
    public Portraits portraits;

    public void SetCharacter(Character character)
    {
        this.character = character;
        PortraitImage portrait = portraits.bank[character.characterUIElements.characterName]["neutral"];
    }
}

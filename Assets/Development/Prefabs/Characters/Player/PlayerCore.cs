using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Yarn.Unity;



public class PlayerCore : MonoBehaviour
{
    private Character character;
    private Movement moveControls;
    private PlayerAttack Attacker;

    private void Start()
    {
        character = GetComponent<Character>();
        moveControls = GetComponent<Movement>();
        Attacker = GetComponent<PlayerAttack>();
        Attacker.Castable = character.weapon;
    }
}

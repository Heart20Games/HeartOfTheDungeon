using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EnemyType", order = 1)]
public class EnemyType : ScriptableObject
{
    public string enemyName = "";
    public int maxHealth = 10;

    public float moveSpeed = 10;
    public float moveTime = 6;
    public float moveCooldown = 3;

    public Weapon weapon;
    public float attackDistance = 4;
    public float attackCooldown = 3;

    public string hurtSoundUrl = "";
    public string fightSoundUrl = "";
    public string walkSoundUrl = "";
    public FMOD.Studio.EventInstance hurtSound;
    public FMOD.Studio.EventInstance fightSound;
    public FMOD.Studio.EventInstance walkSound;
    
    public void Initialize()
    {
        hurtSound = InitializeSound("Hurt", hurtSoundUrl);
        fightSound = InitializeSound("Fight", fightSoundUrl);
        walkSound = InitializeSound("Walk", walkSoundUrl);
    }

    public FMOD.Studio.EventInstance InitializeSound(string soundType, string url)
    {
        FMOD.Studio.EventInstance sound = FMODUnity.RuntimeManager.CreateInstance(url);
        if (!hurtSound.isValid()) { Debug.LogWarning(soundType + " Sound not Valid: " + enemyName); }
        return sound;
    }
}

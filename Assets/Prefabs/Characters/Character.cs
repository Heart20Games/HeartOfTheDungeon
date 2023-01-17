using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] public List<Ability> abilities = new List<Ability>();
    [SerializeField] public List<Weapon> weapons = new List<Weapon>();
    public Weapon weapon;

    public Transform pivot;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

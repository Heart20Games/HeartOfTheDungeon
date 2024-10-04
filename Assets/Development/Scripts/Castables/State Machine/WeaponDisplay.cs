using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotD.Castables
{
    public class TestWeaponDisplay : BaseMonoBehaviour, IWeaponDisplay
    {
        public void DisplayWeapon(Transform weaponArt)
        {
            if (weaponArt != null)
                weaponArt.transform.position = transform.position;
        }
    }

    public interface IWeaponDisplay
    {
        public void DisplayWeapon(Transform weaponArt);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryManager : CharacterInventoryManager
{
    public WeaponItem _currentRightHandWeapon;
    public WeaponItem _currentLefttHandWeapon;

    [Header("Quick SLots")]
    public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[3];
    public int _rightHandWeaponIndex = 0;
    public WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[3];
    public int _leftHandWeaponIndex = 0;
}

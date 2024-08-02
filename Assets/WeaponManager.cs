using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] MeleeWeaponDamageCollider _meleeWeaponDamageCollider;

    private void Awake()
    {
        _meleeWeaponDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
    }

    public void SetWeaponDamage(CharacterManager characterWeildingWeapon ,WeaponItem weapon)
    {
        _meleeWeaponDamageCollider._characterCausingDamage = characterWeildingWeapon;
        _meleeWeaponDamageCollider._physicalDamage = weapon._physicalDamage;
        _meleeWeaponDamageCollider._magicDamage = weapon._magicDamage;  
        _meleeWeaponDamageCollider._fireDamage = weapon._fireDamage;
        _meleeWeaponDamageCollider._holyDamage = weapon._holyDamage;
        _meleeWeaponDamageCollider._lighningDamage = weapon._lightningDamage;
    }
}

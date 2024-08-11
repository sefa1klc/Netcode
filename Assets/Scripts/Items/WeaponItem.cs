using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : Item
{
    [Header("Weapon Model")]
    public GameObject _weaponModel;

    [Header("Requirements")]
    public int StrenghtREQ = 0;
    public int dexREQ = 0;
    public int intREQ = 0;
    public int faithREQ = 0;

    [Header("Weapon Base Damage")]
    public int _physicalDamage = 0;
    public int _magicDamage = 0;
    public int _fireDamage = 0;
    public int _holyDamage = 0;
    public int _lightningDamage = 0;

    [Header("Weapon Poise")]
    public int _poiseDamage = 10;

    [Header("Attack Modifiers")]
    public float _lightAttack_01_Modifier = 1;

    [Header("Stamina Cost Modifiers")]
    public int _baseStaminaCost = 20;
    public float _lightAttackStaminaCountMultiplier = 1;

    [Header("Actions")]
    public WeaponItemActions _RB_Actions;


}


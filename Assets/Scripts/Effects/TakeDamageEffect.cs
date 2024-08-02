using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;


[CreateAssetMenu(menuName = "Character Effects / Instant Effects / Take Damage")]
public class TakeDamageEffect : InstantCharacterEffect
{

    [Header("Character Causing Damage")]
    public CharacterManager _characterCausingDamage;

    [Header("Damage")]
    public float _physicalDamage = 0;
    public float _magicDamage = 0;
    public float _fireDamage = 0;
    public float _lighningDamage = 0;
    public float _holyDamage = 0;

    [Header("Final Damage")]
    private int _finalDamageDeath = 0;

    [Header("Poise")]
    public float _poiseDamage;
    public bool _poiseIsBroken = false;

    [Header("Damage Animations")]
    public bool _playDamageAnimation = true;
    public bool _manuallySelectDamageAnimnation = false;
    public string _damageAnimation;

    [Header("Sound FX")]
    public bool _willPlayDamageSFX = true;
    public AudioClip _elementalDamageSoundFX;

    [Header("Direction Damage Taken From ")]
    public float _angleHitFrom;
    public Vector3 _contactPoint;


    public override void ProcessEffect(CharacterManager character)
    {
        base.ProcessEffect(character);

        //if character will be dead, no additional damage effect should be processed
        if (character._isDead.Value)
        {
            return;
        }

        CalculateDamage(character);

    }

    private void CalculateDamage(CharacterManager character)
    {
        if (!character.IsOwner) return;
        if(_characterCausingDamage != null)
        {

        }


        _finalDamageDeath = Mathf.RoundToInt(_physicalDamage + _magicDamage + _fireDamage + _lighningDamage + _holyDamage);

        if(_finalDamageDeath <= 0)
        {
            _finalDamageDeath = 1;
        }

        Debug.Log("Final Damage" + _finalDamageDeath);

        character._playerNetworkManager.currenthealth.Value -= _finalDamageDeath;
    }
}

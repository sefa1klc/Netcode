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
    public bool _poiseIsBroken = false; //for stunned

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
        if (character._isDead.Value) return;

        CalculateDamage(character);
        PlayDirectioanlBasedDamageAnimation(character);
        PlayDamageVFX(character);
        PlayDamageSFX(character);

        if (character._isDead.Value)
        {
            PlayDeathSFX(character);
        }

    }

    private void CalculateDamage(CharacterManager character)
    {
        if (!character.IsOwner) return;
        if(_characterCausingDamage != null)
        {

        }

        _finalDamageDeath = Mathf.RoundToInt(_physicalDamage + _magicDamage + _fireDamage + _lighningDamage + _holyDamage);

        if (_finalDamageDeath <= 0)
        {
            _finalDamageDeath = 1;
        }
     

        character._playerNetworkManager.currenthealth.Value -= _finalDamageDeath;

        Debug.Log("Final Damage" + _finalDamageDeath);
    }

    private void PlayDamageVFX(CharacterManager character)
    {
        character._characterAffectManager.PlayBloodSplatterVFX(_contactPoint);
    }

    private void PlayDamageSFX(CharacterManager character)
    {
        AudioClip _physicalDamageSFX = WorldSoundFxMAnager.Instance.ChooseRandomSFXFromArray(WorldSoundFxMAnager.Instance._physicalDamage);
        AudioClip _deathSFX = WorldSoundFxMAnager.Instance.ChooseRandomSFXFromArray(WorldSoundFxMAnager.Instance._deathSounds);
        character._characterSoundFXManager.PlaySoundFX(_physicalDamageSFX);
    }

    private void PlayDeathSFX(CharacterManager character)
    {
        AudioClip _deathSFX = WorldSoundFxMAnager.Instance.ChooseRandomSFXFromArray(WorldSoundFxMAnager.Instance._deathSounds);
        character._characterSoundFXManager.PlaySoundFX(_deathSFX);
    }

    private void PlayDirectioanlBasedDamageAnimation(CharacterManager character)
    {
        if (!character.IsOwner) return;

        _poiseIsBroken = true;
        if(_angleHitFrom >= 145 && _angleHitFrom <= 180)
        {
            _damageAnimation = character._characterAnimationsManager.GetRandomAnimationFromList(character._characterAnimationsManager._forwardDamage);
        }
        else if (_angleHitFrom <= -145 && _angleHitFrom >= -180)
        {
            _damageAnimation = character._characterAnimationsManager.GetRandomAnimationFromList(character._characterAnimationsManager._forwardDamage);
        }
        else if (_angleHitFrom >= -45 && _angleHitFrom <= 45)
        {
            _damageAnimation = character._characterAnimationsManager.GetRandomAnimationFromList(character._characterAnimationsManager._backwardDamage);
        }
        else if (_angleHitFrom >= -144 && _angleHitFrom <= -45)
        {
            _damageAnimation = character._characterAnimationsManager.GetRandomAnimationFromList(character._characterAnimationsManager._leftDamage);
        }
        else if(_angleHitFrom >= 45 && _angleHitFrom <= 144)
        {
            _damageAnimation = character._characterAnimationsManager.GetRandomAnimationFromList(character._characterAnimationsManager._rightDamage);
        }

        if(_poiseIsBroken)
        {
            character._characterAnimationsManager._lastDamageAnimationPlayed = _damageAnimation;
            character._characterAnimationsManager.PlayTargetActionAnimation(_damageAnimation,true);
        }
    }
}

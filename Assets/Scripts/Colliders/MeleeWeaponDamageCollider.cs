using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponDamageCollider : DamageCollider
{
    [Header("Attacking Character")]
    public CharacterManager _characterCausingDamage;

    [Header("Weapon Attack Modifiers")]
    public float _lightAttack_01_Modifier;
    public float _HeavyAttack_01_Modifier;
    public float _ChargeAttack_01_Modifier;

    protected override void Awake()
    {
        base.Awake();

        if(_damageCollider == null)
        {
            _damageCollider = GetComponent<Collider>();
        }
        _damageCollider.enabled = false;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        
        CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();
        Debug.Log(damageTarget);
        if (damageTarget != null)
        {
            if (damageTarget == _characterCausingDamage) return;

            _contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            
            DamageTarget(damageTarget);
        }
    }

    protected override void DamageTarget(CharacterManager damageTarget)
    {
        float _finalDamageGiven;
        // we dont want to damage the same target more than once
        if (_characterDamaged.Contains(damageTarget)) return;

        _characterDamaged.Add(damageTarget);

        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectManager.Instance.TakeDamageEffect);
        damageEffect._physicalDamage = _physicalDamage;
        damageEffect._magicDamage = _magicDamage;
        damageEffect._fireDamage = _fireDamage;
        damageEffect._holyDamage = _holyDamage;
        damageEffect._contactPoint = _contactPoint;
        damageEffect._angleHitFrom = Vector3.SignedAngle(_characterCausingDamage.transform.forward,damageTarget.transform.forward,Vector3.up);

        switch (_characterCausingDamage._characterCombatManager._currentAttackType)
        {
            case AttackType.LightAttack_01:
                ApplyAttackDamageModifiers(_lightAttack_01_Modifier, damageEffect);
                break;
            case AttackType.HeavyAttack_01:
                ApplyAttackDamageModifiers(_HeavyAttack_01_Modifier, damageEffect);
                break;
            case AttackType.ChargedAttack_01:
                ApplyAttackDamageModifiers(_ChargeAttack_01_Modifier, damageEffect);
                break;
            default:
                break;
        }

        _finalDamageGiven = Mathf.RoundToInt(damageEffect._physicalDamage + damageEffect._magicDamage + damageEffect._fireDamage + damageEffect._holyDamage);

        Debug.Log("Final Damage Given: " + _finalDamageGiven);

        if (_characterCausingDamage.IsOwner)
        {
            damageTarget._characterNetworkManager.NotifyTheServerofCharacterDamageServerRpc(
                damageTarget.NetworkObjectId,
                _characterCausingDamage.NetworkObjectId,
                damageEffect._physicalDamage,
                damageEffect._magicDamage,
                damageEffect._fireDamage,
                damageEffect._holyDamage,
                damageEffect._poiseDamage,
                damageEffect._angleHitFrom,
                damageEffect._contactPoint.x,
                damageEffect._contactPoint.y,
                damageEffect._contactPoint.z);
        }

        //damageTarget._characterAffectManager.ProcessInstantEffect(damageEffect);
    }

    private void ApplyAttackDamageModifiers(float modifier, TakeDamageEffect damageEffect)
    {
        damageEffect._physicalDamage += modifier;
        damageEffect._magicDamage += modifier;
        damageEffect._fireDamage += modifier;
        damageEffect._holyDamage += modifier;
        damageEffect._poiseDamage += modifier; 
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    [Header("Collider")]
    protected Collider _damageCollider;


    [Header("Damage")]
    public float _physicalDamage = 0;
    public float _magicDamage = 0;
    public float _fireDamage = 0;
    public float _lighningDamage = 0;
    public float _holyDamage = 0;

    [Header("Contact Points")]
    public Vector3 _contactPoint;


    [Header("Characters Damaged")]
    protected List<CharacterManager> _characterDamaged = new List<CharacterManager>();

    private void OnTriggerEnter(Collider other)
    {
        CharacterManager damageTarget = other.GetComponent<CharacterManager>();

        if (damageTarget != null)
        {
            _contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

            DamageTarget(damageTarget);
        }
    }

    protected virtual void DamageTarget(CharacterManager damageTarget)
    {
        // we dont want to damage the same target more than once
        if (_characterDamaged.Contains(damageTarget)) return;

        _characterDamaged.Add(damageTarget);

        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectManager.Instance.TakeDamageEffect);
        damageEffect._physicalDamage = _physicalDamage;
        damageEffect._magicDamage = _magicDamage;   
        damageEffect._fireDamage = _fireDamage; 
        damageEffect._holyDamage = _holyDamage;
        damageEffect._contactPoint = _contactPoint;

        damageTarget._characterAffectManager.ProcessInstantEffect(damageEffect);
        
    }

    public virtual void EnableDamageCollider()
    {
        _damageCollider.enabled = true;
    }

    public virtual void DisableDamageCollider()
    {
        _damageCollider.enabled = false;
        _characterDamaged.Clear();
    }
}

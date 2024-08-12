using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombatManager : MonoBehaviour
{
    protected CharacterManager character;

    [Header("Attack Target")]
    public CharacterManager _currentTarget;

    [Header("Attack Type")]
    public AttackType _currentAttackType;
    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }
}

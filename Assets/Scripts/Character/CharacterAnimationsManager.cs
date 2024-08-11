using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterAnimationsManager : MonoBehaviour
{
    CharacterManager _character;

    [Header("Damage Animation")]
    public string _hitForward_01 = "_hitForward_01";
    public string _hitBackward_01 = "_hitBackward_01";
    public string _hitLeft_01 = "_hitLeft_01";
    public string _hitRight_01 = "_hitRight_01";

    protected virtual void Awake()
    {
        _character = GetComponent<CharacterManager>();
    }

    public void UpdateAnimatorValues()
    {
         
    }

    public virtual void PlayTargetActionAnimation(string targetAnimation, bool isPerformingAction, bool applyRootAction = false)
    {
        _character._anim.applyRootMotion = applyRootAction;
        _character._anim.CrossFade(targetAnimation, 0.2f);

        _character._isPerformingAction = isPerformingAction;

        _character._characterNetworkManager.NotifyTheServerOfActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation,applyRootAction);

       
    }

    public virtual void PlayTargetAttackActionAnimation(
        AttackType attackType,
        string targetAnimation,
        bool isPerformingAction,
        bool applyRootAction = false)
    {
        _character._characterCombatManager._currentAttackType = attackType;
        _character._anim.applyRootMotion = applyRootAction;
        _character._anim.CrossFade(targetAnimation, 0.2f);
        _character._isPerformingAction = isPerformingAction;

        _character._characterNetworkManager.NotifyTheServerOfAttackActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootAction);
    }
}

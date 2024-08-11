using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterAnimationsManager : MonoBehaviour
{
    CharacterManager _character;

    [Header("Damage Animation")]
    public string _lastDamageAnimationPlayed;


    [SerializeField] string _hitForward_01 = "_hitForward_01";
    [SerializeField] string _hitForward_02 = "_hitForward_02";

    [SerializeField] string _hitBackward_01 = "_hitBackward_01";
    [SerializeField] string _hitBackward_02= "_hitBackward_02";

    [SerializeField] string _hitLeft_01 = "_hitLeft_01";
    [SerializeField] string _hitLeft_02 = "_hitLeft_02";

    [SerializeField] string _hitRight_01 = "_hitRight_01";
    [SerializeField] string _hitRight_02 = "_hitRight_02";

    public List<string> _forwardDamage = new List<string>();
    public List<string> _backwardDamage = new List<string>();
    public List<string> _rightDamage = new List<string>();
    public List<string> _leftDamage = new List<string>();

    protected virtual void Awake()
    {
        _character = GetComponent<CharacterManager>();
    }

    protected virtual void Start()
    {
        _forwardDamage.Add(_hitForward_01);
        _forwardDamage.Add(_hitForward_02);

        _backwardDamage.Add(_hitBackward_01);
        _backwardDamage.Add(_hitBackward_02);

        _leftDamage.Add(_hitLeft_01);
        _leftDamage.Add(_hitLeft_02);

        _rightDamage.Add(_hitRight_01);
        _rightDamage.Add(_hitRight_02);
    }

    public string GetRandomAnimationFromList(List<string> animaitonList)
    {
        List<string> finalList = new List<string>();

        foreach (var item in animaitonList)
        {
            finalList.Add(item);
        }

        finalList.Remove(_lastDamageAnimationPlayed);

        for (int i = finalList.Count - 1; i > -1 ; i--)
        {
            if (finalList[i] == null)
            {
                finalList.RemoveAt(i);
            }
        }

        int randomValue = Random.Range(0, finalList.Count);

        return finalList[randomValue];
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

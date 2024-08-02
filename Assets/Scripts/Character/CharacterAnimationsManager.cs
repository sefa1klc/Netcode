using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationsManager : MonoBehaviour
{
    CharacterManager _character;

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
    }
}

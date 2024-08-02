using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterManager : NetworkBehaviour
{
    [Header("Flags")]
    public bool _isPerformingAction = false;

    [Header("Status")]
    public NetworkVariable<bool> _isDead = new NetworkVariable<bool>(false,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);

    [HideInInspector] public Animator _anim;
    [HideInInspector] public CharacterNetworkManager _characterNetworkManager;
    [HideInInspector] public PlayerNetworkManager _playerNetworkManager;
    [HideInInspector] public CharacterAffectManager _characterAffectManager;
    [HideInInspector] public CharacterAnimationsManager _characterAnimationsManager;

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);
        _anim = GetComponent<Animator>();
        _characterNetworkManager = GetComponent<CharacterNetworkManager>();
        _characterAffectManager = GetComponent<CharacterAffectManager>();
        _characterAnimationsManager = GetComponent<CharacterAnimationsManager>();
    }


    protected virtual void Update()
    {

    }   

    public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
       if(IsOwner)
       {
            _characterNetworkManager.currenthealth.Value = 0;
            _isDead.Value = true;

            if (!manuallySelectDeathAnimation)
            {
              _characterAnimationsManager.PlayTargetActionAnimation("Death Backward",true);
            }
       }

        yield return new WaitForSeconds(5);
    }

    public virtual void ReviveCharacter()
    {
        _isDead.Value = true;
    }

}

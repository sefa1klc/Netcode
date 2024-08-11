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
    [HideInInspector] public CharacterCombatManager _characterCombatManager;
    [HideInInspector] public CharacterSoundFXManager _characterSoundFXManager;


    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);
        _anim = GetComponent<Animator>();
        _characterNetworkManager = GetComponent<CharacterNetworkManager>();
        _characterAffectManager = GetComponent<CharacterAffectManager>();
        _characterAnimationsManager = GetComponent<CharacterAnimationsManager>();
        _characterCombatManager = GetComponent<CharacterCombatManager>();
        _characterSoundFXManager = GetComponent<CharacterSoundFXManager>();
    }


    protected virtual void Update()
    {

    }   

    protected virtual void Start()
    {
        IgnoreMyOWnCollider();
    }

    public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
       if(IsOwner)
       {
            _playerNetworkManager.currenthealth.Value = 0;
            _isDead.Value = true;

            Debug.Log(_isDead.Value);

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

    public virtual void IgnoreMyOWnCollider()
    {
        Collider _characterControlCollider = GetComponent<Collider>();   
        Collider[] _damagableCharacterCollider = GetComponentsInChildren<Collider>();
        List<Collider> _ignoreCollider = new List<Collider>();

        foreach(var collider in _damagableCharacterCollider)
        {
            _ignoreCollider.Add(collider);
        }

        _ignoreCollider.Add(_characterControlCollider);

        foreach(var collider in _ignoreCollider)
        {
            foreach(var otherCollider in _ignoreCollider)
            {
                Physics.IgnoreCollision(collider, otherCollider,true);
            }
        }
    }

}

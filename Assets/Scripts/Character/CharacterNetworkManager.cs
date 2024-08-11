using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterNetworkManager : NetworkBehaviour
{
    CharacterManager character;

    [Header("Position")]
    public NetworkVariable<Vector3> _networkPositionDirection = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Resources")]
    public NetworkVariable<float> currentStamina = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> maxStamina = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> currenthealth = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> maxhealth = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Stats")]
    public NetworkVariable<int> vitality = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> endurance = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Bools")]
    public NetworkVariable<bool> isRunning = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isChargingAttack = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }
    public void CheckHP(int oldValue, int newValuw)
    {
        if (currenthealth.Value <= 0)
        {
            StartCoroutine(character.ProcessDeathEvent());
        }

        //prevents(önlemek) us from over healing
        if (character.IsOwner)
        {
            if(currenthealth.Value > maxhealth.Value)
            {
                currenthealth.Value = maxhealth.Value;
            }
        }
    }

    public void OnChargingAttackChanged(bool oldStatus, bool newStatus)
    {
        character._anim.SetBool("isChargingAttack", isChargingAttack.Value);
    }

    [ClientRpc]
    private void UpdateClientPositionAndRotationClientRpc(Vector3 position, Vector3 rotation)
    {
        if (!IsOwner)
        {
            transform.position = position;
            transform.rotation = Quaternion.Euler(rotation);
        }
    }

    [ServerRpc]
    public void NotifyTheServerOfActionAnimationServerRpc(ulong clientID, string targetAnimations, bool applyRootAction = false)
    {
        if (IsServer)
        {
            PlayActionAnimationForAllClientRpc(clientID, targetAnimations, applyRootAction);
        }
    }

    [ClientRpc]
    public void PlayActionAnimationForAllClientRpc(ulong clientID, string targetAnimations, bool applyRootAction = false)
    {
        if(clientID != NetworkManager.Singleton.LocalClientId)
        {
            PerformActionAnimaitonFromServer(targetAnimations, applyRootAction);
        }
    }

    private void PerformActionAnimaitonFromServer(string animatiomID, bool applyRootMotion)
    {
        character._anim.applyRootMotion = applyRootMotion;
        character._anim.CrossFade(animatiomID, 0.2f);
    }

    //Attack Animations
    [ServerRpc]
    public void NotifyTheServerOfAttackActionAnimationServerRpc(ulong clientID, string targetAnimations, bool applyRootAction = false)
    {
        if (IsServer)
        {
            PlayAttackActionAnimationForAllClientRpc(clientID, targetAnimations, applyRootAction);
        }
    }

    [ClientRpc]
    public void PlayAttackActionAnimationForAllClientRpc(ulong clientID, string targetAnimations, bool applyRootAction = false)
    {
        if (clientID != NetworkManager.Singleton.LocalClientId)
        {
            PerformAttackActionAnimaitonFromServer(targetAnimations, applyRootAction);
        }
    }

    private void PerformAttackActionAnimaitonFromServer(string animatiomID, bool applyRootMotion)
    {
        character._anim.applyRootMotion = applyRootMotion;
        character._anim.CrossFade(animatiomID, 0.2f);
    }

    //Damage
    [ServerRpc(RequireOwnership = false)]
    public void NotifyTheServerofCharacterDamageServerRpc(
        ulong damageCharacter,
        ulong characterCausingDamage,
        float physicalDamage,
        float magicDamage,
        float fireDamage,
        float holyDamage,
        float poiseDamage,
        float angleHitFrom,
        float contactpointX,
        float contactpointY,
        float contactpointZ)
    {
        if(IsServer)
        {
            NotifyTheServerofCharacterDamageClientRpc(damageCharacter, 
                characterCausingDamage, physicalDamage, magicDamage,
                fireDamage, holyDamage, poiseDamage, angleHitFrom, 
                contactpointX, contactpointY, contactpointZ);        
        }
    }

    [ClientRpc]
    public void NotifyTheServerofCharacterDamageClientRpc(
          ulong damageCharacterID,
          ulong characterCausingDamageID,
          float physicalDamage,
          float magicDamage,
          float fireDamage,
          float holyDamage,
          float poiseDamage,
          float angleHitFrom,
          float contactpointX,
          float contactpointY,
          float contactpointZ)
    {
        ProcessCharacterDamageFromServer(damageCharacterID, characterCausingDamageID, physicalDamage, magicDamage, fireDamage, holyDamage, poiseDamage, angleHitFrom, contactpointX, contactpointY, contactpointZ);
    }

    public void ProcessCharacterDamageFromServer(
          ulong damageCharacterID,
          ulong characterCausingDamageID,
          float physicalDamage,
          float magicDamage,
          float fireDamage,
          float holyDamage,
          float poiseDamage,
          float angleHitFrom,
          float contactpointX,
          float contactpointY,
          float contactpointZ)
    {
        CharacterManager damagedCharacter = NetworkManager.Singleton.SpawnManager.SpawnedObjects[damageCharacterID].gameObject.GetComponent<CharacterManager>();
        CharacterManager characterCausingDamage = NetworkManager.Singleton.SpawnManager.SpawnedObjects[characterCausingDamageID].gameObject.GetComponent<CharacterManager>();

        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectManager.Instance.TakeDamageEffect);

        damageEffect._physicalDamage = physicalDamage;
        damageEffect._magicDamage = magicDamage;
        damageEffect._fireDamage = fireDamage;
        damageEffect._holyDamage = holyDamage;
        damageEffect._poiseDamage = poiseDamage;
        damageEffect._angleHitFrom = angleHitFrom;
        damageEffect._contactPoint = new Vector3(contactpointX, contactpointY, contactpointZ);
        damageEffect._characterCausingDamage = characterCausingDamage;

        damagedCharacter._characterAffectManager.ProcessInstantEffect(damageEffect);
    }
}


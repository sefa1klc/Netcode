using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkManager : CharacterNetworkManager
{
    PlayerManager player;

    [Header("Equipments")]
    public NetworkVariable<int> _currentWeaponBeingUse = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> _currentRightHandWeaponID = new NetworkVariable<int>(0,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> _currentLeftHandWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> _isUsingRightHand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> _isUsingLeftHand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }

    public void SetCharacterActionHand(bool rightHandActions)
    {
        if (rightHandActions)
        {
            _isUsingLeftHand.Value = false;
            _isUsingRightHand.Value = true;
        }
        else
        {
            _isUsingRightHand.Value = false;
            _isUsingLeftHand.Value = true;
        }
    }
    public void SetNewMaxHealtValue(int oldVitality, int newVitality)
    {
        maxhealth.Value = player._playerStatManager.CalculateHealthBasedOnVitalityLevel(newVitality);
        currenthealth.Value = maxhealth.Value;  
    }

    public void SetNewMaxenduranceValue(int oldStamina, int newStamina)
    {
        maxStamina.Value = player._playerStatManager.CalculateStaminaBasedOnenduranceLevel(newStamina);
        currentStamina.Value = maxStamina.Value;
    }

    public void OnCurrentRightHandWeaponIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon =Instantiate(WorldItemDatabase.Instance.GetWeaponByID(newID)); 
        player._playerInventoryManager._currentRightHandWeapon = newWeapon;
        player._playerEquipmentManager.LoadRightWeapon();

        if (player.IsOwner)
        {
            PlayerUIHudManager.Instance.SetRightWeaponQuickSlotIcon(newID);
        }
    }

    public void OnCurrentLefttHandWeaponIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon =Instantiate(WorldItemDatabase.Instance.GetWeaponByID(newID));
        player._playerInventoryManager._currentLefttHandWeapon = newWeapon;
        player._playerEquipmentManager.LoadLeftWeapon();

        if (player.IsOwner)
        {
            PlayerUIHudManager.Instance.SetLeftWeaponQuickSlotIcon(newID);
        }
    }

    public void OnCurrentWeaponBeingUsedIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(newID));
        player._playerCombatManager._currentWeaponBeingUsed = newWeapon;
    }



    [ServerRpc]
    public void NotifyTheServerOfWeaponActionServerRpc(ulong ClientID, int ActionID, int WeaponId)
    {
        if (IsServer)
        {
            NotifyTheServerOfWeaponActionClientRpc( ClientID,  ActionID, WeaponId);
        }
    }

    [ClientRpc]
    private void NotifyTheServerOfWeaponActionClientRpc(ulong ClientID, int ActionID, int WeaponId)
    {
        if(ClientID != NetworkManager.LocalClientId)
        {
            PerformWeaponBasedAction(ActionID, WeaponId);
        }
    }

    private void PerformWeaponBasedAction( int ActionID, int WeaponId)
    {
        WeaponItemActions weaponItemActions = WorldActionsManager.Instance.GetWeaponItemActionByID(ActionID);

        if(weaponItemActions != null)
        {
            weaponItemActions.AttempToPerformAction(player,WorldItemDatabase.Instance.GetWeaponByID(WeaponId)); 
        }
        else
        {
            Debug.Log("Action is NULL");
        }
    }
}

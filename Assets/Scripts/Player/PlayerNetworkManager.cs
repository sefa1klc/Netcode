using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkManager : CharacterNetworkManager
{
    PlayerManager player;

    [Header("Equipments")]
    public NetworkVariable<int> _currentRightHandWeaponID = new NetworkVariable<int>(0,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> _currentLeftHandWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
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
    }

    public void OnCurrentLefttHandWeaponIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon =Instantiate(WorldItemDatabase.Instance.GetWeaponByID(newID));
        player._playerInventoryManager._currentLefttHandWeapon = newWeapon;
        player._playerEquipmentManager.LoadLeftWeapon();
    }
}

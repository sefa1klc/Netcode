using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerCombatManager : CharacterCombatManager
{
    PlayerManager player;

    public WeaponItem _currentWeaponBeingUsed;


    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }
    public void PerformWeaonBasedAction(WeaponItemActions weaponItemActions, WeaponItem weaponPerformAction)
    {

        if (player.IsOwner)
        {
            weaponItemActions.AttempToPerformAction(player, weaponPerformAction);
            player._playerNetworkManager.NotifyTheServerOfWeaponActionServerRpc(NetworkManager.Singleton.LocalClientId,
                weaponItemActions._actionID, weaponPerformAction._itemID);
        }
        
    }

    public virtual void DrainStaminaBasedAttack()
    {
        if (!player.IsOwner) return;

        if (_currentWeaponBeingUsed == null) return;

        float staminaDeducted = 0;

        switch (_currentAttackType)
        {
            case AttackType.LightAttack_01:
                staminaDeducted = _currentWeaponBeingUsed._baseStaminaCost * _currentWeaponBeingUsed._lightAttackStaminaCountMultiplier;
                break;
            default:
                break;
        }

        player._playerNetworkManager.currentStamina.Value -= Mathf.RoundToInt(staminaDeducted);
        
    }

}

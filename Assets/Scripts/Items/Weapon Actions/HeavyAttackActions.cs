using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character ACtions / Weapon Action / Heavy Attack Actions")]
public class HeavyAttackActions : WeaponItemActions
{
    [SerializeField] string Heavy_attack_01 = "Main_Heavy_Attack_01";

    public override void AttempToPerformAction(PlayerManager playerPerformaction, WeaponItem weaponPerformingAction)
    {
        base.AttempToPerformAction(playerPerformaction, weaponPerformingAction);


        if (!playerPerformaction.IsOwner)
        {
            return;
        }


        if (playerPerformaction._playerNetworkManager.currentStamina.Value <= 0) return;

        PerformHeavyAttack(playerPerformaction, weaponPerformingAction);
    }

    private void PerformHeavyAttack(PlayerManager playerPerformingAciton, WeaponItem weaponPerformingAcito)
    {
       
        if (playerPerformingAciton._playerNetworkManager._isUsingRightHand.Value)
        {
            playerPerformingAciton.CharacterAnimationsManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack_01, Heavy_attack_01, true);
        }

        if (playerPerformingAciton._playerNetworkManager._isUsingLeftHand.Value)
        {

        }
    }
}

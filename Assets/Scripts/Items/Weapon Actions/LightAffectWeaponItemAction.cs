using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Character ACtions / Weapon Action / Light Attack Actions")]
public class LightAffectWeaponItemAction : WeaponItemActions
{

    [SerializeField] string Light_attack_01 = "Main_Light_Attack_01";

    public override void AttempToPerformAction(PlayerManager playerPerformaction, WeaponItem weaponPerformingAction)
    {
        base.AttempToPerformAction(playerPerformaction, weaponPerformingAction);


        if (!playerPerformaction.IsOwner)
        {
            return;
        }


        if (playerPerformaction._playerNetworkManager.currentStamina.Value <= 0) return;

        PerformLighttack(playerPerformaction, weaponPerformingAction);
    }

    private void PerformLighttack(PlayerManager playerPerformingAciton, WeaponItem weaponPerformingAcito)
    {

        if (playerPerformingAciton._playerNetworkManager._isUsingRightHand.Value)
        {
            playerPerformingAciton.CharacterAnimationsManager.PlayTargetAttackActionAnimation(AttackType.LightAttack_01,Light_attack_01,true);
        }

        if (playerPerformingAciton._playerNetworkManager._isUsingLeftHand.Value)
        {

        }
    }
}

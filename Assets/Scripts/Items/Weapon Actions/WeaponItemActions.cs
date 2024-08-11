using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;


[CreateAssetMenu(menuName = "Character ACtions / Weapon Action / Test Actions")]
public class WeaponItemActions : ScriptableObject
{
    public int _actionID;

    public virtual void AttempToPerformAction(PlayerManager playerPerformaction, WeaponItem weaponPerformingAction)
    {
        if (playerPerformaction.IsOwner)
        {
            playerPerformaction._playerNetworkManager._currentWeaponBeingUse.Value = weaponPerformingAction._itemID;
        }

    }
}

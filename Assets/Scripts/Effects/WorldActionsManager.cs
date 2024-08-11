using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldActionsManager : Singleton<WorldActionsManager>
{
    [Header("Weapon Item Actions")]
    public WeaponItemActions[] _weaponItemActions;


    private void Start()
    {
        for(int i = 0; i< _weaponItemActions.Length; i++)
        {
            _weaponItemActions[i]._actionID = i;
        }
    }

    public WeaponItemActions GetWeaponItemActionByID(int id)
    {
        return _weaponItemActions.FirstOrDefault(action => action._actionID == id);
    }
}

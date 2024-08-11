using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModelInstantiationSlot : MonoBehaviour
{
    public WeaponModelSlot _weaponModelSlot;
    public GameObject _currentWeaponModel;

    public void UnLoadWeapon()
    {
        if(_currentWeaponModel != null)
        {
            Destroy(_currentWeaponModel);
        }
    }

    public void LoadWeapon(GameObject weaponModel)
    {
        _currentWeaponModel = weaponModel;
        weaponModel.transform.parent = transform;

        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localRotation = Quaternion.identity;
        weaponModel.transform.localScale = Vector3.one;
    }
}

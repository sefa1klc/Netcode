using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHudManager : Singleton<PlayerUIHudManager>
{
    [Header("Stat Bars")]
    [SerializeField] UI_StatBar _staminaBar;
    [SerializeField] UI_StatBar _HealthBar;

    [Header("Quick Slots")]
    [SerializeField] Image _rightWeaponQuickSlotIcon;
    [SerializeField] Image _leftWeaponQuickSlotIcon;
    [SerializeField] Image _spellWeaponQuickSlotIcon;
    [SerializeField] Image _itemWeaponQuickSlotIcon;



    public void RefreshHUD()
    {
        _staminaBar.gameObject.SetActive(false);
        _staminaBar.gameObject.SetActive(true);
        _HealthBar.gameObject.SetActive(false);
        _HealthBar.gameObject.SetActive(true);

    }

    public void SetMaxHealthValue(int MaxHealth)
    {
        _HealthBar.SetMaxStat(MaxHealth);
    }

    public void SetNewHealtValue(int oldValue, int newValue)
    {
        _HealthBar.SetStat(newValue);
    }

    public void SetNewStaminaValue(float oldValue, float newValue)
    {
        _staminaBar.SetStat(Mathf.RoundToInt(newValue));
    }

    public void SetMaxStaminaValue(int maxStamina)
    {
        _staminaBar.SetMaxStat(maxStamina);
    }

    public void SetRightWeaponQuickSlotIcon(int weaponID)
    {
        WeaponItem weapon = WorldItemDatabase.Instance.GetWeaponByID(weaponID);
        if (weapon == null)
        {
            Debug.Log("item is null");
            _rightWeaponQuickSlotIcon.enabled = false;
            _rightWeaponQuickSlotIcon.sprite = null;
            return;
        }

        if(weapon._itemIcon == null)
        {
            Debug.Log("item has no icon");
            _rightWeaponQuickSlotIcon.enabled = false;
            _rightWeaponQuickSlotIcon.sprite = null;
            return;
        }

        _rightWeaponQuickSlotIcon.sprite = weapon._itemIcon;
        _rightWeaponQuickSlotIcon.enabled = true;
        
    }
    public void SetLeftWeaponQuickSlotIcon(int weaponID)
    {
        WeaponItem weapon = WorldItemDatabase.Instance.GetWeaponByID(weaponID);
        if (weapon == null)
        {
            Debug.Log("item is null");
            _leftWeaponQuickSlotIcon.enabled = false;
            _leftWeaponQuickSlotIcon.sprite = null;
            return;
        }

        if (weapon._itemIcon == null)
        {
            Debug.Log("item has no icon");
            _leftWeaponQuickSlotIcon.enabled = false;
            _leftWeaponQuickSlotIcon.sprite = null;
            return;
        }

        _leftWeaponQuickSlotIcon.sprite = weapon._itemIcon;
        _leftWeaponQuickSlotIcon.enabled = true;

    }
}

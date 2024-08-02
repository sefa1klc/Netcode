using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class PlayerUIHudManager : MonoBehaviour
{
    [SerializeField] UI_StatBar _staminaBar;
    [SerializeField] UI_StatBar _HealthBar;

    
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
}

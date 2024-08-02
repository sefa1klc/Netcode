using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class CharacterStatManager : MonoBehaviour
{
    [Header("Stamina Regeneration")]
    private float _staminaRegenerationTimer = 0f;
    private float _staminaTickTimer = 0f;
    [SerializeField] float _regenerationDelay = 2f;
    [SerializeField] float _staminaRegenerationAmount = 3;

    [HideInInspector] public CharacterManager _characterManager;

   

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);
        _characterManager = GetComponent<CharacterManager>();
    }

    protected virtual void Start()
    {
    }

    public int CalculateHealthBasedOnVitalityLevel(int vitality)
    {
        float health = 0;

        health = vitality * 15;

        return Mathf.RoundToInt(health);
    }
    public int CalculateStaminaBasedOnenduranceLevel(int endurance)
    {
        float stamina = 0;

        stamina = endurance * 10;

        return Mathf.RoundToInt(stamina);
    }

    public virtual void RegenerateStamina()
    {
        if (!_characterManager.IsOwner) return;

        
        if (_characterManager._characterNetworkManager.isRunning.Value) return;

        _staminaRegenerationTimer += Time.deltaTime;

        if (_staminaRegenerationTimer >= _regenerationDelay)
        {
            
            if (_characterManager._characterNetworkManager.currentStamina.Value < _characterManager._characterNetworkManager.maxStamina.Value)
            {
                _staminaTickTimer += Time.deltaTime;
                if (_staminaTickTimer >= 0.1)
                {
                    _staminaTickTimer = 0;
                    _characterManager._characterNetworkManager.currentStamina.Value += _staminaRegenerationAmount;
                }
            }
        }
    }
    public virtual void ResetRegenerateStaminaTimer(float previousStaminaAmount, float currentStaminaAmount)
    {
        if (currentStaminaAmount < previousStaminaAmount)
        {
            _staminaRegenerationTimer = 0;
        }
    }
}

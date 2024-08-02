using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    [Header("Debug Menu")]
    [SerializeField] private bool _respawnCharacter = false;
    [SerializeField] private bool _switchRightWeapon = false;

    CharachterLocomotionManager _characterLocomotionManager;
    [HideInInspector] public PlayerStatManager _playerStatManager;
    [HideInInspector] public CharacterAnimationsManager CharacterAnimationsManager;
    [HideInInspector] public PlayerInventoryManager _playerInventoryManager;
    [HideInInspector] public PlayerEquipmentManager _playerEquipmentManager;

    protected override void Awake()
    {
        base.Awake();

        _characterLocomotionManager = GetComponent<CharachterLocomotionManager>();
        _characterNetworkManager = GetComponent<CharacterNetworkManager>();
        _playerStatManager = GetComponent<PlayerStatManager>();
        _playerNetworkManager = GetComponent<PlayerNetworkManager>();
        CharacterAnimationsManager = GetComponent<CharacterAnimationsManager>();
        _playerInventoryManager = GetComponent<PlayerInventoryManager>();
        _playerEquipmentManager = GetComponent<PlayerEquipmentManager>();   


    }

    protected override void Update()
    {
        base.Update();

        if (!IsOwner) return;

        _playerStatManager.RegenerateStamina();

        DebugMenu();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            // InputManager'ýn hazýr olup olmadýðýný kontrol edin
            if (InputManager.Instance == null)
            {
                Debug.LogError("InputManager instance is null.");
                return;
            }

            InputManager.Instance._playerManager = this;


            _playerNetworkManager.vitality.OnValueChanged += _playerNetworkManager.SetNewMaxHealtValue;
            _playerNetworkManager.endurance.OnValueChanged += _playerNetworkManager.SetNewMaxenduranceValue;

            _playerNetworkManager.currenthealth.OnValueChanged += PlayerUIManager.Instance.HudManager.SetNewHealtValue;
            _characterNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.Instance.HudManager.SetNewStaminaValue;
            _characterNetworkManager.currentStamina.OnValueChanged += _playerStatManager.ResetRegenerateStaminaTimer;


            _characterNetworkManager.maxhealth.Value = _playerStatManager.CalculateHealthBasedOnVitalityLevel(_characterNetworkManager.vitality.Value);
            _characterNetworkManager.maxStamina.Value = _playerStatManager.CalculateStaminaBasedOnenduranceLevel(_characterNetworkManager.endurance.Value);
            _characterNetworkManager.currenthealth.Value = _playerStatManager.CalculateHealthBasedOnVitalityLevel(_characterNetworkManager.vitality.Value);
            _characterNetworkManager.currentStamina.Value = _playerStatManager.CalculateStaminaBasedOnenduranceLevel(_characterNetworkManager.endurance.Value);
            PlayerUIManager.Instance.HudManager.SetMaxStaminaValue(_characterNetworkManager.maxStamina.Value);
        }

        //stats
        _playerNetworkManager.currenthealth.OnValueChanged += _playerNetworkManager.CheckHP;


        //equipments
        _playerNetworkManager._currentRightHandWeaponID.OnValueChanged += _playerNetworkManager.OnCurrentRightHandWeaponIDChange;
        _playerNetworkManager._currentLeftHandWeaponID.OnValueChanged += _playerNetworkManager.OnCurrentLefttHandWeaponIDChange;
    }

    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {


        if (IsOwner)
        {
            PlayerUIManager.Instance.PopUpManager.SenYouDiedPopUp();
        }

        return base.ProcessDeathEvent(manuallySelectDeathAnimation);
    }

    public override void ReviveCharacter()
    {
        base.ReviveCharacter();
        if (IsOwner)
        {
            _playerNetworkManager.currenthealth.Value = _playerNetworkManager.maxhealth.Value;
            _playerNetworkManager.currentStamina.Value = _playerNetworkManager.maxStamina.Value;

            CharacterAnimationsManager.PlayTargetActionAnimation("Empty", false);
        }
    }

    //DELETE LATER THÝS FOR DEBUG
    private void DebugMenu()
    {
        if (_respawnCharacter)
        {
            _respawnCharacter = false;
            ReviveCharacter();
        }


        if (_switchRightWeapon)
        {
            _switchRightWeapon = false;
            _playerEquipmentManager.SwitchRightWeapon();
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

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
     public PlayerCombatManager _playerCombatManager;

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
        _playerCombatManager = GetComponent<PlayerCombatManager>();


    }

    protected override void Update()
    {
        base.Update();

        if (!IsOwner) return;

        _playerStatManager.RegenerateStamina();

        DebugMenu();
    }

    private void OnClientConnectedCallback(ulong clientID)
    {
        WorldGameSesionManager.Instance.AddPlayerToActivePlayerList(this);

        if (!IsServer && IsOwner)
        {
            foreach (var player in WorldGameSesionManager.Instance.players)
            {
                if(player != this)
                {
                    player.LoadOtherPlayerCharacterWhenJoiningServer();
                }
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;

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

            Debug.Log(_playerNetworkManager.currenthealth.Value);
            _playerNetworkManager.currenthealth.OnValueChanged += PlayerUIManager.Instance.HudManager.SetNewHealtValue;
            _playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.Instance.HudManager.SetNewStaminaValue;
            _playerNetworkManager.currentStamina.OnValueChanged += _playerStatManager.ResetRegenerateStaminaTimer;


            _playerNetworkManager.maxhealth.Value = _playerStatManager.CalculateHealthBasedOnVitalityLevel(_playerNetworkManager.vitality.Value);
            _playerNetworkManager.maxStamina.Value = _playerStatManager.CalculateStaminaBasedOnenduranceLevel(_playerNetworkManager.endurance.Value);
            _playerNetworkManager.currenthealth.Value = _playerStatManager.CalculateHealthBasedOnVitalityLevel(_playerNetworkManager.vitality.Value);
            _playerNetworkManager.currentStamina.Value = _playerStatManager.CalculateStaminaBasedOnenduranceLevel(_playerNetworkManager.endurance.Value);
            PlayerUIManager.Instance.HudManager.SetMaxStaminaValue(_playerNetworkManager.maxStamina.Value);
        }

        //stats
        _playerNetworkManager.currenthealth.OnValueChanged += _playerNetworkManager.CheckHP;


        //equipments
        _playerNetworkManager._currentRightHandWeaponID.OnValueChanged += _playerNetworkManager.OnCurrentRightHandWeaponIDChange;
        _playerNetworkManager._currentLeftHandWeaponID.OnValueChanged += _playerNetworkManager.OnCurrentLefttHandWeaponIDChange;
        _playerNetworkManager._currentWeaponBeingUse.OnValueChanged += _playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;
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

    public void LoadOtherPlayerCharacterWhenJoiningServer()
    {
        _playerNetworkManager.OnCurrentRightHandWeaponIDChange(0,_playerNetworkManager._currentRightHandWeaponID.Value);
        _playerNetworkManager.OnCurrentLefttHandWeaponIDChange(0,_playerNetworkManager._currentLeftHandWeaponID.Value);

        
    }

    //DELETE LATER THIS FOR DEBUG
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

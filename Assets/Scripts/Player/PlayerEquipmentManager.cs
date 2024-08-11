using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentManager : CharacterEquipmentManager
{
    PlayerManager player;

    public WeaponModelInstantiationSlot _rightHandSlot;
    public WeaponModelInstantiationSlot _leftHandSlot;

    [SerializeField] WeaponManager _rightWeaponManager;
    [SerializeField] WeaponManager _leftWeaponManager;


    public GameObject _rightHandWeaponModel;
    public GameObject _leftHandWeaponModel;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();


        InitializeWeaponSLots();
    }

    protected override void Start()
    {
        base.Start();
        LoadWeponsOnBothHands();
    }

    private void InitializeWeaponSLots()
    {
        WeaponModelInstantiationSlot[] weaponSlots = GetComponentsInChildren<WeaponModelInstantiationSlot>();
        foreach (var weaponSlot in weaponSlots)
        {
            if(weaponSlot._weaponModelSlot == WeaponModelSlot.RightHand)
            {
                _rightHandSlot = weaponSlot;
            }
            else if (weaponSlot._weaponModelSlot == WeaponModelSlot.LeftHand)
            {
                _leftHandSlot = weaponSlot;
            }
        }
    }

    public void LoadWeponsOnBothHands()
    {
        LoadRightWeapon();
        LoadLeftWeapon();
    }

    public void SwitchRightWeapon()
    {
        if (!player.IsOwner) return;
        
        player.CharacterAnimationsManager.PlayTargetActionAnimation("Swap_Right_weapon_01", false);

        WeaponItem selectedWeapon = null;

        player._playerInventoryManager._rightHandWeaponIndex += 1;

        if(player._playerInventoryManager._rightHandWeaponIndex < 0 || player._playerInventoryManager._rightHandWeaponIndex > 2)
        {
            player._playerInventoryManager._rightHandWeaponIndex = 0;


            float weaponCount = 0;
            WeaponItem firstWeapon = null;
            int firstWeaponPosition = 0;

            for (int i = 0; i < player._playerInventoryManager.weaponsInRightHandSlots.Length; i++)
            {
                if (player._playerInventoryManager.weaponsInRightHandSlots[i]._itemID != WorldItemDatabase.Instance._unarmedWeapon._itemID)
                {
                    weaponCount += 1;

                    if (firstWeapon == null)
                    {
                        firstWeapon = player._playerInventoryManager.weaponsInRightHandSlots[i];
                        firstWeaponPosition = i;
                    }
                }
            }


            if (weaponCount < 1)
            {
                player._playerInventoryManager._rightHandWeaponIndex = -1;
                selectedWeapon = WorldItemDatabase.Instance._unarmedWeapon;
                player._playerNetworkManager._currentRightHandWeaponID.Value = selectedWeapon._itemID;

            }
            else
            {
                player._playerInventoryManager._rightHandWeaponIndex = firstWeaponPosition;
                player._playerNetworkManager._currentRightHandWeaponID.Value = firstWeapon._itemID;
            }

            return;
        }

        foreach(WeaponItem weapon in player._playerInventoryManager.weaponsInRightHandSlots)
        {
            if (player._playerInventoryManager.weaponsInRightHandSlots[player._playerInventoryManager._rightHandWeaponIndex]._itemID != WorldItemDatabase.Instance._unarmedWeapon._itemID)
            {
                selectedWeapon = player._playerInventoryManager.weaponsInRightHandSlots[player._playerInventoryManager._rightHandWeaponIndex];
                player._playerNetworkManager._currentRightHandWeaponID.Value = player._playerInventoryManager.weaponsInRightHandSlots[player._playerInventoryManager._rightHandWeaponIndex]._itemID;
                return;
            }
        }

        if(selectedWeapon == null && player._playerInventoryManager._rightHandWeaponIndex <= 2)
        {
            SwitchRightWeapon();
        }
        
    }
    public void LoadRightWeapon()
    {
        if(player._playerInventoryManager._currentRightHandWeapon != null)
        {
            _rightHandSlot.UnLoadWeapon();

            _rightHandWeaponModel = Instantiate(player._playerInventoryManager._currentRightHandWeapon._weaponModel);
            _rightHandSlot.LoadWeapon(_rightHandWeaponModel);
            _rightWeaponManager = _rightHandWeaponModel.GetComponent<WeaponManager>();
            _rightWeaponManager.SetWeaponDamage(player, player._playerInventoryManager._currentRightHandWeapon);
        }
    }

    public void SwitchLeftWeapon()
    {

    }
    public void LoadLeftWeapon()
    {
        if (player._playerInventoryManager._currentLefttHandWeapon != null)
        {
            _leftHandSlot.UnLoadWeapon();

            _leftHandWeaponModel = Instantiate(player._playerInventoryManager._currentLefttHandWeapon._weaponModel);
            _leftHandSlot.LoadWeapon(_leftHandWeaponModel);
            _leftWeaponManager = _leftHandWeaponModel.GetComponent<WeaponManager>();
            _leftWeaponManager.SetWeaponDamage(player, player._playerInventoryManager._currentLefttHandWeapon);
        }
    }

    //Damage Colliders

    public void OpenDamageCollider()
    {
        if (player._playerNetworkManager._isUsingRightHand.Value)
        {
            _rightWeaponManager._meleeWeaponDamageCollider.EnableDamageCollider();
        }
        else if(player._playerNetworkManager._isUsingLeftHand.Value)
        {
            _leftWeaponManager._meleeWeaponDamageCollider.EnableDamageCollider();
        }
    }


    public void CloseDamageCollider()
    {
        if (player._playerNetworkManager._isUsingRightHand.Value)
        {
            _rightWeaponManager._meleeWeaponDamageCollider.DisableDamageCollider();
        }
        else if (player._playerNetworkManager._isUsingLeftHand.Value)
        {
            _leftWeaponManager._meleeWeaponDamageCollider.DisableDamageCollider();
        }
    }
}

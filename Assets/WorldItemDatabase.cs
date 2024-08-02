using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldItemDatabase : Singleton<WorldItemDatabase>
{
    public WeaponItem _unarmedWeapon;

    [Header("Weapons")]
    [SerializeField] List<WeaponItem> weapons = new List<WeaponItem>();

    [Header("Items")]
    private List<Item> Items = new List<Item>(); //for every item in the game

    private void Awake()
    {
        foreach (var weapon in weapons)
        {
            Items.Add(weapon);
        }

        for (var i = 0; i < Items.Count; i++)
        {
            Items[i]._itemID = i;
        }
    }

    public WeaponItem GetWeaponByID(int ID)
    {
        return weapons.FirstOrDefault(weapons => weapons._itemID == ID);
    }
}

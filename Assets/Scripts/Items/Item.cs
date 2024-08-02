using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{
    [Header("Item Information")]
    public string _itemName;
    public Sprite _itemIcon;
    [TextArea] public string _itemDescription;
    public int _itemID;

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : Singleton<PlayerUIManager>
{
    [HideInInspector] public PlayerUIHudManager HudManager;
    [HideInInspector] public PlayerPopUpManager PopUpManager;

    private void Awake()
    {
        HudManager = GetComponentInChildren<PlayerUIHudManager>();    
        PopUpManager = GetComponentInChildren<PlayerPopUpManager>();
    }
}

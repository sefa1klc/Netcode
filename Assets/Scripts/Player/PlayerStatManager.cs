using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatManager : CharacterStatManager
{
    PlayerManager player;

    protected override void Awake()
    {
        base.Awake();   
        player = GetComponent<PlayerManager>(); 
    }

    protected override void Start()
    {
        base.Start();

        CalculateHealthBasedOnVitalityLevel(player._playerNetworkManager.vitality.Value);
        CalculateStaminaBasedOnenduranceLevel(player._playerNetworkManager.endurance.Value);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects / Instant Effects / Take Stamina Damage")]
public class TakeStaminaDamageEffect : InstantCharacterEffect
{
    public float _staminaDamage;
    public override void ProcessEffect(CharacterManager characterManager)
    {
        CalculateStaminaDamage(characterManager);
    }

    private void CalculateStaminaDamage(CharacterManager characterManager)
    {
        if (characterManager.IsOwner)
        {
            characterManager._characterNetworkManager.currentStamina.Value -= _staminaDamage;
        }
    }
}

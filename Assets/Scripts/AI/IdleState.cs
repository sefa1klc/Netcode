using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "A.I/states/Idle")]
public class IdleState : AIState
{
    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if(aiCharacter._characterCombatManager._currentTarget != null)
        {

            return this;
        }
        else
        {
            aiCharacter.aICharacterCombatManager.FindATargetVialineOfSight(aiCharacter);
            return this;
        }
    }
    
}

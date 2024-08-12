using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUtilityManager : Singleton<WorldUtilityManager>
{
    [Header("Layers")]
    [SerializeField] LayerMask _characterLayers;
    [SerializeField] LayerMask _enviromentsLayers;

    public LayerMask GetCharacterLayers()
    {
        return _characterLayers;
    }

    public LayerMask GetEnviromentsLayers()
    {
        return _enviromentsLayers;
    }

    public bool CanDamageThisTarget(CharacterGroup attackingCharacter
        , CharacterGroup targetCharacter)
    {
        if(attackingCharacter == CharacterGroup.Team01)
        {
            switch(targetCharacter)
            {
                case CharacterGroup.Team01: return false;
                case CharacterGroup.Team02: return true;
                default:
                    break;
            }
        }
        else if(attackingCharacter == CharacterGroup.Team02)
        {
            switch (attackingCharacter)
            {
                case CharacterGroup.Team01: return true;
                case CharacterGroup.Team02: return false;
                default:
                    break;
            }
        }

        return false;
    }
}

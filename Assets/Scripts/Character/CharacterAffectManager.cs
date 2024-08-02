using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAffectManager : MonoBehaviour
{

    CharacterManager _character;

    private void Awake()
    {
        _character = GetComponent<CharacterManager>();
    }
    public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
    {
        effect.ProcessEffect(_character);
    } 
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectManager : CharacterAffectManager
{
    [Header("Debug Delete Later")]
    [SerializeField] InstantCharacterEffect effectToTest;
    [SerializeField] bool _processEffect = false;

    private void Update()
    {
        if (_processEffect)
        {
            _processEffect = false;
            InstantCharacterEffect effect = Instantiate(effectToTest);
            ProcessInstantEffect(effect);
        }
        
    }
}

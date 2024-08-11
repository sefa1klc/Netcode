using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCharacterEffectManager : Singleton<WorldCharacterEffectManager>
{
    [SerializeField] private List<InstantCharacterEffect> _characterEffects;

    [Header("Damage")]
    public TakeDamageEffect TakeDamageEffect;

    [Header("VFX")]
    public GameObject _bloodSplatterVFX;
    private void Awake()
    {
        GenerateEffect();
    }

    private void GenerateEffect()
    {
        for(int i = 0; i < _characterEffects.Count; i++)
        {
            _characterEffects[i].instantEffectID = i;
        }
    }
}

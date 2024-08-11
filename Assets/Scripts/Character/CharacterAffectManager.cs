using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAffectManager : MonoBehaviour
{

    CharacterManager _character;

    [Header("VFX")]
    [SerializeField] GameObject _bloodSplatterVFX;

    private void Awake()
    {
        _character = GetComponent<CharacterManager>();
    }
    public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
    {
        effect.ProcessEffect(_character);
    } 

    public void PlayBloodSplatterVFX(Vector3 contactPoint)
    {
        if(_bloodSplatterVFX != null)
        {
            GameObject bloodSplatter = Instantiate(_bloodSplatterVFX,contactPoint,Quaternion.identity);
        }
        else
        {
            GameObject bloodSplatter = Instantiate(WorldCharacterEffectManager.Instance._bloodSplatterVFX, contactPoint, Quaternion.identity);
        }
    }
}

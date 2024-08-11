using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSoundFxMAnager : Singleton<WorldSoundFxMAnager>
{
    [Header("Damage Sounds")]
    public AudioClip[] _physicalDamage;

    [Header("Death Sounds")]
    public AudioClip[] _deathSounds;


    public AudioClip ChooseRandomSFXFromArray(AudioClip[] array)
    {
        int index = Random.Range(0, array.Length);

        return array[index];
    }
}

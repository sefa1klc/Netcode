using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class CharacterSoundFXManager : MonoBehaviour
{
    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
    private AudioSource source;

    [HideInInspector] public CharacterController _characterController;

    protected virtual void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        source = GetComponent<AudioSource>();
        if (source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
        }
    }
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_characterController.center), FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_characterController.center), FootstepAudioVolume);
        }
    }

    public void PlaySoundFX(AudioClip soundFX, float volume = 1, bool reandomPitch = true, float ptichRandom = 0.1f)
    {
        source.PlayOneShot(soundFX,volume);
        source.pitch = 1;

        if (reandomPitch){
            source.pitch += Random.Range(-ptichRandom, ptichRandom);
        }
    }
}

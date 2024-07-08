using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using TMPro;

public class CameraController : Singleton<CameraController>
{
    [SerializeField] private float _amplitudGain = 0.5f;
    [SerializeField] private float _frequencyGain = 0.5f;

    private CinemachineVirtualCamera _virtualCamera;

    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void FallowPLayer(Transform transform)
    {
        _virtualCamera.Follow = transform;
        var _perlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _perlin.m_AmplitudeGain = _amplitudGain;
        _perlin.m_FrequencyGain = _frequencyGain;
    }
}

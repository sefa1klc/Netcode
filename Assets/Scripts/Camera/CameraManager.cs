using Cinemachine;
using UnityEngine;

namespace Camera
{
    public class CameraManager : Singleton<CameraManager>
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
            var perlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (perlin != null)
            {
                perlin.m_AmplitudeGain = _amplitudGain;
                perlin.m_FrequencyGain = _frequencyGain;
            }
            
        
        }
    }
}

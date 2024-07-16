using Cinemachine;
using UnityEngine;

namespace Camera
{
    public class CameraManager : Singleton<CameraManager>
    {
        //[SerializeField] private float _amplitudGain = 0.5f;
        //[SerializeField] private float _frequencyGain = 0.5f;


        private CinemachineVirtualCamera _virtualCamera;

        private void Awake()
        {
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        }

        public void FallowPLayer(Transform transform)
        {
            if (_virtualCamera != null)  _virtualCamera.Follow = transform;
        }
    }
}

using UnityEngine;
using Cinemachine;
using InputSystem;

namespace Camera
{
    public class CinemachinePovExtension : CinemachineExtension
    {
        [SerializeField] private float _clampAngle = 80f;
        [SerializeField] private float _horizontalSpeed = 10f;
        [SerializeField] private float _verticalSpeed = 10f;
        
        private InputManager _inInputManager;
        private Vector3 _startingRotations;

        protected override void Awake()
        {
            _inInputManager = InputManager.Instance;
            base.Awake();
        }

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (vcam.Follow)
            {
                if (stage == CinemachineCore.Stage.Aim)
                {
                    if (_startingRotations == null) _startingRotations = transform.localRotation.eulerAngles;
                    Vector2 deltaInput = _inInputManager.GetMouseDelta();
                    _startingRotations.x += deltaInput.x * _verticalSpeed * Time.deltaTime;
                    _startingRotations.y += deltaInput.y *_horizontalSpeed * Time.deltaTime;
                    _startingRotations.y = Mathf.Clamp(_startingRotations.y, -_clampAngle, _clampAngle);
                    state.RawOrientation = Quaternion.Euler(-_startingRotations.y, _startingRotations.x , 0f);
                }
            }
        }
    }
}
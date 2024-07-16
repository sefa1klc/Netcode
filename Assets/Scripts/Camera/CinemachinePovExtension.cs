using UnityEngine;
using Cinemachine;

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
                    Vector2 deltaInput = _inInputManager.GetMouseDelta();
                    _startingRotations.x += deltaInput.x * _horizontalSpeed * deltaTime;
                    _startingRotations.y -= deltaInput.y * _verticalSpeed * deltaTime;
                    _startingRotations.y = Mathf.Clamp(_startingRotations.y, -_clampAngle, _clampAngle);
                    state.RawOrientation = Quaternion.Euler(_startingRotations.y, _startingRotations.x, 0f);
                }
            }
        }
    }
}

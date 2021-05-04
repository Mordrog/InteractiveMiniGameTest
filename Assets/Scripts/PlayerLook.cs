using UnityEngine;
using UnityEngine.InputSystem;

namespace InteractiveMiniGame
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerLook : MonoBehaviour
    {
        [SerializeField] public Camera firstPersonCamera;

        [SerializeField] public Vector2 sensivity = new Vector2(8.0f, 0.5f);

        [SerializeField, Range(0, 90)] public float clamp = 85.0f;

        PlayerInput _playerInput;

        Vector2 _cameraLookAxis;

        float _xRotation = 0f;

        void Start()
        {
            _playerInput = GetComponent<PlayerInput>();
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            if (!firstPersonCamera)
                return;

            transform.Rotate(Vector3.up, _cameraLookAxis.x * Time.deltaTime);

            _xRotation -= _cameraLookAxis.y;
            _xRotation = Mathf.Clamp(_xRotation, -clamp, clamp);

            firstPersonCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        }

        public void OnCameraLook(InputAction.CallbackContext context)
        {
            var cameraLookAxis = context.ReadValue<Vector2>();

            if (_playerInput.currentControlScheme == "Keyboard_and_Mouse")
            {
                cameraLookAxis = Vector2.Scale(cameraLookAxis, new Vector2(100.0f/Screen.width, 100.0f/Screen.height));
            }
            else if (_playerInput.currentControlScheme == "Gamepad")
            {
                cameraLookAxis = Vector2.Scale(cameraLookAxis, new Vector2(3.0f, 1.5f));
            }

            _cameraLookAxis = Vector2.Scale(cameraLookAxis, sensivity);
        }
    }
}
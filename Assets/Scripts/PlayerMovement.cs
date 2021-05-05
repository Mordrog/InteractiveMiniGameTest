using UnityEngine.InputSystem;
using UnityEngine;

namespace InteractiveMiniGame
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        private CharacterController _controller;

        [SerializeField] public float speed = 12f;
        [SerializeField] public float gravity = -10f;
        [SerializeField] public float jumpHeight = 2f;

        [SerializeField] public Transform groundCheck;
        [SerializeField] public float groundDistance = 0.4f;
        [SerializeField] public LayerMask groundMask;

        Vector2 _movementDirection;

        Vector3 _gravityVelocity;
        bool _isGrounded;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        void FixedUpdate()
        {
            Vector3 move = transform.right * _movementDirection.x + transform.forward * _movementDirection.y;

            _controller.Move(move * speed * Time.deltaTime);


            _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (_isGrounded && _gravityVelocity.y < 0)
            {
                _gravityVelocity.y = -2f;
            }

            _gravityVelocity.y += gravity * Time.deltaTime;

            _controller.Move(_gravityVelocity * Time.deltaTime);
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            _movementDirection = context.ReadValue<Vector2>();
        }

        // Had to add somewhere :D
        public void ExitGame(InputAction.CallbackContext context)
        {
            if (context.performed)
                Application.Quit();
        }
    }
}

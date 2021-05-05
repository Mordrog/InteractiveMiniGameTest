using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InteractiveMiniGame.RussianEggs
{
    public class RussianEggsController : MonoBehaviour, IInteractable
    {
        [Tooltip("Russian minigame main logic")]
        [SerializeField] public RussianEggsMainGame russianEggsMain;

        [Tooltip("Action map for minigame")]
        [SerializeField] public string actionMap;

        [Header("Action references names")]
        [SerializeField] public string exitGame;
        [SerializeField] public string upRight;
        [SerializeField] public string downRight;
        [SerializeField] public string upLeft;
        [SerializeField] public string downLeft;

        [Header("Virtual buttons")]
        [SerializeField] public VirtualButton upRightButton;
        [SerializeField] public VirtualButton downRightButton;
        [SerializeField] public VirtualButton upLeftButton;
        [SerializeField] public VirtualButton downLeftButton;

        public bool CanInteract { get; private set; } = true;

        private PlayerInput _currentPlayerInput;
        private CloseUpController _curretnCloseUpController;
        private string _previousPlayerMap;

        public void DoInteract(Interactor interactor)
        {
            CanInteract = false;

            _currentPlayerInput = interactor.GetComponent<PlayerInput>();

            if (!_currentPlayerInput)
            {
                CanInteract = true;
                Debug.LogError($"Interactor object {interactor.gameObject} does not have PlayerInput component!");
                return;
            }

            _previousPlayerMap = _currentPlayerInput.currentActionMap.name;
            StartCoroutine(SwitchControls(_currentPlayerInput, actionMap));

            _curretnCloseUpController = interactor.GetComponent<CloseUpController>();

            if (_curretnCloseUpController)
            {
                _curretnCloseUpController.PerformCloseUp(transform, () =>
                {
                    russianEggsMain.StartGame();
                    BindPlayerInput(_currentPlayerInput);
                });
            }
            else
            {
                russianEggsMain.StartGame();
                BindPlayerInput(_currentPlayerInput);
            }
        }

        // There is error with callbacks in new input system that can cause stackoverflow exception when map action is changed
        // which can happen on interaction. Ref: https://issuetracker.unity3d.com/issues/inputsystem-switchcurrentactionmap-causes-a-stackoverflow-when-called-by-each-pahse-of-an-action
        // Bug should be fixed in InputSystem 1.1.0
        IEnumerator SwitchControls(PlayerInput playerInput, string actionMap)
        {
            yield return new WaitForSeconds(0.1f);

            playerInput?.SwitchCurrentActionMap(actionMap);

            yield return null;
        }

        void BindPlayerInput(PlayerInput playerInput)
        {
            var exitGameAction = playerInput.actions.FindActionMap(actionMap).FindAction(exitGame);
            var upRightAction = playerInput.actions.FindActionMap(actionMap).FindAction(upRight);
            var downRightAction = playerInput.actions.FindActionMap(actionMap).FindAction(downRight);
            var upLeftAction = playerInput.actions.FindActionMap(actionMap).FindAction(upLeft);
            var downLeftAction = playerInput.actions.FindActionMap(actionMap).FindAction(downLeft);

            exitGameAction.performed += OnExitGame;
            upRightAction.performed += OnUpRight;
            downRightAction.performed += OnDownRight;
            upLeftAction.performed += OnUpLeft;
            downLeftAction.performed += OnDownLeft;
        }

        void UnbindPlayerInput(PlayerInput playerInput)
        {
            var exitGameAction = playerInput.actions.FindActionMap(actionMap).FindAction(exitGame);
            var upRightAction = playerInput.actions.FindActionMap(actionMap).FindAction(upRight);
            var downRightAction = playerInput.actions.FindActionMap(actionMap).FindAction(downRight);
            var upLeftAction = playerInput.actions.FindActionMap(actionMap).FindAction(upLeft);
            var downLeftAction = playerInput.actions.FindActionMap(actionMap).FindAction(downLeft);

            exitGameAction.performed -= OnExitGame;
            upRightAction.performed -= OnUpRight;
            downRightAction.performed -= OnDownRight;
            upLeftAction.performed -= OnUpLeft;
            downLeftAction.performed -= OnDownLeft;
        }

        public void OnExitGame(InputAction.CallbackContext context)
        {
            russianEggsMain.ExitGame();
            UnbindPlayerInput(_currentPlayerInput);

            StartCoroutine(SwitchControls(_currentPlayerInput, _previousPlayerMap));
            _currentPlayerInput = null;

            _curretnCloseUpController?.PerformReturnObject(() => CanInteract = true);
            _curretnCloseUpController = null;
        }

        public void OnUpRight(InputAction.CallbackContext context)
        {
            if (russianEggsMain.gameStarted && !russianEggsMain.gameOver)
                russianEggsMain.wolfPosition = InGamePosition.UpRight;

            ClickButton(upRightButton);
        }

        public void OnDownRight(InputAction.CallbackContext context)
        {
            if (russianEggsMain.gameStarted && !russianEggsMain.gameOver)
                russianEggsMain.wolfPosition = InGamePosition.DownRight;

            ClickButton(downRightButton);
        }

        public void OnUpLeft(InputAction.CallbackContext context)
        {
            if (russianEggsMain.gameStarted && !russianEggsMain.gameOver)
                russianEggsMain.wolfPosition = InGamePosition.UpLeft;

            ClickButton(upLeftButton);
        }

        public void OnDownLeft(InputAction.CallbackContext context)
        {
            if (russianEggsMain.gameStarted && !russianEggsMain.gameOver)
                russianEggsMain.wolfPosition = InGamePosition.DownLeft;

            ClickButton(downLeftButton);
        }

        void ClickButton(VirtualButton button)
        {
            StartCoroutine(ClickButton(0.2f));

            IEnumerator ClickButton(float time)
            {
                for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / time)
                {
                    button.On = true;
                    yield return null;
                }

                button.On = false;
            }
        }
    }
}
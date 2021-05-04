using UnityEngine;
using UnityEngine.InputSystem;

namespace InteractiveMiniGame.UI
{
    [RequireComponent(typeof(Canvas))]
    public class HUD : MonoBehaviour
    {
        // Will propably need no scene dependant solution
        [SerializeField] GameObject player;

        [SerializeField] RectTransform crossHair;

        [SerializeField] RectTransform interactableInfo;
        [SerializeField] RectTransform russianEggsInstruction;

        private PlayerInput _playerInput;
        private Interactor _interactor;
        private CloseUpController _closeUpController;

        void Awake()
        {
            _playerInput = player.GetComponent<PlayerInput>();
            _interactor = player.GetComponent<Interactor>();
            _closeUpController = player.GetComponent<CloseUpController>();

        }


        void Update()
        {
            UpdateInteractableInfo();

            crossHair.gameObject.SetActive(_interactor.currentInteractable == null);
        }

        void UpdateInteractableInfo()
        {
            if (_interactor.currentInteractable != null && !_closeUpController.closedUpObject)
            {
                interactableInfo.gameObject.SetActive(true);

                if (_interactor.currentInteractable is RussianEggs.RussianEggsController)
                {
                    russianEggsInstruction.gameObject.SetActive(true);
                }
            }
            else
            {
                interactableInfo.gameObject.SetActive(false);
                russianEggsInstruction.gameObject.SetActive(false);
            }
        }
    }
}
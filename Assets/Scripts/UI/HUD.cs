using UnityEngine;
using UnityEngine.InputSystem;

namespace InteractiveMiniGame.UI
{
    [RequireComponent(typeof(Canvas))]
    public class HUD : MonoBehaviour
    {
        // Will propably need no scene dependant solution
        [SerializeField] public GameObject player;

        [SerializeField] public RectTransform crossHair;

        [SerializeField] public RectTransform interactableInfo;

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

            crossHair.gameObject.SetActive(_interactor.CurrentInteractable == null);
        }

        void UpdateInteractableInfo()
        {
            if (_interactor.CurrentInteractable != null && !_closeUpController.ClosedUpObject)
            {
                interactableInfo.gameObject.SetActive(true);
            }
            else
            {
                interactableInfo.gameObject.SetActive(false);
            }
        }
    }
}
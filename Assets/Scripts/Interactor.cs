using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InteractiveMiniGame
{
    public class Interactor : MonoBehaviour
    {
        [SerializeField] public Camera firstPersonCamera;

        [SerializeField] public float interactDistance = 2.0f;

        [SerializeField] public float radius = 0.1f;

        [SerializeField] public LayerMask interactMask;

        public IInteractable CurrentInteractable { get; private set; }

        void FixedUpdate()
        {
            if (!firstPersonCamera)
                return;

            Ray raycastRay = new Ray(firstPersonCamera.transform.position, firstPersonCamera.transform.forward);
            if (Physics.SphereCast(raycastRay, radius, out RaycastHit raycastHit, interactDistance, interactMask, QueryTriggerInteraction.Collide))
            {
                CurrentInteractable = raycastHit.transform.GetComponentInParent<IInteractable>();

                if (CurrentInteractable == null)
                {
                    Debug.LogError($"Gameobject {raycastHit.transform.gameObject} have interactable mask but does not implement IInteractable interface!");
                    return;
                }
            }
            else
                CurrentInteractable = null;
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (CurrentInteractable != null && CurrentInteractable.CanInteract)
                CurrentInteractable.DoInteract(this);
        }
    }
}
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

        public IInteractable currentInteractable { get; private set; }

        void FixedUpdate()
        {
            if (!firstPersonCamera)
                return;

            Ray raycastRay = new Ray(firstPersonCamera.transform.position, firstPersonCamera.transform.forward);
            if (Physics.SphereCast(raycastRay, radius, out RaycastHit raycastHit, interactDistance, interactMask, QueryTriggerInteraction.Collide))
            {
                currentInteractable = raycastHit.transform.GetComponentInParent<IInteractable>();

                if (currentInteractable == null)
                {
                    Debug.LogError($"Gameobject {raycastHit.transform.gameObject} have interactable mask but does not implement IInteractable interface!");
                    return;
                }
            }
            else
                currentInteractable = null;
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (currentInteractable != null && currentInteractable.CanInteract)
                currentInteractable.DoInteract(this);
        }
    }
}
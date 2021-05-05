using System;
using System.Collections;
using UnityEngine;

namespace InteractiveMiniGame
{
    public class CloseUpController : MonoBehaviour
    {
        [Tooltip("Object that will be used as reference to close up to")]
        [SerializeField] public Transform objectToCloseTo;

        [Tooltip("Scale of how close object should be closed to")]
        [SerializeField] public float closeUpScaleOffset = 0.4f;

        public Transform closedUpObject { get; private set; }

        public delegate void OnPerformEnd();

        public Vector3 ObjectCloseUpPosition => objectToCloseTo.position + objectToCloseTo.forward * closeUpScaleOffset;

        private Vector3 _originalObjectPosition;
        private Quaternion _originalObjectRotation;

        public void PerformCloseUp(Transform objectToClose, OnPerformEnd onPerformClosedUp)
        {
            if (closedUpObject == objectToClose)
                return;

            closedUpObject = objectToClose;
            _originalObjectPosition = closedUpObject.position;
            _originalObjectRotation = closedUpObject.rotation;
            StartCoroutine(PerformCloseUp(closedUpObject, onPerformClosedUp, 1.0f));
        }

        IEnumerator PerformCloseUp(Transform objectToClose, OnPerformEnd onPerformClosedUp, float time)
        {
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / time)
            {
                objectToClose.position = Vector3.Lerp(objectToClose.position, ObjectCloseUpPosition, t);
                objectToClose.rotation = Quaternion.Lerp(objectToClose.rotation, objectToCloseTo.rotation, t);
                yield return null;
            }

            onPerformClosedUp?.Invoke();
        }

        public void PerformReturnObject(OnPerformEnd onPerformReturned)
        {
            if (!closedUpObject)
                return;

            StartCoroutine(PerformReturnObject(onPerformReturned, 1.0f));
        }

        IEnumerator PerformReturnObject(OnPerformEnd onPerformReturned, float time)
        {
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / time)
            {
                closedUpObject.position = Vector3.Lerp(closedUpObject.position, _originalObjectPosition, t);
                closedUpObject.rotation = Quaternion.Lerp(closedUpObject.rotation, _originalObjectRotation, t);
                yield return null;
            }

            closedUpObject = null;
            onPerformReturned?.Invoke();
        }
    }
}
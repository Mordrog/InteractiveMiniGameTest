using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveMiniGame
{
    public interface IInteractable
    {
        bool CanInteract { get; }

        void DoInteract(Interactor interactor);
    }
}
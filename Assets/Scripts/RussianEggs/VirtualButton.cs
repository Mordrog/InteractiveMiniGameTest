using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveMiniGame.RussianEggs
{
    public class VirtualButton : MonoBehaviour
    {
        [SerializeField] new SpriteRenderer renderer;

        [SerializeField] Sprite onState;

        [SerializeField] Sprite offState;

        private bool _on;

        public bool On {
            get => _on;
            set
            {
                renderer.sprite = value ? onState : offState;
                _on = value;
            }
        }
    }
}
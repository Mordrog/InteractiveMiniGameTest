using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveMiniGame.RussianEggs
{
    public class VirtualButton : MonoBehaviour
    {
        [SerializeField] public SpriteRenderer buttonRenderer;

        [SerializeField] public Sprite onState;

        [SerializeField] public Sprite offState;

        private bool _on;

        public bool On
        {
            get => _on;
            set
            {
                buttonRenderer.sprite = value ? onState : offState;
                _on = value;
            }
        }
    }
}
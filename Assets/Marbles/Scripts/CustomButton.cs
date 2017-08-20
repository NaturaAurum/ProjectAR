using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

namespace ProjectAR.Assets.Marbles.Scripts
{
    public class ButtonEvent : UnityEvent<GameObject>{}
    [DefaultExecutionOrder(-9999)]
    public class CustomButton : MonoBehaviour, IPointerClickHandler
    {
        private ButtonEvent _events;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            _events = new ButtonEvent();
        }

        public void AddListener(UnityAction<GameObject> call)
        {
            _events.AddListener(call);
        }

        public void RemoveListener(UnityAction<GameObject> call)
        {
            _events.RemoveListener(call);
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            _events.Invoke(gameObject);
        }
    }
}
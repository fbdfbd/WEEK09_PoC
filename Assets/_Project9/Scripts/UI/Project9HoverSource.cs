using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project9.UI
{
    public sealed class Project9HoverSource : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
    {
        private Action<PointerEventData> _onEnter;
        private Action<PointerEventData> _onMove;
        private Action _onExit;

        public void Configure(
            Action<PointerEventData> onEnter,
            Action<PointerEventData> onMove,
            Action onExit)
        {
            _onEnter = onEnter;
            _onMove = onMove;
            _onExit = onExit;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _onEnter?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _onExit?.Invoke();
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            _onMove?.Invoke(eventData);
        }
    }
}

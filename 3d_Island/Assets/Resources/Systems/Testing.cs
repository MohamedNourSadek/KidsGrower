using System;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;

////TODO: custom icon for OnScreenButton component

namespace UnityEngine.InputSystem.OnScreen
{
    [AddComponentMenu("Input/On-Screen Button")]
    public class OnScreenButton : OnScreenControl, IPointerDownHandler, IPointerUpHandler
    {
        public void OnPointerUp(PointerEventData eventData)
        {
            SendValueToControl(0.0f);
        }

        public void OnPointerDown(PointerEventData eventData)  
        {
            SendValueToControl(1.0f);
        }

        [SerializeField] private bool m_UsePressure;

        [InputControl(layout = "Button")]
        [SerializeField]
        private string m_ControlPath;

        protected override string controlPathInternal
        {
            get => m_ControlPath;
            set => m_ControlPath = value;
        }
    }
}

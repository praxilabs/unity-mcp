using UnityEngine.InputSystem;

namespace Praxilabs.Input
{
    public class MouseInputManager : Inputs
    {
        public static InputAction leftPressAction;
        public static InputAction rightPressAction;
        public static InputAction middlePressAction;
        public static InputAction screenPosition;
        public static InputAction mouseScrollWheel;
        public static InputAction zoomAction;
        public static InputAction resetAction;

        protected override void Initialize()
        {
            base.Initialize();

            leftPressAction = inputController.ScreenPress.LeftPress;
            rightPressAction = inputController.ScreenPress.RightPress;
            middlePressAction = inputController.ScreenPress.MiddlePress;
            zoomAction = inputController.ScreenPress.Zoom;
            resetAction = inputController.ScreenPress.Reset;

            screenPosition = inputController.ScreenPosition.Position;

            mouseScrollWheel = inputController.Axis.MouseScrollWheel;
        }

        private void OnEnable()
        {
            leftPressAction.Enable();
            rightPressAction.Enable();
            middlePressAction.Enable();
            zoomAction.Enable();
            resetAction.Enable();
            screenPosition.Enable();
            mouseScrollWheel.Enable();
        }

        private void OnDisable()
        {
            leftPressAction.Disable();
            rightPressAction.Disable();
            middlePressAction.Disable();
            zoomAction.Disable();
            resetAction.Disable();
            screenPosition.Disable();
            mouseScrollWheel.Disable();
        }
    }
}
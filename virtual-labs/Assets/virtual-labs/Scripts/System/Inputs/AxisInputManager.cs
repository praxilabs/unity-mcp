using UnityEngine.InputSystem;

namespace Praxilabs.Input
{
    public class AxisInputManager : Inputs
    {
        public static InputAction horizontal;
        public static InputAction vertical;
        public static InputAction mouseAxis;

        protected override void Initialize()
        {
            base.Initialize();

            horizontal = inputController.Axis.Horizontal;
            vertical = inputController.Axis.Vertical;
            mouseAxis = inputController.Axis.MouseAxis;
        }

        private void OnEnable()
        {
            horizontal.Enable();
            vertical.Enable();
            mouseAxis.Enable();
        }

        private void OnDisable()
        {
            horizontal.Disable();
            vertical.Disable();
            mouseAxis.Disable();
        }
    }
}
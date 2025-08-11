using UnityEngine.InputSystem;

namespace Praxilabs.Input
{
    public class KeyboardInputManager : Inputs
    {

        //right shift + I
        public static InputAction browseInstructions;
        //left shift + left ctrl + U
        public static InputAction showStages;
        //Z
        public static InputAction lockRotation;
        //X
        public static InputAction lockPosition;
        //C
        public static InputAction freeze;
        //Q
        public static InputAction birdEye;
        //T
        public static InputAction settingMenu;
        //P
        public static InputAction interestPoint;
        //Tab
        public static InputAction switchCameraSide;

        protected override void Initialize()
        {
            base.Initialize();

            browseInstructions = inputController.KeyBoard.BrowseInstructions;
            showStages = inputController.KeyBoard.ShowStages;
            lockRotation = inputController.KeyBoard.LockRotation;
            lockPosition = inputController.KeyBoard.LockPosition;
            freeze = inputController.KeyBoard.Freeze;
            birdEye = inputController.KeyBoard.BirdEye;
            settingMenu = inputController.KeyBoard.SettingMenu;
            interestPoint = inputController.KeyBoard.InterestPoint;
            switchCameraSide = inputController.KeyBoard.SwitchCameraSide;
        }

        private void OnEnable()
        {
            browseInstructions.Enable();
            showStages.Enable();
            lockRotation.Enable();
            lockPosition.Enable();
            freeze.Enable();
            birdEye.Enable();
            settingMenu.Enable();
            interestPoint.Enable();
            switchCameraSide.Enable();
        }

        private void OnDisable()
        {
            browseInstructions.Disable();
            showStages.Disable();
            lockRotation.Disable();
            lockPosition.Disable();
            freeze.Disable();
            birdEye.Disable();
            settingMenu.Disable();
            interestPoint.Disable();
            switchCameraSide.Disable();
        }
    }
}
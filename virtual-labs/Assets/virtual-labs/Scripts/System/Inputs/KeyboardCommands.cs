using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Praxilabs.Input
{
    public class KeyboardCommands : MonoBehaviour
    {
        //Will be changed later after integrating new event manager
        [HideInInspector] public UnityEvent SpaceKeyPressedEvent;
        [HideInInspector] public UnityEvent ShowExpStagesEvent;
        [HideInInspector] public UnityEvent BrowseInstructions;
        [HideInInspector] public UnityEvent SwitchCameraSide;

        void Update()
        {
            if (KeyboardInputManager.lockRotation.IsPressed())
            {
                SpaceKeyPressedEvent.Invoke();
            }
            if (KeyboardInputManager.showStages.IsPressed())
            {
                ShowExpStagesEvent.Invoke();
            }
            if (KeyboardInputManager.browseInstructions.IsPressed())
            {
                BrowseInstructions.Invoke();
            }
            if(KeyboardInputManager.switchCameraSide.WasPerformedThisFrame())
            {
                SwitchCameraSide?.Invoke();
            }
        }

        public bool GetKey(Key keyCode)
        {
            return Keyboard.current[keyCode].isPressed;
        }
    }
}
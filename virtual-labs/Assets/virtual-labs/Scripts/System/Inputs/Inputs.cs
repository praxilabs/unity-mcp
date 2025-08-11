using UnityEngine;

namespace Praxilabs.Input
{
    public class Inputs : MonoBehaviour
    {
        public static InputController inputController;
        
        private void Awake()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            inputController = new InputController();
        }
    }    
}
using UnityEngine;
using UnityEngine.UI;

namespace Praxilabs
{
    [RequireComponent(typeof(Button))]
    public class TabUI : MonoBehaviour 
    {
        public Button Btn {get; private set;}

        private void Awake()
        {
            Btn = GetComponent<Button>();
        }
    }
}
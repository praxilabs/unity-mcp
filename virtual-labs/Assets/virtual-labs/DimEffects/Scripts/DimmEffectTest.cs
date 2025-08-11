using UnityEngine;

namespace DimmEffect
{
	public class DimmEffectTest: MonoBehaviour
	{
		[SerializeField] private DimmEffectManager _dimmEffectManager;

        //for testing
        [SerializeField] private GameObject _focusObject;
        [ContextMenu("Add To Focus")]
        public void AddToFocus() => _dimmEffectManager.AddToFocus(_focusObject);
        [ContextMenu("Remove from Focus")]
        public void RemoveFromFocus() => _dimmEffectManager.RemoveFromFocus(_focusObject);
    }

}
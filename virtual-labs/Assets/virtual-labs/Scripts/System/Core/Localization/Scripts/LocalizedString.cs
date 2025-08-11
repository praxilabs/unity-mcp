using System;
using UnityEngine;

namespace LocalizationSystem
{
	[Serializable]
	public class LocalizedString
	{
        public string table;
        public string key;

        [Newtonsoft.Json.JsonIgnore] public string Value { get; protected set; }
        public event Action<string> OnStringUpdateEvent;

        public LocalizedString()
        {
            LocalizationManager.OnLocaleChanged += Refresh;
        }

        public void Refresh()
        {
            Debug.Log($"Getting value for table/key ({table}/{key})");
            Value = LocalizationManager.Instance.GetLocalizedString(table, key);
            OnStringUpdateEvent?.Invoke(Value);
        }

        ~LocalizedString()
        {
            LocalizationManager.OnLocaleChanged -= Refresh;
        }
    }

}
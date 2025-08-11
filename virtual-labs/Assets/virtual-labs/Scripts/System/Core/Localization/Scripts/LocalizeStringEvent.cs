using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LocalizationSystem
{
    public class LocalizeStringEvent : MonoBehaviour
    {
        [SerializeField] private LocalizedString _localizedString;
        public LocalizedString LocalizedString
        {
            get
            {
                return _localizedString;
            }
            set
            {
                _localizedString.OnStringUpdateEvent -= UpdateStringFunction;
                _localizedString = value;
                _localizedString.OnStringUpdateEvent += UpdateStringFunction;
                UpdateStringFunction(_localizedString.Value);
            }
        }

        public UnityEngine.Events.UnityEvent<string> UpdateString;
      
        private void Start()
        {
            Refresh();
            if (_localizedString is not null)
            {
                UpdateStringFunction(_localizedString.Value);
                _localizedString.OnStringUpdateEvent -= UpdateStringFunction;
                _localizedString.OnStringUpdateEvent += UpdateStringFunction;
            }
        }

        private void UpdateStringFunction(string updatedString)
        {
            print($"Updating localized string mono ({_localizedString.table},{_localizedString.key})");
            UpdateString.Invoke(updatedString);
        }
        public void Refresh()
        {
            _localizedString.Refresh();
        }
    }
}
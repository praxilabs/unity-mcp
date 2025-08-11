using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
namespace SideMenu
{
    public class SideMenuButton : MonoBehaviour
    {
        [SerializeField] protected RectTransform _myRectTransform;

        [SerializeField] protected GameObject _yellowNotification;
        [SerializeField] protected TextMeshProUGUI _notificationText;

        [SerializeField] protected GameObject _grayHighlight;
        [SerializeField] private Image _buttonImage;
        /// <summary>
        /// Sprite of icon of the button, like notification icon, book icon, table icon, etc.
        /// </summary>
        //[field: SerializeField] public Image ButtonIcon { get; protected set; }
        public Sprite Icon { set => _buttonImage.sprite = value; get => _buttonImage.sprite; }
        [SerializeField] protected Button _myButton;
        /// <summary>
        /// Family of the button, used to stack buttons in an expandable button if added more than one to same island.
        /// </summary>
        [field: SerializeField] public string Family { get; set; }
        /// <summary>
        /// Id of the button, used to identify buttons for deletion.<br/>
        /// ** Not unique **
        /// </summary>
        [field: SerializeField] public string Id { get; set; }

        public UnityEvent onButtonClick;

        /// <summary>
        /// Procedure for what will happen on clicking, calls <see cref="onButtonClick"/>
        /// </summary>
        public virtual void Click()
        {
            onButtonClick?.Invoke();
        }
        /// <summary>
        /// Sets the text in the yellow cirlce if needed
        /// </summary>
        /// <param name="text">Text to be shown in yellow circle</param>
        public void SetNotificationText(string text)
        {
            _notificationText.text = text;
        }
        /// <summary>
        /// Sets weather the notification yellow circle should be shown or not, with it's text.
        /// </summary>
        /// <param name="showNotification">Show notification circle or not?</param>
        public void SetNotification(bool showNotification)
        {
            _yellowNotification.SetActive(showNotification);
        }
        /// <summary>
        /// Sets the hightlight (gray circle) to be shown or hidden.
        /// </summary>
        /// <param name="show"><Should the gray highlight circle be shown ?/param>
        public void SetHighlight(bool show)
        {
            _grayHighlight.SetActive(show);
        }
        /// <summary>
        /// Enables the button or disables it.
        /// </summary>
        public void SetButtonActive(bool active)
        {
            _myButton.gameObject.SetActive(active);
        }
        /// <summary>
        /// Enables the button interaction or disables it.
        /// </summary>
        public void SetButtonInteractable(bool interactable)
        {
            _myButton.interactable = interactable;
        }
    }
}

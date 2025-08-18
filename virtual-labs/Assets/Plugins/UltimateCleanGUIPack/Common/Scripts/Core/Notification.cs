// Copyright (C) 2015-2021 gamevanilla - All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement.
// A Copy of the Asset Store EULA is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace UltimateClean
{
    /// <summary>
    /// The component used for the notifications in the kit. A notification
    /// is composed of a title label and a message label, and can be animated
    /// in a variety of ways: pop, fade, shake and slide.
    /// </summary>
    public class Notification : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
#pragma warning disable 649
        public int id;
        [SerializeField]
        private TextMeshProUGUI titleLabel;
        [SerializeField]
        private TextMeshProUGUI messageLabel;

        [SerializeField]
        private RuntimeAnimatorController popAnimator;
        [SerializeField]
        private RuntimeAnimatorController fadeAnimator;
        [SerializeField]
        private RuntimeAnimatorController shakeAnimator;
        [SerializeField]
        private RuntimeAnimatorController slideLeftAnimator;
        [SerializeField]
        private RuntimeAnimatorController slideRightAnimator;
        [SerializeField]
        private RuntimeAnimatorController slideUpAnimator;
        [SerializeField]
        private RuntimeAnimatorController slideDownAnimator;
        [SerializeField]
        private RuntimeAnimatorController slideRightAndFadeAnimator;
        [SerializeField]
        private UnityEngine.UI.Slider notificationTimerSlider;
#pragma warning restore 649

        public Action OnCompleted;

        private Animator animator;

        private bool isHovered = false;
        private float elapsedTime = 0f;

        public bool IsPendingDestruction { get; set; }
        public Action<Notification, bool> OnHoverChanged;
        public Action<Notification> OnReadyToDestroy;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void UpdateText(string title, string message)
        {
            titleLabel.text = title;
            messageLabel.text = message;
        }

        public void LaunchWithID(int notificationID, NotificationType type, NotificationPositionType position, float duration, string title, string message)
        {
            id = notificationID;
            LaunchContent(type, position, duration, title, message);
        }

        public void Launch(NotificationType type, NotificationPositionType position, float duration, string title, string message)
        {
            LaunchContent(type, position, duration, title, message);
        }

        private void LaunchContent(NotificationType type, NotificationPositionType position, float duration, string title, string message)
        {
            titleLabel.text = title;
            messageLabel.text = message;
            switch (type)
            {
                case NotificationType.Pop:
                    animator.runtimeAnimatorController = popAnimator;
                    break;

                case NotificationType.Fade:
                    animator.runtimeAnimatorController = fadeAnimator;
                    break;

                case NotificationType.Shake:
                    animator.runtimeAnimatorController = shakeAnimator;
                    break;

                case NotificationType.SlideLeft:
                    animator.runtimeAnimatorController = slideLeftAnimator;
                    break;

                case NotificationType.SlideRight:
                    animator.runtimeAnimatorController = slideRightAnimator;
                    break;

                case NotificationType.SlideUp:
                    animator.runtimeAnimatorController = slideUpAnimator;
                    break;

                case NotificationType.SlideDown:
                    animator.runtimeAnimatorController = slideDownAnimator;
                    break;
                case NotificationType.SlideRightAndFade:
                    animator.runtimeAnimatorController = slideRightAndFadeAnimator;
                    break;
            }

            var rectTransform = gameObject.GetComponent<RectTransform>();
            var size = rectTransform.sizeDelta;
            var newPos = rectTransform.anchoredPosition;

            switch (position)
            {
                case NotificationPositionType.TopLeft:
                {
                    rectTransform.anchorMin = new Vector2(0, 1);
                    rectTransform.anchorMax = new Vector2(0, 1);
                    newPos.x += size.x/2;
                    newPos.y -= size.y/2;
                }
                break;

                case NotificationPositionType.TopMiddle:
                {
                    rectTransform.anchorMin = new Vector2(0.5f, 1);
                    rectTransform.anchorMax = new Vector2(0.5f, 1);
                    newPos.y -= size.y/2;
                }
                break;

                case NotificationPositionType.TopRight:
                {
                    rectTransform.anchorMin = new Vector2(1, 1);
                    rectTransform.anchorMax = new Vector2(1, 1);
                    newPos.x -= size.x/2;
                    newPos.y -= size.y/2;
                }
                break;

                case NotificationPositionType.BottomLeft:
                {
                    rectTransform.anchorMin = new Vector2(0, 0);
                    rectTransform.anchorMax = new Vector2(0, 0);
                    newPos.x += size.x/2;
                    newPos.y += size.y/2;
                }
                break;

                case NotificationPositionType.BottomMiddle:
                {
                    rectTransform.anchorMin = new Vector2(0.5f, 0);
                    rectTransform.anchorMax = new Vector2(0.5f, 0);
                    newPos.y += size.y/2;
                }
                break;

                case NotificationPositionType.BottomRight:
                {
                    rectTransform.anchorMin = new Vector2(1, 0);
                    rectTransform.anchorMax = new Vector2(1, 0);
                    newPos.x -= size.x/2;
                    newPos.y += size.y/2;
                }
                break;
            }

            rectTransform.anchoredPosition = newPos;

            animator.Rebind();

            StartCoroutine(RunNotificationSequence(duration));
        }

        private IEnumerator RunNotificationSequence(float duration)
        { 
            NotificationsHandler.AddActiveNotification(this);
            if (notificationTimerSlider != null)
            {
                notificationTimerSlider.maxValue = duration;
                notificationTimerSlider.value = duration;
                animator.SetTrigger("Open");
                
                while (elapsedTime < duration)
                {
                    // Only increase elapsed time if not hovering
                    if (!isHovered)
                    {
                        float deltaTime = Time.deltaTime;
                        elapsedTime += deltaTime;
                    }
                    
                    notificationTimerSlider.value = duration - elapsedTime;
                    yield return null;
                }
                
                notificationTimerSlider.value = 0;
            }
            else
            {
                animator.SetTrigger("Open");
                
                // For notifications without slider, we still need to track elapsed time manually
                while (elapsedTime < duration)
                {
                    if (!isHovered)
                    {
                        elapsedTime += Time.deltaTime;
                    }
                    yield return null;
                }
            }

            animator.SetTrigger("Close");
            yield return new WaitForSeconds(1.0f);
            
            OnCompleted?.Invoke();
            OnReadyToDestroy?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(IsPendingDestruction) return;
            isHovered = true;
            OnHoverChanged?.Invoke(this, isHovered);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(IsPendingDestruction) return;
            isHovered = false;
            OnHoverChanged?.Invoke(this, isHovered);
        }
    }
}
using System.Collections.Generic;
using UltimateClean;
using UnityEngine;

public class NotificationsHandler : MonoBehaviour
{
    private static List<Notification> _activeNotifications = new List<Notification>();
    private static Notification _currentlyHoveredNotification = null;

    public static void AddActiveNotification(Notification notification)
    {
        if(_activeNotifications == null) 
            _activeNotifications = new List<Notification>();

        _activeNotifications.Add(notification);
        notification.OnHoverChanged += HandleNotificationHover;
        notification.OnReadyToDestroy += HandleNotificationReadyToDestroy;
        
        // Sort notifications by their hierarchy position
        SortNotificationsList();
    }

    private static void HandleNotificationHover(Notification notification, bool isHovering)
    {
         if (isHovering)
        {
            _currentlyHoveredNotification = notification;
        }
        else if (_currentlyHoveredNotification == notification)
        {
            _currentlyHoveredNotification = null;
            
            CheckPendingDestructions();
        }
    }

    private static void HandleNotificationReadyToDestroy(Notification notification)
    {
        // If no notification is being hovered, destroy immediately
        if (_currentlyHoveredNotification == null)
        {
            DestroyNotification(notification);
            return;
        }

        // If this notification is below the hovered one in the hierarchy, destroy immediately
        if (IsBelowHoveredNotification(notification))
        {
            DestroyNotification(notification);
            return;
        }

        // If this notification is above the hovered one, mark it as pending destruction
        notification.IsPendingDestruction = true;
    }

    private static void CheckPendingDestructions()
    {
        // Create a copy of the list to avoid modification during iteration
        List<Notification> notificationsToCheck = new List<Notification>(_activeNotifications);
        
        foreach (var notification in notificationsToCheck)
        {
            if (notification.IsPendingDestruction)
            {
                DestroyNotification(notification);
            }
        }
    }

    private static void DestroyNotification(Notification notification)
    {
        _activeNotifications.Remove(notification);
        notification.OnHoverChanged -= HandleNotificationHover;
        notification.OnReadyToDestroy -= HandleNotificationReadyToDestroy;
        Destroy(notification.gameObject);
    }

    private static bool IsBelowHoveredNotification(Notification notification)
    {
         if (_currentlyHoveredNotification == null)
            return false;
            
        // Get sibling indices to determine if one is below the other
        int hoveredIndex = _currentlyHoveredNotification.transform.GetSiblingIndex();
        int checkIndex = notification.transform.GetSiblingIndex();
        
        // In a vertical layout group, higher index means lower in the visual hierarchy
        return checkIndex > hoveredIndex;
    }

    private static void SortNotificationsList()
    {
        // Sort notifications by their sibling index
        _activeNotifications.Sort((a, b) => 
            a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
    }

    private void OnDestroy()
    {
        _activeNotifications = null;
        _currentlyHoveredNotification = null;
    }
}

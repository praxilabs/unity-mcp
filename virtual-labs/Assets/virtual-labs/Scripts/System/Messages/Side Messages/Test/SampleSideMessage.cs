using System.Collections;
using System.Collections.Generic;
using UltimateClean;
using UnityEngine;
using static SideMessagesManager;

public class SampleSideMessage : MonoBehaviour
{


    [SerializeField] private SideMessagesManager sideMessagesManager;
    [SerializeField] private GameObject typePrefab;
    [SerializeField] private string title;
    [SerializeField] private string message;
    [SerializeField] private NotificationType myAnimation = NotificationType.SlideRightAndFade;
    [SerializeField] private float duration;
    [SerializeField] private MessageType _messageType;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.2f);

        SideMessageData myMs2 = new SideMessageData()
        {
            //TypePrefab = typePrefab,
            Title = title,
            Message = message,
            Animation = myAnimation,
            Duration = duration
        };
        sideMessagesManager.OpenSideMessage(_messageType, myMs2);
    }

}

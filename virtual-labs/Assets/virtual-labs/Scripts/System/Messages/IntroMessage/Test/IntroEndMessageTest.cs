using Praxilabs.UIs;
using System.Collections;
using UnityEngine;

public class IntroEndMessageTest : MonoBehaviour
{
    [SerializeField] IntroEndMessagesManager messagesManager;
    [SerializeField] IntroEndMessageType introEndMessageType;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.2f);
        messagesManager.Open(0);
    }
}

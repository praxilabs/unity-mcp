using UnityEngine;
using static SideMessagesManager;

public static class CreateSideMessages
{
    private static SideMessagesManager _sideMessagesManager;

    public static void CreateSideMessage(MessageType messageType, SideMessageData sideMessageData)
    {
        GetManager();


        _sideMessagesManager.OpenSideMessage(messageType, sideMessageData);
    }

    public static void CreateSideMessage(MessageType messageType, SideMessageData sideMessageData, Vector2 offset)
    {
        GetManager();


        _sideMessagesManager.OpenSideMessage(messageType, sideMessageData, offset);
    }

    public static void CreateSideMessage(int messageID)
    {
        GetManager();


        _sideMessagesManager.OpenSideMessage(messageID);
    }

    public static void CreateSideMessage(int messageID, Vector2 offset)
    {
        GetManager();

        _sideMessagesManager.OpenSideMessage(messageID, offset);
    }

    private static void GetManager()
    {
        if (_sideMessagesManager == null)
            ResolveObjects();
    }

    private static void ResolveObjects()
    {
        _sideMessagesManager = ExperimentItemsContainer.Instance.Resolve<SideMessagesManager>();
    }
}
#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Object = System.Object;

[CreateAssetMenu(fileName = "EventScriptCreator", menuName = "Praxilabs/Events/Events Creator ", order = 1)]
public class EventScriptCreator : ScriptableObject
{
    public List<EventContainer> eventsContainer = new List<EventContainer>();
    [Space]
    public EventKeyType TypeToAdd;
    [HideInInspector]
    public bool scriptAlreadyCreated = false;
    private List<string> eventsNameChecker;

    #region Generate C# Script From Scriptable Object Content
    public void UpdateEventScript()
    {
        string fileName = this.name;
        string scriptPath = $"Assets/Scripts/System/Events/EventsGeneratedScripts/";

        if (scriptAlreadyCreated)
        {
            File.WriteAllText(scriptPath + fileName + ".cs", WriteEventContent(fileName));
            Debug.Log("Events Script is Updated");
        }
        else
        {
            if (FileExists(fileName))
                Debug.LogError("Script with this name is already exists");
            else
            {
                File.WriteAllText(scriptPath + fileName + ".cs", WriteEventContent(fileName));
                scriptAlreadyCreated = true;
                Debug.Log("Events Script is Created");
            }
        }

        foreach (var eventContainer in eventsContainer)
            eventContainer.UpdateEventCount();
        AssetDatabase.Refresh();
    }
    private string WriteEventContent(string scriptName)
    {
        string warning = $"// The Content of this script is \n " +
            $"//code generated any modifications will be overriden \n ";

        string scriptContent = $"\npublic class {scriptName} \n{{\n\t{CreateScriptContent()}\n}}";

        return warning + scriptContent;
    }

    private StringBuilder CreateScriptContent()
    {
        StringBuilder builder = new StringBuilder();
        eventsNameChecker = new List<string>();
        for (int i = 0; i < eventsContainer.Count; i++)
        {
            builder.Append(
                CreateEventsKeys(eventsContainer[i].eventList, eventsContainer[i].eventType.ToString() + "EventKey"));
        }
        return builder;
    }
    private StringBuilder CreateEventsKeys(List<EventName> events, string eventType)
    {
        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < events.Count; i++)
        {
            if (!eventsNameChecker.Contains(events[i].eventName))
            {
                builder.Append($" public static {eventType} {events[i].eventName} = new {eventType}(\"{ events[i].eventName}\");\n");
                eventsNameChecker.Add(events[i].eventName);
            }
            else
            {
                throw new ArgumentException($"Event name {events[i].eventName} exists more than once");
            }
        }
        return builder;
    }
    private bool FileExists(string scriptName)
    {
        DirectoryInfo dir = new DirectoryInfo("Assets/");
        FileInfo[] files = dir.GetFiles(scriptName + ".cs", SearchOption.AllDirectories);
        return files.Length > 0;
    }
    #endregion

    public void AddEventTypeToTheList()
    {
        if (IsTypeExist(TypeToAdd))
        {
            Debug.Log("Type Already Exist");
            return;
        }
        else
            eventsContainer.Add(new EventContainer(TypeToAdd));
    }

    private bool IsTypeExist(EventKeyType eventType)
    {
        for (int i = 0; i < eventsContainer.Count; i++)
        {
            if (eventsContainer[i].eventType == eventType)
                return true;
        }
        return false;
    }
}
#endif
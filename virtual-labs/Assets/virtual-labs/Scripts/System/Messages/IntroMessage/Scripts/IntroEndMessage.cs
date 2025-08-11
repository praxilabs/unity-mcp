using Praxilabs.UIs;
using System;
using System.Collections.Generic;

[Serializable]
public class IntroEndMessage
{
    public IntroEndMessageType messageType;
    public List<IntroEndMessageContent> Messages { get; set; }
}
[Serializable]
public class IntroEndMessageContent
{
    public string headerText;
    public string bodyText;
    public string footerText;
}


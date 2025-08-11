using UnityEngine;

public class SampleMiddleMessage : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            MiddleMessageData middleMessageData = new MiddleMessageData{
            MessageTitle = "Hola Hola",
            TitleDescription = "Booo",
            MessageHeader = "Particle System Vol 2",
            Message = "Your graph holds the key to unlocking Planck's constant (h). The slope of the best-fit line is your secret handshake. Use it (along with the electron charge) in the formula: h = (e * slope). See how close you get to the accepted value! Analyze, calculate, then move on!)",
            ActionOneText = "Confirm",
            ActionTwoText = "Cancel"
            };
            MiddleMessage middleMessage = MiddleMessagesManager.Instance.OpenMiddleMessage(MessageType.Informative, MiddleMessageType.TwoAction, middleMessageData);
            middleMessage.ActionOneButton.onClick.AddListener(PrintText);
        }   
        if(Input.GetKeyDown(KeyCode.U))
        {
            MiddleMessageData middleMessageData = new MiddleMessageData{
            MessageTitle = "This is real",
            MessageHeader = "Particle System Vol 53",
            Message = "Your graph holds the key to unlocking Planck's constant (h). The slope of the best-fit line is your secret handshake. Use it (along with the electron charge) in the formula: h = (e * slope). See how close you get to the accepted value! Analyze, calculate, then move on!)  Your graph holds the key to unlocking Planck's constant (h). The slope of the best-fit line is your secret handshake. Use it (along with the electron charge) in the formula: h = (e * slope). See how close you get to the accepted value! Analyze, calculate, then move on!) Your graph holds the key to unlocking Planck's constant (h). The slope of the best-fit line is your secret handshake. Use it (along with the electron charge) in the formula: h = (e * slope). See how close you get to the accepted value! Analyze, calculate, then move on!) Your graph holds the key to unlocking Planck's constant (h). The slope of the best-fit line is your secret handshake. Use it (along with the electron charge) in the formula: h = (e * slope). See how close you get to the accepted value! Analyze, calculate, then move on!) Your graph holds the key to unlocking Planck's constant (h). The slope of the best-fit line is your secret handshake. Use it (along with the electron charge) in the formula: h = (e * slope). See how close you get to the accepted value! Analyze, calculate, then move on!)",
            ActionOneText = "Confirm",
            ActionTwoText = "Delete"
            };
            MiddleMessage middleMessage = MiddleMessagesManager.Instance.OpenMiddleMessage(MessageType.Warning, MiddleMessageType.TwoAction, middleMessageData);
            middleMessage.ActionTwoButton.onClick.AddListener(PrintText);
        }   
        if(Input.GetKeyDown(KeyCode.I))
        {
            MiddleMessageData middleMessageData = new MiddleMessageData{
            MessageTitle = "Check Please",
            MessageHeader = "Particle System",
            Message = "Your graph holds the key to unlocking Planck's constant (h). The slope of the best-fit line is your secret handshake. Use it (along with the electron charge) in the formula: h = (e * slope). See how close you get to the accepted value! Analyze, calculate, then move on!) Your graph holds the key to unlocking Planck's constant (h). The slope of the best-fit line is your secret handshake. Use it (along with the electron charge) in the formula: h = (e * slope). See how close you get to the accepted value! Analyze, calculate, then move on!)",
            ActionOneText = "Done"
            };
            MiddleMessage middleMessage = MiddleMessagesManager.Instance.OpenMiddleMessage(MessageType.Error, MiddleMessageType.OneAction, middleMessageData);
            middleMessage.ActionOneButton.onClick.AddListener(PrintText);
        }   
    }

    private void PrintText()
    {
        print("YOU CLICKED a BUTTON!!");
    }
}
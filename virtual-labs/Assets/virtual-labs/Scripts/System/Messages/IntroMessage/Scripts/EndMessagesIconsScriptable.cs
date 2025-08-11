using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[CreateAssetMenu(fileName = "EndMessagesIcons", menuName = "Messages/End Messages Icons")]
public class EndMessagesIconsScriptable : ScriptableObject
{

    [SerializeField] private Sprite _oxi;
    [SerializeField] private Sprite _cup;

    public Sprite Oxi => _oxi;
    public Sprite Cup => _cup;
}


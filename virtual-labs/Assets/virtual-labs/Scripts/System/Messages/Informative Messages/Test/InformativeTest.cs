using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformativeTest : MonoBehaviour
{
    [SerializeField] private GameObject _informativePrefab;
    [SerializeField] private InformativeMessagesDataScriptable _infoMessageSO;
    void Start()
    {
        GameObject tmp =Instantiate(_informativePrefab);
        tmp.GetComponent<InformativeMessagesManager>().Open(_infoMessageSO);

    }


}

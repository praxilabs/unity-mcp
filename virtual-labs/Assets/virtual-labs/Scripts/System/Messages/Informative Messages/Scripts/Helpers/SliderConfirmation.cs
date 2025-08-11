using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SliderConfirmation : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Button _confirmationButton;
    void Start()
    {
        _slider = GetComponent<Slider>();
        _confirmationButton.GetComponent<Button>().onClick.AddListener(DisableRootObject);
    }
    void Update()
    {
        if (Input.GetMouseButton(0) || Mouse.current.leftButton.isPressed)
        {
            if (_slider != null)
            {
                if(_slider.value >= 1)
                {
                    _confirmationButton.gameObject.SetActive(true);
                    _slider.gameObject.SetActive(false);
                }
            }
        }
    }

    public void DisableRootObject()
    {
        transform.root.gameObject.SetActive(false);
    }
}

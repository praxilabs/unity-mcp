using Praxilabs.CameraSystem;
using System.Collections;
using UnityEngine;

public class CameraUIMovement : Singleton<CameraUIMovement>
{
    private Coroutine _moveCoroutine;

    private RotateCommand _rotateCommand;
    private MoveCommand _moveCommand;

    public void OnButtonPress(string direction)
    {
        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);

        switch (direction)
        {
            case "MoveForward":
                _moveCoroutine = StartCoroutine(CameraMovement(1, 0));
                break;
            case "MoveBackward":
                _moveCoroutine = StartCoroutine(CameraMovement(-1, 0));
                break;
            case "MoveRight":
                _moveCoroutine = StartCoroutine(CameraMovement(0, 1));
                break;
            case "MoveLeft":
                _moveCoroutine = StartCoroutine(CameraMovement(0, -1));
                break;
            case "1stPersonRotateLeft":
                _moveCoroutine = StartCoroutine(FirstPersonRotate(-CameraManager.Instance.currentCamera.GetComponent<CameraSettings>().rotationSpeed));
                break;
            case "1stPersonRotateRight":
                _moveCoroutine = StartCoroutine(FirstPersonRotate(CameraManager.Instance.currentCamera.GetComponent<CameraSettings>().rotationSpeed));
                break;  
            case "3rdPersonRotateLeft":
                _moveCoroutine = StartCoroutine(ThirdPersonRotate(-1));
                break;
            case "3rdPersonRotateRight":
                _moveCoroutine = StartCoroutine(ThirdPersonRotate(1));
                break;
        }
    }

    public void OnButtonRelease()
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }
    }

    private IEnumerator FirstPersonRotate(float direction)
    {
        _rotateCommand = new RotateCommand();
        _rotateCommand.camera = CameraManager.Instance.currentCamera;
        _rotateCommand.Refresh();

        while (true)
        {
            _rotateCommand.UIHorizontalRotate(direction);
            yield return null;
        }
    }
    
    private IEnumerator ThirdPersonRotate(float direction)
    {
        _rotateCommand = new RotateCommand();
        _rotateCommand.camera = CameraManager.Instance.currentCamera;
        _rotateCommand.Refresh();

        while (true)
        {
            _rotateCommand.ThirdPersonRotate(direction);
            yield return null;
        }
    }

    private IEnumerator CameraMovement(float verticalDirection, float horizontalDirection)
    {
        _moveCommand = new MoveCommand();
        _moveCommand.verticalValue = verticalDirection;
        _moveCommand.horizontalValue = horizontalDirection;
        _moveCommand.isUsingUI = true;

        while (true)
        {
            _moveCommand.Execute();
            yield return null;
        }
    }
}

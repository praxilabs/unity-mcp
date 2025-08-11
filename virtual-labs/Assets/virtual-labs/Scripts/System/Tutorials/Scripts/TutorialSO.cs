using System;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "Tutorials Data SO", menuName = "Tutorials/Tutorial Data")]
public class TutorialSO : ScriptableObject
{
    public List<TutorialObjectData> _tutorialModules = new List<TutorialObjectData>();
}
[Serializable]
public class TutorialObjectData
{

    public string _title;
    public TutorialModuleStatus Status;
    public List<TutorialData> _data;
}
[Serializable]
public class TutorialData
{
    public string _title;
    public string _info;
    public Vector2 _cardPositionPlaceholder;
    public Vector2 _cardAnchorMin;
    public Vector2 _cardAnchorMax;
    public TutorialImageData _imageData;
    public TutorialAnimationData _animationData;
}
[Serializable]
public class TutorialImageData
{
    public Sprite _image;
    public Vector2 _imageDimensions;
    public Vector2 _imagePlaceholderPosition;
    public Vector2 _imageAnchorMin;
    public Vector2 _imageAnchorMax;
    [Space]
    public Vector2 _arrowImagePosition;
    public float _arrowImageAngle;
}
[Serializable]
public class TutorialAnimationData
{
    public bool _canAnimate;
    public bool _isReversed;
    public Vector2 _animationStartPosition;

}

public enum TutorialModuleStatus
{
    Unfinished,
    Finished
}

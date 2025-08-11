using UnityEngine;

public class CableController : MonoBehaviour
{
    [SerializeField] private GameObject _headPlugPrefab;
    [SerializeField] private GameObject _tailPlugPrefab;

    [SerializeField] private GameObject _headPlugParent;
    [SerializeField] private GameObject _tailPlugParent;
    [SerializeField] private SplineGenerator splineGenerator;

    private GameObject _newHeadPlug;
    private GameObject _newTailPlug;

    [HideInInspector]
    [SerializeField] 
    private GameObject _oldHeadPlug;
    [HideInInspector]
    [SerializeField]
    private GameObject _oldTailPlug;

    private Vector3 _oldHeadPlugPosition;
    private Vector3 _oldHeadPlugRotation;
    private Vector3 _oldHeadPlugScale;

    private Vector3 _oldTailPlugPosition;
    private Vector3 _oldTailPlugRotation;
    private Vector3 _oldTailPlugScale;

    private void Start()
    {
        _oldTailPlug = _tailPlugPrefab;
        GenerateSpline();
    }

    public void UpdateHeadPlug()
    {
        SaveHeadData();
        if (_oldHeadPlug != null)
            DestroyImmediate(_oldHeadPlug);

        _newHeadPlug = Instantiate(_headPlugPrefab, _headPlugParent.transform);
        _oldHeadPlug = _newHeadPlug;
        RestoreHeadData(_newHeadPlug);
    }

    public void GenerateTailPlug()
    {
        SaveTailData();

        if (_oldTailPlug != null)
            DestroyImmediate(_oldTailPlug);

        _newTailPlug = Instantiate(_tailPlugPrefab, _tailPlugParent.transform);
        _oldTailPlug = _newTailPlug;
        RestoreTailData(_newTailPlug);
    }

    private void SaveHeadData()
    {
        if (_oldHeadPlug == null)
            return;

        _oldHeadPlugPosition = _oldHeadPlug.transform.localPosition;
        _oldHeadPlugRotation = _oldHeadPlug.transform.localEulerAngles;
        _oldHeadPlugScale = _oldHeadPlug.transform.localScale;
    }

    private void SaveTailData()
    {
        if (_oldTailPlug == null)
            return;

        _oldTailPlugPosition = _oldTailPlug.transform.localPosition;
        _oldTailPlugRotation = _oldTailPlug.transform.localEulerAngles;
        _oldTailPlugScale = _oldTailPlug.transform.localScale;
    }

    private void RestoreHeadData(GameObject headPlug)
    {
        headPlug.transform.localPosition = _oldHeadPlugPosition;
        headPlug.transform.localRotation = Quaternion.Euler(_oldHeadPlugRotation);
        headPlug.transform.localScale = _oldHeadPlugScale;
    }

    private void RestoreTailData(GameObject tailPlug)
    {
        tailPlug.transform.localPosition = _oldTailPlugPosition;
        tailPlug.transform.localRotation = Quaternion.Euler(_oldTailPlugRotation);
        tailPlug.transform.localScale = _oldTailPlugScale;
    }

    public void GenerateSpline()
    {
        splineGenerator.GenerateSpline();
    }
}
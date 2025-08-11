using UnityEngine;
using Dreamteck.Splines;
using System.Collections.Generic;
public class SplineGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> _splinePoints = new List<GameObject>();
    [SerializeField] private Material _splineMaterial;
    [SerializeField] private float _radius = 0.01f;
    [SerializeField] private int _sides = 8;

    [SerializeField] private SplineComputer _spline;

    private void Start()
    {
        _spline.Rebuild();
    }

    public void GenerateSpline()
    {
        if (_splinePoints.Count < 2)
        {
            Debug.Log("<color=#E4003A>At least two points are needed to generate a spline.</color>");
            return;
        }

        GenerateSplinePoints();
        GenerateTube();
        SetMaterial();
        MarkSceneDirty();
    }

    private void GenerateSplinePoints()
    {
        _spline = GetComponent<SplineComputer>();
        if (_spline == null)
            _spline = gameObject.AddComponent<SplineComputer>();

        // Create the spline points
        List<SplinePoint> points = new List<SplinePoint>();
        for (int i = 0; i < _splinePoints.Count; i++)
            points.Add(new SplinePoint(_splinePoints[i].transform.position));

        SaveSplineData(points.ToArray());
    }

    private void GenerateTube()
    {
        TubeGenerator tubeGenerator = GetComponent<TubeGenerator>();
        if (tubeGenerator == null)
            tubeGenerator = gameObject.AddComponent<TubeGenerator>();

        tubeGenerator.spline = _spline;
        tubeGenerator.size = _radius;
        tubeGenerator.sides = _sides;

        tubeGenerator.updateMethod = TubeGenerator.UpdateMethod.Update;
    }

    private void SetMaterial()
    {
        GetComponent<MeshRenderer>().material = _splineMaterial;  
    }

    private void SaveSplineData(SplinePoint[] splinePoints)
    {
        _spline.SetPoints(splinePoints);
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(_spline);
#endif
    }

    private void MarkSceneDirty()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            // Mark the GameObject as dirty to ensure changes are saved
            UnityEditor.EditorUtility.SetDirty(this);

            // Mark the scene as dirty
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif
    }
}
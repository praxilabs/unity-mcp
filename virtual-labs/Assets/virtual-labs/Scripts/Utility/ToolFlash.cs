using Praxilabs.CameraSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolFlash : MonoBehaviour
{
    public bool startFlash;
    [SerializeField] private float _startAlpha = 0;
    [SerializeField] private float _endAlpha = 0.5f;

    [Range(0, 1), SerializeField] private float _alpha;
    [SerializeField] private Color _flashColor = new Color(0.14f, 0.14f, 0.14f, 0);
    private List<Material> _toolMaterials = new List<Material>();

    private void Awake()
    {
        foreach (var renderer in this.GetComponentsInChildren<Renderer>())
            foreach (var material in renderer.materials)
                if (material.name.Contains("Dark_Material"))
                    _toolMaterials.Add(material);
    }

    private void Update()
    {
        if (startFlash)
        {
            _alpha = Mathf.Lerp(_startAlpha, _endAlpha, Mathf.PingPong(Time.time * 2, 0.75f));
            ChangeMaterialsAlpha();
        }
    }

    public void ResetMaterialsAlpha()
    {
        _alpha = 0;
        ChangeMaterialsAlpha();
    }

    private void ChangeMaterialsAlpha()
    {
        _flashColor.a = _alpha;

        foreach (var material in _toolMaterials)
            material.color = _flashColor;
    }
}
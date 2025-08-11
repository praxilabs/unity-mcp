using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class DeviceHoverDetector : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _deviceID;
    [SerializeField] private bool _shouldTintColorForWhite;
    [SerializeField] private List<Renderer> _devicePartsRenderers;
    [Header("Target Position Settings")]
    [SerializeField] private Vector3 _targetPosition;
    [SerializeField] private float _worldSpaceOffset;
    [Header("References"), Space]
    [SerializeField] private List<Hologram> _holograms;
    private bool _isHologramFlashing;

    public Vector3 TargetPosition
    {
        get => _targetPosition;
        set => _targetPosition = value;
    }
    
    private void OnMouseEnter()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        _isHologramFlashing = IsHologramFlashing();
            
        DeviceTooltipManager.Instance.ShowTooltip(_targetPosition, _deviceID, _worldSpaceOffset, _devicePartsRenderers, _isHologramFlashing, _shouldTintColorForWhite);
    }
    
    private void OnMouseExit()
    {
        DeviceTooltipManager.Instance.HideTooltip();
    }

    private bool IsHologramFlashing()
    {
        for(int i = 0; i < _holograms.Count; i++)
        {
            if(_holograms[i].IsFlashing)
                return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a sphere at the target position
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(_targetPosition, 0.04f);

        // Draw a line from object to target
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, _targetPosition);
    }
}
using DG.Tweening;
using LiquidVolumeFX;
using Praxilabs.Input;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LiquidVolumeHelper))]
[RequireComponent(typeof(LiquidVolumeLiquidSettings))]
public class LiquidVolumeController : MonoBehaviour
{
    private LiquidVolume _liquidVolume;
    [Tooltip("Liquid Volume script on the glass container")]
    public LiquidVolume LiquidVolume 
    { 
        get 
        { 
            if(_liquidVolume == null)
                _liquidVolume = GetComponentInChildren<LiquidVolume>();
            return _liquidVolume; 
        } 
    }

    [Tooltip("Capacity of the container in ml")]
    public float containerCapacity;

    // [Tooltip("Spill controller script doesn't have to be added if the container will not use dynamic spill")]
    // public SpillController spillController;

    private LiquidVolumeHelper _helper;
    public LiquidVolumeHelper Helper
    {
        get
        {
            if (_helper == null)
                _helper = GetComponent<LiquidVolumeHelper>();
            return _helper;
        }
        private set => _helper = value;
    }

    private LiquidVolumeLiquidSettings _liquidSettings;
    public LiquidVolumeLiquidSettings LiquidSettings
    {
        get
        {
            if (_liquidSettings == null)
                _liquidSettings = GetComponent<LiquidVolumeLiquidSettings>();
            return _liquidSettings;
        }
        private set => _liquidSettings = value;
    }

    public float liquidLevel;
    public float CurrentLiquidLevel { get => LiquidVolume.level; }

    private List<GameObject> _floatingObjects = new List<GameObject>();

    #region Move Object To Liquid Surface
    /// <summary>
    /// Moves an object to liquid surface and maintains same Y position as the liquid 
    /// Select PositionOnly to have the object be affected with turbulence and move up and down
    /// Select Simple to have the surface stay at the liquid surface without movement (ignore turbulence)
    /// </summary>
    /// <param name="objectName">Object to move to the surface</param>
    /// <param name="buoyancyEffect">Simple and PositionOnly</param>
    public void MoveObjectToLiquidSurface(string objectName, BuoyancyEffect buoyancyEffect, float xAxis, float zAxis)
    {
        GameObject Obj = GameObject.Find(objectName);
        if (Obj == null)
        {
            Debug.Log("Color=#FFA725>Couldn't find the object gameobject, will not continue</Color>");
            return;
        }

        _floatingObjects.Add(Obj);

        Obj.tag = "Untagged";

        DragDropManager.Instance.Drop(true);
        DOTween.Kill(Obj);

        // if (Obj.GetComponent<FloatingObjectSimple>())
        //     Obj.GetComponent<FloatingObjectSimple>().liquidVolume = LiquidVolume;
        // else
        // {
        //     Debug.Log("Color=#FFA725>Couldn't find FloatingObjectSimple component on the liquid container, will add it</Color>");
        //     Obj.AddComponent<FloatingObjectSimple>();
        //     Obj.GetComponent<FloatingObjectSimple>().liquidVolume = LiquidVolume;
        // }

        // Obj.GetComponent<FloatingObjectSimple>().buoyancyEffect = buoyancyEffect;

        LiquidVolume.MoveToLiquidSurface(Obj.transform, buoyancyEffect, GetComponentInChildren<LiquidVolume>().transform);

        Vector3 position;
        if (xAxis == 0 && zAxis == 0)
            position = transform.position;
        else
        {
            position = transform.position;
            position.x += xAxis;
            position.z += zAxis;
        }

        Obj.transform.position = position;
        Obj.transform.parent = this.transform;
    }

    /// <summary>
    /// Removes the object from the liquid surface and returns it to it's initial position
    /// </summary>
    /// <param name="objectName">Object name to return it to it's initial position</param>
    public void RemoveObjectFromLiquidSurface(string objectName)
    {
        for (int i = 0; i < _floatingObjects.Count; i++)
        {
            // if (_floatingObjects[i].name != objectName && !_floatingObjects[i].GetComponent<FloatingObjectSimple>())
            //     continue;

            // Destroy(_floatingObjects[i].GetComponent<FloatingObjectSimple>());
            _floatingObjects[i].transform.parent = null;
            if (_floatingObjects[i].GetComponent<DraggableObject>())
            {
                _floatingObjects[i].GetComponent<DraggableObject>().ResetPosition();
                _floatingObjects.RemoveAt(i);
            }
            else
                Debug.LogError("The object " + objectName + " doesn't have RoutineTools or DraggingObject components can't remove it from the liquid");
            break;
        }
    }
    #endregion
}
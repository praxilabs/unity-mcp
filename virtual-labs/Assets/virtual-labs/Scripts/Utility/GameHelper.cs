using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameHelper : MonoBehaviour
{
    public static GameObject GetClickedObjectWithTag(out RaycastHit hit, string tag)
    {
        GameObject target = GetClickedObject(out hit);

        if (!target)
            return null;

        if (target.CompareTag(tag))
            return target;

        return null;
    }

    public static GameObject GetClickedObject(out RaycastHit hit)
    {

        GameObject target = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction, out hit))
        {
            target = hit.collider.gameObject;
        }

        return target;
    }

    public static RaycastHit[] GetClickedObjects()
    {
        RaycastHit[] hits;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        hits = Physics.RaycastAll(ray.origin, ray.direction, 100.0F);
        return hits;
    }

    public static HashSet<int> GenerateRandomNumbers(HashSet<int> ranandomNumbers, int maxNumber)
    {
        while (ranandomNumbers.Count < maxNumber)
        {
            int num = UnityEngine.Random.Range(0, maxNumber);
            ranandomNumbers.Add(num);
        }
        return ranandomNumbers;
    }

    /// <summary>
    /// Convert Hex Color (string) into RBG Color to be used 
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    public static Color ConvertHexToColor(string hex)
    {
        hex = hex.Replace("0x", "");
        hex = hex.Replace("#", "");

        byte a = 255;

        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        if (hex.Length == 8)
        {
            a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        }
        return new Color32(r, g, b, a);
    }

    /// <summary>
    /// sent time in seconds
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - 60 * minutes;
        int milliseconds = (int)(1000 * (time - minutes * 60 - seconds));
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    public static string FormatTimeInHours(float time)
    {
        int hours = (int)time / 3600;
        int minutes = (int)((time - (hours * 3600)) / 60);
        int seconds = (int)time - (60 * minutes + 3600 * hours);
        return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }

    public static void RestartCoroutine(MonoBehaviour mb, ref Coroutine coroutine, IEnumerator routine)
    {
        if (coroutine != null)
        {
            mb.StopCoroutine(coroutine);
        }

        coroutine = mb.StartCoroutine(routine);
    }

    public static void StopAndNullifyCoroutine(MonoBehaviour mb, ref Coroutine coroutine)
    {
        if (coroutine != null)
        {
            mb.StopCoroutine(coroutine);
            coroutine = null;
        }
    }
}

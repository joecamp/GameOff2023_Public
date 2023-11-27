using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

public static class Utils
{
    public static bool Approximation(float a, float b, float tolerance = .01f)
    {
        return (Mathf.Abs(a - b) < tolerance);
    }

    public static Vector3 GetRandomAngledDirection(Vector3 direction, float angle)
    {
        Quaternion randomRotation = Quaternion.Euler(
            Random.Range(-angle, angle),
            Random.Range(-angle, angle),
            Random.Range(-angle, angle)
        );

        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(randomRotation);
        Vector3 randomDirection = rotationMatrix.MultiplyVector(direction);

        return randomDirection.normalized;
    }

    public static Quaternion GetRandomRotationYAxis()
    {
        float randomYAngle = Random.Range(0f, 360f);
        return Quaternion.Euler(0f, randomYAngle, 0f);
    }

    public static Vector2 WorldToCanvasPoint(Vector3 worldPoint, Canvas canvas, RectTransform canvasRectTransform, Camera camera)
    {
        Vector2 canvasSize = canvasRectTransform.sizeDelta;

        Vector3 screenPoint = camera.WorldToViewportPoint(worldPoint);
        Vector2 screenPoint2D = new Vector2(screenPoint.x, screenPoint.y);

        float canvasPointX = screenPoint2D.x * canvasSize.x - 0.5f * canvasSize.x;
        float canvasPointY = screenPoint2D.y * canvasSize.y - 0.5f * canvasSize.y;

        return new Vector2(canvasPointX, canvasPointY);
    }

    public static T GetCopyOf<T>(this Component comp, T other) where T : Component
    {
        System.Type type = comp.GetType();
        if (type != other.GetType()) return null; // type mis-match
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
        PropertyInfo[] pinfos = type.GetProperties(flags);
        foreach (var pinfo in pinfos)
        {
            if (pinfo.CanWrite)
            {
                try
                {
                    pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                }
                catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
            }
        }
        FieldInfo[] finfos = type.GetFields(flags);
        foreach (var finfo in finfos)
        {
            finfo.SetValue(comp, finfo.GetValue(other));
        }
        return comp as T;
    }

    public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
    {
        return go.AddComponent<T>().GetCopyOf(toAdd) as T;
    }
}
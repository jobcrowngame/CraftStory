using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class VectorTool
{
    /// <summary>
    /// angle to vetor
    /// </summary>
    /// <param angle="angle"></param>
    /// <returns></returns>
    public static Vector2 AngleToVector2(float angle)
    {
        var radian = angle * (Mathf.PI / 180);
        return new Vector2(Mathf.Sin(radian), Mathf.Cos(radian)).normalized;
    }

    /// <summary>
    /// vector to angle
    /// </summary>
    /// <param vector="vector"></param>
    /// <returns></returns>
    public static float Vector2ToAngle(Vector2 vector)
    {
        return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// get eulerAngles
    /// </summary>
    /// <param name="from"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static Vector3 GetEulerAngles(Transform from, Transform target)
    {
        Vector3 dir = target.position - from.position;
        return Vector3.zero;
    }
}
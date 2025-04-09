using UnityEngine;
// 实现贝塞尔曲线的类
public class BezierCurve
{
    /// <summary>
    /// Return a point of quadratic Bezier curve.
    /// 返回二次贝塞尔曲线上的点。
    /// </summary>
    public static Vector3 QuadraticPoint(Vector3 startPoint, Vector3 endPoint, Vector3 controlPoint, float by)
    {
        return Vector3.Lerp(
            Vector3.Lerp(startPoint, controlPoint, by), 
            Vector3.Lerp(controlPoint, endPoint, by), 
            by);
    }

    /// <summary>
    /// Return a point of cubic Bezier curve.
    /// 返回三次贝塞尔曲线上的点。
    /// </summary>
    public static Vector3 CubicPoint(Vector3 startPoint, Vector3 endPoint, Vector3 controlPointStart, Vector3 controlPointEnd, float t)
    {

        return QuadraticPoint(
            Vector3.Lerp(startPoint, controlPointStart, t), 
            Vector3.Lerp(controlPointEnd, endPoint, t),
            Vector3.Lerp(controlPointStart, controlPointEnd, t), t);
    }
}
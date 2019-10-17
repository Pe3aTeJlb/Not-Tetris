using System.Collections.Generic;
using UnityEngine;

public static class Triangulation
{
    public static List<Vector2> GetResult(List<Vector2> points)
    {
        var results = new List<Vector2>();
        var checkPoints = new List<Vector2>();
        for (int j = points.Count - 1; j >= 0; j--)
        {
            var a = points[(j + points.Count - 1) % points.Count];
            var b = points[j];
            var c = points[(j + 1) % points.Count];
            if (GetClockwiseCoef(a - b, c - b) < 0f)
                checkPoints.Add(b);
        }
        for (int j = points.Count - 1; j >= 0; j--)
        {
            var a = points[(j + points.Count - 1) % points.Count];
            var b = points[j];
            var c = points[(j + 1) % points.Count];
            if (GetClockwiseCoef(a - b, c - b) > 0f && !TriangleStrictlyAnyContains(a, b, c, checkPoints))
            {
                results.AddRange(new[] { a, b, c });
                points.RemoveAt(j);
                j = points.Count - 1;
            }
        }
        return results;
    }

    private static float GetClockwiseCoef(Vector2 v1, Vector2 v2)
    { return Mathf.Sign(v1.x * v2.y - v1.y * v2.x); }

    private static bool TriangleStrictlyAnyContains(Vector2 a, Vector2 b, Vector2 c, List<Vector2> points)
    {
        if (points.Count == 0)
            return false;
        var ky1 = (b.y - a.y);
        var ky2 = (c.y - b.y);
        var ky3 = (a.y - c.y);
        var kx1 = (b.x - a.x);
        var kx2 = (c.x - b.x);
        var kx3 = (a.x - c.x);
        for (int i = 0; i < points.Count; i++)
        {
            var point = points[i];
            var a1 = (a.x - point.x) * ky1 - kx1 * (a.y - point.y);
            var b1 = (b.x - point.x) * ky2 - kx2 * (b.y - point.y);
            var c1 = (c.x - point.x) * ky3 - kx3 * (c.y - point.y);
            if ((a1 < 0 && b1 < 0 && c1 < 0) || (a1 > 0 && b1 > 0 && c1 > 0))
                return true;
        }
        return false;
    }
}
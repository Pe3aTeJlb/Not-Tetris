using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Triangulation
{
    public static void GetResult(List<Vector3> points, bool clockwise, Vector3 upAxis, out Vector3[] verticles, out int[] triangles, out Vector2[] uvcoords)
    {
        var qto = Quaternion.FromToRotation(Vector3.up, upAxis);
        var qfrom = Quaternion.FromToRotation(upAxis, Vector3.up);
        //Triangulation
        var ps = points.Select(v => qto*v).ToArray();
        var y = ps.Select(v => v.y).Sum()/ps.Length;
        
        var pto = ps.Select(v => new Vector2(v.x, v.z)).ToList();
        var result = GetResult(pto, true);
        var pfrom = result.Select(v => new Vector3(v.x, y, v.y)).Select(v => qfrom*v).ToArray();

        //Get Verticles
        verticles = pfrom;
        triangles = new int[verticles.Length];
        for (int i = 0; i < verticles.Length; i++)
            triangles[i] = i;
        uvcoords = result.ToArray();
    }

    public static List<Vector2> GetResult(List<Vector2> points, bool clockwise)
    {
        var results = new List<Vector2>();
        var lines = points.Select((t, i) => new Line2(t, points[(i + 1) % points.Count])).ToList();

        int j = points.Count;
        while (j --> 0)
        {
            var a = points[(j + points.Count - 1) % points.Count];
            var b = points[j];
            var c = points[(j + 1) % points.Count];
            var angle = AngleStraight((a - b).normalized, (c - b).normalized);
            var notHavePointsInTriangle = points.All(x => !TriangleStrictlyContains(a, b, c, x));
            var angleCoef = clockwise ? 1 : -1;
            if (angle*angleCoef > 0f && notHavePointsInTriangle && (!Intersect(lines, a, b) && !Intersect(lines, b, c) && !Intersect(lines, a, c)))
            {
                results.AddRange(new[] {a, b, c});
                points.RemoveAt(j);
                j = points.Count;
            }
        }
        return results;
    }

    private static bool Intersect(IEnumerable<Line2> ls, Vector2 start, Vector2 end)
        { return ls.Where(x => !x.CommonFaces(new Line2(start, end))).Any(x => x.Intersect(new Line2(start, end))); }

    private static float AngleStraight(Vector2 v1, Vector2 v2)
        { return Mathf.Atan2(Vector3.Dot(Vector3.forward, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * Mathf.Rad2Deg; }

    private static bool TriangleStrictlyContains(Vector2 a, Vector2 b, Vector2 c, Vector2 point)
    {
        float a1 = (a.x - point.x) * (b.y - a.y) - (b.x - a.x) * (a.y - point.y);
        float b1 = (b.x - point.x) * (c.y - b.y) - (c.x - b.x) * (b.y - point.y);
        float c1 = (c.x - point.x) * (a.y - c.y) - (a.x - c.x) * (c.y - point.y);
        return ((a1 < 0 && b1 < 0 && c1 < 0) || (a1 > 0 && b1 > 0 && c1 > 0));
    }

    private struct Line2
    {
        private readonly Vector2 start, end;

        public Line2(Vector2 start, Vector2 end)
        {
            this.start = start;
            this.end = end;
        }

        public bool Intersect(Line2 b)
        {
            var n = (b.end - b.start).normalized;
            if (n == (end - b.start).normalized || n == (start - b.end).normalized)
                return true;

            var A1 = end.y - start.y;
            var B1 = start.x - end.x;
            var C1 = -A1 * start.x - B1 * start.y;

            var A2 = b.end.y - b.start.y;
            var B2 = b.start.x - b.end.x;
            var C2 = -A2 * b.start.x - B2 * b.start.y;

            var f1 = A1 * b.start.x + B1 * b.start.y + C1;
            var f2 = A1 * b.end.x + B1 * b.end.y + C1;
            var f3 = A2 * start.x + B2 * start.y + C2;
            var f4 = A2 * end.x + B2 * end.y + C2;

            return f1 * f2 < Mathf.Epsilon && f3 * f4 < Mathf.Epsilon; // строгое пересечение
        }

        public bool CommonFaces(Line2 b)
            { return b.start == start || b.start == end || b.end == start || b.end == end; }
    }
}
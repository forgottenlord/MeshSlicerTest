using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CalculatorB : MonoBehaviour
{
    public Transform[] corners;
    //private FlatParams flatParams;
    private Plane flatParams;
    public MeshFilter MF;
    private float height;
    public void Start()
    {
        Vector3[] verts = GetPositions(corners);
        /*Quaternion rot = MF.transform.rotation;

        Vector3[] rotatedVerts = new Vector3[verts.Length];
        for (int n = 0; n < verts.Length; n++)
        {
            rotatedVerts[n] = rot * verts[n];
        }*/
        GameObject go = new GameObject("border");
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localEulerAngles = Vector3.zero;
        DrawLineRenderer(go, verts, Color.red, 0.1f, false);


        GameObject perp = new GameObject("perp");
        perp.transform.SetParent(transform);
        perp.transform.localPosition = GetAverage(verts);
        perp.transform.localEulerAngles = Vector3.zero;
        height = CalcHeightVector(corners[1].position, corners[2].position, 1.0f / 27.0f);
        Vector3 sliceCenter = perp.transform.up * height + transform.position;
        DrawLineRenderer(perp, new Vector3[] { Vector3.zero, perp.transform.up * height }, Color.green, 0.1f, false);

        Debug.Log("flat params: "+ (flatParams = GetFlatEq(verts)));
        var line = Slice(MF);
        GameObject unSorted = new GameObject("unSorted");
        DrawLineRenderer(unSorted, line.ToArray(), Color.gray, 0.1f, false);

        GameObject SortedXY = new GameObject("SortedXY");
        DrawLineRenderer(SortedXY, line.OrderBy(v =>
        {
            /* float arc;
             if (v.z > 0)
                 arc = (float)(System.Math.Atan((double)(v.x * v.z)));
             else
                 arc = (float)(System.Math.Atan((double)(v.x * v.z)));
            Debug.Log("arc:" + arc + "   x: " + v.x + "   y: " + v.y + "   z: " + v.z);
            return arc;*/

            float dot;
            if (v.z < 0)
                dot = Vector3.Angle(-transform.right, v);
            else
                dot = -Vector3.Angle(transform.right, v);

            Debug.Log("arc:" + dot + "   x: " + v.x + "   y: " + v.y + "   z: " + v.z);
            return dot;
        }).ToArray(), Color.white, 0.1f, false);
    }

    private Vector3[] GetPositions(Transform[] transforms)
    {
        List<Vector3> positions = new List<Vector3>();
        for (int n = 0; n < transforms.Length; n++)
        {
            positions.Add(transforms[n].localPosition);
        }
        return positions.ToArray();
    }
    /// <summary>
    /// Добавляем LineRenderer.
    /// </summary>
    /// <param name="vertices"></param>
    private void DrawLineRenderer(GameObject go, Vector3[] vertices, Color color, float height, bool useWorldSpace = true)
    {
        LineRenderer LR = go.AddComponent<LineRenderer>();
        LR.material = new Material(Shader.Find("Unlit/Color"));
        LR.material.SetColor("_Color", color);
        LR.useWorldSpace = useWorldSpace;
        LR.widthCurve = new AnimationCurve(new Keyframe[]{ new Keyframe(0, height), new Keyframe(1, height) });
        LR.positionCount = vertices.Length;
        LR.SetPositions(vertices);
    }

    /// <summary>
    /// Получаем центр из массива точек.
    /// </summary>
    private Vector3 GetAverage(Vector3[] corners)
    {
        Vector3 avr = Vector3.zero;
        for (int n = 0; n < corners.Length; n++)
        {
            avr += new Vector3(corners[n].z, corners[n].y, corners[n].x);
            /*GameObject go = new GameObject();
            go.transform.SetParent(transform);
            go.transform.localPosition = corners[n];*/
        }
        return avr / corners.Length;
    }

    /// <summary>
    /// Вычисляем реальный размер эталонного отрезка.
    /// </summary>
    /// <param name="begin">начало отрезка</param>
    /// <param name="end">конец отрезка</param>
    /// <param name="factor">отношение</param>
    public float CalcHeightVector(Vector3 begin, Vector3 end, float factor)
    {
        /*Debug.Log("begin " + begin);
        Debug.Log("end " + end);
        Debug.Log("magnitude " + (begin - end).magnitude);*/
        return (Mathf.Abs((begin - end).magnitude)) * factor;
    }

    /// <summary>
    /// Построение среза по плоскости.
    /// </summary>
    /// <param name="MF"></param>
    private List<Vector3> Slice(MeshFilter MF)
    {
        Mesh mesh = MF.mesh;
        Quaternion rot = MF.transform.localRotation;
        List<Vector3> sliceVertices = new List<Vector3>();
        Vector3[] verts = mesh.vertices;
        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int i1 = (i + 0);
            int i2 = (i + 1);
            int i3 = (i + 2);

            Vector3[] vertices = new Vector3[]
            {
                rot*verts[triangles[i1]],
                rot*verts[triangles[i2]],
                rot*verts[triangles[i3]]
            };


            if (TriangleIsCrossPlane(vertices))
            {
                //sliceVertices.Add(rot * GetAverage(vertices)+new Vector3(0,height,0));
                Vector3 v = GetAverage(vertices);
                sliceVertices.Add(new Vector3(v.z,v.y,v.x));
            }
        }
        return sliceVertices;
    }


    /// <summary>
    /// Получаем уравнение плоскости. Выше или ниже плоскости.
    /// </summary>
    /*private bool GetPointRelatedOnFlat(Vector3 point)
    {
        return flatParams.a * point.x +
            flatParams.b * point.y +
            flatParams.c * point.z -
            flatParams.d > 0;
    }*/

    /// <summary>
    /// Пересекает ли треугольник искомую плоскость.
    /// Если все вершины с одной стороны плоскости - значит НЕ пересекает.
    /// </summary>
    /// <returns></returns>
    private bool TriangleIsCrossPlane(Vector3[] triangle)
    {
        //https://www.youtube.com/watch?v=6FAXjzsZIR4
        int t = 0;
        //string str = string.Empty;
        /*for (int n = 0; n < 3; n++)
        {
            t +=GetPointRelatedOnFlat(triangle[n]) ?1:0;
            //str += GetPointRelatedOnFlat(triangle[n]) ? "yes" : "no";
        }*/
        t += flatParams.SameSide(triangle[0], triangle[1]) ? 1 : 0;
        t += flatParams.SameSide(triangle[1], triangle[2]) ? 1 : 0;
        t += flatParams.SameSide(triangle[2], triangle[0]) ? 1 : 0;
        //Debug.Log(str);
        return t < 3 && t > 0;
    }

    /// <summary>
    /// Получаем коэффициенты в уравнении прямой по трём её точкам.
    /// </summary>
    /// <param name="points"></param>
    /// <returns>, ABCD</returns>
    private Plane GetFlatEq(Vector3[] p)
    {
        //x - p[0].x        y - p[0].y          z - p[0].z = 0
        //p[1].x - p[0].x   p[1].y - p[0].y     p[1].z - p[0].z = 0
        //p[2].x - p[0].x   p[2].y - p[0].y     p[2].z - p[0].z = 0

        ///Vector3 avrg = GetAverage(new Vector3[] { p[0], p[1], p[2] });

        /*float a_ = (p[1].y - p[0].y * p[2].z - p[0].z - p[2].y - p[0].y * p[1].z - p[0].z) * p[0].x;
        float b_ = (p[1].x - p[0].x * p[2].z - p[0].z - p[2].x - p[0].x * p[1].z - p[0].z) * p[0].y;
        float c_ = (p[1].x - p[0].x * p[2].y - p[0].y - p[2].x - p[0].x * p[1].y - p[0].y) * p[0].z;*/


        /*float a11 = avrg.x - p[0].x; float a12 = avrg.y - p[0].y; float a13 = avrg.z - p[0].z;
        float a21 = p[1].x - p[0].x; float a22 = p[1].y - p[0].y; float a23 = p[1].z - p[0].z;
        float a31 = p[2].x - p[0].x; float a32 = p[2].y - p[0].y; float a33 = p[2].z - p[0].z;


        Matrix3x3 matrix = new Matrix3x3(new float[] { a11, a12, a13, a21, a22, 23, a31, a32, a33 });
        float degradant = matrix.GetDeterminant();*/

        /*x1* a + y1 * b + z1 * c + d = 0;
        x2* a + y2 * b + z2 * c + d = 0;
        x3* a + y3 * b + z3 * c + d = 0;*/


        /*Matrix3x3 matrix = new Matrix3x3(new float[] {
        x - x1,  y + y1,  z - z1,
        x2 - x2, y2 + y2, z2 - z1,
        x3 - x3, y3 + y3, z3 - z1});*/

        Plane plane = new Plane(p[0], p[1], p[2]);
        //plane.Set3Points();

        return plane;

        /*return flatParams = new FlatParams()
        {
            a = a_,
            b = b_,
            c = c_,
            d = height,
        };*/
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calculator : MonoBehaviour
{
    private Vector3 temp = new Vector3(0, 0, 0);
    private Vector3 bottomright = new Vector3(7.06552431138356f, 3.152303227479417f, 0.5798208855947808f);
    private Vector3 bottomleft = new Vector3(3.520846622706104f, -7.299853716696669f, 0.852738011666962f);
    private Vector3 topleft = new Vector3(-11.26581889156483f, -2.161200109859713f, 1.420210236748185f);
    private Vector3 topright = new Vector3(-7.721422806121932f, 8.215609420586972f, 0.7073115381738011f);
    public void Start()
    {
        Vector3[] verts = new Vector3[] { bottomright, bottomleft, topleft, topright };
        Vector3 avr = GetAverage(verts);
        LineRenderer LR = gameObject.AddComponent<LineRenderer>();
        LR.material = new Material(Shader.Find("Unlit/Color"));
        LR.material.SetColor("_Color", Color.red);
        LR.useWorldSpace = false;
        LR.positionCount = verts.Length;
        LR.SetPositions(verts);
        GameObject go = new GameObject();
        go.transform.SetParent(transform);
        go.transform.localPosition = avr;
        Debug.Log(avr);
    }

    /// <summary>
    /// Получаем центр из массива точек.
    /// </summary>
    private Vector3 GetAverage(Vector3[] corners)
    {
        Vector3 avr = Vector3.zero;// corners[0];
        for (int n = 0; n < corners.Length; n++)
        {
            avr += corners[n];
            GameObject go = new GameObject();
            go.transform.SetParent(transform);
            go.transform.localPosition = corners[n];
        }
        return avr / corners.Length;
    }
}

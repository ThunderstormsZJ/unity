using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Algorithm.Bezier;
using Algorithm.Combination;

public class BezierCurve : MonoBehaviour {

    public Transform[] pointers;
    public Material lineMat;

    private List<Vector3> posList;
    private float tDelta = 0.02f;
    private LineRenderer LR;
    private bool isDraw = false;

    private void Awake()
    {
        LR = GetComponent<LineRenderer>();
        Debug.Log("Awake");
    }

    // Use this for initialization
    void Start () {
        CachePosList();
	}

	
	// Update is called once per frame
	void Update () {
		
	}

    private void CachePosList()
    {
        posList = new List<Vector3>();
        foreach (Transform pointer in pointers)
        {
            posList.Add(pointer.position);
        }
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(50, 0, 100, 20), "测试1"))
        {
            //new CombinationUtils().test();
            Debug.Log(BezierUtils.Draw4JBezierCurve(posList[0], posList[1], posList[2], posList[3], 0.2f));
            Debug.Log(BezierUtils.DrawNJBezierCurve(posList.GetRange(0, 4).ToArray(), 0.2f));
            Debug.Log(BezierUtils.DrawCatmullRomByBezier(posList[0], posList[1], posList[2], posList[3], 0.2f));
            Debug.Log(BezierUtils.DrawCatmullRom(posList[0], posList[1], posList[2], posList[3], 0.2f));
        }
        if (GUI.Button(new Rect(50, 30, 100, 20), "测试2"))
        {
            new CombinationUtils().test2();
        }

        if (GUI.Button(new Rect(50, 60, 100, 20), "划线"))
        {
            LR.positionCount = 2;
            LR.SetPosition(0, posList[0]);
            LR.SetPosition(1, posList[posList.Count - 1]);
        }
    }

    private void OnDrawGizmos()
    {
        CachePosList();
        DrawNJLine();
    }

    private void OnDrawGizmosSelected()
    {
        //Draw3JLine();
    }

    private void OnRenderObject()
    {
        DrawLineByGL(posList[0], posList[posList.Count - 1]);
    }

    private void DrawNJLine()
    {
        Gizmos.color = Color.black;
        Vector3 prev = posList[0];
        for (float dist = 0; dist <= 1; dist += tDelta)
        {
            Vector3 next = BezierUtils.DrawNJBezierCurve(posList.ToArray(), dist);
            Gizmos.DrawLine(prev, next);
            prev = next;
        }

        Gizmos.DrawLine(prev, BezierUtils.DrawNJBezierCurve(posList.ToArray(), 1));
    }

    private void Draw3JLine()
    {
        
        Gizmos.color = Color.black;
        Vector3 prev = posList[0];
        for (float dist = 0; dist <= 1; dist += tDelta)
        {
            Vector3 next = BezierUtils.Draw3JBezierCurve(posList[0], posList[1], posList[2], dist);
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
        Gizmos.DrawLine(prev, BezierUtils.Draw3JBezierCurve(posList[0], posList[1], posList[2], 1));
    }

    // 2D划线
    private void DrawLineByGL(Vector3 start, Vector3 end)
    {
        if (!isDraw) return;
        if (!lineMat)
        {
            Debug.LogError("Please Assign a material on the inspector");
            return;
        }
        GL.PushMatrix();//保存当前Matirx
        lineMat.SetPass(0);//刷新当前材质
        //GL.LoadOrtho();//设置pixelMatrix
        GL.Color(Color.red);
        GL.Begin(GL.LINES);

        GL.Vertex3(start.x, start.y, start.z);
        GL.Vertex3(end.x, end.y, end.z);

        GL.End();
        GL.PopMatrix();//读取之前的Matrix
    }

    private static ulong CalcCombinationNumber(int n, int k)
    {
        ulong[] result = new ulong[n + 1];
        for (int i = 1; i <= n; i++)
        {
            result[i] = 1;
            for (int j = i - 1; j >= 1; j--)
                result[j] += result[j - 1];
            result[0] = 1;
        }
        return result[k];
    }
}

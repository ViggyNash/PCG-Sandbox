using UnityEngine;
using System.Collections;
using System;

public class RandSpline : MonoBehaviour {

    private BezierSpline spline;

    public bool randomSegmentNum;
    public float segments;

    public bool allowHeight;
    public float splineIntervalScaling;
    public float splineRadius;
    public float lengthScaling;
    public float heightScaling;

    public int frequency;
    public bool lookForward;
    public Transform[] items;

    public Transform[] list;

    public bool fixedSeed;
    public int seed;

    // Use this for initialization
    void Awake()
    {
        if (!allowHeight)
        {
            heightScaling = 0;
        }
        if (fixedSeed)
        {
            UnityEngine.Random.seed = seed;
        }
        else Debug.Log("Seed = " + UnityEngine.Random.seed);

        spline = GetComponent<BezierSpline>();

        SetPoints();

        spline.Loop = true;

        AddRoads();

    }

    private void SetPoints()
    {
        if (randomSegmentNum)
        {
            segments = Mathf.Floor(UnityEngine.Random.Range(0, 4));
        }
        Debug.Log("Num segments: " + segments);

        while (segments > 0)
        {
            spline.AddCurve();
            segments--;
        }

        Vector3 randPoint;
        int splineLength = spline.points.Length;
        for (int i = 0; i < splineLength; i++)
        {
            Vector3 pos = UnityEngine.Random.insideUnitSphere * splineRadius;
            pos = (pos / 2) + splineRadius / 2 * Vector3.ProjectOnPlane(pos, Vector3.up);
            /*randPoint = pos + new Vector3(UnityEngine.Random.Range(-splineIntervalScaling, splineIntervalScaling)
                                    , 0 , -splineIntervalScaling * i);*/
            randPoint = pos + splineIntervalScaling * new Vector3(Mathf.Sin(Mathf.PI * 2 * ((float)i / splineLength)), 0
                                                                 , Mathf.Cos(Mathf.PI * 2 * ((float)i / splineLength)));
            Debug.Log(((float)i / splineLength) + " " + splineIntervalScaling * new Vector3(Mathf.Sin(Mathf.PI * 2 * (i / splineLength)), 0
                                                                 , Mathf.Cos(Mathf.PI * 2 * (i / splineLength))));
            randPoint.y = randPoint.y * heightScaling;
            spline.points[i] = randPoint;
            spline.SetControlPointMode(i, BezierControlPointMode.Mirrored);
        }
    }

    Vector3[] vertices;
    int[] triangles;
    public Vector3[] points;

    private void AddRoads()
    {
        if (frequency <= 0 || items == null || items.Length == 0)
        {
            return;
        }
        float stepSize = frequency * items.Length;
        if (spline.Loop || stepSize == 1)
        {
            stepSize = 1f / stepSize;
        }
        else
        {
            stepSize = 1f / (stepSize - 1);
        }
        points = new Vector3[frequency];
        for (int p = 0, f = 0; f < frequency; f++)
        {
            points[f] = spline.GetPoint(f * stepSize);
            points[f + 1] = spline.GetPoint(f * stepSize);
            

            //GameObject loc = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube));
            //loc.transform.position = position;
        }
    }

    private class Segment
    {
        Triangle[] triangles;

        public Segment(Vector3 start, Vector3 end)
        {

        }
    }

    private class Triangle
    {
        Vector3[] vertices = new Vector3[3];

        public Triangle(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            vertices[0] = p1;
            vertices[1] = p2;
            vertices[2] = p3;
        }
    }
	
}

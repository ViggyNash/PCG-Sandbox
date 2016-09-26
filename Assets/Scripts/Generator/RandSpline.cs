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
            //Debug.Log(((float)i / splineLength) + " " + splineIntervalScaling * new Vector3(Mathf.Sin(Mathf.PI * 2 * (i / splineLength)), 0
            //                                                     , Mathf.Cos(Mathf.PI * 2 * (i / splineLength))));
            randPoint.y = randPoint.y * heightScaling;
            spline.points[i] = randPoint;
            spline.SetControlPointMode(i, BezierControlPointMode.Mirrored);
        }
    }

    Vector3[] vertices;
    int[] triangles;
    public Vector3[] points;

    public float segmentWidth;

    private void AddRoads()
    {
        if (frequency <= 0)
        {
            return;
        }
        float stepSize = frequency;
        if (spline.Loop || stepSize == 1)
        {
            stepSize = 1f / stepSize;
        }
        else
        {
            stepSize = 1f / (stepSize - 1);
        }
        points = new Vector3[frequency];
        Segment[] segments = new Segment[frequency - 1];
        Segment newSegment;
        for (int f = 0; f < frequency - 1; f++)
        {
            points[f] = spline.GetPoint(f * stepSize);
            points[f + 1] = spline.GetPoint((f + 1) * stepSize);

            newSegment = new Segment(points[f], points[f + 1]
                                    , spline.GetVelocity(f * stepSize), spline.GetVelocity((f + 1) * stepSize)
                                    , segmentWidth);

            //GameObject loc = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube));
            //loc.transform.position = position;
        }

        //newSegment = new Segment(points[frequency], points[0]);
    }

    private class Segment
    {
        Triangle[] triangles;
        Vector3 orthogonalStart;
        Vector3 orthogonalEnd;

        //Creates a 4 triangle, 6 point segment
        public Segment(Vector3 start, Vector3 end, Vector3 vectorStart, Vector3 vectorEnd, float segmentWidth)
        {
            triangles = new Triangle[4];

            start = Vector3.ProjectOnPlane(start, Vector3.up);
            end = Vector3.ProjectOnPlane(end, Vector3.up);

            orthogonalStart = Vector3.Cross(vectorStart, Vector3.up).normalized * segmentWidth;
            orthogonalEnd = Vector3.Cross(vectorEnd, Vector3.up).normalized * segmentWidth;

            triangles[0] = new Triangle(orthogonalStart, orthogonalEnd, start);
            triangles[1] = new Triangle(start, orthogonalEnd, end);
            triangles[2] = new Triangle(start, end, -orthogonalEnd);
            triangles[3] = new Triangle(start, -orthogonalEnd, -orthogonalStart);
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

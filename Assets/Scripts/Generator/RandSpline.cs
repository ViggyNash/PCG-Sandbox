using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RandSpline : MonoBehaviour {

    private BezierSpline spline;
    private MeshFilter mesh;

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
        mesh = GetComponent<MeshFilter>();

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

        vertices = new Vector3[3 * (frequency)];
        triangles = new int[12 * (frequency - 1)];
        points = new Vector3[frequency];
        Segment[] segments = new Segment[frequency];
        Segment newSegment;
        for (int f = 0; f < frequency - 1; f++)
        {
            points[f] = spline.GetPoint(f * stepSize);
            points[f + 1] = spline.GetPoint((f + 1) * stepSize);

            newSegment = new Segment(points[f], points[f + 1]
                                    , spline.GetVelocity(f * stepSize), spline.GetVelocity((f + 1) * stepSize)
                                    , segmentWidth);
            segments[f] = newSegment;

            //GameObject loc = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube));
            //loc.transform.position = position;
        }

        initMeshData(segments);
        Mesh newMesh = new Mesh();
        newMesh.name = "Road Mesh";
        newMesh.vertices = vertices;
        for (int i = 0; i < triangles.Length; i++)
        {
            Debug.Log(i % 12 + " " + triangles[i]);
            if (i % 12 == 11)
            {
                Debug.Log(" ");
            }
        }

        Debug.Log(vertices.Length);
        newMesh.triangles = triangles;

        mesh.mesh = newMesh;
    }

    //Populates "vertices" and "triangles" arrays to prepare for sending to Mesh API
    private void initMeshData(Segment[] segments){
        vertices[0] = segments[0].orthogonalStartPlus;
        vertices[1] = segments[0].start;
        vertices[2] = segments[0].orthogonalStartMinus;

        int length = segments.Length - 2;
        int j;
        for (int i = 0; i < length; i++)
        {
            j = ((i) * 3);

            //Debug.Log(i * 3 + " " + vertices.Length);
            vertices[3 + j] = segments[i].orthogonalEndPlus;
            vertices[4 + j] = segments[i].end;
            vertices[5 + j] = segments[i].orthogonalEndMinus;

            j = ((i + 1) * 3);

            triangles[i * 12] = j - 3;
            triangles[1 + (i * 12)] = j - 2;
            triangles[2 + (i * 12)] = j;
            triangles[3 + (i * 12)] = j - 2;
            triangles[4 + (i * 12)] = j;
            triangles[5 + (i * 12)] = j + 1;
            triangles[6 + (i * 12)] = j - 2;
            triangles[7 + (i * 12)] = j + 1;
            triangles[8 + (i * 12)] = j + 2;
            triangles[9 + (i * 12)] = j - 2;
            triangles[10 + (i * 12)] = j - 1;
            triangles[11 + (i * 12)] = j + 2;
        }

        j = vertices.Length - 3;

        triangles[length * 12] =  - 3;
        triangles[1 + (length * 12)] = j - 2;
        triangles[2 + (length * 12)] = j;
        triangles[3 + (length * 12)] = j - 2;
        triangles[4 + (length * 12)] = j;
        triangles[5 + (length * 12)] = j + 1;
        triangles[6 + (length * 12)] = j - 2;
        triangles[7 + (length * 12)] = j + 1;
        triangles[8 + (length * 12)] = j + 2;
        triangles[9 + (length * 12)] = j - 2;
        triangles[10 + (length * 12)] = j - 1;
        triangles[11 + (length * 12)] = j + 2;

    }

    private class Segment
    {
        public Triangle[] triangles;
        public Vector3 start;
        public Vector3 end;
        public Vector3 orthogonalStartPlus;
        public Vector3 orthogonalEndPlus;
        public Vector3 orthogonalStartMinus;
        public Vector3 orthogonalEndMinus;

        //Creates a 4 triangle, 6 point segment
        public Segment(Vector3 start, Vector3 end, Vector3 vectorStart, Vector3 vectorEnd, float segmentWidth)
        {
            triangles = new Triangle[4];

            this.start = start;
            this.end = end;

            vectorStart = Vector3.ProjectOnPlane(vectorStart, Vector3.up);
            vectorEnd = Vector3.ProjectOnPlane(vectorEnd, Vector3.up);

            this.orthogonalStartPlus = Vector3.Cross(vectorStart, Vector3.up).normalized * segmentWidth;
            this.orthogonalEndPlus = Vector3.Cross(vectorEnd, Vector3.up).normalized * segmentWidth;

            this.orthogonalStartMinus = Vector3.Cross(Vector3.up, vectorStart).normalized * segmentWidth;
            this.orthogonalEndMinus = Vector3.Cross(Vector3.up, vectorEnd).normalized * segmentWidth;

            triangles[0] = new Triangle(orthogonalStartPlus, orthogonalEndPlus, start);
            triangles[1] = new Triangle(start, orthogonalEndPlus, end);
            triangles[2] = new Triangle(start, end, orthogonalEndMinus);
            triangles[3] = new Triangle(start, orthogonalEndMinus, orthogonalStartMinus);
        }
    }

    private class Triangle
    {
        public Vector3[] vertices = new Vector3[3];

        public Triangle(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            vertices[0] = p1;
            vertices[1] = p2;
            vertices[2] = p3;
        }
    }
	
}

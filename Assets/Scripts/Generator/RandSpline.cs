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
        //Determine control point locations
       /* for (int i = 0; i < splineLength; i++)
        {
            if (i % 3 == 0)
            {
                Vector3 pos = UnityEngine.Random.insideUnitSphere * splineRadius;
                randPoint = pos + splineIntervalScaling * new Vector3(Mathf.Sin(Mathf.PI * 2 * ((float)i / splineLength)), 0
                                                                 , Mathf.Cos(Mathf.PI * 2 * ((float)i / splineLength)));
                spline.points[i] = randPoint;
            }
            else if (i % 3 == 1)
            {
                Vector3 relDir = Vector3.Cross(Vector3.up, spline.points[i - 1]).normalized;
                Quaternion offset = Quaternion.AngleAxis((float)UnityEngine.Random.Range(-85, 85), Vector3.up);
                Vector3 pos = (offset * relDir).normalized;
                //Vector3 pos = UnityEngine.Random.insideUnitSphere * splineRadius;
            }
        }*/

        for (int i = 0; i < splineLength; i++)
        {
            Vector3 pos = UnityEngine.Random.insideUnitSphere * splineRadius;
            pos = (pos / 2) + splineRadius / 2 * Vector3.ProjectOnPlane(pos, Vector3.up);
            randPoint = pos + new Vector3(UnityEngine.Random.Range(-splineIntervalScaling, splineIntervalScaling)
                                    , 0 , -splineIntervalScaling * i);
            randPoint = pos + splineIntervalScaling * new Vector3(Mathf.Sin(Mathf.PI * 2 * ((float)i / splineLength)), 0
                                                                 , Mathf.Cos(Mathf.PI * 2 * ((float)i / splineLength)));
            randPoint.y = randPoint.y * heightScaling;
            spline.points[i] = randPoint;
            spline.SetControlPointMode(i, BezierControlPointMode.Mirrored);
        }
    }

    public Vector3[] vertices;
    public int[] triangles;

    public Vector3[] points;
    public Vector3[] vectors;

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

        //Initialize arrays
        vertices = new Vector3[3 * frequency];
        triangles = new int[12 * frequency];
        points = new Vector3[frequency];
        vectors = new Vector3[frequency];
        Segment[] segments = new Segment[frequency];
        Segment newSegment;

        //Define segments by start and end points
        points[0] = spline.GetPoint(0);
        vectors[0] = spline.GetVelocity(0);
        for (int f = 1; f < frequency; f++)
        {
            points[f] = spline.GetPoint(f * stepSize);
            vectors[f] = spline.GetVelocity(f * stepSize);

            newSegment = new Segment(points[f - 1], points[f], vectors[f - 1], vectors[f], segmentWidth);
            segments[f - 1] = newSegment;

            //GameObject loc = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube));
            //loc.transform.position = position;
        }

        initMeshData(segments);
        Mesh newMesh = new Mesh();
        newMesh.name = "Road Mesh";
        newMesh.vertices = vertices;
        /*for (int i = 0; i < triangles.Length; i++)
        {
            Debug.Log(i % 12 + " " + triangles[i]);
            if (i % 12 == 11)
            {
                Debug.Log(" ");
            }
        }*/

        Debug.Log(vertices.Length);
        newMesh.triangles = triangles;

        mesh.mesh = newMesh;
    }

    //Populates "vertices" and "triangles" arrays to prepare for sending to Mesh API
    private void initMeshData(Segment[] segments)
    {

        vertices[0] = segments[0].points[0];
        vertices[1] = segments[0].points[1];
        vertices[2] = segments[0].points[2];

        for (int i = 0; i < segments.Length - 1; i++)
        {
            for (int j = 3; j < 6; j++)
            {
                //Debug.Log((i + j) + " " + segments[i].points[j]);
                vertices[(i * 3) + j] = segments[i].points[j];
            }
            for (int j = 0; j < 4; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    triangles[(i * 12) + (j * 3) + k] = segments[i].triangles[j].indices[k] + (i * 3);
                }
            }
        }

        int baseIndex = vertices.Length - 3 ;

        triangles[triangles.Length - 12] = baseIndex;
        triangles[triangles.Length - 11] = 0;
        triangles[triangles.Length - 10] = baseIndex + 1;
        triangles[triangles.Length - 9] = baseIndex + 1;
        triangles[triangles.Length - 8] = 0;
        triangles[triangles.Length - 7] = 1;
        triangles[triangles.Length - 6] = baseIndex + 1;
        triangles[triangles.Length - 5] = 1;
        triangles[triangles.Length - 4] = 2;
        triangles[triangles.Length - 3] = baseIndex + 1;
        triangles[triangles.Length - 2] = 2;
        triangles[triangles.Length - 1] = baseIndex + 2;

    }

    private class Segment
    {
        public Triangle[] triangles = new Triangle[4];
        public Vector3[] points = new Vector3[6];

        //Creates a 4 triangle, 6 point segment
        public Segment(Vector3 start, Vector3 end, Vector3 vectorStart, Vector3 vectorEnd, float segmentWidth)
        {
            this.points[0] = Vector3.Cross(vectorStart, Vector3.up).normalized * segmentWidth + start;
            this.points[1] = start;
            this.points[2] = Vector3.Cross(Vector3.up, vectorStart).normalized * segmentWidth + start;
            this.points[3] = Vector3.Cross(vectorEnd, Vector3.up).normalized * segmentWidth + end;
            this.points[4] = end;
            this.points[5] = Vector3.Cross(Vector3.up, vectorEnd).normalized * segmentWidth + end;

            this.triangles[0] = new Triangle(0, 3, 1);
            this.triangles[1] = new Triangle(1, 3, 4);
            this.triangles[2] = new Triangle(1, 4, 5);
            this.triangles[3] = new Triangle(1, 5, 2);
        }
    }

    private class Triangle
    {
        public int[] indices = new int[3];

        public Triangle(int p1, int p2, int p3)
        {
            indices[0] = p1;
            indices[1] = p2;
            indices[2] = p3;
        }
    }
	
}

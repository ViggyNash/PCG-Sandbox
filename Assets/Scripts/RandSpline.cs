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

        list = AddRoads();

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

    private Transform[] AddRoads()
    {
        if (frequency <= 0 || items == null || items.Length == 0)
        {
            return null;
        }
        float stepSize = frequency * items.Length;
        if (spline.Loop || stepSize == 1)
        {
            stepSize = 1f / stepSize;
        }
        else {
            stepSize = 1f / (stepSize - 1);
        }
        Transform[] segmentList = new Transform[spline.points.Length];
        
        for (int p = 0, f = 0; f < frequency; f++)
        {
            for (int i = 0; i < items.Length; i++, p++)
            {
                Transform item = Instantiate(items[i]) as Transform;
                Vector3 position = spline.GetPoint(p * stepSize);
                item.transform.rotation = Quaternion.LookRotation(spline.GetVelocity(p * stepSize));
                item.localScale -= new Vector3(0, 0, (spline.GetVelocity(p * stepSize).magnitude * lengthScaling) * item.localScale.x);
                //Debug.Log((spline.GetVelocity(p * stepSize).magnitude * lengthScaling) * item.localScale.x);
                item.transform.localPosition = position;
                if (lookForward)
                {
                    item.transform.LookAt(position + spline.GetDirection(p * stepSize));
                }
                item.transform.parent = transform;
                segmentList[i] = item;
            }
        }
        return segmentList;
    }

	
}

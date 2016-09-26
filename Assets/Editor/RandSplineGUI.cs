using UnityEngine;
using UnityEditor;
using NUnit.Framework;

[CustomEditor(typeof(RandSpline))]
public class RandSplineGUI : Editor{

    private RandSpline spline;

    public void OnSceneGUI()
    {
        spline = target as RandSpline;


    }
}

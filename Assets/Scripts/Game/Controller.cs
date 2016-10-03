using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {

    Rigidbody rbody;
    public float forceScale;

    public Transform camera;

    public GameObject road;
    Vector3 startPos;
    RandSpline spline;

	// Use this for initialization
	void Start () {
        spline = road.GetComponent<RandSpline>();
        startPos = spline.points[0] + Vector3.up * 2;
        transform.position = startPos;

        rbody = GetComponent<Rigidbody>();
	}

    public void Reset()
    {
        rbody.velocity = Vector3.zero;
        transform.position = startPos;
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        if (Input.GetKey(KeyCode.UpArrow))
        {
            rbody.AddForce(Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up) * forceScale);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rbody.AddForce(-Vector3.ProjectOnPlane(camera.transform.right, Vector3.up) * forceScale);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rbody.AddForce(Vector3.ProjectOnPlane(camera.transform.right, Vector3.up) * forceScale);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            rbody.AddForce(-Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up) * forceScale * 2);
        }
    }


}

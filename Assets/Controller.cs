using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {

    Rigidbody rbody;
    public float forceScale;

    public GameObject road;
    RandSpline spline;

	// Use this for initialization
	void Start () {
        spline = road.GetComponent<RandSpline>();
        transform.position = spline.list[0].transform.position + Vector3.up * 2;

        rbody = GetComponent<Rigidbody>();
	}

    public void Reset()
    {
        rbody.velocity = Vector3.zero;
        transform.position = spline.list[0].transform.position + Vector3.up * 2;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rbody.AddForce(Vector3.forward * forceScale);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rbody.AddForce(Vector3.left * forceScale);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rbody.AddForce(Vector3.right * forceScale);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            rbody.AddForce(Vector3.back * forceScale);
        }
    }


}

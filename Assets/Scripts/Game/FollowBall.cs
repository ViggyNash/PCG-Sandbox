using UnityEngine;
using System.Collections;

public class FollowBall : MonoBehaviour {

    public GameObject actor;
    public GameObject road;

    public float followDistance;
    public float verticalOffset;

	// Use this for initialization
	void Start () {
        Vector3 offset = road.GetComponent<RandSpline>().points[0]
                        - road.GetComponent<RandSpline>().spline.GetVelocity(0).normalized * followDistance
                        + Vector3.up * verticalOffset;

        transform.position = actor.transform.position + offset;

	}
	
	// Update is called once per frame
    Vector3 distance, offset;
	void FixedUpdate () {
        distance = transform.position - actor.transform.position;
        distance.y = 1f;
        offset = distance.normalized * followDistance;
        //offset = Vector3.back * 3f;

        transform.position = Vector3.Lerp(transform.position, actor.transform.position + offset, .2f);
        transform.LookAt(actor.transform);
    }
}

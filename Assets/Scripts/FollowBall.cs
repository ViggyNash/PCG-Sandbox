using UnityEngine;
using System.Collections;

public class FollowBall : MonoBehaviour {

    public GameObject actor;

	// Use this for initialization
	void Start () {
        transform.position = actor.transform.position + Vector3.up * 2 + Vector3.back * 3;

	}
	
	// Update is called once per frame
    Vector3 distance, offset;
	void FixedUpdate () {
        distance = transform.position - actor.transform.position;
        distance.y = 1f;
        offset = distance.normalized * 3f;
        //offset = Vector3.back * 3f;

        transform.position = Vector3.Lerp(transform.position, actor.transform.position + offset, .5f);
        transform.LookAt(actor.transform);
    }
}

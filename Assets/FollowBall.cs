using UnityEngine;
using System.Collections;

public class FollowBall : MonoBehaviour {

    public GameObject actor;

	// Use this for initialization
	void Start () {
        transform.position = actor.transform.position + Vector3.up + Vector3.back * 3;

	}
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.position = Vector3.Lerp(transform.position, actor.transform.position + Vector3.up + Vector3.back * 3, .1f);
        transform.LookAt(actor.transform);
    }
}

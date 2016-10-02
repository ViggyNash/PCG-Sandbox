using UnityEngine;
using System.Collections;

public class LookAtPosition : MonoBehaviour {

    public Transform sphere;
    public float verticalOffset;

	// Use this for initialization
	void Start () {
        transform.position = sphere.position + Vector3.up * verticalOffset;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.position = sphere.position + Vector3.up * verticalOffset;
    }
}

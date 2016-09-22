using UnityEngine;
using System.Collections;

public class Reset : MonoBehaviour {

	// Use this for initialization
	void OnTriggerEnter(Collider collider)
    {
        collider.gameObject.GetComponent<Controller>().Reset();
        Debug.Log("ball reset");
    }
}

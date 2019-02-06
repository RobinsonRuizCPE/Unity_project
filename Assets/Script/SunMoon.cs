using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunMoon : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.LookAt(Vector3.zero);
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(this.transform.position, Vector3.right, 10.0f * Time.deltaTime);
        
        
	}
}

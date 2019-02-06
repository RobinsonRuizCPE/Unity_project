using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daynnight : MonoBehaviour {
    public Light _light;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        _light.transform.Rotate(Mathf.Sin(Time.time)*360.0f, 0.0f, 0.0f);
	}
}

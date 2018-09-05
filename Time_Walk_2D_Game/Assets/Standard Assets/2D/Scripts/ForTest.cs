using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForTest : MonoBehaviour {
public float distancePerSecond;
	// Use this for initialization
	void Start () {
	}
	// Update is called once per frame
	void Update () {
		transform.Translate(distancePerSecond*Time.deltaTime,0,0);
	}

}

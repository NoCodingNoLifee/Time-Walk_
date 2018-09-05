using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
public GameObject player;
	// Use this for initialization
	void Start () {
		transform.position=player.transform.position-Vector3.right*1f;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

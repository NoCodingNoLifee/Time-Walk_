using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyEnemy : MonoBehaviour {

	[SerializeField] private Transform target;
	[SerializeField] private Transform enemy;
	[SerializeField] private float move_speed=25f;
	[SerializeField] private float vision=50f;
	[SerializeField] private float look_dir;

	void Awake()
	{
		look_dir=(target.position.x - enemy.position.x + vision);
	}

	void FixedUpdate()
	{

		if (target.position.x<=look_dir)
		{
			enemy.position+=gameObject.transform.forward*move_speed*Time.deltaTime;
		}
	}
}

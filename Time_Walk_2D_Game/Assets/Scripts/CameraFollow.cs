using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour 
{
	public float xMargin;	
	public float yMargin;		
	public float xSmooth;		
	public float ySmooth;
	public float xPosOffset;
	private float dirOffset;
	private float targetX = 0;
	private float nowTargetX = 0; 
	//public Vector2 maxXAndY;		
	//public Vector2 minXAndY;		


	private Transform player;

	void Awake()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
	}


	bool CheckXMargin()
	{
		return Mathf.Abs(transform.position.x - player.position.x) > xMargin;
	}


	bool CheckYMargin()
	{
		return Mathf.Abs(transform.position.y - player.position.y) > yMargin;
	}


	void FixedUpdate()
	{
		TrackPlayer();
	}
	
	
	void TrackPlayer()
	{
		float targetX = transform.position.x;
		float targetY = transform.position.y;
		//localScale
		if (player.localScale.x < 0) { dirOffset = -xPosOffset; }
		else { dirOffset = xPosOffset; }

		//if(CheckXMargin())
			targetX = Mathf.Lerp(transform.position.x + dirOffset, player.position.x, xSmooth * Time.deltaTime);

		//if(CheckYMargin())
			targetY = Mathf.Lerp(transform.position.y, player.position.y, ySmooth * Time.deltaTime);


		/*targetX = Mathf.Clamp(targetX, minXAndY.x, maxXAndY.x);
		targetY = Mathf.Clamp(targetY, minXAndY.y, maxXAndY.y);*/
		transform.position = new Vector3(targetX, targetY, transform.position.z);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hero : MonoBehaviour {

Rigidbody2D rb;
 public int Life=100;
	// Use this for initialization
	void Start () {
	Debug.Log("The game is started");
	rb=GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
		{
			JumpUp();		
		}

		if (Input.GetAxis("Horizontal")!=0)
		Flip();

		if (Life==0)
		{
			Invoke("ReloadFuckingLevel",1);
		}
	}

	void FixedUpdate()
	{
		rb.velocity=new Vector2(Input.GetAxis("Horizontal")*12f, rb.velocity.y);
	}

	void JumpUp()
	{
		rb.AddForce(transform.up*14f,ForceMode2D.Impulse);
	}

	void Flip()
	{
		if (Input.GetAxis("Horizontal")>0)
		transform.localRotation=Quaternion.Euler(0,0,0);

		if (Input.GetAxis("Horizontal")<0)
		transform.localRotation=Quaternion.Euler(0,180,0);
	}

	void ReloadFuckingLevel()
	{
		Application.LoadLevel(Application.loadedLevel);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
	if (other.gameObject.tag=="givelife")
		{
			Life+=25;
			Destroy(other.gameObject);
		}	
}
void OnTriggerStay2D(Collider2D other)
{
	if (other.gameObject.tag=="Enemy")
		{
			Life-=25;
		}	
}



	/*void OnCollisionEnter2D(Collision2D other) 666
	{
		if (other.gameObject.tag!="ground")
		{
			Debug.Log("Ground");
		}
	}*/

	void OnGUI()
	{
		GUI.Box(new Rect(0,0,100,30),"Life = "+ Life);
	}
}

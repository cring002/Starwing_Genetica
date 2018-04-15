﻿using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public GameObject target;      

	Vector3 offset = new Vector3(0, 20,  30);
	Quaternion rotationOffset = Quaternion.Euler(15,180,0);

	void Start () 
	{
	}


	void FixedUpdate () 
	{
		Vector3 velocity = target.GetComponent<Rigidbody>().velocity;
		Vector3 targetVec = offset+(offset*(velocity.magnitude/1000));
		targetVec.z += offset.z*(velocity.magnitude/2000);//Extra Z for more feel of speed
		transform.localPosition = Vector3.SmoothDamp (transform.localPosition, targetVec, ref velocity, 0.01f);

		transform.rotation = target.transform.rotation * rotationOffset;
	}

}
	
	

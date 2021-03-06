﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
A script to be attached to drawn objects.
*/
public class ObjectController : MonoBehaviour
{
	public float totalLifetime; //set in Inspector: in seconds
	GameObject arrow;
	float screenTop;
	GameObject camera;
	Vector3 actualMassCenter;
	float startTime;
    // Start is called before the first frame update
    void Start()
    {
		camera = GameObject.FindGameObjectsWithTag("MainCamera")[0];
        arrow = this.gameObject.transform.Find("OffscreenArrow").gameObject;
		arrow.GetComponent<SpriteRenderer>().enabled = false;
		screenTop = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y * 0.80f;
		startTime = Time.time;
    }

    void Update()
    {
		//check if and where the offscreen arrow should be shown
		actualMassCenter = this.gameObject.transform.TransformPoint(this.GetComponent<Rigidbody2D>().centerOfMass);
        arrow.transform.position = new Vector3(actualMassCenter.x, screenTop, -2f);
		arrow.transform.rotation = Quaternion.EulerAngles(0,0,0);
		if (actualMassCenter.y >= screenTop){
			arrow.GetComponent<SpriteRenderer>().enabled = true;
		} else {
			arrow.GetComponent<SpriteRenderer>().enabled = false;
		}
		
		//check if lifetime has expired
		if (Time.time - startTime >= totalLifetime){
			Disappear();
		}
    }
	
	public void Disappear(){
		Destroy(this.gameObject);
	}
}

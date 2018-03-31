using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movetest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
    }
    Vector3 Speed = new Vector3(10,0,0),SpeedDown = new Vector3(0,0,0);
    // Update is called once per frame
    void Update () {
        Vector3 NextPosition = transform.position + Speed * Time.deltaTime - SpeedDown * Time.deltaTime;
        SpeedDown.y += 1*(Time.deltaTime);
        NextPosition.y -= 0.5F *1* (Time.deltaTime)* (Time.deltaTime);
        transform.position = Vector3.Slerp(transform.position, NextPosition, 1);
    }
}

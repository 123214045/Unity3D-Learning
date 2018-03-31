using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon_Move : MonoBehaviour {
    public Transform origin;
    public float speed = 4;
    float ry, rx;
    // Use this for initialization  
    void Start()
    {
        rx = Random.Range(10, 60);
        ry = Random.Range(10, 60);
    }

    // Update is called once per frame  
    void Update()
    {
        this.transform.RotateAround(origin.position, new Vector3(0, rx, ry), speed * Time.deltaTime);
    }
}

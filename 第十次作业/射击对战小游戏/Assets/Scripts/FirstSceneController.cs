using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstSceneController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Instantiate(Resources.Load<GameObject>("Prefabs/Plane"), new Vector3(0, -0.5f, 0), Quaternion.identity);
    }
    // Update is called once per frame
    void Update()
    {

    }
}

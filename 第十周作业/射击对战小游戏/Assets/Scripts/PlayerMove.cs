using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMove : NetworkBehaviour
{

	public GameObject bulletPrefab;
    private Camera myCamera;

    public override void OnStartLocalPlayer()
	{
		GetComponent<MeshRenderer>().material.color = Color.red;
        GetComponent<Rigidbody>().freezeRotation = true;
        myCamera = Camera.main;
    }

    void Update()
	{
		if (!isLocalPlayer)
			return;

        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical") * 0.1f;
        //移动和旋转
        transform.Translate(0, 0, z * 0.7f);
        transform.Rotate(0, x, 0);
        myCamera.transform.position = transform.position + transform.forward * -3 + new Vector3(0, 4, 0);
        myCamera.transform.forward = transform.forward + new Vector3(0, -0.5f, 0);
        if (Input.GetKeyDown(KeyCode.Space))
		{
			CmdFire();
		}
        //防止碰撞发生后的旋转
        if (this.gameObject.transform.localEulerAngles.x != 0 || gameObject.transform.localEulerAngles.z != 0)
        {
            gameObject.transform.localEulerAngles = new Vector3(0, gameObject.transform.localEulerAngles.y, 0);
        }
        if (gameObject.transform.position.y != 0)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);
        }
    }

	[Command]
	void CmdFire()
	{
		// This [Command] code is run on the server!

		// create the bullet object locally
		var bullet = (GameObject)Instantiate(
			bulletPrefab,
            new Vector3((transform.position + transform.forward).x,0.2f,(transform.position + transform.forward).z),
			Quaternion.identity);

		bullet.GetComponent<Rigidbody>().velocity = transform.forward * 4;

		// spawn the bullet on the clients
		NetworkServer.Spawn(bullet);

		// when the bullet is destroyed on the server it will automaticaly be destroyed on clients
		Destroy(bullet, 3.0f);      
	}
}

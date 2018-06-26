using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemySpawner : NetworkBehaviour  {

	public GameObject enemyPrefab;
	public int numEnemies;

    private void Start()
    {
        GetComponent<Rigidbody>().freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
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

    public override void OnStartServer()
	{
		for (int i=0; i < numEnemies; i++)
		{
			var pos = new Vector3(
				Random.Range(-8.0f, 8.0f),
				0.2f,
				Random.Range(-8.0f, 8.0f)
			);

			var rotation = Quaternion.Euler( Random.Range(0,180), Random.Range(0,180), Random.Range(0,180));

			var enemy = (GameObject)Instantiate(enemyPrefab, pos, rotation);
			NetworkServer.Spawn(enemy);
		}
	}
}

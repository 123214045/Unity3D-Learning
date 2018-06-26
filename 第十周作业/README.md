## Unity3D - 联网射击小游戏
---
> [视频地址](https://www.bilibili.com/video/av25647565/)
> [博客地址](https://segmentfault.com/a/1190000015397298)
---
### 游戏效果
![整体效果1](https://segmentfault.com/img/bVbcLG6)
![整体效果2](https://segmentfault.com/img/bVbcLG8)
---
### 游戏实现
1. 第一步是在项目中创建 NetworkManager 对象：
    ![网络管理空对象](https://segmentfault.com/img/bVbcLHe)
    * 从菜单 Game Object -> Create Empty 添加一个新的空游戏对象。
    * 在层次结构视图中选择它。
    * 将对象重命名为“NetworkManager”，使用右键上下文菜单中或单击对象的名称并键入。
    * 在对象的检查器窗口中，单击添加组件按钮
    * 找到组件 Network -> NetworkManager 并将其添加到对象。该组件管理游戏的网络状态。
    * 找到组件 Network -> NetworkManagerHUD 并将其添加到对象。该组件在您的游戏中提供了一个简单的用户界面来控制网络状态
2. 将场记挂载到网络空对象上创建游戏场景：
    ```cs
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
    ```
3. 设置玩家对象预制
    * 玩家预制设置：
    ![玩家预制设置](https://segmentfault.com/img/bVbcLHm?w=555&h=1292)
    * 下一步是设置代表游戏中玩家对象的 Unity Prefab。默认情况下，NetworkManager 通过克隆玩家预制来为 Client 实例化玩家对象。在这个例子中，玩家对象将是一个简单的立方体。
    * 从菜单 Game Object -> 3D Object -> Cube 创建一个新的立方体。
    * 在层次视图中找到该立方体并选择它。
    * 将对象重命名为 “PlayerCube”。
    * 在对象的检查器窗口中，单击添加组件按钮。
    * 将组件 Network -> NetworkIdentity 添加到对象。该组件用于标识服务器和客户端之间的对象。
    * 将 NetworkIdentity 上的 “Local Player Authority” 复选框设置为 true。这将允许客户端控制玩家对象的移动
    * 将立方体对象拖放到资源窗口中制作预制件。创建一个名为 “PlayerCube” 的预制件
    * 从场景中删除 PlayerCube 对象
4. 联网玩家对象的运动
    * 注册玩家预制：
    ![注册玩家预制](https://pmlpml.github.io/unity3d-learning/images/ch13/UNetTut5.png)
    * 编写 PlayerMove 脚本，使玩家的移动被联网同步：
    ```cs
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
    ```
4. 联网子弹。创建一个球体的子弹元素，添加rigibody和球状碰撞组件添加进Spawn Info中的注册预制目录中：
    ![子弹设置](https://segmentfault.com/img/bVbcLHz?w=555&h=1290)
    ![联网子弹](https://pmlpml.github.io/unity3d-learning/images/ch13/UNetTut12.png)
    * 子弹碰撞脚本：
    ```cs
    public class Bullet : MonoBehaviour {
        void OnCollisionEnter(Collision collision)
        {
            var hit = collision.gameObject;
            var hitCombat = hit.GetComponent<Combat>();
            if (hitCombat != null)
            {
                hitCombat.TakeDamage(10);
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
    ```
---
### 实现难点
* 实现第三人称视角时，一开始想的办法是将摄像机绑定为游戏对象的子元素。但是在联机时，由于摄像机优先级相同优先渲染后加载的，所有视角会切换到后进入的人身上。所有只能采用移动摄像机，使摄像机永远在第三人称的固定位置跟随的方式解决。
    ```cs
    //添加到PlayerMove脚本中的相应部分
    public override void OnStartLocalPlayer(){
                private Camera myCamera;

                public override void OnStartLocalPlayer()
                {
                    myCamera = Camera.main;
                }

                void Update()
                {
                    myCamera.transform.position = transform.position + transform.forward * -3 + new Vector3(0, 4, 0);
                    myCamera.transform.forward = transform.forward + new Vector3(0, -0.5f, 0);
                }
    }
    ```
---
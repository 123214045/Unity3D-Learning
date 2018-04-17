# 打飞碟小游戏
这次的主要任务是写一个打飞碟的小游戏，熟悉一下之前的MVC模式和动作分离，可以复用一部分前面的代码。
[博客传送门](https://segmentfault.com/a/1190000014431406)
# 游戏场景截图
这次建立了一个简单的天空盒和地面。场记依旧加载到空对象GameObject上，并且控制飞碟加载。
![打飞碟 - 1](http://img2.ph.126.net/SeJVVPeWhpt0mH1QrqzJJg==/6597277574356277600.png)
![打飞碟 - 2](http://img2.ph.126.net/u-dSDfnzYkgsIUskHAVu8w==/6631604327377143074.png)
# 代码组织结构
![代码组织结构](http://img1.ph.126.net/TxC10VGkUL5oS6ewzmaFog==/1941614389450441264.png)
1. 这次代码的核心是Disk和DiskFactory，也就是用工厂模式来控制物体的产生和复用。
    * Disk类：储存了一些基本属性，并且可以通过这些属性直接修改gameObject的属性。
    ```cs
        public class Disk : MonoBehaviour {
            public Vector3 StartPoint { get { return gameObject.transform.position; } set { gameObject.transform.position = value; } }
            public Color color { get { return gameObject.GetComponent<Renderer>().material.color; } set { gameObject.GetComponent<Renderer>().material.color = value; } }
            public float speed { get;set; }
            public Vector3 Direction { get { return Direction; } set { gameObject.transform.Rotate(value); } }
        }
    ```
    * DiskFactory类：Disk的道具工厂，减少游戏对象的销毁次数，复用游戏对象，且屏蔽创建和销毁的业务逻辑，使程序易于扩展。具体在这个代码中，DiskFactory还负责在生产Disk时随机指定起始位置，方向，速度，颜色。
    ![Disk Factory](http://img0.ph.126.net/FVXrSXX_WL_xFuzOs9SXRA==/6631536157656308532.png)
    ```cs
        public class DiskFactory { //开始时继承了MonoBehaviour后来发现不继承也可以
        public GameObject diskPrefab;
        public static DiskFactory DF = new DiskFactory();

        private Dictionary<int, Disk> used = new Dictionary<int, Disk>();//used是用来保存正在使用的飞碟 
        private List<Disk> free = new List<Disk>();//free是用来保存未激活的飞碟 

        private DiskFactory()
        {
            diskPrefab = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/disk"));//获取预制的游戏对象
            diskPrefab.AddComponent<Disk>();
            diskPrefab.SetActive(false);
        }

        public void FreeDisk()
        {
            foreach (Disk x in used.Values)
            {
                if (!x.gameObject.activeSelf)
                {
                    free.Add(x);
                    used.Remove(x.GetInstanceID());
                    return;
                }
            }
        }

        public Disk GetDisk(int round)  
        {
            FreeDisk();
            GameObject newDisk = null;
            Disk diskdata;
            if (free.Count > 0)
            {
                //从之前生产的Disk中拿出可用的
                newDisk = free[0].gameObject;
                free.Remove(free[0]);
            }
            else
            {
                //克隆预制对象，生产新Disk
                newDisk = GameObject.Instantiate<GameObject>(diskPrefab, Vector3.zero, Quaternion.identity);
            }
            newDisk.SetActive(true);
            diskdata = newDisk.AddComponent<Disk>();

            int swith;

            /** 
            * 根据回合数来生成相应的飞碟,难度逐渐增加。
            */
            float s;
            if (round == 1)
            {
                swith = Random.Range(0, 3);
                s = Random.Range(30, 40);
            }
            else if (round == 2)
            {
                swith = Random.Range(0, 4);
                s = Random.Range(40, 50);
            }
            else {
                swith = Random.Range(0, 6);
                s = Random.Range(50, 60);
            } 
            
            switch (swith)  
            {  
                
                case 0:  
                    {  
                        diskdata.color = Color.yellow;  
                        diskdata.speed = s;  
                        float RanX = UnityEngine.Random.Range(-1f, 1f) < 0 ? -1 : 1;  
                        diskdata.Direction = new Vector3(RanX, 1, 0);
                        diskdata.StartPoint = new Vector3(Random.Range(-130, -110), Random.Range(30,90), Random.Range(110,140));
                        break;  
                    }  
                case 1:  
                    {  
                        diskdata.color = Color.red;  
                        diskdata.speed = s + 10;  
                        float RanX = UnityEngine.Random.Range(-1f, 1f) < 0 ? -1 : 1;  
                        diskdata.Direction = new Vector3(RanX, 1, 0);
                        diskdata.StartPoint = new Vector3(Random.Range(-130, -110), Random.Range(30, 80), Random.Range(110, 130));
                        break;  
                    }  
                case 2:  
                    {  
                        diskdata.color = Color.black;  
                        diskdata.speed = s + 15;  
                        float RanX = UnityEngine.Random.Range(-1f, 1f) < 0 ? -1 : 1;  
                        diskdata.Direction = new Vector3(RanX, 1, 0);
                        diskdata.StartPoint = new Vector3(Random.Range(-130,-110), Random.Range(30, 70), Random.Range(90, 120));
                        break;  
                    }
                case 3:
                    {
                        diskdata.color = Color.yellow;
                        diskdata.speed = -s;
                        float RanX = UnityEngine.Random.Range(-1f, 1f) < 0 ? -1 : 1;
                        diskdata.Direction = new Vector3(RanX, 1, 0);
                        diskdata.StartPoint = new Vector3(Random.Range(130, 110), Random.Range(30, 90), Random.Range(110, 140));
                        break;
                    }
                case 4:
                    {
                        diskdata.color = Color.red;
                        diskdata.speed = -s - 10;
                        float RanX = UnityEngine.Random.Range(-1f, 1f) < 0 ? -1 : 1;
                        diskdata.Direction = new Vector3(RanX, 1, 0);
                        diskdata.StartPoint = new Vector3(Random.Range(130, 110), Random.Range(30, 80), Random.Range(110, 130));
                        break;
                    }
                case 5:
                    {
                        diskdata.color = Color.black;
                        diskdata.speed = -s - 15;
                        float RanX = UnityEngine.Random.Range(-1f, 1f) < 0 ? -1 : 1;
                        diskdata.Direction = new Vector3(RanX, 1, 0);
                        diskdata.StartPoint = new Vector3(Random.Range(130, 110), Random.Range(30, 70), Random.Range(90, 120));
                        break;
                    }
            }
            used.Add(diskdata.GetInstanceID(), diskdata); //添加到使用中
            diskdata.name = diskdata.GetInstanceID().ToString();
            return diskdata;  
        }
    }
    ```
2. 上次已经写过了运动分离版的牧师与魔鬼了，这次可以将一部分运动分离的代码直接复用，主要的修改就是重写MoveToAction使Disk做类似有水平初速自由落体的运动（其实可以用刚体属性然后添加重力，此处我为了少修改代码而没有选择使用），然后重写一下具体在这个场景中的CCActionManager类。下面是重写的代码：
    * 重写的MoveToAction类：
    ```cs
        public class CCMoveToAction : SSAction
        {
            public float speedx;
            public float speedy = 0;

            private CCMoveToAction() { }
            public static CCMoveToAction getAction(float speedx)
            {
                CCMoveToAction action = CreateInstance<CCMoveToAction>();
                action.speedx = speedx;
                return action;
            }

            public override void Update()
            {
                //模拟加速度为10的抛物线运动
                this.transform.position += new Vector3(speedx*Time.deltaTime, -speedy*Time.deltaTime+(float)-0.5*10*Time.deltaTime*Time.deltaTime,0);
                speedy += 10*Time.deltaTime;
                if (transform.position.y <= 1)//被点击和运动到地面都满足回收条件
                {
                    destroy = true;
                    CallBack.SSActionCallback(this);
                }
            }

            public override void Start()
            {

            }
        }
    ```
    * 重写的CCActionManager类：
    ```cs
        using Interfaces;
        public class CCActionManager : SSActionManager, SSActionCallback
        {
            int count = 0;//记录所有在移动的碟子的数量
            public SSActionEventType Complete = SSActionEventType.Completed;

            public void MoveDisk(Disk Disk)
            {
                count++;
                Complete = SSActionEventType.Started;
                CCMoveToAction action = CCMoveToAction.getAction(Disk.speed);
                addAction(Disk.gameObject,action,this);
            }

            public void SSActionCallback(SSAction source) //运动事件结束后的回调函数
            {
                count--;
                Complete = SSActionEventType.Completed;
                source.gameObject.SetActive(false);
            }

            public bool IsAllFinished() //主要为了防止游戏结束时场景还有对象但是GUI按钮已经加载出来
            {
                if (count == 0) return true;
                else return false;
            }
        }
    ```
3. 重写一下Interfaces，根据交互的需要定义函数。
    ```cs
        namespace Interfaces
        {
            public interface ISceneController
            {
                void LoadResources();
            }

            public interface UserAction
            {
                void Hit(Vector3 pos); // 发生点击事件，传送点击位置给场记
                void Restart();//重新开始游戏
                int GetScore();
                bool RoundStop();
                void RoundPlus();
            }

            public enum SSActionEventType : int { Started, Completed }

            public interface SSActionCallback
            {
                void SSActionCallback(SSAction source);
            }
        }
    ```
4. 最后最重要的就是重写场记了，导演和之前还是一样的。
    ```cs
        using System.Collections;
        using System.Collections.Generic;
        using UnityEngine;
        using Interfaces;

        public class FirstSceneController : MonoBehaviour, ISceneController, UserAction
        {
            int score = 0;
            int round = 1;
            int tral = 0;
            bool start = false;
            CCActionManager Manager;
            DiskFactory DF;

            void Awake()
            {
                SSDirector director = SSDirector.getInstance();
                director.currentScenceController = this;
                DF = DiskFactory.DF;
                Manager = GetComponent<CCActionManager>();
            }

            // Use this for initialization
            void Start () {
                
            }

            // Update is called once per frame
            int count = 0;
            void Update () {
                if(start == true)
                {
                    count++;
                    if (count >= 80)
                    {
                        count = 0;

                        if(DF == null)
                        {
                            Debug.LogWarning("DF is NUll!");
                            return;
                        }
                        tral++;
                        Disk d = DF.GetDisk(round);
                        Manager.MoveDisk(d);
                        if (tral == 10)
                        {
                            round++;
                            tral = 0;
                        }
                    }
                }
            }

            public void LoadResources()
            {
                
            }

            public void Hit(Vector3 pos)
            {
                Ray ray = Camera.main.ScreenPointToRay(pos);

                RaycastHit[] hits;
                hits = Physics.RaycastAll(ray);
                for (int i = 0; i < hits.Length; i++)
                {
                    RaycastHit hit = hits[i];

                    if (hit.collider.gameObject.GetComponent<Disk>() != null)
                    {
                        Color c = hit.collider.gameObject.GetComponent<Renderer>().material.color;
                        if (c == Color.yellow) score += 1;
                        if (c == Color.red) score += 2;
                        if (c == Color.black) score += 3;
                        
                        hit.collider.gameObject.transform.position = new Vector3(0, -5, 0);
                    }

                }
            }

            public int GetScore()
            {
                return score;
            }

            public void Restart()
            {
                score = 0;
                round = 1;
                start = true;
            }
            public bool RoundStop()
            {
                if (round > 3)
                {
                    start = false;
                    return Manager.IsAllFinished();
                }
                else return false;
            }
            public int GetRound()
            {
                return round;
            }
        }
    ```
5. 用户界面GUI，这次GUI还算简单，主要加了显示分数，时间，以及轮次的面板。
    ```cs
        public class InterfaceGUI : MonoBehaviour {
            UserAction UserActionController;
            public GameObject t;
            bool ss = false;
            float S;
            float Now;
            int round = 1;
            // Use this for initialization
            void Start () {
                UserActionController = SSDirector.getInstance().currentScenceController as UserAction;
                S = Time.time;
            }

            private void OnGUI()
            {
                if(!ss) S = Time.time;
                GUI.Label(new Rect(1000, 50, 500, 500),"Score: " + UserActionController.GetScore().ToString() + "  Time:  " + ((int)(Time.time - S)).ToString() + "  Round:  " + round );
                if (!ss && GUI.Button(new Rect(Screen.width / 2 - 30, Screen.height / 2 - 30, 100, 50), "Start"))
                {
                    S = Time.time;
                    ss = true;
                    UserActionController.Restart();
                }
                if (ss)
                {
                    round = UserActionController.GetRound();
                    if (Input.GetButtonDown("Fire1"))
                    {

                        Vector3 pos = Input.mousePosition;
                        UserActionController.Hit(pos);

                    }
                    if (round > 3)
                    {
                        round = 3;
                        if (UserActionController.RoundStop())
                        {
                            ss = false;
                        }
                    }
                }
            }
        }
    ```
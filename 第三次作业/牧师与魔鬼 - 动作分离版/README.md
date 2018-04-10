Unty3D - 2 牧师与魔鬼 - 动作分离版
---
>* [博客传送门](https://segmentfault.com/a/1190000014283425)
# 游戏场景截图：
![游戏场景 - 1](http://img1.ph.126.net/p8CQCd6j5Z4JaAP82aR_Vw==/6632675251700494000.png)
![游戏场景 - 2](http://img0.ph.126.net/xl_bJ8JbrlBa3JJDwz5zEQ==/6597979062774900426.png)
---
# 游戏对象使用小结：
> 1. 常见游戏对象有空对象，摄像机，光线，天空盒，地形，3D对象，声音，UI系统和粒子系统以及特效。
> 2. 游戏对象均具有Active，Name，Tag，Layer属性。
> 3. 根据游戏场景氛围的不同，可以改变光源类型来烘托场景气氛。
---
# 游戏组织结构：
>这次与基础版的区别主要是实现动作分离，上一次的动作是由专门的类来完成的，并且每个对象各自管理一部分自己的移动属性。这次将这些动作管理分离出来，实现动作和物体属性的分离。
1. UML图 ![UML](http://img0.ph.126.net/HRPf3-IpyjmW1SWDSOZjew==/5717536330737864813.png)
---
2. SSAction类：SSAction是所有动作的基类，ScriptableObject 是不需要绑定 GameObject 对象的可编程基类。
```cs
    public class SSAction : ScriptableObject 
    {

        public bool enable = true;
        public bool destroy = false;

        public GameObject gameObject;
        public Transform transform;
        public SSActionCallback CallBack;

        public virtual void Start()
        {
            throw new System.NotImplementedException();
        }

        public virtual void Update()
        {
            throw new System.NotImplementedException();
        }
    }
```
---
3. CCMoveToAction类：实现简单的动作，并且管理内存回收以及重写Update()函数实现物体的运动。
```cs
public class CCMoveToAction : SSAction
{
    public Vector3 target;
    public float speed;

    private CCMoveToAction() { }
    public static CCMoveToAction getAction(Vector3 target, float speed)
    {
        CCMoveToAction action = ScriptableObject.CreateInstance<CCMoveToAction>();
        action.target = target;
        action.speed = speed;
        return action;
    }

    public override void Update()
    {
        this.transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if (transform.position == target)
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
---
4. CCSequenceAction类：创建动作执行序列，按要求循环执行保存的动作序列。
```cs
public class CCSequenceAction : SSAction
{
    public List<SSAction> sequence;
    public int repeat = 1; // 1->only do it for once, -1->repeat forever
    public int currentActionIndex = 0;

    public static CCSequenceAction getAction(int repeat, int currentActionIndex, List<SSAction> sequence)
    {
        CCSequenceAction action = ScriptableObject.CreateInstance<CCSequenceAction>();
        action.sequence = sequence;
        action.repeat = repeat;
        action.currentActionIndex = currentActionIndex;
        return action;
    }

    public override void Update()
    {
        if (sequence.Count == 0) return;
        if (currentActionIndex < sequence.Count)
        {
            sequence[currentActionIndex].Update();
        }
    }

    public void SSActionCallback(SSAction source)
    {
        source.destroy = false;
        this.currentActionIndex++;
        if (this.currentActionIndex >= sequence.Count)
        {
            this.currentActionIndex = 0;
            if (repeat > 0) repeat--;
            if (repeat == 0)
            {
                this.destroy = true;
                this.CallBack.SSActionCallback(this);
            }
        }
    }

    public override void Start()
    {
        foreach (SSAction action in sequence)
        {
            action.gameObject = this.gameObject;
            action.transform = this.transform;
            action.CallBack = this;
            action.Start();
        }
    }

    void OnDestroy()
    {
        foreach (SSAction action in sequence)
        {
            DestroyObject(action);
        }
    }
}
```
---
5. SSActionCallback接口：定义了事件处理接口，所有事件管理者都必须实现这个接口来实现事件调度。
```cs
public interface SSActionCallback
{
    void SSActionCallback(SSAction source);
}
```
---
6. SSActionManager类：动作管理的基类，使用上述的移动方法，实现游戏对象与动作的绑定，确定回调函数消息的接收对象。
```cs
public class SSActionManager : MonoBehaviour
{
    private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();
    private List<SSAction> waitingToAdd = new List<SSAction>();
    private List<int> watingToDelete = new List<int>();

    protected void Update()
    {
        foreach (SSAction ac in waitingToAdd)
        {
            actions[ac.GetInstanceID()] = ac;
        }
        waitingToAdd.Clear();

        foreach (KeyValuePair<int, SSAction> kv in actions)
        {
            SSAction ac = kv.Value;
            if (ac.destroy)
            {
                watingToDelete.Add(ac.GetInstanceID());
            }
            else if (ac.enable)
            {
                ac.Update();
            }
        }

        foreach (int key in watingToDelete)
        {
            SSAction ac = actions[key];
            actions.Remove(key);
            DestroyObject(ac);
        }
        watingToDelete.Clear();
    }

    public void addAction(GameObject gameObject, SSAction action, SSActionCallback ICallBack)
    {
        action.gameObject = gameObject;
        action.transform = gameObject.transform;
        action.CallBack = ICallBack;
        waitingToAdd.Add(action);
        action.Start();
    }
}
```
7. FirstSceneActionManager类：当前场景下的动作管理的具体实现，与场景控制基类配合，实现对当前场景的直接管理。挂载到图像中的Main空对象上实现对预制加载的场景的管理。
```cs
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;

public class FirstSceneActionManager : SSActionManager, SSActionCallback
{
    public SSActionEventType Complete = SSActionEventType.Completed;

    public void BoatMove(BoatController Boat)
    {
        Complete = SSActionEventType.Started;
        CCMoveToAction action = CCMoveToAction.getAction(Boat.GetDestination(), Boat.GetMoveSpeed());
        addAction(Boat.GetGameObject(), action, this);
        Boat.ChangeState();
    }

    public void GameObjectsMove(GameObjects GameObject, Vector3 Destination)
    {
        Complete = SSActionEventType.Started;
        Vector3 CurrentPos = GameObject.GetPosition();
        Vector3 MiddlePos = CurrentPos;
        if (Destination.y > CurrentPos.y)
        {
            MiddlePos.y = Destination.y;
        }
        else
        {
            MiddlePos.x = Destination.x;
        }
        SSAction action1 = CCMoveToAction.getAction(MiddlePos, GameObject.GetMoveSpeed());
        SSAction action2 = CCMoveToAction.getAction(Destination, GameObject.GetMoveSpeed());
        SSAction seqAction = CCSequenceAction.getAction(1, 0, new List<SSAction> { action1, action2 });
        this.addAction(GameObject.GetGameobject(), seqAction, this);
    }

    public void SSActionCallback(SSAction source)
    {
        Complete = SSActionEventType.Completed;
    }
}
```
8. 修改场记实现使用动作分离的动作管理:
```cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;

public class FirstController : MonoBehaviour, ISceneController, UserAction
{
    InteracteGUI UserGUI;
    public CoastController fromCoast;
    public CoastController toCoast;
    public BoatController boat;
    private GameObjects[] GameObjects;

    private FirstSceneActionManager FCActionManager;

    void Start()
    {
        FCActionManager = GetComponent<FirstSceneActionManager>();
    }

    void Awake()
    {
        SSDirector director = SSDirector.getInstance();
        director.currentScenceController = this;
        UserGUI = gameObject.AddComponent<InteracteGUI>() as InteracteGUI;
        GameObjects = new GameObjects[6];
        LoadResources();
    }

    public void LoadResources()
    {
        fromCoast = new CoastController("from");
        toCoast = new CoastController("to");
        boat = new BoatController();
        GameObject water = Instantiate(Resources.Load("Perfabs/Water", typeof(GameObject)), new Vector3(0, 0.5F, 0), Quaternion.identity, null) as GameObject;
        water.name = "water";
        for (int i = 0; i < 3; i++)
        {
            GameObjects s = new GameObjects("priest");
            s.setName("priest" + i);
            s.setPosition(fromCoast.getEmptyPosition());
            s.getOnCoast(fromCoast);
            fromCoast.getOnCoast(s);
            GameObjects[i] = s;
        }

        for (int i = 0; i < 3; i++)
        {
            GameObjects s = new GameObjects("devil");
            s.setName("devil" + i);
            s.setPosition(fromCoast.getEmptyPosition());
            s.getOnCoast(fromCoast);
            fromCoast.getOnCoast(s);
            GameObjects[i + 3] = s;
        }
    }

    public void ObjectIsClicked(GameObjects Objects)
    {
        if (FCActionManager.Complete == SSActionEventType.Started) return;
        if (Objects.isOnBoat())
        {
            CoastController whichCoast;
            if (boat.get_State() == -1)
            { // to->-1; from->1
                whichCoast = toCoast;
            }
            else
            {
                whichCoast = fromCoast;
            }

            boat.GetOffBoat(Objects.getName());
            FCActionManager.GameObjectsMove(Objects,whichCoast.getEmptyPosition());
            Objects.getOnCoast(whichCoast);
            whichCoast.getOnCoast(Objects);

        }
        else
        {
            Debug.Log("On Coast!");
            CoastController whichCoast = Objects.getCoastController(); // obejects on coast

            if (boat.getEmptyIndex() == -1)
            {      
                return;
            }

            if (whichCoast.get_State() != boat.get_State())   // boat is not on the side of character
                return;

            whichCoast.getOffCoast(Objects.getName());
            FCActionManager.GameObjectsMove(Objects, boat.getEmptyPosition());
            Objects.getOnBoat(boat);
            boat.GetOnBoat(Objects);
        }
        UserGUI.SetState = Check();
    }

    public void MoveBoat()
    {
        if (FCActionManager.Complete == SSActionEventType.Started || boat.isEmpty()) return;
        FCActionManager.BoatMove(boat);
        UserGUI.SetState = Check();
    }

    int Check()
    {   // 0->not finish, 1->lose, 2->win
        int from_priest = 0;
        int from_devil = 0;
        int to_priest = 0;
        int to_devil = 0;

        int[] fromCount = fromCoast.GetobjectsNumber();
        from_priest += fromCount[0];
        from_devil += fromCount[1];

        int[] toCount = toCoast.GetobjectsNumber();
        to_priest += toCount[0];
        to_devil += toCount[1];

        if (to_priest + to_devil == 6)      // win
            return 2;

        int[] boatCount = boat.GetobjectsNumber();
        if (boat.get_State() == -1)
        {   // boat at toCoast
            to_priest += boatCount[0];
            to_devil += boatCount[1];
        }
        else
        {   // boat at fromCoast
            from_priest += boatCount[0];
            from_devil += boatCount[1];
        }
        if (from_priest < from_devil && from_priest > 0)
        {       // lose
            return 1;
        }
        if (to_priest < to_devil && to_priest > 0)
        {
            return 1;
        }
        return 0;           // not finish
    }

    public void Restart()
    {
        boat.reset();
        fromCoast.reset();
        toCoast.reset();
        foreach (GameObjects gameobject in GameObjects)
        {
            gameobject.reset();
        }
    }
}
```
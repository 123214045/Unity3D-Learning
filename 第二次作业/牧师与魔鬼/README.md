Unty3D - 2 牧师与魔鬼
---
>* [博客传送门](https://segmentfault.com/a/1190000014161302)
# 游戏规则：
* 帮助3个牧师（方块）和3个魔鬼（圆球）渡河。
* 船上最多可以载2名游戏角色。
* 船上有游戏对象时才可以移动。
* 当有一侧岸的魔鬼数多余牧师数时（包括船上的魔鬼和牧师），魔鬼就会失去控制，吃掉牧师（如果这一侧没有牧师则不会失败），游戏失败。
* 当所有游戏角色都到达对岸时，游戏胜利。
---
玩家动作| 发生条件
---- | ---
上船 | 船上有空位且点击想上船的对象
下船 | 船上有对象且点击想下船的对象
开船 | 点击船
重新开始|点击restart按钮
---
# 游戏截图：
![开始游戏](http://img0.ph.126.net/dnQ5z0PJ3hsCYUOBW8CQ-g==/6631702183912138197.png)

![游戏失败](http://img2.ph.126.net/dwuHFrXWqPKq8Pc9-Uravg==/2000724134559637484.png)

![游戏胜利](http://img0.ph.126.net/QRrquZDn4n2tRWWxHq-kpw==/6597330350914396545.png)

****
# 游戏组织结构：
    1. 导演SSDirector：
        此处导演使用单例模式保证导演实例有且仅有一个，导演只负责在场景初始化时控制对应场景的场记:
```cs
        public class SSDirector : System.Object {

        private static SSDirector _instance;

        public ISceneController currentScenceController { get; set; }
        public bool running { get; set; }

        public static SSDirector getInstance()
        {
            if (_instance == null)
            {
                _instance = new SSDirector();
            }
            return _instance;
        }

        public int getFPS()
        {
            return Application.targetFrameRate;
        }

        public void setFPS(int fps)
        {
            Application.targetFrameRate = fps;
        }
    }
```
---
    2. 接口命名空间namespace Interfaces：
        由于MVC模式的要求，为了降低耦合，必须将需要提供给用户界面的接口API暴露出来，写在同一个命名空间里便于管理：
```cs
namespace Interfaces
{
    public interface ISceneController
    {
        void LoadResources();
    }

    public interface UserAction
    {
        void MoveBoat();
        void ObjectIsClicked(GameObjects characterCtrl);
        void Restart();
    }

}
```
    3. 场记FirstController：
        场记FirstController负责实例化接口ISceneController和UserAction，实现加载资源和响应用户操作：
>* 资源加载时，将对应元素创建到指定位置：
```cs
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
```
>* 响应用户点击对象事件:
```cs
public void ObjectIsClicked(GameObjects Objects)
    {
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
            Objects.moveToPosition(whichCoast.getEmptyPosition());
            Objects.getOnCoast(whichCoast);
            whichCoast.getOnCoast(Objects);

        }
        else
        {                                   
            CoastController whichCoast = Objects.getCoastController(); // obejects on coast

            if (boat.getEmptyIndex() == -1)
            {      
                return;
            }

            if (whichCoast.get_State() != boat.get_State())   // boat is not on the side of character
                return;

            whichCoast.getOffCoast(Objects.getName());
            Objects.moveToPosition(boat.getEmptyPosition());
            Objects.getOnBoat(boat);
            boat.GetOnBoat(Objects);
        }
        UserGUI.SetState = Check();
    }
```
>* 判断游戏处在的状态：（继续进行，游戏胜利，游戏结束）
```cs
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
```
---
4. 可移动对象的移动控制：
>*  移动方法在可移动对象创建的时候会通过Instance.AddComponent(typeof(Move))函数添加到对象中，Move类作为一个专门的类来控制对象移动，通过SetDestination（）函数直接完成移动的设定，提供了移动的简单方法：
```cs
public class Move : MonoBehaviour
{

    readonly float Speed = 20;

    Vector3 Target;
    Vector3 Middle;
    int Move_State = 0;  // 0->no need to move, 1->object moving , 2->boat moving to dest
    bool To_Middle = true;

    void Update()
    {
        if (Move_State == 1)
        {
            if (To_Middle)
            {
                transform.position = Vector3.MoveTowards(transform.position, Middle, Speed * Time.deltaTime);
                if (transform.position == Middle) To_Middle = false;
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, Target, Speed * Time.deltaTime);
                if (transform.position == Target) Move_State = 0;
            }
        }
        else if (Move_State == 2)
        {
            transform.position = Vector3.MoveTowards(transform.position, Target, Speed * Time.deltaTime);
            if (transform.position == Target)
            {
                To_Middle = true;
                Move_State = 0;
            }
        }
    }

    public void SetDestination(Vector3 Position)
    {
        if (Move_State != 0) return;
        Target = Middle = Position;
        To_Middle = true;
        if (transform.position.y == Target.y)
        {
            Move_State = 2;
        }
        else
        {
            Move_State = 1;
            if (transform.position.y < Target.y)
            {
                Middle.x = transform.position.x;
            }
            else if (transform.position.y > Target.y)
            {
                Middle.y = transform.position.y;
            }
        }
    }

    public void Reset()
    {
        Move_State = 0;
        To_Middle = true;
    }
}
```
5. 游戏对象（牧师，魔鬼）的初始化以及提供给其他类使用的一系列方法:
```cs
public class GameObjects
{
    readonly GameObject Instance;
    readonly Move Move;
    readonly ClickGUI clickGUI;
    readonly int characterType; // 0->priest, 1->devil

    bool _isOnBoat = false;
    CoastController coastController;


    public GameObjects(string Type)
    {

        if (Type == "priest")
        {
            Instance = Object.Instantiate(Resources.Load("Perfabs/Priest", typeof(GameObject)), Vector3.zero, Quaternion.identity, null) as GameObject;
            characterType = 0;
        }
        else
        {
            Instance = Object.Instantiate(Resources.Load("Perfabs/Devil", typeof(GameObject)), Vector3.zero, Quaternion.identity, null) as GameObject;
            characterType = 1;
        }
        Move = Instance.AddComponent(typeof(Move)) as Move;

        clickGUI = Instance.AddComponent(typeof(ClickGUI)) as ClickGUI;
        clickGUI.setController(this);
    }

    public void setName(string name)
    {
        Instance.name = name;
    }

    public void setPosition(Vector3 pos)
    {
        Instance.transform.position = pos;
    }

    public void moveToPosition(Vector3 destination)
    {
        Move.SetDestination(destination);
    }

    public int getType()
    {   // 0->priest, 1->devil
        return characterType;
    }

    public string getName()
    {
        return Instance.name;
    }

    public void getOnBoat(BoatController boatCtrl)
    {
        coastController = null;
        Instance.transform.parent = boatCtrl.getGameobj().transform;
        _isOnBoat = true;
    }

    public void getOnCoast(CoastController coastCtrl)
    {
        coastController = coastCtrl;
        Instance.transform.parent = null;
        _isOnBoat = false;
    }

    public bool isOnBoat()
    {
        return _isOnBoat;
    }

    public CoastController getCoastController()
    {
        return coastController;
    }

    public void reset()
    {
        Move.Reset();
        coastController = (SSDirector.getInstance().currentScenceController as FirstController).fromCoast;
        getOnCoast(coastController);
        setPosition(coastController.getEmptyPosition());
        coastController.getOnCoast(this);
    }
}
```
6. 河岸有两类，一类是From也就是出发的地方，另一类是To也就是目标位置。创建了几个空位置用来存储对象以及判断是否有对象已经把该位置给占了，提供方法给实例用来判断是否有空位置以及空位置的三维坐标：
```cs
public class CoastController
{
    readonly GameObject coast;
    readonly Vector3 from_pos = new Vector3(9, 1, 0);
    readonly Vector3 to_pos = new Vector3(-9, 1, 0);
    readonly Vector3[] positions;
    readonly int State;    // to->-1, from->1

    GameObjects[] passengerPlaner;

    public CoastController(string _State)
    {
        positions = new Vector3[] {new Vector3(6.5F,2.25F,0), new Vector3(7.5F,2.25F,0), new Vector3(8.5F,2.25F,0),
                new Vector3(9.5F,2.25F,0), new Vector3(10.5F,2.25F,0), new Vector3(11.5F,2.25F,0)};

        passengerPlaner = new GameObjects[6];

        if (_State == "from")
        {
            coast = Object.Instantiate(Resources.Load("Perfabs/Ston", typeof(GameObject)), from_pos, Quaternion.identity, null) as GameObject;
            coast.name = "from";
            State = 1;
        }
        else
        {
            coast = Object.Instantiate(Resources.Load("Perfabs/Ston", typeof(GameObject)), to_pos, Quaternion.identity, null) as GameObject;
            coast.name = "to";
            State = -1;
        }
    }

    public int getEmptyIndex()
    {
        for (int i = 0; i < passengerPlaner.Length; i++)
        {
            if (passengerPlaner[i] == null)
            {
                return i;
            }
        }
        return -1;
    }

    public Vector3 getEmptyPosition()
    {
        Vector3 pos = positions[getEmptyIndex()];
        pos.x *= State;
        return pos;
    }

    public void getOnCoast(GameObjects ObjectControl)
    {
        int index = getEmptyIndex();
        passengerPlaner[index] = ObjectControl;
    }

    public GameObjects getOffCoast(string passenger_name)
    {   // 0->priest, 1->devil
        for (int i = 0; i < passengerPlaner.Length; i++)
        {
            if (passengerPlaner[i] != null && passengerPlaner[i].getName() == passenger_name)
            {
                GameObjects charactorCtrl = passengerPlaner[i];
                passengerPlaner[i] = null;
                return charactorCtrl;
            }
        }
        Debug.Log("cant find passenger on coast: " + passenger_name);
        return null;
    }

    public int get_State()
    {
        return State;
    }

    public int[] GetobjectsNumber()
    {
        int[] count = { 0, 0 };
        for (int i = 0; i < passengerPlaner.Length; i++)
        {
            if (passengerPlaner[i] == null)
                continue;
            if (passengerPlaner[i].getType() == 0)
            {   // 0->priest, 1->devil
                count[0]++;
            }
            else
            {
                count[1]++;
            }
        }
        return count;
    }

    public void reset()
    {
        passengerPlaner = new GameObjects[6];
    }
}
```
7. 最后一个就是船的控制函数了，其实和河岸以及游戏对象的性质都很像，只不过是既有河岸的位置特性也有游戏对象的移动属性，按照需求写出相应的判断即可：
```cs
public class BoatController
{
    readonly GameObject boat;
    readonly Move Moving;
    readonly Vector3 fromPosition = new Vector3(5, 1, 0);
    readonly Vector3 toPosition = new Vector3(-5, 1, 0);
    readonly Vector3[] from_positions;
    readonly Vector3[] to_positions;

    int State; // to->-1; from->1
    GameObjects[] passenger = new GameObjects[2];

    public BoatController()
    {
        State = 1;

        from_positions = new Vector3[] { new Vector3(4.5F, 1.5F, 0), new Vector3(5.5F, 1.5F, 0) };
        to_positions = new Vector3[] { new Vector3(-5.5F, 1.5F, 0), new Vector3(-4.5F, 1.5F, 0) };

        boat = Object.Instantiate(Resources.Load("Perfabs/Boat", typeof(GameObject)), fromPosition, Quaternion.identity, null) as GameObject;
        boat.name = "boat";

        Moving = boat.AddComponent(typeof(Move)) as Move;
        boat.AddComponent(typeof(ClickGUI));
    }

    public int getEmptyIndex()
    {
        for (int i = 0; i < passenger.Length; i++)
        {
            if (passenger[i] == null)
            {
                return i;
            }
        }
        return -1;
    }

    public bool isEmpty()
    {
        for (int i = 0; i < passenger.Length; i++)
        {
            if (passenger[i] != null)
            {
                return false;
            }
        }
        return true;
    }

    public Vector3 getEmptyPosition()
    {
        Vector3 pos;
        int emptyIndex = getEmptyIndex();
        if (State == -1)
        {
            pos = to_positions[emptyIndex];
        }
        else
        {
            pos = from_positions[emptyIndex];
        }
        return pos;
    }

    public void GetOnBoat(GameObjects ObjectControl)
    {
        int index = getEmptyIndex();
        passenger[index] = ObjectControl;
    }

    public GameObjects GetOffBoat(string passenger_name)
    {
        for (int i = 0; i < passenger.Length; i++)
        {
            if (passenger[i] != null && passenger[i].getName() == passenger_name)
            {
                GameObjects charactorCtrl = passenger[i];
                passenger[i] = null;
                return charactorCtrl;
            }
        }
        Debug.Log("Cant find passenger in boat: " + passenger_name);
        return null;
    }

    public GameObject getGameobj()
    {
        return boat;
    }

    public int get_State()
    { // to->-1; from->1
        return State;
    }

    public int[] GetobjectsNumber()
    {
        int[] count = { 0, 0 };// [0]->priest, [1]->devil
        for (int i = 0; i < passenger.Length; i++)
        {
            if (passenger[i] == null)
                continue;
            if (passenger[i].getType() == 0)
            {
                count[0]++;
            }
            else
            {
                count[1]++;
            }
        }
        return count;
    }

    public void Move()
    {
        if (State == -1)
        {
            Moving.SetDestination(fromPosition);
            State = 1;
        }
        else
        {
            Moving.SetDestination(toPosition);
            State = -1;
        }
    }

    public void reset()
    {
        Moving.Reset();
        if (State == -1)
        {
            Move();
        }
        passenger = new GameObjects[2];
    }
}
```
8. GUI分为直接与用户进行界面交互的InteracteGUI和响应用户点击游戏对象事件的ClickGUI，并且使用UserAction接口来控制游戏：
```cs
public class InteracteGUI : MonoBehaviour {
    UserAction UserAcotionController;
    public int SetState { get { return GameState; } set { GameState = value; } }
    static int GameState = 0;

	// Use this for initialization
	void Start () {
        UserAcotionController = SSDirector.getInstance().currentScenceController as UserAction;
    }

    private void OnGUI()
    {
        if (GameState == 1)
        {
            GUI.Label(new Rect(Screen.width / 2 -30, Screen.height / 2 - 30, 100, 50), "Gameover!");
            if (GUI.Button(new Rect(Screen.width / 2 - 70, Screen.height / 2, 140, 70), "Restart"))
            {
                GameState = 0;
                UserAcotionController.Restart();
            }
        }
        else if (GameState == 2)
        {
            GUI.Label(new Rect(Screen.width / 2 - 30, Screen.height / 2 - 30 , 100, 50), "Win!");
            if (GUI.Button(new Rect(Screen.width / 2 - 70, Screen.height / 2, 140, 70), "Restart"))
            {
                GameState = 0;
                UserAcotionController.Restart();
            }
        }
    }
}

public class ClickGUI : MonoBehaviour{
    UserAction UserAcotionController;
    GameObjects GameObjectsInScene;

    public void setController(GameObjects characterCtrl)
    {
        GameObjectsInScene = characterCtrl;
    }

    void Start()
    {
        UserAcotionController = SSDirector.getInstance().currentScenceController as UserAction;
    }

    void OnMouseDown()
    {
        if (gameObject.name == "boat")
        {
            UserAcotionController.MoveBoat();
        }
        else
        {
            UserAcotionController.ObjectIsClicked(GameObjectsInScene);
        }
    }
}
```
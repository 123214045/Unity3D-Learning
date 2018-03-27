Unity3D-1
---------

####1. **游戏对象（GameObjects）** 和 **资源（Assets）**的区别与联系。
>* **游戏对象**：直接出现在游戏的场景中，是资源整合的具体表现，对象通过层次结构来组织，通过整体-部分的关系构成层次结构。
>* **资源**：资源通过文件夹的形式组织，包含常用的图像，视频，脚本文件，预制文件等等，可以被一个或者多个对象使用。
---
####2. 下载游戏案例，分别总结资源、对象组织的结构（指资源的目录组织结构与游戏对象树的层次结构)。
>资源的组织结构：
```
Asserts
  | - Animations
    | - MainMenu
    | - UI
  | - Audio
    | - Music
    | - SFX
  | - Data
    | - Agents
    | - Alignments
    | - Levels
  | - Documentation
  | - Fonts
  | - Gizmos
  | - Icon
  | - Materials
    | - Levels
    | - Particles
    | - Player
    | - Projectiles
    | - UI
    | - Units
  | - Models
  | - PostProcessing
  | - Prefabs
  | - Scenes
  | - Scripts
  | - Sprites
  | - Textures
  | - UI
```
>游戏对象的组织结构：
```
GameObject
    | - Player
    | - Enemy
    | - Surroundings
        | - Bulidings
        | - Trees
        ...
    | - Camera
    | - Music
    | - Audio
    | - Environment
```
---
####3. 编写一个代码，使用 debug 语句来验证 MonoBehaviour 基本行为或事件触发的条件:
* 基本行为包括 Awake() Start() Update() FixedUpdate() LateUpdate()
* 常用事件包括 OnGUI() OnDisable() OnEnable()

```cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {
    void Awake() {
        Debug.Log ("Awake");
    }
    void Start () {
        Debug.Log ("Start");
    }
    void Update () {
        Debug.Log ("Update");
    }
    void FixedUpdate() {
        Debug.Log ("FixedUpdate");
    }
    void LateUpdate() {
        Debug.Log ("LateUpdate");
    }
    void OnGUI() {
        Debug.Log ("GUI");
    }
    void Reset() {
        Debug.Log ("Reset");
    }
    void OnDisable() {
        Debug.Log ("Disable");
    }
    void OnDestroy() {
        Debug.Log ("Destroy");
    }
}
```
---
####4. 查找脚本手册，了解 GameObject，Transform，Component 对象:
* 翻译官方对三个对象的描述：

> * **GameObject**: Unity场景中所有实例的基类。
> * **Transform**: 对象的位置改变，旋转和缩放。
> * **Component**: 附着在GameObject上所有事物的基类。
* 描述下图 table 对象（实体）的属性、table 的 Transform 的属性、 table 的部件：
![NO](https://pmlpml.github.io/unity3d-learning/images/ch02/ch02-homework.png)
> UML图
![Answer](http://img2.ph.126.net/nzosFSJdNQSS0ovxTsYnXA==/6632701639979540690.jpg)
---
####5. 整理相关学习资料，编写简单代码验证以下技术的实现:
>* 查找对象：
>   * 通过名字查找：public static GameObject Find(string name)；
>   * 通过标签查找单个对象：public static GameObject FindWithTag(string tag)；
>   * 通过标签查找多个对象：public static GameObject[] FindGameObjectsWithTag(string tag)；
>* 添加对象：public static GameObect CreatePrimitive(PrimitiveTypetype)；
>* 遍历对象树：foreach (Transform child in transform)；
>* 清除所有子对象：foreach (Transform child in transform) { Destroy(child.gameObject)；
---
####6. 资源预设（Prefabs）与 对象克隆 (clone)
>    * 预设是一种灵活定义可视化交互对象的方法。
>    * 预设与克隆都能创建出相同的对象。预设与实例化的对象有关联，而对象克隆本体和克隆出的对象是不相影响的。
>    * 制作table预制并将其实例化：

>```
>public class NewBehaviourScript : MonoBehaviour
>{
>   private void Start()
>   {
>       GameObject table = Instantiate(table_prefab, new Vector3(0, 0, 0));
>   }
>}
>```
---
####7. 解释组合模式（Composite Pattern / 一种设计模式）。使用 BroadcastMessage() 方法向子对象发送消息.
>组合模式允许用户将对象组合成树形结构来表现”部分-整体”的层次结构，使得客户以一致的方式处理单个对象以及对象的组合。组合模式实现的关键地方是——简单对象和复合对象必须实现相同的接口。这就是组合模式能够将组合对象和简单对象进行一致处理的原因。
* 父类对象方法：

```
public class NewBehaviourScript : MonoBehaviour {
    void test() {
        Debug.Log("HelloWorld!");
    }
    void Start () {
        this.BroadcastMessage("test");
    }
}
```
* 子类对象方法：

```
public class NewBehaviourScript1 : MonoBehaviour {
    void test() {
        Debug.Log("HelloWorld!");
    }
}
```
####8. 井字棋小游戏。
[井字棋小游戏](https://segmentfault.com/a/1190000013971785);
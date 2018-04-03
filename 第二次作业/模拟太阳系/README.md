Unity3D - 模拟太阳系
===
>[博客地址](https://segmentfault.com/a/1190000014158774)
![模拟太阳系动态图（如不能显示请点进去访问）](http://img0.ph.126.net/1cTlW0X5BvD2qa9NswfZsg==/6597644811240043059.gif)
---
首先是制作太阳系中的每个行星，基本上都是先创建Sophere，然后改变起始位置，添加材质和贴图，这里就不赘述了。
![模拟太阳系 - 1](http://img2.ph.126.net/6h-k86zP-1VbuMM8R6oxuQ==/6597273176309746922.png)
给每个行星创建材质包：
![行星材质](http://img2.ph.126.net/X61PWGWAJUQdY-Ufd1XFiA==/1650850738508353508.png)


之后就是创建一个行星的移动脚本使得行星绕太阳公转起来，这里需要注意的就是随机选取或者自己设一个参照轴，使得每颗行星公转的法平面不同。
```cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {
    public Transform origin;
    public float speed;
    float ry, rx;
    // Use this for initialization  
    void Start()
    {
        speed = Random.Range(9, 12);
        rx = Random.Range(10, 60);
        ry = Random.Range(10, 60);
    }

    // Update is called once per frame  
    void Update()
    {
        this.transform.RotateAround(origin.position, new Vector3(0, rx, ry), speed * Time.deltaTime);
    }
}
```
将脚本挂载到所有行星上后所有行星就能动起来了。但是行星还不能自转，于是添加一个自转脚本挂载到所有星球上：
```cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.RotateAround(this.transform.position, Vector3.up, Random.Range(1, 3));	
	}
}
```
这时所有的行星的移动就已经搞定了，需要注意的就是月亮绕地球的旋转需要一个单独的脚本，设定以地球为旋转圆心：
```cs
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
        rx = Random.Range(10, 60);//随机选取旋转轴向量
        ry = Random.Range(10, 60);
    }

    // Update is called once per frame  
    void Update()
    {
        this.transform.RotateAround(origin.position, new Vector3(0, rx, ry), speed * Time.deltaTime);
    }
}
```
最后发现太阳系太过孤单，太空怎么能少了星海作为背景？添加一个背景板，贴上星空的图片作为背景美化一下：
![星空背景](http://img0.ph.126.net/n8aG_g6tHv0CdQlKpPdT5A==/6597344644565938944.png)
到这里就大致完成了。
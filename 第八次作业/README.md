## UGUI实现背包系统
### UGUI简介

* UI 即 User Interface（用户界面）的简称。在许多软件中，采用狭义的概念，特指窗体、面板、按钮、文本框等人们熟悉的人机交互元素，及其组织与风格（也称皮肤）。Unity UI 系统采用上述狭义概念。

* Unity 目前支持两套完全不同风格的 UI 系统：
    1. IMGUI（Immediate Mode GUI）及时模式图形界面。它是代码驱动的 UI 系统，即没有图形化设计界面，只能在 OnGUI 阶段用 GUI 系列的类绘制各种 UI 元素。
    2. Unity GUI / UGUI 是面向对象的 UI 系统。所有 UI 元素都是游戏对象，有友好的图形化设计界面， 在场景渲染阶段渲染这些 UI 元素。

* UGUI设计界面可视化，动画效果和元素更加丰富，支持多模式和多摄像机渲染。
---
### 本次任务
使用UGUI实现一个背包系统，包括下列四项主要效果：
1. 实现背包的主要页面。
2. 背包系统中放置物品的格子和物品图片。
3. 实现鼠标拖动物体调整背包。
4. 简单的动画效果，如下列例子：
![官方示例](https://ww3.sinaimg.cn/large/6177e8b1gw1f42sxsw6drg20z80i9npm.gif)
#### 实现效果
>[实现效果视频]()

>[博客地址](https://segmentfault.com/a/1190000015191139)
---
### 制作过程
1. 首先是添加一个Canvas，命名为BackPack并且设置屏幕空间为摄像机，渲染摄像机为主摄像机。
    ![Canvas设置](https://segmentfault.com/img/bVbbT3a?w=2559&h=808)
2. 然后建立背包中所有的格子。通过组件Grid LayOut Group实现自动布局，然后添加Image对象填充。
    ![背包格子布局与制作](https://segmentfault.com/img/bVbbT3a?w=2559&h=808)
    ![背包格子效果](https://segmentfault.com/img/bVbbT3w?w=1627&h=787)
3. 演示GIF中有一个背包和物品栏随鼠标移动的效果，所以仿照这种方法，建立一个脚本获取鼠标位置然后将Bag和Wear两个物品栏的朝向设为鼠标的方向。
    ```cs
        public class FollowMouse : MonoBehaviour {

            public Vector2 range = new Vector2(7f, 5f);
            Transform mTrans;
            Quaternion mStart;
            Vector2 mRot = Vector2.zero;
            void Start()
            {
                mTrans = transform;
                mStart = mTrans.localRotation;
            }
            void Update()
            {
                Vector3 pos = Input.mousePosition;
                float halfWidth = Screen.width * 0.5f;
                float halfHeight = Screen.height * 0.5f;
                float x = Mathf.Clamp((pos.x - halfWidth) / halfWidth, -2f, 2f);
                float y = Mathf.Clamp((pos.y - halfHeight) / halfHeight, -2f, 2f);
                mRot = Vector2.Lerp(mRot, new Vector2(x, y), Time.deltaTime * 5f);
                //旋转方向与鼠标所在方向一致
                mTrans.localRotation = mStart * Quaternion.Euler(-mRot.y * range.y, -mRot.x * range.x, 0f);
            }
        }
    ```
4. 接下来要完成的任务就是同时显示游戏对象和UI界面。
    * 这里首先添加了一个摄像机PlayerCamera，然后调整好位置用来显示玩家的人物：
        ![玩家摄像机](https://segmentfault.com/img/bVbbT3h?w=730&h=582)
    * 然后再分别对两个摄像机进行设置，让主摄像机只显示UI，玩家摄像机显示人物对象，最后再调整摄像机显示的布局Viewport Rect使玩家显示在中间：
        ![主摄像机参数](https://segmentfault.com/img/bVbbT3l?w=592&h=681)
        ![玩家摄像机参数](https://segmentfault.com/img/bVbbT3p?w=595&h=709)
    * 给游戏对象添加动画：
        ![玩家动画](https://segmentfault.com/img/bVbbT3r?w=1221&h=790)
    * 最后是显示效果：
        ![游戏显示效果](https://segmentfault.com/img/bVbbT3O?w=1320&h=640)
5. 这个时候就要开始考虑如何实现拖拽了,先得承认这一部分非常难，读了好久别人的代码才能搬砖的。首先这里有三个与拖拽有关的接口IBeginDragHandler, IDragHandler, IEndDragHandler，实现的拖拽类必须继承这些接口，然后再具体实现。这里的实现方法是拖拽物品的Image对象，其实还考虑过交换图片，但是由于在鼠标点击处创建一个临时对象然后将图片传给它，然后再来传递比较复杂，所以采用了新建了所有存储物品的Image对象，然后直接拖动这些对象的方法。
    * 新建存储了物品的Image列表。
    ![Items](https://segmentfault.com/img/bVbbT30?w=324&h=421)
    * OnBeginDrag函数在开始拖动时调用，设置显示优先级以及记录起点位置。
        ```cs
        canvasGroup.blocksRaycasts = false;//让event trigger忽略自身，这样才可以让event trigger检测到它下面一层的对象,如包裹或物品格子等
        lastEnter = eventData.pointerEnter;
        lastEnterNormalColor = lastEnter.GetComponent<Image>().color;
        originalPosition = myTransform.position;//拖拽前记录起始位置
        gameObject.transform.SetAsLastSibling();//保证当前操作的对象能够优先渲染，即不会被其它对象遮挡住
        ```
    * OnDrag函数实现了Image对象的拖拽效果。
        ```cs
        public void OnDrag(PointerEventData eventData)
        {
            Vector3 globalMousePos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(myRectTransform, eventData.position, eventData.pressEventCamera, out globalMousePos))
            {
                myRectTransform.position = globalMousePos;
            }
            GameObject curEnter = eventData.pointerEnter;
            bool inItemGrid = EnterItemGrid(curEnter);
            if (inItemGrid)
            {
                Image img = curEnter.GetComponent<Image>();
                lastEnter.GetComponent<Image>().color = lastEnterNormalColor;
                if (lastEnter != curEnter)
                {
                    lastEnter.GetComponent<Image>().color = lastEnterNormalColor;
                    lastEnter = curEnter;//记录当前物品格子以供下一帧调用
                }
                //当前格子设置高亮
                img.color = highLightColor;
            }
        }
        ```
    * OnEndDrag函数则是最后停止拖拽时进行判断，分为拖拽到了方格位置，另一个物品对象上和空位置三种情况。分别进行判断处理。
        ```cs
        public void OnEndDrag(PointerEventData eventData)
        {
            GameObject curEnter = eventData.pointerEnter;
            //拖拽到的空区域中（如包裹外），恢复原位
            if (curEnter == null)
            {
                myTransform.position = originalPosition;
            }
            else
            {
                //移动至物品格子上
                if (curEnter.name == "UI_ItemGrid")
                {
                    myTransform.position = curEnter.transform.position;
                    originalPosition = myTransform.position;
                    curEnter.GetComponent<Image>().color = lastEnterNormalColor;//当前格子恢复正常颜色
                }
                else
                {
                    //移动至包裹中的其它物品上
                    if (curEnter.name == eventData.pointerDrag.name && curEnter != eventData.pointerDrag)
                    {
                        Vector3 targetPostion = curEnter.transform.position;
                        curEnter.transform.position = originalPosition;
                        myTransform.position = targetPostion;
                        originalPosition = myTransform.position;
                    }
                    else//拖拽至其它对象上面（包裹上的其它区域）
                    {
                        myTransform.position = originalPosition;
                    }
                }
            }
            lastEnter.GetComponent<Image>().color = lastEnterNormalColor;//上一帧的格子恢复正常颜色
            canvasGroup.blocksRaycasts = true;//确保event trigger下次能检测到当前对象
        }
        ```
### 后续优化
将玩家对象的摄像机改为仅深度，在Game界面显示的对象只有游戏对象。或者取消第二个摄像机，将玩家对象摆在适当的位置，将主摄像机调整为SkyBox，然后再加上背景板也可以实现相同的效果，并且游戏对象不会遮住拖动的物品。
![主摄像机最终参数](https://segmentfault.com/img/bVbbT3L?w=593&h=723)
![最终效果](https://segmentfault.com/img/bVbbT3H?w=1958&h=821)
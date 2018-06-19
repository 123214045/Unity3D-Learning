## Unity3D学习 - P&D有限状态机
---
> [视频地址](https://www.bilibili.com/video/av25201984/)
> [博客地址](https://segmentfault.com/a/1190000015322948)
---
#### 主要代码实现
> 状态转换图
![状态转换图](https://img-blog.csdn.net/20180616193054369?watermark/2/text/aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L0REZ2hzb3Q=/font/5a6L5L2T/fontsize/400/fill/I0JBQkFCMA==/dissolve/70)
1. 本次作业要求实现自动过河的牧师与魔鬼有限状态机。主要是在之前实现过的牧师与魔鬼运动分离版的基础上，对原代码进行修改，以提供方法给自动过河类AutoMove来调用。
2. 本次的重点AutoMove类。首先该类设计为单例模式，通过静态变量autoMove来调用该类的方法。然后声明所需变量。
    ```cs
        public static AutoMove autoMove = new AutoMove();
        public FirstController firstScene;
        private int devilNum;
        private int priestNum;
        private int BoatCoast; // -1 -> left, 1 -> right.
        //P：船运载一个牧师
        //D：船运载一个恶魔
        //PP：船运载两个牧师
        //DD：船运载两个恶魔
        //PD：船运载一个牧师，一个恶魔
        private enum Boataction {empty, P, D, PP, DD, PD }
        private bool isFinished = true;
        private Boataction nextState;
        private int count = 0;
        private int num = 0;

        private AutoMove() { }
    ```
3. 然后是每次点击Next按钮将会直接调用的move函数，该函数首先调用getNext函数得到当前岸边的状态，确认接下来要进行的过程，再调用DoAction函数执行流程。
    ```cs
        public void move()
        {
            if (isFinished)
            {
                isFinished = false;
                Debug.Log(count);
                int[] fromCount = firstScene.fromCoast.GetobjectsNumber();
                priestNum = fromCount[0];
                devilNum = fromCount[1];
                BoatCoast = firstScene.boat.get_State();
                if (count == 0)
                {
                    nextState = getNext();
                    if ((int)nextState >= 3)
                    {
                        num = 2;
                    }
                    else if ((int)nextState > 0) num = 1;
                    else num = 0;
                    count++;
                }
                Debug.Log("next state is " + nextState);
                DoAction();
            }
        }
    ```
4. DoACtion主要是按顺序调用各种模拟点击动作。 
    ```cs
        private void DoAction()
        {
            if (count == 1 && num != 0)
            {
                if (nextState == Boataction.D)
                {
                    devilOnBoat();
                }
                else if (nextState == Boataction.DD)
                {
                    devilOnBoat();
                }
                else if (nextState == Boataction.P)
                {
                    priestOnBoat();
                }
                else if (nextState == Boataction.PP)
                {
                    priestOnBoat();
                }
                else if (nextState == Boataction.PD)
                {
                    priestOnBoat();
                }
                count++;
            }
            else if (num == 2 && count == 2)
            {
                if (nextState == Boataction.DD)
                {
                    devilOnBoat();
                }
                else if (nextState == Boataction.PP)
                {
                    priestOnBoat();
                }
                else if (nextState == Boataction.PD)
                {
                    devilOnBoat();
                }
                count++;
            }
            else if((num == 1 && count == 2) || (num == 2 && count == 3) || (num == 0 && count == 1))
            {
                firstScene.MoveBoat();
                count++;
            }
            else if ((num == 1 && count >= 3) || (num == 2 && count >= 4) || (num == 0 && count>=2))
            {
                GetOffBoat();
            }
            isFinished = true;
        }
    ```
5. DoAction函数进行的动作分为三种，第一种devilOnBoat模拟点击魔鬼让其上船，第二种priestOnBoat模拟点击天使，第三种船移动。
    ```cs
        private void GetOffBoat()
        {
            if((priestNum == 0 && devilNum == 2) || (priestNum == 0 && devilNum == 0))
            {
                if (firstScene.boat.get_State() == -1)
                {
                    foreach (var x in firstScene.boat.passenger)
                    {
                        if (x != null)
                        {
                            firstScene.ObjectIsClicked(x);
                            break;
                        }
                    }
                    if (firstScene.boat.isEmpty()) count = 0;
                }
                else count = 0;
            }
            else if (((priestNum == 0 && devilNum == 1)) && firstScene.boat.get_State() == 1)
            {
                count = 0;
            }
            else
            {
                foreach (var x in firstScene.boat.passenger)
                {
                    if (x != null && x.getType() == 1)
                    {
                        firstScene.ObjectIsClicked(x);
                        count = 0;
                        break;
                    }
                }
                if (count != 0)
                {
                    foreach (var x in firstScene.boat.passenger)
                    {
                        if (x != null)
                        {
                            firstScene.ObjectIsClicked(x);
                            count = 0;
                            break;
                        }
                    }
                }
            }
        }

        private void priestOnBoat()
        {
            if(BoatCoast == 1)
            {
                foreach(var x in firstScene.fromCoast.passengerPlaner)
                {
                    if (x!=null && x.getType() == 0)
                    {
                        firstScene.ObjectIsClicked(x);
                        return;
                    }
                }
            }
            else
            {
                foreach (var x in firstScene.toCoast.passengerPlaner)
                {
                    if (x != null && x.getType() == 0)
                    {
                        firstScene.ObjectIsClicked(x);
                        return;
                    }
                }
            }
        }

        private void devilOnBoat()
        {
            if (BoatCoast == 1)
            {
                foreach (var x in firstScene.fromCoast.passengerPlaner)
                {
                    if (x != null && x.getType() == 1)
                    {
                        firstScene.ObjectIsClicked(x);
                        return;
                    }
                }
            }
            else
            {
                foreach (var x in firstScene.toCoast.passengerPlaner)
                {
                    if (x != null && x.getType() == 1)
                    {
                        firstScene.ObjectIsClicked(x);
                        return;
                    }
                }
            }
        }
    ```
6. 得到下一步的方法根据状态图确认下一步进行的操作和目标。
    ```cs
        private Boataction getNext()
        {
            Boataction next = Boataction.empty;
            if (BoatCoast == 1)
            {
                if (devilNum == 3 && priestNum == 3)//3P3DB
                {
                    next = Boataction.PD;
                }
                else if (devilNum == 2 && priestNum == 3)//3P2DB
                {
                    next = Boataction.DD;
                }
                else if (devilNum == 1 && priestNum == 3)//3P1DB
                {
                    next = Boataction.PP;
                }
                else if (devilNum == 2 && priestNum == 2)//2P2DB
                {
                    next = Boataction.PP;
                }
                else if (devilNum == 3 && priestNum == 0)//3DB
                {
                    next = Boataction.DD;
                }
                else if (devilNum == 1 && priestNum == 1)//1P1DB
                {
                    next = Boataction.PD;
                }
                else if (devilNum == 2 && priestNum == 0)//2DB
                {
                    next = Boataction.D;
                }
                else if (devilNum == 1 && priestNum == 2)//2P1DB
                {
                    next = Boataction.P;
                }
                else if (devilNum == 2 && priestNum == 1)//1P2DB
                {
                    next = Boataction.P;
                }
                else if (devilNum == 1 && priestNum == 0)//1DB
                {
                    next = Boataction.D;
                }
                else if(devilNum == 3 && priestNum == 2)//2P3DB
                {
                    next = Boataction.D;
                }
                else next = Boataction.empty;
            }
            else
            {
                if (devilNum == 2 && priestNum == 2)//2P2D
                {
                    next = Boataction.empty;
                }
                else if (devilNum == 1 && priestNum == 3)//3P1D
                {
                    next = Boataction.empty;
                }
                else if (devilNum == 2 && priestNum == 3)//3P2D
                {
                    next = Boataction.D;
                }
                else if (devilNum == 0 && priestNum == 3)//3P
                {
                    next = Boataction.empty;
                }
                else if (devilNum == 1 && priestNum == 1)//1P1D
                {
                    next = Boataction.D;
                }
                else if (devilNum == 2 && priestNum == 0)//2D
                {
                    next = Boataction.D;
                }
                else if (devilNum == 1 && priestNum == 0)//1D
                {
                    next = Boataction.empty;
                }
                else next = Boataction.empty;
            }
            return next;
        }
    ```
---
> 这次初步实现了简单的“智能”系统，这种系统可以方便的运用于对付简单NPC智能。这次是在之前作业基础上实现的，实现的时候因为第一次没考虑给这次留接口，所以写的时候还是挺困难的，以后要注意一下。
---
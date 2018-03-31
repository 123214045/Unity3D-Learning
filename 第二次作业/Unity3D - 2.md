Unty3D - 2
===
1. 游戏对象运动的本质：
> 游戏对象运动的本质是游戏对象间相对位置的改变。
---
2. 用至少三种方法实现物体的抛物线运动:
    1. 直接改变物体的的位置：Y=X^2
        ```cs
        using System.Collections;
        using System.Collections.Generic;
        using UnityEngine;

        public class movetest : MonoBehaviour {

            // Use this for initialization
            void Start () {
                
            }

            // Update is called once per frame
            void Update () {
                this.transform.position += Vector3.right * Time.deltaTime;
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.x * this.transform.position.x, 0);
            }
        }
        ```
    2. 使用transform.Translate方法，根据加速度改变Y轴上的速度，然后让物体运动方向和速度发生改变实现抛物线：
        ```cs
        using System.Collections;
        using System.Collections.Generic;
        using UnityEngine;

        public class movetest : MonoBehaviour {

            // Use this for initialization
            void Start () {
                
            }
            float t = 1;
            // Update is called once per frame
            void Update () {
                t += Time.deltaTime;
                float speed = 1 * t;
                transform.Translate(transform.forward* 1 *Time.deltaTime, Space.World);
                transform.Translate(transform.up * t * Time.deltaTime, Space.World);
            }
        }
        ```
    3. 将物体定义为刚体，给予重力和初速度实现抛物运动：
        ```cs
        using System.Collections;
        using System.Collections.Generic;
        using UnityEngine;

        public class movetest : MonoBehaviour {

            // Use this for initialization
            void Start () {
                
            }
            
            // Update is called once per frame
            void Update () {
                Rigidbody a = this.gameObject.AddComponent<Rigidbody>();
                a.useGravity = true;
                a.velocity = Vector3.left * 5;
            }
        }
        ```
    4. Vector3.Slerp插值实现：
        ```cs
            using System.Collections;
            using System.Collections.Generic;
            using UnityEngine;

            public class movetest : MonoBehaviour {

                // Use this for initialization
                void Start () {
                    
                }
                Vector3 Speed = new Vector3(10,0,0),SpeedDown = new Vector3(0,0,0);
                // Update is called once per frame
                void Update () {
                    Vector3 NextPosition = transform.position + Speed * Time.deltaTime - SpeedDown * Time.deltaTime;
                    SpeedDown.y += 1*(Time.deltaTime);
                    NextPosition.y -= 0.5F *1* (Time.deltaTime)* (Time.deltaTime);
                    transform.position = Vector3.Slerp(transform.position, NextPosition, 1);
                }
            }
        ```
---
3. 
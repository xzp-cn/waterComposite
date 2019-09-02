using System;
using UnityEngine;

namespace xuexue.common.drag2dtool
{
    /// <summary>
    /// 一个拖拽物体的记录
    /// </summary>
    public class DragRecord
    {
        public DragRecord(Transform obj, Camera camera)
        {
            dragObj = obj;
            dragControlPoint = camera.WorldToScreenPoint(obj.position);//三维物体坐标转屏幕坐标
            caluCamera = camera;
            offset = obj.position - caluCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dragControlPoint.z));//三维物体坐标转屏幕坐标
        }

        /// <summary>
        /// 拖拽物体
        /// </summary>
        public Transform dragObj;

        /// <summary>
        /// 射线检测打到物体的点(屏幕坐标)
        /// </summary>
        private Vector3 dragControlPoint;

        /// <summary>
        /// 射线检测打到物体的点-拖拽物体之间的距离
        /// </summary>
        private Vector3 offset;

        /// <summary>
        /// 计算使用的基准相机
        /// </summary>
        private Camera caluCamera;

        /// <summary>
        /// 停止拖拽的事件
        /// </summary>
        public event Action<DragRecord> EventStopDrag;

        /// <summary>
        /// 是否停止拖拽
        /// </summary>
        public bool isStop = false;

        /// <summary>
        /// 当鼠标这个按钮抬起的时候停止拖拽
        /// </summary>
        private int mouseBtn = -1;

        /// <summary>
        /// 拖拽过程中的回调事件
        /// </summary>
        public System.Action<DragRecord> mouseMoveDrag;

        /// <summary>
        /// 设置由哪个按键抬起来终止拖拽，产生Stop Event,并且设置回调。
        /// key值为0,1,2
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="key">按键值0,1,2</param>
        /// <returns></returns>
        public DragRecord SetStopWhenMouseUp(Action<DragRecord> proc, int MouseBtn)
        {
            if (mouseBtn != -1)//只能被设置一次
            {
                return this;
            }
            mouseBtn = MouseBtn;//设置参数到成员字段
            EventStopDrag += proc;
            EventStopDrag += DebugRelease;
            return this;
        }

        /// <summary>
        /// 拖拽回调事件
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="MouseBtn"></param>
        /// <returns></returns>
        public DragRecord SetOnMouseMove(Action<DragRecord> proc, int MouseBtn)
        {
            if (mouseBtn != -1)//只能被设置一次
            {
                return this;
            }
            mouseBtn = MouseBtn;//设置参数到成员字段
            mouseMoveDrag = proc;
            return this;
        }

        /// <summary>
        /// 使用当前的触摸状态去更新这个拖拽物体的位置
        /// </summary>
        internal void Update()
        {
            if (caluCamera == null || isStop)
            {
                return;
            }
            //得到现在鼠标的2维坐标系位置
            Vector3 curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dragControlPoint.z);
            //将当前鼠标的2维位置转换成3维位置，再加上鼠标的移动量
            Vector3 curPosition = caluCamera.ScreenToWorldPoint(curScreenSpace) + offset;
            //curPosition就是物体应该的移动向量赋给transform的position属性
            dragObj.position = curPosition;

            mouseMoveDrag?.Invoke(this);

            if (mouseBtn == -1)
            {
                mouseBtn = 0;
            }
            if (Input.GetMouseButtonUp(mouseBtn))
            {
                try
                {
                    EventStopDrag?.Invoke(this);
                }
                catch (Exception e)
                {
                    Debug.Log("DragRecord.Update():执行用户事件EventStopDrag异常" + e.Message);
                }
                isStop = true;
            }
        }

        void DebugRelease(DragRecord pro)
        {
            Debug.Log("DragRecord.Update():执行执行用户事件EventStopDrag:" + dragObj.name);
        }

    }
}

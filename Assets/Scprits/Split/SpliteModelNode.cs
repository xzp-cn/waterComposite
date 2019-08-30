
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author       : Running
** Time         : 18.6.25
** Describtion  : 拆分单个模型
*/

namespace Runing.common.splitetool
{
    /// <summary>
    /// 一个可拆分单元的数据，现在记录了坐标信息。
    /// </summary>
    internal struct UnitData
    {
        /// <summary>
        /// 部件对象
        /// </summary>
        public Transform nodeTransform;

        /// <summary>
        /// 部件的局部坐标
        /// </summary>
        public Vector3 localPosition;

        //旋转记录（待加）

        public Vector3 localAngle;
    };

    public class SpliteModelNode : MonoBehaviour
    {
        /// <summary>
        /// 视口坐标端点（左下）
        /// </summary>
        private Vector3 m_LeftBottomPoint = new Vector3(0.1f, 0.1f, 0);

        /// <summary>
        /// 视口坐标端点（右上）
        /// </summary>
        private Vector3 m_RightTopPoint = new Vector3(0.9f, 0.9f, 0);

        /// <summary>
        /// 吸附距离
        /// </summary>
        private float m_AbsorbDistance = 0.5f;

        /// <summary>
        /// 储存部件的起始位置，key值为这个子节点本身Transform
        /// </summary>
        private Dictionary<Transform, UnitData> m_UnitOriginDataDic = new Dictionary<Transform, UnitData>();

        /// <summary>
        /// 储存部件的随机位置，key值为这个子节点本身Transform
        /// </summary>
        private Dictionary<Transform, UnitData> m_UnitSpliteDataDic = new Dictionary<Transform, UnitData>();

        /// <summary>
        /// 移动时间
        /// </summary>
        private float m_MoveTime = 0.5f;

        /// <summary>
        /// 照物体的摄像机
        /// </summary>
        private Camera m_Camera;

        /// <summary>
        /// 部件原始位置记录字典
        /// </summary>
        internal Dictionary<Transform, UnitData> UnitOriginDataDic { get { return m_UnitOriginDataDic; } }

        /// <summary>
        /// 先设置模型数据，再进行其他操作。
        /// </summary>
        /// <param name="camera">摄像机</param>
        /// <param name="transform">当前操作的模型对象</param>
        /// <param name="units">能被分离的部件列表</param>
        public void SetNodeInfo(Camera camera, Transform[] units)
        {
            m_Camera = camera;
            m_UnitOriginDataDic.Clear();
            m_UnitSpliteDataDic.Clear();

            //取部件信息来保存
            for (int i = 0; i < units.Length; i++)
            {
                UnitData data = new UnitData();

                if (null != units[i])
                {
                    data.localPosition = units[i].localPosition;
                    data.localAngle = units[i].localEulerAngles;
                    data.nodeTransform = units[i];

                    m_UnitOriginDataDic.Add(units[i], data);
                    m_UnitSpliteDataDic.Add(units[i], data);
                }
                else
                {
                    Debug.LogError("SpliteModelNode.SetNodeInfo(): 部件列表中有空对象");
                }
            }
        }

        /// <summary>
        /// 分离部件
        /// </summary>
        public void Splite()
        {
            //获取随机位置并移动
            foreach (var kv in m_UnitOriginDataDic)
            {
                UnitData unitData = kv.Value;
                Vector3 randomPos = GetRandomPosition(unitData);
                unitData.localPosition = unitData.nodeTransform.parent.InverseTransformPoint(randomPos);
                m_UnitSpliteDataDic[kv.Key] = unitData;

                unitData.nodeTransform.DOMove(randomPos, m_MoveTime);
            }
        }

        /// <summary>
        /// 恢复部件
        /// </summary>
        public void Restore(Action callBack = null)
        {
            foreach (var kv in m_UnitSpliteDataDic)
            {
                if (!m_UnitOriginDataDic.ContainsKey(kv.Key))
                {
                    Debug.LogError("SpliteModelNode.Restore(): 字典中不存在部件名字");
                }
                else
                {
                    Vector3 originPosition = m_UnitOriginDataDic[kv.Key].localPosition;
                    Vector3 originAngle= m_UnitOriginDataDic[kv.Key].localAngle;
                    if (originPosition != kv.Value.nodeTransform.position)//如果坐标移动出去了再恢复
                        kv.Value.nodeTransform.DOLocalMove(originPosition, m_MoveTime);
                    if (originPosition != kv.Value.nodeTransform.position)//如果旋转变了再恢复
                        kv.Value.nodeTransform.DOLocalRotate(originAngle, m_MoveTime);
                }
            }


            //全部恢复完成后，调用回调通知
            if (callBack != null)
            {
                StartCoroutine(ReStoreCallBackHandler(callBack));
            }
        }

        
        
        public IEnumerator ReStoreCallBackHandler(Action action)
        {
            yield return new WaitForSeconds(m_MoveTime);
            action?.Invoke();
        }

        /// <summary>
        /// 在屏幕中生成随机位置
        /// </summary>
        /// <param name="originUnitData">原始坐标数据</param>
        /// <returns></returns>
        private Vector3 GetRandomPosition(UnitData originUnitData)
        {
            Vector3 unitPosition = originUnitData.nodeTransform.TransformPoint(originUnitData.localPosition);
            Vector3 randomPosition = Vector3.zero;

            do
            {
                float rangeX = UnityEngine.Random.Range(m_LeftBottomPoint.x, m_RightTopPoint.x);
                float rangeY = UnityEngine.Random.Range(m_LeftBottomPoint.y, m_RightTopPoint.y);
                float z = m_Camera.WorldToViewportPoint(unitPosition).z;//深度z不改变
                randomPosition = m_Camera.ViewportToWorldPoint(new Vector3(rangeX, rangeY, z));
            }//如果随机距离小于吸附距离就再次随机
            while (m_AbsorbDistance > Vector3.Distance(unitPosition, randomPosition));

            return randomPosition;
        }

        /// <summary>
        /// 当前部件是否处在应该被吸附回起始点的位置
        /// </summary>
        /// <param name="unit">部件</param>
        /// <returns></returns>
        public bool IsAbsorb(Transform unit)
        {
            return IsAbsorb(unit, unit.position);
        }

        /// <summary>
        /// 是否能吸附
        /// </summary>
        /// <param name="unitName">部件名字</param>
        /// <param name="worldPosition">部件的世界坐标</param>
        /// <returns></returns>
        private bool IsAbsorb(Transform unit, Vector3 worldPosition)
        {
            if (!m_UnitOriginDataDic.ContainsKey(unit))
                return false;

            Vector3 originPosition = unit.parent.TransformPoint(m_UnitOriginDataDic[unit].localPosition);
            float distance = Vector3.Distance(originPosition, worldPosition);
            return (m_AbsorbDistance > distance ? true : false);
        }

        /// <summary>
        /// 吸附 操作
        /// </summary>
        /// <param name="unit"></param>
        internal void Absorb(Transform unit)
        {
            if (!m_UnitOriginDataDic.ContainsKey(unit))
                return;

            Vector3 originPosition = m_UnitOriginDataDic[unit].localPosition;
            Vector3 originAngle = m_UnitOriginDataDic[unit].localAngle;
            Vector3 currentPosition = unit.position;
            unit.DOLocalMove(originPosition, m_MoveTime);
            unit.DOLocalRotate(originAngle, m_MoveTime);
        }

        /// <summary>
        /// 脚本被销毁时就清除SpliteTool中的对应记录
        /// </summary>
        private void OnDestroy()
        {
            SpliteTool.Instance.DeleteModelObject(this.transform);
        }
    }
}
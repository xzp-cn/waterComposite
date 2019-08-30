using Runing.common.splitetool;
using System;
using System.Collections.Generic;
using UnityEngine;

/* Author		: Running
** Time			: 18.6.25
** Describtion	: 分离模型的入口
*/

namespace Runing
{
    public class SpliteTool
    {
        public static SpliteTool Instance { get; } = new SpliteTool();

        private Dictionary<Transform, SpliteModelNode> m_NodeDataDic = new Dictionary<Transform, SpliteModelNode>();

        int splitPartCount = 0;

        List<Transform> alreadyRestoreObjs = new List<Transform>();

        /// <summary>
        /// 先调用此函数设置模型数据，再进行其他操作。会创建一个SpliteModelNode脚本挂载到modelObject物体上。
        /// </summary>
        /// <param name="camera">照相机</param>
        /// <param name="modelObject">当前操作的模型对象</param>
        /// <param name="units">能被分离的部件列表</param>
        public void SetObjectAndInfo(Camera camera, Transform modelObject, Transform[] units)
        {
            if (null == camera || null == modelObject || null == units)
                return;

            SpliteModelNode node = null;
            if (!modelObject.GetComponent<SpliteModelNode>())
            {
                node = modelObject.gameObject.AddComponent<SpliteModelNode>();
            }
            else
            {
                node = modelObject.gameObject.GetComponent<SpliteModelNode>();
            }

            node.SetNodeInfo(camera, units);
            m_NodeDataDic[modelObject] = node;
            splitPartCount = units.Length;
            alreadyRestoreObjs = new List<Transform>();
        }

        /// <summary>
        /// 模型分离
        /// </summary>
        /// <param name="modelObject"></param>
        public void Splite(Transform modelObject)
        {
            if (m_NodeDataDic.ContainsKey(modelObject))
            {
                m_NodeDataDic[modelObject].Splite();
            }
            else
            {
                Debug.LogWarning("SpliteTool.Splite():  不存在此对象");
            }
        }

        /// <summary>
        /// 还原
        /// </summary>
        /// <param name="modelObject">当前操作的模型对象</param>
        public void Restore(Transform modelObject,Action action = null)
        {
            if (m_NodeDataDic.ContainsKey(modelObject))
            {
                m_NodeDataDic[modelObject].Restore(action);
            }
            else
            {
                Debug.LogWarning("SpliteTool.Restore():  不存在此对象");
            }
        }

        /// <summary>
        /// 部件是否能吸附回起始点
        /// </summary>
        /// <param name="unit">部件对象</param>
        /// <returns>是否吸附成功</returns>
        public bool IsAbsorb(Transform unit)
        {
            bool isAbsorb = false;
            Transform modelObject = GetModelObjectWithUnit(unit);

            if (null == modelObject || !m_NodeDataDic.ContainsKey(modelObject) || null == unit)
            {
                Debug.LogWarning("SpliteTool.IsAbsorb():  不存在此对象");
            }
            else
            {
                isAbsorb = m_NodeDataDic[modelObject].IsAbsorb(unit);
            }

            if (!isAbsorb) //没有吸附，则判断是否是从已吸附的地方拖出部件
            {
                if (alreadyRestoreObjs.Contains(unit))
                {
                    alreadyRestoreObjs.Remove(unit);
                }
            }
            return isAbsorb;
        }

        /// <summary>
        /// 吸附回起始位置
        /// </summary>
        /// <param name="unit">部件节点</param>
        public void Absorb(Transform unit,Action restoreOverCallBack = null)
        {
            Transform modelObject = GetModelObjectWithUnit(unit);
            if (null == modelObject || !m_NodeDataDic.ContainsKey(modelObject) || null == unit)
            {
                Debug.LogWarning("SpliteTool.Absorb():  不存在此对象");
                return;
            }
            else
            {
                //吸附的同时，判断是否是完成吸附
                m_NodeDataDic[modelObject].Absorb(unit);
                if (!alreadyRestoreObjs.Contains(unit))
                {
                    alreadyRestoreObjs.Add(unit);
                    if (alreadyRestoreObjs.Count == splitPartCount)
                    {
                        restoreOverCallBack?.Invoke();
                    }
                }               
            }
        }

        /// <summary>
        /// 删除模型对象
        /// </summary>
        /// <param name="modelObject"></param>
        internal void DeleteModelObject(Transform modelObject)
        {
            if (m_NodeDataDic.ContainsKey(modelObject))
            {
                m_NodeDataDic.Remove(modelObject);
            }
        }

        /// <summary>
        /// 获取部件对应的根节点模型对象
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        private Transform GetModelObjectWithUnit(Transform unit)
        {
            foreach (var NodeKV in m_NodeDataDic)
            {
                SpliteModelNode spliteModelNode = NodeKV.Value;//根节点
                foreach (var unitKV in spliteModelNode.UnitOriginDataDic)
                {
                    UnitData unitData = unitKV.Value;
                    if (unitKV.Value.nodeTransform == null)
                    {
                        //可能应该删掉这条记录
                        Debug.LogError("SpliteTool.GetModelObjectWithUnit(): 模型中一部分被删除了！");
                    }
                    if (unit == unitData.nodeTransform)
                    {
                        return spliteModelNode.transform;
                    }
                }
            }
            return null;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xuexue.common.drag2dtool
{

    /// <summary>
    /// 2D拖拽辅助工具类的Obj
    /// </summary>
    public class Drag2DToolObj : MonoBehaviour
    {

        /// <summary>
        /// 记录所有拖拽物体的字典
        /// </summary>
        internal Dictionary<GameObject, DragRecord> _dictDrag = new Dictionary<GameObject, DragRecord>();

        /// <summary>
        /// 计算使用的基准相机
        /// </summary>
        //internal Camera caluCamera;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //if (caluCamera == null)
            //{
            //    return;
            //}
            List<GameObject> needDelete = null;
            foreach (var kvp in _dictDrag)
            {
                if (kvp.Key != null)
                {
                    kvp.Value.Update();
                    if (kvp.Value.isStop)
                    {
                        if (needDelete == null)
                        {
                            needDelete = new List<GameObject>();
                        }
                        needDelete.Add(kvp.Key);
                    }
                }
            }

            if (needDelete != null)
            {
                foreach (var item in needDelete)
                {
                    _dictDrag.Remove(item);
                }
            }
        }
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace operatemodeltool
{
    /// <summary>
    /// 2D模型操作辅助功能（放大、缩小、旋转）
    /// </summary>
    public class OperationModelTool
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static OperationModelTool Instance { get; } = new OperationModelTool();

        /// <summary>
        /// 构造函数
        /// </summary>
        private OperationModelTool()
        {
            //下面默认使用了目前模块自带的绘制系统
            _operationModelTool = new GameObject("OperationModelTool");

            //设置这个物体不被销毁
            UnityEngine.Object.DontDestroyOnLoad(_operationModelTool);
            _mono = _operationModelTool.AddComponent<OperationModelToolObj>();//挂上一个BoundsUpdate脚本
        }

        /// <summary>
        /// 这个类使用的辅助的U3D的Object对象，用来实时Update
        /// </summary>
        GameObject _operationModelTool;
        OperationModelToolObj _mono;

        
        /// <summary>
        /// 添加物体进行缩放
        /// </summary>
        /// <param name="gos"></param>
        /// <param name="scale"></param>
        public void AddScaleObject(List<Transform> gos)
        {
            _mono.AddScaleObjs(gos);
        }

        public void SetObjectScale(List<Transform> gos, float scale)
        {
            _mono.SetScaleObj(gos, scale);
        }

        public void DeleScaleObject(List<Transform> gos)
        {
            _mono.DeleteScaleObj(gos);
        }

        public int GetScaleObjCount()
        {
            return _mono.modelCount;
        }

        /// <summary>
        /// 添加物体进行旋转
        /// </summary>
        /// <param name="gos"></param>
        /// <param name="scale"></param>
        public void AddRotaObject(GameObject go)
        {
            _mono._rotaModel = go;
        }

       

        public void DeleRotaObject()
        {
            _mono._rotaModel = null;
        }
    }
}

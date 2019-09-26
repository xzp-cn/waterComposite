using UnityEngine;

namespace xuexue.common.drag2dtool
{
    /// <summary>
    /// 2D拖拽的辅助功能
    /// </summary>
    public class Drag2DTool
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static Drag2DTool Instance { get; } = new Drag2DTool();

        /// <summary>
        /// 构造函数
        /// </summary>
        private Drag2DTool()
        {
            //下面默认使用了目前模块自带的绘制系统
            _drag2DToolObj = new GameObject("Drag2DTool");

            //设置这个物体不被销毁
            UnityEngine.Object.DontDestroyOnLoad(_drag2DToolObj);
            _mono = _drag2DToolObj.AddComponent<Drag2DToolObj>();//挂上一个BoundsUpdate脚本
        }

        /// <summary>
        /// 这个类使用的辅助的U3D的Object对象，用来实时Update
        /// </summary>
        GameObject _drag2DToolObj;
        Drag2DToolObj _mono;

        /// <summary>
        /// 设置这个工具使用的相机
        /// </summary>
        /// <param name="camera"></param>
        public void SetCamera(Camera camera)
        {
            //_mono.caluCamera = camera;
        }

        /// <summary>
        /// 添加一个拖拽物体
        /// </summary>
        /// <param name="go"></param>
        /// <param name="dragControlPoint">射线检测打到物体的点</param>
        /// <returns></returns>
        public DragRecord addDragObj(GameObject go, Camera camera)
        {
            DragRecord dr = new DragRecord(go.transform, camera);
            if (!_mono._dictDrag.ContainsKey(go))
            {
                _mono._dictDrag.Add(go, dr);
            }

            return _mono._dictDrag[go];
        }


        /// <summary>
        /// 主动删除拖拽物体
        /// </summary>
        public void deleteDragObj(GameObject go)
        {
            if (go != null)
            {
                DragRecord dr;
                if (_mono._dictDrag.TryGetValue(go, out dr))
                {
                    dr.isStop = true;
                }
            }
        }
        /// <summary>
        /// 清空所有的拖拽物体
        /// </summary>
        public void clearDragObj()
        {
            _mono._dictDrag.Clear();
        }

        /// <summary>
        /// 添加一个物体到缩放里面
        /// </summary>
        /// <param name="go"></param>
        public void AddScaleObject(GameObject go)
        {

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSpace;
using liu;

namespace operatemodeltool
{
    public class OperationModelToolObj : MonoBehaviour
    {
        //传入的物体以及初始的Scale字典
        internal List<Transform> _listScale = new List<Transform>();
        GameObject[] _modes;
        GameObject _rotationGo;
        Vector3 _modelCenterWorldPos;
        Vector3 _initMousePos;
        Vector3 _curInputMoudlePos;

        public int modelCount
        {
            get
            {
                int count = 0;
                if (_modes == null)
                {
                    return count;
                }
                foreach (var item in _modes)
                {
                    if (item == null)
                    {
                        _modes = null;
                        _listScale.Clear();
                        return 0;
                    }
                }
                return _modes.Length;
            }
        }

        //GlobalConfig globalConfig;
        //GlobalConfig _globalConfig
        //{
        //    get
        //    {
        //        if (globalConfig == null)
        //        {
        //            globalConfig = GameObject.FindObjectOfType<GlobalConfig>();
        //        }
        //        return globalConfig;
        //    }
        //}

        internal GameObject _rotaModel
        {
            get
            {
                return _rotationGo;
            }
            set
            {
                _rotationGo = value;
                if (_rotationGo != null)
                {
                    //var e = ECSHelper.GetCWModel(_rotationGo);
                    //if (e.cMOperation.mode == (int)OperationMode.scale)
                    //{
                    //    var boundsLocal = _rotationGo.GetBoundsLocal();
                    //    _modelCenterWorldPos = _rotationGo.transform.TransformPoint(boundsLocal.center);

                    //}
                    //else
                    //{
                        var boundsLocal = new GameObject[] { _rotationGo }.GetBoundsLocal();
                        _modelCenterWorldPos = _rotationGo.transform.TransformPoint(boundsLocal.center);
                    //}
                    if (!Monitor23DMode.instance.is3D)
                    {
                        _initMousePos = Input.mousePosition;
                    }
                    else
                    {
                        _initMousePos = FCore.penPosition*1000;
                    }
                }
            }
        }
        //这个是模型出生的高度
        float _modelHeight = 0.5f;
        public float _speed = 0.8f;
        float _x_dis = 0;
        float _y_dis = 0;

        // Use this for initialization
        void Start()
        {
            // _globalConfig = GameObject.FindObjectOfType<GlobalConfig>();
        }


        // Update is called once per frame
        void Update()
        {
            if (_rotaModel != null)
            {
                RotaModel();
            }
            if (Input.GetMouseButtonUp(0))
            {

                _rotaModel = null;
            }
        }


        void RotaModel()
        {
            _x_dis = GetAxis("Mouse X");
            _y_dis = GetAxis("Mouse Y");
            _rotaModel.transform.RotateAround(_modelCenterWorldPos, Vector3.up, -_x_dis);
            _rotaModel.transform.RotateAround(_modelCenterWorldPos, Vector3.right, _y_dis);
        }

        //获取鼠标或者触摸的偏移，类似Input.GetAxis功能
        float GetAxis(string axis)
        {
            if (Monitor23DMode.instance.is3D)
            {
                _curInputMoudlePos = FCore.penPosition * 1000;
            }
            else
            {
                _curInputMoudlePos = Input.mousePosition;
            }
            if (axis == "Mouse X")
            {
                _initMousePos.x = Mathf.Lerp(_initMousePos.x, _curInputMoudlePos.x, _speed);
                if (Mathf.Abs(_initMousePos.x - _curInputMoudlePos.x) < 0.001f)
                {
                    _initMousePos.x = _curInputMoudlePos.x;
                }
                return _curInputMoudlePos.x - _initMousePos.x;
            }
            else if (axis == "Mouse Y")
            {
                _initMousePos.y = Mathf.Lerp(_initMousePos.y, _curInputMoudlePos.y, _speed);
                if (Mathf.Abs(_initMousePos.y - _curInputMoudlePos.y) < 0.001f)
                {
                    _initMousePos.y = _curInputMoudlePos.y;
                }
                return _curInputMoudlePos.y - _initMousePos.y;
            }
            return 0;
        }

        public void AddScaleObjs(List<Transform> transforms)
        {
            foreach (var item in transforms)
            {
                if (!_listScale.Contains(item))
                {
                    _listScale.Add(item);
                }
            }
            List<GameObject> gos = new List<GameObject>();
            foreach (var item in _listScale)
            {
                var go = item.gameObject;
                gos.Add(go);
            }
            _modes = gos.ToArray();
        }

        /// <summary>
        /// 设置物体的缩放
        /// </summary>
        /// <param name="transforms"></param>
        /// <param name="scale"></param>
        public void SetScaleObj(List<Transform> transforms, float sliderScale)
        {
            if (_modes == null) return;
            foreach (var item in _modes)
            {
                if (item == null)
                {
                    _modes = null;
                    _listScale.Clear();
                    return;
                }
            }
            foreach (var item in _listScale)
            {            
                var boundsLocal = _modes.GetBoundsLocal();
                float x = boundsLocal.GetXLen();
                float y = boundsLocal.GetYLen();
                float z = boundsLocal.GetZLen();
                var minLength = Mathf.Max(x, Mathf.Max(y, z));
                var scale = _modelHeight / minLength;
                //变化范围在原始大小的0.25倍到2.25倍
                var tempScale = sliderScale * 2f + 0.25f;
                //var modelScale = tempScale * initScale.x;
                var modelScale = tempScale * scale;
                if (tempScale > 2.25 || tempScale < 0.25f)
                    continue;
                var tran = item;
                tran.localScale = Vector3.one * modelScale;
                //tran.GetComponentInParent<BoxCollider>().size =  new Vector3(boundsLocal.GetXLen(), boundsLocal.GetYLen(), boundsLocal.GetZLen()) * tempScale;
            }
        }

        public void DeleteScaleObj(List<Transform> transforms)
        {
            for (int i = 0; i < transforms.Count; i++)
            {
                var item = transforms[i];
                if (_listScale.Contains(item))
                {
                    _listScale.Remove(item);
                }
            }
        }

        //剔除标签以免影响Bounds中心点
        //private GameObject[] TranGetChild(Transform tran, GameEntity entity)
        //{
        //    var listNote = entity.cWModel.model.info.listNode;
        //    var nodeNames = new List<string>();
        //    foreach (var item in listNote)
        //    {
        //        nodeNames.Add(item.name);
        //    }

        //    Queue<GameObject> result = new Queue<GameObject>();
        //    Queue<GameObject> queue = new Queue<GameObject>();//BFS队列
        //    queue.Enqueue(tran.gameObject);
        //    while (queue.Count > 0)
        //    {
        //        GameObject front = queue.Dequeue();
        //        var childName = front.name;
        //        if (nodeNames.Contains(childName)) continue;//这个节点是标签了，直接跳到下一个queue
        //        result.Enqueue(front.gameObject);
        //        for (int i = 0; i < front.transform.childCount; i++)
        //        {
        //            queue.Enqueue(front.transform.GetChild(i).gameObject);
        //        }
        //    }
        //    return result.ToArray();
        //}
    }
}
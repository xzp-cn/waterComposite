using liu;
using operatemodeltool;
using UnityEngine;
using xuexue.common.drag2dtool;

namespace FSpace
{
    /// <summary>
    /// 简单拖拽的示例
    /// </summary>
    public class SimpleDrag : MonoBehaviour
    {

        /// <summary>
        /// 创建的笔的射线物体
        /// </summary>
        GameObject _penObj;

        [HideInInspector]
        public PenRay tempPenRay;

        /// <summary>
        /// 是否在点击的时候震动一下
        /// </summary>
        public bool enableShake = true;



        Camera camera2D;
        void Start()
        {
            //设置屏幕为3D显示模式
            // FCore.SetScreen3D();

            FCore.EventKey0Down += OnKey0Down;
            FCore.EventKey0Up += OnKey0Up;

            FCore.EventKey1Down += OnKey0Down;
            FCore.EventKey1Up += OnKey0Up;

            _penObj = new GameObject("penRay");
            tempPenRay = _penObj.AddComponent<PenRay>();


            //通过3DUI物体找到挂在在上面的UIButton3D脚本。
            // uibutton3d = FindObjectOfType<UIButton3D>();
            camera2D = Monitor23DMode.instance.camera2D;
        }

        void OnApplicationQuit()
        {
            //在程序退出的时候设置屏幕为2D显示
            FCore.SetScreen2D();
        }

        /// <summary>
        /// 记录当前拖拽的物体
        /// </summary>
        [HideInInspector]
        public GameObject _curDragObj;

        private GameObject Raycast(out RaycastHit raycastHit)
        {
            if (Physics.Raycast(FCore.penRay, out raycastHit, tempPenRay.rayLength))
            {
                return raycastHit.collider.gameObject;
            }
            return null;
        }


        /// <summary>
        /// 3D 拖拽或者移动
        /// </summary>
        private void OnKey0Down()
        {
            RaycastHit raycastHit;
            GameObject dragObj = Raycast(out raycastHit);

            if (dragObj != null)
            {
                if (Monitor23DMode.instance.is3D)
                {
                    _curDragObj = dragObj;

                    bool isClick = false;
                    if (ClickAction != null)
                    {
                        isClick = ClickAction(dragObj);
                        if (isClick)
                        {
                            return;
                        }
                    }

                    if (GlobalConfig.Instance.operationModel == OperationModel.Move)//移动物体
                    {

                        //添加抓取的物体
                        FCore.addDragObj(_curDragObj, raycastHit.distance, true);
                    }
                    else if (GlobalConfig.Instance.operationModel == OperationModel.Rotate)//旋转物体
                    {
                        OperationModelTool.Instance.AddRotaObject(_curDragObj);
                    }

                    if (enableShake)
                    {
                        FCore.PenShake();//震动一下
                    }

                    GlobalConfig.Instance._curOperateObj = _curDragObj;

                }
            }
        }

        public void OnKey0Up()
        {
            //移出抓取的物体
            FCore.deleteDragObj(_curDragObj);

            //移除旋转的物体
            OperationModelTool.Instance.DeleRotaObject();

            _curDragObj = null;
        }

        private void Update()
        {
            if (Monitor23DMode.instance.is3D == false)//这个判断不需要 如果需要在2/3D都能用鼠标拖拽的话
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Drag2DObj();
                }

                if (Input.GetMouseButtonUp(0))
                {
                    OnMouseBtnUp();
                }
            }
        }

        void OnMouseBtnUp()
        {
            Drag2DTool.Instance.clearDragObj();
            OperationModelTool.Instance.DeleRotaObject();
            _curDragObj = null;
        }

        public System.Func<GameObject, bool> ClickAction;
        void Drag2DObj()
        {
            RaycastHit raycastHit;
            //int defaultLayer = LayerMask.NameToLayer("Default");//这个层是模型
            Ray ray = Monitor23DMode.instance.camera2D.ScreenPointToRay(Input.mousePosition);
            var uiDis = 1000f;//鼠标到UI的距离
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                uiDis = Monitor23DMode.instance.f3DSpaceInputModule.hitUIDis;
            }
            GameObject dragObj = null;
            if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity/*, 1 << defaultLayer*/))
            {
                if (uiDis < raycastHit.distance)//通过鼠标到UI跟鼠标到物体的距离判断是否进行对模型操作
                {
                    return;
                }
                dragObj = raycastHit.collider.gameObject;
                _curDragObj = dragObj;

                bool isClick = false;
                if (ClickAction != null)
                {
                    isClick = ClickAction(dragObj);
                    if (isClick)
                    {
                        return;
                    }
                }

                if (GlobalConfig.Instance.operationModel == OperationModel.Move)
                {
                    Drag2DTool.Instance.addDragObj(_curDragObj, camera2D);

                }
                else if (GlobalConfig.Instance.operationModel == OperationModel.Rotate)
                {
                    OperationModelTool.Instance.AddRotaObject(_curDragObj);
                }

                GlobalConfig.Instance._curOperateObj = _curDragObj;
            }
        }

    }
}

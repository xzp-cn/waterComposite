
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 调整AR相机渲染的RawImage大小，以适应AR相机视口大小
/// </summary>
public class ReSizeARImg : MonoBehaviour

{

    public float _farDistance = 10;//远视口距离

    public float _nearDistance = 3;//近视口距离



    private Camera _camera;

    private Transform _camTrans;


    private RectTransform _imgTrans;

    void Start()

    {

        _camera =transform.GetComponentInParent<Camera>();

        _camTrans = _camera.transform;

        ResizeImage();


    }

    void ResizeImage()
    {
        float distance = Vector3.Distance(_camTrans.position, transform.position);

        float farClipPlane = _camera.farClipPlane;

        float nearClipPlane = _camera.nearClipPlane;

        var initWorldPos = transform.position;

        initWorldPos.z = Mathf.Clamp(initWorldPos.z, nearClipPlane, farClipPlane);

        var initTransformLocalPos = transform.localPosition;

        transform.localPosition = new Vector3(0, 0, initTransformLocalPos.z);

        _imgTrans = transform.GetComponentInChildren<RawImage>().transform as RectTransform;

        _imgTrans.localPosition = Vector3.zero;

        distance = Vector3.Distance(_camTrans.position, transform.position);

        Vector3[] corners = GetCorners(distance);

        Vector3 point1 = corners[0]; //UpperLeft
        Vector3 point2 = corners[1]; //LowerRight
        Vector3 point3 = corners[3]; //LowerRight

        point1 = transform.InverseTransformPoint(point1);
        point2 = transform.InverseTransformPoint(point2);
        point3 = transform.InverseTransformPoint(point3);

        float width = Vector3.Distance(point1, point2);
        float height = Vector3.Distance(point2, point3);
        _imgTrans.sizeDelta = new Vector2(width, height);
    }

    void OnDrawGizmos()

    {

        //OnDrawFarView();

        //OnDrawNearView();

        //OnDrawFOV();

    }



    //远视口

    void OnDrawFarView()

    {

        Vector3[] corners = GetCorners(_farDistance);



        // for debugging

        Debug.DrawLine(corners[0], corners[1], Color.yellow); // UpperLeft -> UpperRight

        Debug.DrawLine(corners[1], corners[3], Color.yellow); // UpperRight -> LowerRight

        Debug.DrawLine(corners[3], corners[2], Color.yellow); // LowerRight -> LowerLeft

        Debug.DrawLine(corners[2], corners[0], Color.yellow); // LowerLeft -> UpperLeft





        //中心线

        Vector3 vecStart = _camTrans.transform.position;

        Vector3 vecEnd = vecStart;

        vecEnd += _camTrans.forward * _farDistance;

        Debug.DrawLine(vecStart, vecEnd, Color.red);

    }



    //近视口

    void OnDrawNearView()

    {

        Vector3[] corners = GetCorners(_nearDistance);



        // for debugging

        Debug.DrawLine(corners[0], corners[1], Color.red);//左上-右上

        Debug.DrawLine(corners[1], corners[3], Color.red);//右上-右下

        Debug.DrawLine(corners[3], corners[2], Color.red);//右下-左下

        Debug.DrawLine(corners[2], corners[0], Color.red);//左下-左上

    }



    void OnDrawFOV()

    {

        float halfFOV = (_camera.fieldOfView * 0.5f) * Mathf.Deg2Rad;//一半fov

        float halfHeight = _farDistance * Mathf.Tan(halfFOV);//distance距离位置，相机视口高度的一半



        //起点

        Vector3 vecStart = _camTrans.position;



        //上中

        Vector3 vecUpCenter = vecStart;

        vecUpCenter.y -= halfHeight;

        vecUpCenter.z += _farDistance;



        //下中

        Vector3 vecBottomCenter = vecStart;

        vecBottomCenter.y += halfHeight;

        vecBottomCenter.z += _farDistance;



        Debug.DrawLine(vecStart, vecUpCenter, Color.blue);

        Debug.DrawLine(vecStart, vecBottomCenter, Color.blue);

    }







    //获取相机视口四个角的坐标

    //参数 distance  视口距离

    Vector3[] GetCorners(float distance)

    {

        Vector3[] corners = new Vector3[4];



        //fov为垂直视野  水平fov取决于视口的宽高比  以度为单位





        float halfFOV = (_camera.fieldOfView * 0.5f) * Mathf.Deg2Rad;//一半fov

        float aspect = _camera.aspect;//相机视口宽高比



        float height = distance * Mathf.Tan(halfFOV);//distance距离位置，相机视口高度的一半

        float width = height * aspect;//相机视口宽度的一半



        //左上

        corners[0] = _camTrans.position - (_camTrans.right * width);//相机坐标 - 视口宽的一半

        corners[0] += _camTrans.up * height;//+视口高的一半

        corners[0] += _camTrans.forward * distance;//+视口距离



        // 右上

        corners[1] = _camTrans.position + (_camTrans.right * width);//相机坐标 + 视口宽的一半

        corners[1] += _camTrans.up * height;//+视口高的一半

        corners[1] += _camTrans.forward * distance;//+视口距离



        // 左下

        corners[2] = _camTrans.position - (_camTrans.right * width);//相机坐标 - 视口宽的一半

        corners[2] -= _camTrans.up * height;//-视口高的一半

        corners[2] += _camTrans.forward * distance;//+视口距离



        // 右下

        corners[3] = _camTrans.position + (_camTrans.right * width);//相机坐标 + 视口宽的一半

        corners[3] -= _camTrans.up * height;//-视口高的一半

        corners[3] += _camTrans.forward * distance;//+视口距离



        return corners;

    }

}

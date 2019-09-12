using UnityEngine;

/// <summary>
/// 用来调整 VRCamera 和 ARCamera 看到的画面与 Camera2D一致
/// </summary>
public class SetCameraSamePos : MonoBehaviour
{

    public Transform Camera2D, ARCamera, VRCamera;

    /// <summary>
    /// 因为投影矩阵相同，只需要设置三个相机的位置一致就可以保持三个画面观看的位置和角度一致
    /// </summary>
    private void OnEnable()
    {
        ARCamera.position = VRCamera.position = Camera2D.position;
    }
}

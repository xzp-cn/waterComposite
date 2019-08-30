using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Runing;
using liu;
using UnityEngine.UI;

/// <summary>
/// 模型拆分和组合
/// </summary>
public class SpliteControl : MonoBehaviour {

    /// <summary>
    /// 拆分的物体
    /// </summary>
    public Transform spliteObj;

    /// <summary>
    /// 拆分物体的部件数据
    /// </summary>
    Transform[] needSpliteParts;
    /// <summary>
    /// 拆分部件的数据
    /// </summary>
    List<Transform> splitList = new List<Transform>();

    public static SpliteControl Instance;

    /// <summary>
    /// 拆分的状态
    /// </summary>
    public enum SpliteState
    {
        Splite,
        Combine,
    }

    public SpliteState curSpliteState;
    private void Awake()
    {
        Instance = this;
        //---------------第二步，设置当前拆分状态------------------
        curSpliteState = SpliteState.Combine;
        //---------------第二步，设置当前拆分状态------------------
    }

    private void Start()
    {
        //---------------第一步，获取拆分数据------------------
        for (int i = 0; i < spliteObj.childCount; i++)
        {
            //把需要拆分的部件添加进来
            splitList.Add(spliteObj.GetChild(i));
        }

        //因为拆分的工具是Transform[]，因此把找到的部件从List传给Transform[]
        needSpliteParts = new Transform[splitList.Count];
        for (int i = 0; i < splitList.Count; i++)
        {
            needSpliteParts[i] = splitList[i];
        }
        //---------------第一步，获取拆分数据------------------
    }

    /// <summary>
    /// 全部拆分的按钮都被吸附
    /// </summary>
    public void AllSplitePartIsAbsorb()
    {
        OnClickSpliteToggle(toggleTransform);
    }

    /// <summary>
    /// 为方便吸附完成调用，才存储
    /// </summary>
    Transform toggleTransform;
    public void OnClickSpliteToggle(Transform toggle)
    {
        toggleTransform = toggle;
        //找到Toggle的文字
        Text toggleText = toggle.Find("Label").GetComponent<Text>();
        if(toggleText.text == "拆分")
        {
            SpliteModel();
            toggleText.text = "组合";
            curSpliteState = SpliteState.Splite;
        }
        else
        {
            CombineModel();
            toggleText.text = "拆分";
            curSpliteState = SpliteState.Combine;
        }
    }

    //---------------第三步，编写拆分和还原的实现------------------

    /// <summary>
    /// 拆分模型
    /// </summary>
    void SpliteModel()
    {
        //一般 modelObject 挂需要拆分的父物体就行
        SpliteTool.Instance.SetObjectAndInfo(Monitor23DMode.instance.camera2D, spliteObj, needSpliteParts);

        //设置完基础的信息之后，进行拆分
        //如果觉得这种拆分比较散，也可以自己去设置位置。但是SpliteTool.Instance.SetObjectAndInfo还是需要设置的
        //为了吸附，存储数据
        //如果需要加吸附功能，则加在拖拽那部分（SpliteSimpleDrag），在拖拽松开的时候，去判断是否到吸附的距离，是的话则调用吸附
        //如果是最后一个，则调用OnClickSpliteToggle 方法进行吸附
        SpliteTool.Instance.Splite(spliteObj);
    }

    /// <summary>
    /// 还原模型
    /// </summary>
    void CombineModel()
    {
        SpliteTool.Instance.Restore(spliteObj);
    }

    //---------------第三步，编写拆分和还原的实现------------------

}

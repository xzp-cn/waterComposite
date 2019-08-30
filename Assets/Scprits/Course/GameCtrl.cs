using FSpace;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class GameCtrl : MonoBehaviour
{
    public string sceneName = "chemical";//桌子上的仪器。
    Dictionary<string, GameObject> sceenDic = new Dictionary<string, GameObject>();
    public GameObject curScene;
    string[] sceneArray = new string[] { "Electrolyte", "Detection", "Principle" };//电解，检验，原理。
    AnimationOper beakerAniOper;
    MRSystem msys;
    Transform root;
    Transform UI;
    public MRSystem MrSys
    {
        get
        {
            if (msys == null)
            {
                msys = Tools.GetScenesObj("MRSystem").GetComponent<MRSystem>();
            }
            return msys;
        }
    }
    private void Awake()
    {

        PreInit();
        Tools.GetScenesObj(string.Empty);
    }
    private void Start()
    {
        Init();
    }
    /// <summary>
    /// 全局数据的配置
    /// </summary>
    void PreInit()
    {
        MrSys.isAutoSlant = false;
        MrSys.ViewerScale = 8;
    }
    /// <summary>
    /// 对底部功能的初始化，
    /// </summary>
    private void Init()
    {
        //场景底部按钮
        UI = Tools.GetScenesObj("UI").transform;
        Transform middleUI = UI.Find("InprojectionIgnoreCanvas/BottomCenter/MiddleUI").transform;
        Toggle[] tgs = middleUI.GetComponentsInChildren<Toggle>();
        string[] btnNames = new string[] { "电解", "检验", "原理" };
        for (int i = 0; i < tgs.Length; i++)
        {
            string scene = sceneArray[i];
            tgs[i].transform.Find("Label").GetComponent<Text>().text = btnNames[i];
        }
        tgs[0].isOn = true;
        tgs[0].onValueChanged.AddListener(OnElectrolyteToggleClick);
        tgs[1].onValueChanged.AddListener(OnDetectionToggleClick);
        tgs[2].onValueChanged.AddListener(OnPrincipleToggleClick);

        //侧边按钮隐藏
        Transform left = UI.Find("InprojectionIgnoreCanvas/Left");
        string[] leftBtns = new string[] { "UIButton", "UIButton (1)", "UIButton (2)" };
        for (int i = 0; i < leftBtns.Length; i++)
        {
            left.Find(leftBtns[i]).gameObject.SetActive(false);
        }
        Toggle[] togs = left.Find(leftBtns[1]).GetComponentsInChildren<Toggle>();
        string[] togNames = new string[] { "蒸馏水", "滴加氢氧化钠的蒸馏水" };
        for (int i = 0; i < togs.Length; i++)
        {
            togs[i].name = togNames[i];
            int index = i;
            togs[i].onValueChanged.AddListener((isOn) =>
            {
                WaterNaohClick(isOn, index);
            });
        }


        GameObject pn = ResManager.GetPrefab("SceneRes/posNeg");
        pn.name = "posNeg";
        pn.transform.SetParent(left.parent, false);
        pn.transform.SetAsFirstSibling();

        //场景资源加载
        //3D
        Transform chemical = transform.Find("chemical");
        if (chemical == null)
        {
            GameObject scene = ResManager.GetPrefab("SceneRes/chemical");
            scene.name = "chemical";
            scene.transform.SetParent(transform);
        }
        root = transform.Find("root");
        if (root == null)
        {
            root = new GameObject("root").transform;
            root.SetParent(chemical);
            root.localPosition = Vector3.zero;
            root.localRotation = Quaternion.Euler(Vector3.zero);
            root.localScale = Vector3.one;
        }
        Transform desk = ResManager.GetPrefab("SceneRes/desk").transform;
        desk.SetParent(root, false);
        //烧杯动画
        beakerAniOper = desk.Find("pour").gameObject.GetAnimatorOper();
        beakerAniOper.OnPause();
        //桌子烧杯玻璃棒隐藏
        desk.Find("pour/group16/shaobei/hx_hxyq_sb").gameObject.SetActive(false);
        desk.Find("pour/hx_hxyq_blb").gameObject.SetActive(false);
        //3d物体点击注册
        FSpace.SimpleDrag simpleDrag = Tools.GetScenesObj("SimpleDrag").GetScript<FSpace.SimpleDrag>();
        simpleDrag.ClickAction = ClickSwitchOffCallback;



        //黑板内容显示。         
        string blackBoadStr = "\n\u3000\u3000实验仪器:\n\u3000\u3000水电解器、学生电源、蒸馏水、滴加少量氢氧化钠的蒸馏水、火柴、玻璃棒";
        SetBlackboardShow(blackBoadStr);
    }
    void SetBlackboardShow(string str)
    {
        TextCtrl textCtrl = UIManager.Instance.GetUI<TextCtrl>("panelText");
        textCtrl.SetText(str);
        UIManager.Instance.SetUIDepth("panelText", 0);
    }

    void WaterNaohClick(bool isOn, int sibIndex)
    {
        if (isOn)
        {
            if (sibIndex == 0)
            {

            }
            else
            {

            }
        }
    }

    /// <summary>
    /// 仪器复原。
    /// </summary>
    void ResetInstruments()
    {
        Transform water = root.Find("tuopan/water");
        water.localPosition = new Vector3(0.5765f, -0.2132f, -0.0691f);

        Transform naoh = root.Find("tuopan/naoh");
        water.localPosition = new Vector3(0.0048f, 0.0489974f, -0.058f);

        Transform blb = root.Find("tuopan/bolibang");
        water.localPosition = new Vector3(-0.103f, 0.107f, .0164f);

        Transform huochai = root.Find("tuopan/huochai");
        huochai.localPosition = new Vector3(0.1629f, 0.2f, -0.2f);

        LiquidCtrl middle = root.Find("desk/pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1/middle").GetComponent<LiquidCtrl>();
        middle.Level = 0;
        LiquidCtrl left = root.Find("desk/pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1/left").GetComponent<LiquidCtrl>();
        left.Level = 0;
        LiquidCtrl right = root.Find("desk/pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1/right").GetComponent<LiquidCtrl>();
        right.Level = 0;

        Transform switchoff = root.Find("desk/pour/hx_hxyq_sdjq/switchoff");
        switchoff.localEulerAngles = Vector3.zero;
    }
    /// <summary>
    /// 主界面底下电解按钮点击事件注册。
    /// </summary>
    /// <param name="isOn"></param>
    /// <param name="sceneName"></param>
    void OnElectrolyteToggleClick(bool isOn)
    {
        if (isOn)
        {
            OnElectrolyteCallback();
        }
    }
    void OnElectrolyteCallback()
    {
        string str = $"\n\u3000\u3000步骤:\n\u3000\u3000步骤1：往电解器玻璃管里注满蒸馏水，打开“通电”开关，" +
            $"观察现象\n\u3000\u3000步骤2：往电解器玻璃管里注满滴加氢氧化钠的蒸馏水，打开“通电”开关，观察现象";
        SetBlackboardShow(str);
        //重置仪器
        ResetInstruments();
        //侧边按钮显示。


    }

    /// <summary>
    /// 检测按钮点击事件注册。
    /// </summary>
    /// <param name="isOn"></param>
    /// <param name="sceneName"></param>
    void OnDetectionToggleClick(bool isOn)
    {
        if (isOn)
        {
            OnDetectionCallback();
        }
    }
    void OnDetectionCallback()
    {

    }
    /// <summary>
    /// 检测按钮点击事件注册。
    /// </summary>
    /// <param name="isOn"></param>
    /// <param name="sceneName"></param>
    void OnPrincipleToggleClick(bool isOn)
    {
        if (isOn)
        {
            OnPrincipleCallback();
        }
    }
    void OnPrincipleCallback()
    {

    }
    /// <summary>
    /// 模块之间切换
    /// </summary>
    /// <param name="sceneName"></param>
    public void SceneLoad()
    {
        //当前处理。
        if (curScene != null)
        {
            curScene.SendMessage("Dispose");//销毁。
        }

        //下一个场景加载。    
        GameObject next = ResManager.GetPrefab("SceneRes/" + sceneName);
        next.name = sceneName;

        curScene = next;
        curScene.transform.SetParent(transform, false);
    }


    /// <summary>
    /// 3D物体点击处理
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    bool ClickSwitchOffCallback(GameObject go)
    {
        bool click = false;
        switch (go.name)
        {
            case "switchoff":
                click = true;
                SwitchOff(go);
                break;
            default:
                break;
        }
        return click;
    }
    /// <summary>
    /// 电源开关点击处理
    /// </summary>
    void SwitchOff(GameObject go)
    {
        float x = go.transform.localEulerAngles.x;
        if (x != 0)
        {
            go.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
        else
        {
            go.transform.localRotation = Quaternion.Euler(new Vector3(-30, 0, 0));
        }
    }
    /// <summary>
    /// 数据的初始化
    /// </summary>   

    /// <summary>
    /// 移除事件监听
    /// </summary>
    void RemoveAllListeners()
    {
        FSpace.SimpleDrag simpleDrag = Tools.GetScenesObj("SimpleDrag").GetScript<FSpace.SimpleDrag>();
        simpleDrag.ClickAction = null;
    }
    private void OnDestroy()
    {

    }
}

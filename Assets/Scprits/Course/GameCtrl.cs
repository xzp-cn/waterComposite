﻿using FSpace;
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
    Transform canvas;
    JiantouCtrl jiantouCtrl;
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
    FSpace.SimpleDrag simpleDrag;
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

        if (simpleDrag == null)
        {
            simpleDrag = Tools.GetScenesObj("SimpleDrag").GetScript<FSpace.SimpleDrag>();
            //simpleDrag.ClickAction = ClickSwitchOffCallback;//点击事件注册
            //simpleDrag.DragActionCallback = DragCallback;//拖拽事件注册  
        }
        simpleDrag.canDrag = false;
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
            tgs[i].onValueChanged.RemoveAllListeners();
        }
        //tgs[0].isOn = true;       
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
        string[] togNames = new string[] { "蒸馏水", "滴加氢氧化\n钠的蒸馏水" };
        for (int i = 0; i < togs.Length; i++)
        {
            Text txt = togs[i].transform.Find("Label").GetComponent<Text>();
            txt.text = togNames[i];
            txt.fontSize = i == 1 ? 19 : 24;
            int index = i;
            togs[i].onValueChanged.RemoveAllListeners();
            togs[i].onValueChanged.AddListener((isOn) =>
            {
                WaterNaohClick(isOn, index);
            });
        }

        Transform posNeg = left.parent.Find("posNeg");
        if (posNeg == null)
        {

            GameObject pn = ResManager.GetPrefab("SceneRes/posNeg");
            pn.name = "posNeg";
            pn.transform.SetParent(left.parent, false);
            pn.transform.SetAsFirstSibling();
        }


        //场景资源加载
        //3D     
        if (curScene == null)
        {
            curScene = ResManager.GetPrefab("SceneRes/chemical");
            curScene.name = "chemical";
            curScene.transform.SetParent(transform);
        }
        root = transform.Find("root");
        if (root == null)
        {
            root = new GameObject("root").transform;
            root.SetParent(curScene.transform);
            root.localPosition = Vector3.zero;
            root.localRotation = Quaternion.Euler(Vector3.zero);
            root.localScale = Vector3.one;
        }
        Transform desk = ResManager.GetPrefab("SceneRes/desk").transform;
        desk.SetParent(root, false);
        //烧杯动画
        beakerAniOper = desk.Find("pour").gameObject.GetAnimatorOper();
        beakerAniOper.PlayForward("pour");
        beakerAniOper.OnPause();
        //桌子烧杯玻璃棒隐藏
        desk.Find("pour/group16/shaobei/hx_hxyq_sb").gameObject.SetActive(false);
        desk.Find("pour/hx_hxyq_blb").gameObject.SetActive(false);
        //托盘隐藏
        Transform tuopan = desk.Find("tuopan");
        tuopan.GetComponent<MeshRenderer>().enabled = false;
        tuopan.localPosition = new Vector3(0.365f, -0.2132f, -0.0691f);

        root.Find("desk/tuopan/huochai").gameObject.SetActive(false);

        //黑板内容显示。         
        string blackBoadStr = "\n\u3000\u3000实验仪器:\n\u3000\u3000水电解器、学生电源、蒸馏水、滴加少量氢氧化钠的蒸馏水、火柴、玻璃棒";
        SetBlackboardShow(blackBoadStr);

        //烧杯溶液标签显示。
        canvas = UI.Find("InprojectionIgnoreCanvas");
        Transform water = canvas.Find("water");
        if (water == null)
        {
            TextCtrl water_text = new GameObject("water").GetScript<TextCtrl>();
            water_text.SetText("蒸馏水");
            water_text.transform.SetParent(canvas, false);
            water_text.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 100);
            water_text.transform.localPosition = new Vector3(-313.5f, -455f, 2.251f);
            water_text.transform.SetAsFirstSibling();
        }

        Transform naoh = canvas.Find("naoh");
        if (naoh == null)
        {
            TextCtrl naoh_text = new GameObject("naoh").GetScript<TextCtrl>();
            naoh_text.SetText("NAOH溶液");
            naoh_text.transform.SetParent(canvas, false);
            naoh_text.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 100);
            naoh_text.transform.localPosition = new Vector3(-540, -441, 2.334f);
            naoh_text.transform.SetAsFirstSibling();
        }
        beakerAniOper.timePointEvent = null;
    }
    /// <summary>
    /// 设置黑板显示内容
    /// </summary>
    /// <param name="str"></param>
    void SetBlackboardShow(string str)
    {
        TextCtrl textCtrl = UIManager.Instance.GetUI<TextCtrl>("panelText");
        textCtrl.SetText(str);
        UIManager.Instance.SetUIDepth("panelText", 0);
    }
    /// <summary>
    /// 电解模块右侧按钮点击注册
    /// </summary>
    /// <param name="isOn"></param>
    /// <param name="sibIndex"></param>
    void WaterNaohClick(bool isOn, int sibIndex)
    {
        if (isOn)
        {
            root.Find("desk/tuopan/huochai").gameObject.SetActive(false);
            if (sibIndex == 0)//蒸馏水
            {
                WaterMoudle();
            }
            else//naoh溶液
            {
                NaohMoudle();
            }
            simpleDrag.canDrag = true;
        }
    }
    /// <summary>
    /// 仪器复原。
    /// </summary>
    void ResetInstruments()
    {
        if (curScene != null)
        {
            DestroyImmediate(curScene);
            Init();
        }
        if (simpleDrag != null)
        {
            simpleDrag.ClickAction = null;
            simpleDrag.DragCallback = null;
            beakerAniOper.timePointEvent = null;
        }

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
        ResetInstruments();
        string str = $"\n\u3000\u3000步骤:\n\u3000\u3000步骤1：往电解器玻璃管里注满蒸馏水，打开“通电”开关，" +
            $"观察现象\n\u3000\u3000步骤2：往电解器玻璃管里注满滴加氢氧化钠的蒸馏水，打开“通电”开关，观察现象";
        SetBlackboardShow(str);
        Transform left = UI.Find("InprojectionIgnoreCanvas/Left");
        Transform btn = left.Find("UIButton (1)");
        btn.gameObject.SetActive(true);
        //重置仪器
        //侧边按钮显示。
        Debug.Log("OnElectrolyteCallback:  " + "电解重置");
    }

    /// <summary>
    /// 电解水模块
    /// </summary>
    void WaterMoudle()
    {
        OnElectrolyteCallback();


        root.Find("desk/tuopan/naoh").gameObject.SetActive(false);

        root.Find("desk/tuopan/water").gameObject.SetActive(true);
        root.Find("desk/tuopan/bolibang").gameObject.SetActive(true);

        UI.Find("InprojectionIgnoreCanvas/water").gameObject.SetActive(true);
        UI.Find("InprojectionIgnoreCanvas/naoh").gameObject.SetActive(false);


        if (jiantouCtrl == null)
        {
            jiantouCtrl = ResManager.GetPrefab("SceneRes/jiantou").GetScript<JiantouCtrl>();
        }
        jiantouCtrl.SetJiantou(new Vector3(0.0128f, -0.0076f, -0.0303f));

        //
        //电解玻璃管倒水检测区域
        BoxCollider bc = root.Find("desk/pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1/middle").gameObject.GetBoxCollider();
        bc.center = new Vector3(0, 0.3f, 0);
        bc.size = new Vector3(0.28f, 0.34f, 0.05f);
        //simpleDrag.ClickAction = ClickSwitchOffCallback;//点击事件注册

        bool hasClickWater = false;
        simpleDrag.ClickAction = (clickObj) =>//玻璃棒点击提示
        {
            bool is3D = false;
            if (clickObj.name == "bolibang")
            {
                jiantouCtrl.SetJiantou(new Vector3(0.014f, -0.0131f, -0.071f));
                is3D = true;
                root.Find("desk/tuopan/bolibang").gameObject.SetActive(false);
                root.Find("desk/pour/hx_hxyq_blb").gameObject.SetActive(true);

                bool pass = false;
                bool pass1 = false;
                bool pass2 = false;
                bool pass3 = false;
                beakerAniOper.timePointEvent = (t) =>
                {
                    //Debug.Log(t);
                    if (t >= 98 && t <= 100 && !pass)
                    {
                        pass = true;
                        if (!hasClickWater)
                        {
                            beakerAniOper.OnPause();
                        }
                    }

                    if (t >= 160 && t <= 163 && !pass1)// 水粒子特效播放
                    {
                        pass1 = true;
                        //
                        ParticleSystem ps1 = root.Find("desk/lizi_shui").GetComponent<ParticleSystem>();
                        ParticleSystem.MainModule main = ps1.main;
                        main.loop = true;
                        ps1.Play();
                        //Debug.LogError(ps1.isPlaying);

                        //中间水管水流控制
                        LiquidCtrl middleCtrl = root.Find("desk/pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1/middle").gameObject.GetScript<LiquidCtrl>();
                        middleCtrl.Level = 0;
                        middleCtrl.speed = 0.17f;
                        middleCtrl.flowDir = LiquidCtrl.flowDirection.up;
                        middleCtrl.Limit = new Vector2(0.4f, 0.95f);
                        //右边水柱水流控制
                        LiquidCtrl leftCtrl = root.Find("desk/pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1/left").gameObject.GetScript<LiquidCtrl>();
                        leftCtrl.Level = 0;
                        leftCtrl.speed = 0.1f;
                        leftCtrl.flowDir = LiquidCtrl.flowDirection.up;
                        leftCtrl.Limit = new Vector2(0.4f, 0.94f);
                        //左边水流控制
                        LiquidCtrl rightCtrl = root.Find("desk/pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1/right").gameObject.GetScript<LiquidCtrl>();
                        rightCtrl.Level = 0;
                        rightCtrl.speed = 0.1f;
                        rightCtrl.flowDir = LiquidCtrl.flowDirection.up;
                        rightCtrl.Limit = new Vector2(0.4f, 0.94f);

                    }

                    if (t >= 345 && t <= 347 && !pass2)
                    {
                        pass2 = true;

                        ParticleSystem ps1 = root.Find("desk/lizi_shui").GetComponent<ParticleSystem>();
                        ps1.Stop();

                        LiquidCtrl middleCtrl = root.Find("desk/pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1/middle").gameObject.GetScript<LiquidCtrl>();
                        middleCtrl.flowDir = LiquidCtrl.flowDirection.down;
                        middleCtrl.speed = 0.12f;
                        middleCtrl.Limit = new Vector2(0f, 0.8f);
                    }

                    if (t >= 390 && t <= 392 && !pass3)
                    {
                        pass3 = true;
                        beakerAniOper.timePointEvent = null;
                        PowerSourceClick();
                    }
                };
            }

            beakerAniOper.OnContinue();

            simpleDrag.ClickAction = (obj) =>//水杯点击提示
            {
                bool is3d = true;
                if (obj.name == "water")
                {
                    hasClickWater = true;

                    root.Find("desk/tuopan/water").gameObject.SetActive(false);
                    root.Find("desk/pour/group16/shaobei/hx_hxyq_sb").gameObject.SetActive(true);
                    jiantouCtrl.Show(false);

                    canvas.Find("water").gameObject.SetActive(false);

                    beakerAniOper.OnContinue();
                }
                return is3d;
            };
            return is3D;
        };
    }
    /// <summary>
    /// 学生电源按钮点击
    /// </summary>
    void PowerSourceClick()
    {
        jiantouCtrl.SetJiantou(new Vector3(-0.0029f, 0.0018f, 0));
        simpleDrag.ClickAction = (obj) =>
          {
              bool is3D = false;
              if (obj.name == "switchoff")//电源开关管理
              {
                  simpleDrag.ClickAction = null;
                  jiantouCtrl.Show(false);
                  SwitchOff(obj);
                  Createbubble();
              }
              return is3D;
          };
    }
    /// <summary>
    /// 开关点击处理
    /// </summary>
    /// <param name="obj"></param>
    void SwitchOff(GameObject obj)
    {
        if (obj == null)
        {
            float x = obj.transform.localEulerAngles.x;
            if (x != 0)
            {
                obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
            }
            else
            {
                obj.transform.localRotation = Quaternion.Euler(new Vector3(-30, 0, 0));
            }
        }
    }
    /// <summary>
    /// 电解产发生气泡
    /// </summary>
    void Createbubble()
    {
        ParticleSystem qipao_right = root.Find("desk/qipao_right").GetComponent<ParticleSystem>();
        Transform right_trigger = qipao_right.transform.Find("right_Trigger");
        BoxCollider box;
        if (right_trigger == null)
        {
            box = new GameObject("right_Trigger").GetBoxCollider();
            box.transform.localPosition = new Vector3(-0.0008f, 0, 0.2941f);
            box.size = new Vector3(0.015f, 0.015f, 0.01f);
            ParticleSystem.TriggerModule trigger = qipao_right.trigger;
            trigger.SetCollider(0, box);
        }

        ParticleSystem qipao_left = root.Find("desk/qipao_left").GetComponent<ParticleSystem>();
        Transform left_trigger = qipao_left.transform.Find("left_Trigger");
        if (left_trigger == null)
        {
            box = new GameObject("left_Trigger").GetBoxCollider();
            box.transform.localPosition = new Vector3(0, 0, 0.2918f);
            box.size = new Vector3(0.015f, 0.015f, 0.01f);
            ParticleSystem.TriggerModule trigger = qipao_right.trigger;
            trigger.SetCollider(0, box);
        }
    }


    /// <summary>
    /// naoh 模块
    /// </summary>
    void NaohMoudle()
    {
        root.Find("desk/tuopan/water").gameObject.SetActive(false);

        root.Find("desk/tuopan/naoh").gameObject.SetActive(true);
        root.Find("desk/tuopan/bolibang").gameObject.SetActive(true);

        UI.Find("InprojectionIgnoreCanvas/water").gameObject.SetActive(false);
        UI.Find("InprojectionIgnoreCanvas/naoh").gameObject.SetActive(true);
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
    /// 原理按钮点击事件注册。
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
    void DragCallback(string name)
    {
        if (name == "water" || name == "naoh")
        {
            UI.Find("InprojectionIgnoreCanvas/" + name).gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 电源开关点击处理
    /// </summary>

    /// <summary>
    /// 移除事件监听
    /// </summary>
    void RemoveAllListeners()
    {
        FSpace.SimpleDrag simpleDrag = Tools.GetScenesObj("SimpleDrag").GetScript<FSpace.SimpleDrag>();
        simpleDrag.ClickAction = null;
        simpleDrag.DragCallback = null;
    }
    private void OnDestroy()
    {

    }
}



public class JiantouCtrl : MonoBehaviour
{
    private void Start()
    {
        this.name = "jiantou";
        Transform root = Tools.GetScenesObj("GameCtrl").transform.Find("chemical/root");
        transform.SetParent(root, false);
        transform.localScale = new Vector3(0.05f, 0.05f, 1);
        transform.Find("GQ").localPosition = new Vector3(0, -0.024f, 0);
    }
    /// <summary>
    /// 设置箭头属性
    /// </summary>
    public void SetJiantou(Vector3 pos)
    {
        transform.localPosition = pos;
        transform.localScale = Vector3.one * 0.05f;
        Show(true);
    }
    /// <summary>
    /// 箭头的显示和隐藏
    /// </summary>
    public void Show(bool isShow)
    {
        transform.gameObject.SetActive(isShow);
    }
}

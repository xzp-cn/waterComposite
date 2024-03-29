
using DG.Tweening;
using FSpace;
using liu;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
//[ExecuteInEditMode]
public class GameCtrl : MonoBehaviour
{
    public GameObject ARCam, Cam2D;

    public string sceneName = "chemical";//桌子上的仪器。
    //Dictionary<string, GameObject> sceenDic = new Dictionary<string, GameObject>();
    //public GameObject curScene;
    string[] sceneArray = new string[] { "Electrolyte", "Detection", "Principle" };//电解，检验，原理。
    AnimationOper beakerAniOper, shuiAni;
    MRSystem msys;
    Transform root;
    Transform UI;
    Transform canvas;
    Transform hcPar;
    Transform dragPar;
    JiantouCtrl jiantouCtrl;
    float leftbubbleSpeed, rightbubbleSpeed, leftTriggerSpeed, rightTriggerSpeed = 0;//左右两边气泡速度
    bool isNaoh = false;
    struct FireNum
    {
        public int huochaiPosNum;
        public int huochaiNegNum;
        public int mtNegNum;
        public int mtPosNum;
        public FireNum(int a)
        {
            huochaiPosNum = 0;
            huochaiNegNum = 0;
            mtNegNum = 0;
            mtPosNum = 0;
        }
        public void Reset()
        {
            huochaiNegNum = 0;
            huochaiPosNum = 0;
            mtNegNum = 0;
            mtPosNum = 0;
        }
        public bool AllOver()
        {
            bool over = false;
            if (huochaiNegNum > 0 && huochaiPosNum > 0 && mtNegNum > 0 && mtPosNum > 0)
            {
                over = true;
            }
            return over;
        }
    }
    FireNum mFireNum;
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
    Camera3D cam3D;
    private void Awake()
    {
        PreInit();
        Tools.GetScenesObj(string.Empty);
        cam3D = Tools.GetScenesObj("Camera3D").GetComponent<Camera3D>();
        Debug.Log(cam3D.name);
    }
    private void OnEnable()
    {
        //ARCam.transform.position = Cam2D.transform.position;
    }
    private void Start()
    {

        MrSys.transform.localPosition = new Vector3(0, 0, 0.3f);
        Init();
    }
    /// <summary>
    /// 全局数据的配置
    /// </summary>
    void PreInit()
    {
        MrSys.isAutoSlant = false;
        MrSys.ViewerScale = 5;

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
        DOTween.CompleteAll();
        DOTween.KillAll(true);
        MrSys.transform.localPosition = new Vector3(0, 0, 0.3f);

        //场景底部按钮
        UI = Tools.GetScenesObj("UI").transform;
        if (UI == null)
        {
            UI = Tools.GetScenesObj("MRSystem").transform.Find("UI");
        }

        Transform middleUI = UI.Find("InprojectionIgnoreCanvas/BottomCenter/MiddleUI").transform;
        Toggle[] tgs = middleUI.GetComponentsInChildren<Toggle>();
        string[] btnNames = new string[] { "电解", "检验", "原理" };
        for (int i = 0; i < tgs.Length; i++)
        {
            //string scene = sceneArray[i];
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
        string[] togNames = new string[] { "蒸馏水", "滴加氢氧化钠\n的蒸馏水" };
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

        //微观场景
        Toggle microTogs = left.Find(leftBtns[0]).GetComponentInChildren<Toggle>();
        Text mtxt = microTogs.transform.Find("Label").GetComponent<Text>();
        mtxt.text = "返回";
        mtxt.fontSize = 37;
        microTogs.onValueChanged.RemoveAllListeners();
        microTogs.onValueChanged.AddListener((isOn) =>
        {
            OnPrincipleCallback();
        });



        Transform posNeg = left.parent.Find("posNeg");
        if (posNeg == null)
        {
            GameObject pn = ResManager.GetPrefab("SceneRes/posNeg");
            pn.name = "posNeg";
            pn.transform.SetParent(left.parent, false);
            pn.transform.SetAsFirstSibling();
            pn.transform.Find("pos").localPosition = new Vector3(-100, -358f, 0);
            pn.transform.Find("neg").localPosition = new Vector3(290, -360, 0);
            posNeg = pn.transform;
        }
        posNeg.gameObject.SetActive(true);
        //场景资源加载
        //3D          
        Transform chemical = transform.Find("chemical");
        if (chemical == null)
        {
            chemical = ResManager.GetPrefab("SceneRes/chemical").transform;
            chemical.name = "chemical";
            chemical.SetParent(transform);
            chemical.localPosition = new Vector3(1.458f, 0.864f, -1.77f);
            chemical.localRotation = Quaternion.Euler(new Vector3(0, 270, 0));
            chemical.localScale = Vector3.one;
            chemical.Find("scene/F_swsys_hg_hg0111 (1)/pingmu (3)/frame").localScale = new Vector3(0.8f, 1f, 1f);
        }

        root = transform.Find("chemical/root");
        if (root == null)
        {
            root = new GameObject("root").transform;
            root.SetParent(chemical);
            root.localPosition = new Vector3(2.631f, -0.379f, 1.113f);
            root.localRotation = Quaternion.Euler(new Vector3(0, 270, 0));
            root.localScale = Vector3.one * 2;
        }
        Transform desk = ResManager.GetPrefab("SceneRes/desk").transform;
        desk.SetParent(root, false);
        desk.localPosition = Vector3.zero;
        desk.localScale = Vector3.one;
        //烧杯动画
        beakerAniOper = desk.Find("pour").gameObject.GetAnimatorOper();
        beakerAniOper.PlayForward("pour");
        beakerAniOper.OnPause();
        shuiAni = root.Find("desk/shui").gameObject.GetAnimatorOper();
        shuiAni.OnPause();
        //桌子烧杯玻璃棒隐藏
        desk.Find("pour/group16/shaobei/hx_hxyq_sb").gameObject.SetActive(false);
        desk.Find("pour/hx_hxyq_blb").gameObject.SetActive(false);

        //仪器两边活塞设置
        Transform rightBtn = desk.Find("pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1/polySurface20");
        Transform niu = rightBtn.Find("niu");
        if (niu == null)
        {
            niu = ResManager.GetPrefab("SceneRes/niu").transform;
            niu.SetParent(rightBtn);
            niu.localPosition = Vector3.zero;
            niu.localRotation = Quaternion.Euler(90, 180, 0);
            niu.localScale = Vector3.one;
        }
        Transform leftBtn = desk.Find("pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1/polySurface23");
        niu = leftBtn.Find("niu");
        if (niu == null)
        {
            niu = ResManager.GetPrefab("SceneRes/niu").transform;
            niu.SetParent(leftBtn);
            niu.localPosition = Vector3.zero;
            niu.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
            niu.localScale = Vector3.one;
        }

        //托盘隐藏        
        Transform tuopan = desk.Find("tuopan");
        if (tuopan != null)
        {
            tuopan.GetComponent<MeshRenderer>().enabled = false;
        }

        Transform mTuopan = desk.Find("mTuopan");
        if (mTuopan == null)
        {
            mTuopan = ResManager.GetPrefab("SceneRes/tuopan").transform;
            mTuopan.SetParent(root);
            mTuopan.localPosition = new Vector3(0.43f, -0.2f, -0.033f);
            mTuopan.localScale = new Vector3(1, 0.1f, 1f);
            mTuopan.name = "mTuopan";
        }

        //root.Find("desk/tuopan/huochai").gameObject.SetActive(false);

        //黑板内容显示。         
        string blackBoadStr = "\n\u3000\u3000实验仪器:\n\u3000\u3000水电解器、学生电源、蒸馏水、滴加少量氢氧化钠的蒸馏水、火柴、玻璃棒";
        SetBlackboardShow(blackBoadStr);

        //烧杯溶液标签显示。
        canvas = UI.Find("InprojectionIgnoreCanvas");
        Transform water = canvas.Find("water");
        if (water == null)
        {
            Text water_text = new GameObject("water").GetScript<Text>();
            water_text.text = "蒸馏水";
            water_text.transform.SetParent(canvas, false);
            water_text.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 100);
            water_text.transform.localPosition = new Vector3(-156.8f, -400, 0.737f);
            water_text.transform.SetAsFirstSibling();
            water_text.font = Font.CreateDynamicFontFromOSFont("Arial", 30);
            water_text.fontSize = 26;
        }

        Transform naoh = canvas.Find("naoh");
        if (naoh == null)
        {
            Text naoh_text = new GameObject("naoh").AddComponent<Text>();
            naoh_text.text = "NaOH溶液";
            naoh_text.transform.SetParent(canvas, false);
            naoh_text.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 100);
            naoh_text.transform.localPosition = new Vector3(-215.6f, -403.9f, 0.774f);
            naoh_text.transform.SetAsFirstSibling();
            naoh_text.font = Font.CreateDynamicFontFromOSFont("Arial", 30);
            naoh_text.fontSize = 26;
        }

        Transform huochai = canvas.Find("huochai");
        if (huochai == null)
        {
            Text huocha_texti = new GameObject("huochai").AddComponent<Text>();
            huocha_texti.text = "点燃的木条";
            huocha_texti.transform.SetParent(canvas, false);
            huocha_texti.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 100);
            huocha_texti.transform.localPosition = new Vector3(-336.9f, -365f, 0.489f);
            huocha_texti.transform.SetAsFirstSibling();
            huocha_texti.font = Font.CreateDynamicFontFromOSFont("Arial", 30);
            huocha_texti.fontSize = 26;
        }

        Transform mutiao = canvas.Find("mutiao");
        if (mutiao == null)
        {
            Text mutiao_text = new GameObject("mutiao").AddComponent<Text>();
            mutiao_text.text = "带火星的木条";
            mutiao_text.transform.SetParent(canvas, false);
            mutiao_text.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 100);
            mutiao_text.transform.localPosition = new Vector3(-493.8f, -388f, 0.489f);
            mutiao_text.transform.SetAsFirstSibling();
            mutiao_text.font = Font.CreateDynamicFontFromOSFont("Arial", 30);
            mutiao_text.fontSize = 26;
        }

        //黑板ui重置
        Transform equ = canvas.Find("equation");
        if (equ != null)
        {
            equ.gameObject.SetActive(false);
        }

        //化学方程式展示
        Transform chemicalEquation = canvas.Find("chemicalEquation");
        if (chemicalEquation != null)
        {
            chemicalEquation.gameObject.SetActive(false);
        }

        Transform pText = canvas.Find("panelText");
        if (pText == null)
        {
            pText.gameObject.SetActive(true);
        }
        //posNeg.gameObject.SetActive(false);

        Transform micro_posNeg = canvas.Find("micro_posNeg");
        if (micro_posNeg != null)
        {
            micro_posNeg.gameObject.SetActive(false);
        }


        Transform switchOff = canvas.Find("switchOff");
        if (switchOff != null)
        {
            switchOff.gameObject.SetActive(false);
        }
        //课件名称显示
        canvas.Find("Top/Image/Text").GetComponent<Text>().text = "水的组成";
        Transform naohText = canvas.Find("naohText");
        if (naohText != null)
        {
            naohText.gameObject.SetActive(false);
        }
        //开关重置
        root.Find("desk/pour/hx_hxyq_sdjq/switchoff").localRotation = Quaternion.Euler(Vector3.zero);
        root.Find("desk/pour/hx_hxyq_sdjq/switchoff").GetComponent<BoxCollider>().enabled = false;
        //火柴设置
        hcPar = tuopan.Find("hcPar");
        if (hcPar == null)
        {
            hcPar = new GameObject("hcPar").transform;
            hcPar.SetParent(tuopan);
            hcPar.localScale = Vector3.one;
            hcPar.localPosition = new Vector3(-0.26f, 0.131f, -0.1002f);
        }
        string[] hcMt = new string[] { FireEnum.huochai.ToString("g"), FireEnum.mutiao.ToString("g") };
        for (int i = 0; i < hcMt.Length; i++)
        {
            string fname = hcMt[i];
            GameObject obj = ResManager.GetPrefab("SceneRes/" + fname);
            Transform par = hcPar;
            obj.transform.SetParent(par);
            obj.name = fname;
            Vector3 pos = fname == FireEnum.huochai.ToString("g") ? new Vector3(0.06f, 0, 0) : new Vector3(-0.06f, 0, 0);
            obj.transform.localPosition = pos;

            obj.transform.localScale = new Vector3(1, 3, 1);
            /*FireCtrl fCtrl =*/
            obj.GetScript<FireCtrl>();
        }

        //
        Transform h2oPar = root.Find("desk/shuidi/H2oPar");
        if (h2oPar != null)
        {
            h2oPar.gameObject.SetActive(false);
        }
        //
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
        //textCtrl.transform.localPosition = new Vector3(308.3f, 98.95f, 1.602f);
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
            //root.Find("desk/tuopan/huochai").gameObject.SetActive(false);            
            hcPar.gameObject.SetActive(false);
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
        if (root != null)
        {
            FCore.clearDragObj();
            DestroyImmediate(root.gameObject);
            root = null;
        }
        Transform huaxue_weiguan = transform.Find("huaxue_weiguan");
        if (huaxue_weiguan != null)
        {
            huaxue_weiguan.gameObject.SetActive(false);
        }

        if (simpleDrag != null)
        {
            simpleDrag.ClickAction = null;
            simpleDrag.DragCallback = null;
            beakerAniOper.timePointEvent = null;
        }
        isNaoh = false;
        CancelInvoke();


        Init();
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
    void OnElectrolyteCallback(bool isWater = true)
    {
        ResetInstruments();
        string str = $"\n\u3000\u3000步骤:\n\u3000\u3000步骤1：往电解器玻璃管里注满蒸馏水，打开“通电”开关，" +
            $"观察现象\n\u3000\u3000步骤2：往电解器玻璃管里注满滴加氢氧化钠的蒸馏水，打开“通电”开关，观察现象";
        SetBlackboardShow(str);
        Transform left = UI.Find("InprojectionIgnoreCanvas/Left");
        Transform btn = left.Find("UIButton (1)");
        btn.gameObject.SetActive(true);
        hcPar.gameObject.SetActive(false);

        Transform chemical = transform.Find("chemical");
        if (chemical != null)
        {
            chemical.gameObject.SetActive(true);
        }
        //重置仪器
        if (isWater)
        {
            WaterDefault();
        }
        Transform pn = canvas.Find("posNeg");
        if (pn != null)
        {
            pn.gameObject.SetActive(true);
        }
        //标签隐藏
        Transform huochai = canvas.Find("huochai");
        if (huochai != null)
        {
            huochai.gameObject.SetActive(false);
        }

        Transform mutiao = canvas.Find("mutiao");
        if (mutiao != null)
        {
            mutiao.gameObject.SetActive(false);
        }
        //侧边按钮显示。
        Debug.Log("OnElectrolyteCallback:  " + "电解重置");
    }
    /// <summary>
    /// 电解水模块
    /// </summary>
    void WaterMoudle()
    {
        OnElectrolyteCallback(true);
    }
    void WaterDefault()
    {
        Transform left = UI.Find("InprojectionIgnoreCanvas/Left");
        string[] leftBtns = new string[] { "UIButton", "UIButton (1)", "UIButton (2)" };
        Toggle[] togs = left.Find(leftBtns[1]).GetComponentsInChildren<Toggle>();
        togs[0].isOn = true;

        isNaoh = false;

        root.Find("desk/tuopan/naoh").gameObject.SetActive(false);

        root.Find("desk/tuopan/water").gameObject.SetActive(true);
        root.Find("desk/tuopan/bolibang").gameObject.SetActive(true);

        UI.Find("InprojectionIgnoreCanvas/water").gameObject.SetActive(true);
        UI.Find("InprojectionIgnoreCanvas/naoh").gameObject.SetActive(false);

        Transform mutiaoUI = canvas.Find("mutiao");
        if (mutiaoUI != null)
        {
            mutiaoUI.gameObject.SetActive(false);
        }
        Transform huochaiUI = canvas.Find("huochai");
        if (mutiaoUI != null)
        {
            huochaiUI.gameObject.SetActive(false);
        }

        leftbubbleSpeed = 0.17f;
        rightbubbleSpeed = 0.17f;
        leftTriggerSpeed = 0.018f;
        rightTriggerSpeed = 0.04f;

        PlayMoudleAnimation("water");
    }
    /// <summary>
    /// 电解Naoh溶液模块
    /// </summary>
    void NaohMoudle()
    {
        OnElectrolyteCallback(false);


        root.Find("desk/tuopan/water").gameObject.SetActive(false);

        root.Find("desk/tuopan/naoh").gameObject.SetActive(true);
        root.Find("desk/tuopan/bolibang").gameObject.SetActive(true);
        Transform naoh = root.Find("desk/tuopan/naoh");
        naoh.localPosition = new Vector3(0.047f, 0.04899f, 0.026f);

        UI.Find("InprojectionIgnoreCanvas/water").gameObject.SetActive(false);
        Transform naohUI = UI.Find("InprojectionIgnoreCanvas/naoh");
        naohUI.localPosition = new Vector3(-92f, -400f, 0.736f);
        naohUI.gameObject.SetActive(true);

        Transform mutiaoUI = canvas.Find("mutiao");
        if (mutiaoUI != null)
        {
            mutiaoUI.gameObject.SetActive(false);
        }
        Transform huochaiUI = canvas.Find("huochai");
        if (huochaiUI != null)
        {
            huochaiUI.gameObject.SetActive(false);
        }

        leftbubbleSpeed = 0.4f * 1.3f;
        rightbubbleSpeed = 0.4f * 1.3f;
        leftTriggerSpeed = 0.018f * 1.2f;
        rightTriggerSpeed = 0.04f * 1.2f;
        PlayMoudleAnimation("naoh");
    }
    /// <summary>
    /// 
    /// </summary>
    void PlayMoudleAnimation(string moudleName)//naoh和water
    {
        if (jiantouCtrl == null)
        {
            jiantouCtrl = ResManager.GetPrefab("SceneRes/jiantou").GetScript<JiantouCtrl>();
        }
        jiantouCtrl.SetJiantou(new Vector3(0.396f, -0.169f, 0.061f));

        //
        //电解玻璃管倒水检测区域
        BoxCollider bc = root.Find("desk/pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1/middle").gameObject.GetBoxCollider();
        bc.center = new Vector3(0, 0.3f, 0);
        bc.size = new Vector3(0.28f, 0.34f, 0.05f);
        //simpleDrag.ClickAction = ClickSwitchOffCallback;//点击事件注册

        GlobalConfig.Instance.operationModel = OperationModel.Stay;
        bool hasClickWater = false;
        simpleDrag.ClickAction = (clickObj) =>//玻璃棒点击提示
        {
            bool is3D = false;
            if (clickObj.name == "bolibang")
            {
                jiantouCtrl.SetJiantou(new Vector3(0.3368f, -0.0846f, -0.0882f));
                is3D = true;
                root.Find("desk/tuopan/bolibang").gameObject.SetActive(false);
                root.Find("desk/pour/hx_hxyq_blb").gameObject.SetActive(true);
                GameObject sb = root.Find("desk/pour/group16/shaobei/hx_hxyq_sb").gameObject;

                bool pass = false;
                bool pass1 = false;
                bool pass2 = false;
                bool pass3 = false;
                bool pass4 = false;
                beakerAniOper.timePointEvent = (t) =>
                {
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

                        //中间水柱控制
                        FlowWater(posNeg.middle, LiquidCtrl.flowDirection.up, 0.15f, new Vector2(0.4f, 0.98f), 0);
                        //左边水柱水流控制                    
                        FlowWater(posNeg.left, LiquidCtrl.flowDirection.up, 0.11f, new Vector2(0.4f, 0.94f));
                        //右边水流控制                       
                        FlowWater(posNeg.right, LiquidCtrl.flowDirection.up, 0.11f, new Vector2(0.4f, 0.94f), 0);

                        //底部容器水面控制
                        LiquidCtrl LCtrl = root.Find("desk/shuidi").gameObject.GetScript<LiquidCtrl>();
                        LCtrl.speed = 0.18f;
                        LCtrl.flowDir = LiquidCtrl.flowDirection.up;
                        LCtrl.Level = 0;
                        LCtrl.Limit = new Vector2(0.4f, 1f);
                        //烧杯水流控制
                        LCtrl = sb.GetScript<LiquidCtrl>();
                        LCtrl.speed = 0.1f;
                        LCtrl.flowDir = LiquidCtrl.flowDirection.down;
                        LCtrl.Level = 1;
                        LCtrl.Limit = new Vector2(0, 1);

                        //中间液面下降

                        //相机拉近                     
                        UI.SetParent(MrSys.transform);

                        //MrSys.transform.DOLocalMove(new Vector3(0.25f, 0, 4), 1f);
                    }

                    if (t >= 200 && t <= 202 && !pass4)
                    {
                        pass4 = true;
                        shuiAni.PlayForward("pingzi22", 200);
                        shuiAni.OnContinue();
                        //MrSys.transform.DOLocalMove(new Vector3(0, 0, 3), 1f);
                    }

                    if (t >= 345 && t <= 347 && !pass2)
                    {
                        pass2 = true;

                        ParticleSystem ps1 = root.Find("desk/lizi_shui").GetComponent<ParticleSystem>();
                        ps1.Stop();

                        LiquidCtrl middleCtrl = root.Find("desk/pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1/middle").gameObject.GetScript<LiquidCtrl>();
                        middleCtrl.flowDir = LiquidCtrl.flowDirection.down;
                        middleCtrl.speed = 0.05f;
                        middleCtrl.Limit = new Vector2(0.908f, 0.905f);
                    }

                    if (t >= 390 && t <= 392 && !pass3)
                    {
                        pass3 = true;
                        beakerAniOper.timePointEvent = null;
                        PowerSourceClick();
                    }
                };

                beakerAniOper.OnContinue();

                simpleDrag.ClickAction = (obj) =>//水杯点击提示
                {
                    bool is3d = true;
                    if (obj.name == moudleName)
                    {
                        hasClickWater = true;

                        root.Find("desk/tuopan/" + moudleName).gameObject.SetActive(false);
                        sb.SetActive(true);
                        LiquidCtrl sbCtrl = sb.GetScript<LiquidCtrl>();
                        sbCtrl.Level = 1;

                        jiantouCtrl.Show(false);
                        canvas.Find(moudleName).gameObject.SetActive(false);

                        beakerAniOper.OnContinue();
                    }
                    return is3d;
                };
            }
            return is3D;
        };
        isNaoh = moudleName == "naoh";
    }
    /// <summary>
    /// 学生电源按钮点击
    /// </summary>
    void PowerSourceClick()
    {
        //cam3D.enabled = false;
        //MrSys.transform.DOLocalMove(Vector3.zero, 2f).onComplete = () =>
        //  {
        //      cam3D.enabled = true;
        //  };
        //if (Monitor23DMode.instance.is3D)
        //{
        //    cam3D.enabled = false;
        //    cam3D.transform.DOLocalMove(new Vector3(0.0875f, 0.67f, -1.64f), 2).onComplete = () =>
        //       {
        //           cam3D.enabled = false;
        //       };
        //}
        root.Find("desk/pour/hx_hxyq_sdjq/switchoff").GetComponent<BoxCollider>().enabled = true;
        jiantouCtrl.SetJiantou(new Vector3(-0.1655f, -0.1327f, 0.124f));
        simpleDrag.ClickAction = (obj) =>
          {
              bool is3D = true;
              if (obj.name == "switchoff")//电源开关管理
              {
                  obj.GetBoxCollider().enabled = false;
                  Debug.Log("电源开关处理");
                  simpleDrag.ClickAction = null;
                  jiantouCtrl.Show(false);
                  SwitchOff(obj);
                  Createbubble();//生成气泡
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
        if (obj != null)
        {
            float x = obj.transform.localEulerAngles.x;
            //Debug.LogError(x);
            if (x != 0)
            {
                obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
            }
            else
            {
                obj.transform.localRotation = Quaternion.Euler(new Vector3(330, 0, 0));
            }
        }
    }
    /// <summary>
    /// 电解产发生气泡
    /// </summary>    
    void Createbubble(bool isFinish = true)
    {
        float delayTime = 6;
        float bubbleSpeed = 0.17f;
        int particleNumLeft = 1000;
        int particleNumRight = 2000;
        int rateOverLeft = 60;
        int rateOverRight = 300;
        if (isNaoh)
        {
            delayTime = 4;
            bubbleSpeed = 0.4f;
            particleNumLeft = 2000;
            particleNumRight = 10000;
            rateOverLeft = 300;
            rateOverRight = 800;
        }
        leftbubbleSpeed = rightbubbleSpeed = bubbleSpeed;
        leftTriggerSpeed = (0.2918f - 0.16624f) / delayTime;
        rightTriggerSpeed = (0.2941f - 0.06941f) / delayTime;


        ParticleSystem qipao_right = root.Find("desk/qipao_right").GetComponent<ParticleSystem>();
        Transform right_trigger = qipao_right.transform.Find("right_Trigger");
        BoxCollider box;
        BubbleCtrl bLeft, bRight;
        if (right_trigger == null)
        {
            box = new GameObject("right_Trigger").GetBoxCollider();
            right_trigger = box.transform;
            box.transform.SetParent(qipao_right.transform);
            box.transform.localPosition = new Vector3(-0.0008f, 0, 0.2941f);
            box.size = new Vector3(0.1f, 0.01f, 0.1f);
            ParticleSystem.TriggerModule trigger = qipao_right.trigger;
            trigger.inside = ParticleSystemOverlapAction.Kill;
            trigger.enabled = true;
            trigger.SetCollider(0, box);

            ParticleSystem.MainModule main = qipao_right.main;
            main.maxParticles = particleNumRight;

            ParticleSystem.EmissionModule em = qipao_right.emission;
            em.rateOverTime = rateOverRight;
        }
        bRight = right_trigger.gameObject.GetScript<BubbleCtrl>();
        ParticleSystem.VelocityOverLifetimeModule right_velocity = qipao_right.velocityOverLifetime;
        //rightbubbleSpeed = 0.15f;
        right_velocity.z = rightbubbleSpeed;
        qipao_right.Play();

        ParticleSystem qipao_left = root.Find("desk/qipao_left").GetComponent<ParticleSystem>();
        qipao_left.transform.localPosition = new Vector3(0.175f, -0.193f, -0.007f);
        Transform left_trigger = qipao_left.transform.Find("left_Trigger");
        if (left_trigger == null)
        {
            box = new GameObject("left_Trigger").GetBoxCollider();
            left_trigger = box.transform;
            box.transform.SetParent(qipao_left.transform);
            box.transform.localPosition = new Vector3(0, 0, 0.2918f);
            box.size = new Vector3(0.1f, 0.01f, 0.1f);
            ParticleSystem.TriggerModule trigger = qipao_left.trigger;
            trigger.inside = ParticleSystemOverlapAction.Kill;
            trigger.enabled = true;
            trigger.SetCollider(0, box);

            ParticleSystem.MainModule main = qipao_left.main;
            main.maxParticles = particleNumLeft;

            ParticleSystem.EmissionModule em = qipao_left.emission;
            em.rateOverTime = rateOverLeft;
        }
        bLeft = left_trigger.gameObject.GetScript<BubbleCtrl>();

        ParticleSystem.VelocityOverLifetimeModule left_velocity = qipao_left.velocityOverLifetime;
        //leftbubbleSpeed = 0.12f;
        left_velocity.z = leftbubbleSpeed;
        qipao_left.Play();


        DOTween.To(
       () => { return 0; },
       (a) =>
       {
       },
       1,
       4
   ).onComplete = () =>
     {

         float leftSpeed = (0.94f - 0.75f) / delayTime;
         float rightSpeed = (0.94f - 0.6f) / delayTime;
         float midSpeed = (0.94f - 0.76f) / delayTime;
         //正极水柱水流控制           
         FlowWater(posNeg.left, LiquidCtrl.flowDirection.down, leftSpeed, new Vector2(0.75f, 0.94f), 0.94f);
         //负极水流控制
         FlowWater(posNeg.right, LiquidCtrl.flowDirection.down, rightSpeed, new Vector2(0.6f, 0.94f), 0.94f);
         //中间水柱液面下降
         FlowWater(posNeg.middle, LiquidCtrl.flowDirection.down, midSpeed, new Vector2(0.76f, 0.94f), 0.91f);
         //中间水柱消失       
         //正极气泡trigger控制
         float leftTg = (0.2941f - 0.167f) / delayTime;
         float rightTg = (0.2941f - 0.07f) / delayTime;
         bLeft.SetFlow(BubbleCtrl.flowDirection.down, leftTriggerSpeed, 0.167f);
         //负极气泡trigger控制
         bRight.SetFlow(BubbleCtrl.flowDirection.down, rightTriggerSpeed, 0.07f);
     };

        //点解结束气泡停止  
        ///关闭开关提示
        DOTween.To(
          () => { return 0; },
          (a) =>
          {
          },
          1,
          delayTime + 3
      ).onComplete = () =>
      {
          Transform switchOff = canvas.Find("switchOff");
          if (switchOff != null)
          {
              switchOff.gameObject.SetActive(true);
          }
          else
          {
              switchOff = ResManager.GetPrefab("SceneRes/switchOff").transform;
              switchOff.SetParent(canvas);
              switchOff.name = "switchOff";
              switchOff.localPosition = new Vector3(478, -390, 0.36f);
              switchOff.localScale = Vector3.one;
              switchOff.SetAsFirstSibling();
          }

          qipao_right = root.Find("desk/qipao_right").GetComponent<ParticleSystem>();
          qipao_right.Stop();
          qipao_left = root.Find("desk/qipao_left").GetComponent<ParticleSystem>();
          qipao_left.Stop();//右边气泡停止

          if (isFinish)
          {
              float delay = isNaoh ? 10 : 10 / 1.1f;
              StartCoroutine(ElectronicFinish(delay));
          }
      };


    }
    /// <summary>
    /// 正负极水流控制
    /// </summary>
    /// <param name="pn"> 正负极</param>
    /// <param name="fd">流向</param>
    /// <param name="speed">水流速度</param>
    /// <param name="limit">高度控制</param>
    void FlowWater(posNeg pn, LiquidCtrl.flowDirection fd, float speed, Vector2 limit, float defaultLevel = 0)
    {
        LiquidCtrl LCtrl = root.Find("desk/pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1/" + pn.ToString("g")).gameObject.GetScript<LiquidCtrl>();
        LCtrl.speed = speed;
        LCtrl.flowDir = fd;
        LCtrl.Level = defaultLevel;
        LCtrl.Limit = limit;
    }
    /// <summary>
    /// 电解水结束
    /// </summary>
    IEnumerator ElectronicFinish(float delay)
    {
        yield return delay;
        if (isNaoh)
        {
            RectTransform rt = canvas.Find("naohText")?.GetComponent<RectTransform>();
            if (rt == null)
            {
                rt = new GameObject("naohText").AddComponent<RectTransform>();
                rt.SetAsFirstSibling();
                rt.SetParent(canvas);
                rt.sizeDelta = new Vector2(1000, 300);
                rt.anchoredPosition3D = new Vector3(80f, -19.2f, -1.351f);
                rt.localScale = Vector3.one;
                Text txt = rt.gameObject.AddComponent<Text>();
                txt.font = Font.CreateDynamicFontFromOSFont("Arial", 34);
                txt.text = "\n\u3000\u3000与单独电解纯净蒸馏水有何不同？说明什么?";
                txt.fontSize = 40;
            }
            else
            {
                rt.SetAsFirstSibling();
                rt.SetParent(canvas);
                rt.sizeDelta = new Vector2(1000, 300);
                rt.anchoredPosition3D = new Vector3(80f, -19.2f, -1.351f);
                rt.localScale = Vector3.one;
            }

            //
            DOTween.To(
                () => { return 0; },
                (a) => { },
                0,
                1).onComplete = () =>
                {
                    rt.DOLocalMove(new Vector3(94.125f, -19.2f, 1.595f), 2f).onComplete = () =>
                    {
                        Text txt = rt.GetComponent<Text>();
                        txt.fontSize = 50;

                        rt.sizeDelta = new Vector2(1491.7f, 253f);
                        rt.anchoredPosition3D = new Vector3(94.125f, -19.2f, 1.595f);
                        rt.localScale = Vector3.one;

                        Transform naohBtn = rt.Find("naohTxtBtn");
                        if (naohBtn == null)
                        {
                            Button answerBtn = ResManager.GetPrefab("SceneRes/Button").GetComponent<Button>();
                            answerBtn.name = "naohTxtBtn";
                            answerBtn.transform.SetParent(rt, false);
                            answerBtn.transform.localScale = Vector3.one;
                            RectTransform naohRt = answerBtn.GetComponent<RectTransform>();
                            naohRt.anchoredPosition3D = new Vector3(677, -68, 0);
                            naohRt.sizeDelta = new Vector2(100, 100);
                            Text btnTxt = naohRt.GetComponentInChildren<Text>();
                            btnTxt.text = "答案";
                            btnTxt.fontSize = 35;
                            answerBtn.GetComponent<Image>().color = new Color32(233, 255, 0, 100);
                            answerBtn.onClick.AddListener(() =>
                            {
                                Text showAnswer = rt.GetComponent<Text>();
                                showAnswer.fontSize = 42;
                                showAnswer.text = "\n\u3000\u3000与单独电解纯净蒸馏水有何不同？说明什么?\n\u3000\u3000比单独电解蒸馏水速度快。说明水中加入少量氢氧化钠可增强导电性";
                            });
                        }
                    };
                };

        }
        GameObject obj = root.Find("desk/pour/hx_hxyq_sdjq/switchoff").gameObject;
        SwitchOff(obj);
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
        mFireNum = new FireNum(0);
        ResetInstruments();

        Transform chemical = transform.Find("chemical");
        if (chemical != null)
        {
            chemical.gameObject.SetActive(true);
        }

        string str = $"\n\u3000\u3000步骤:\n\u3000\u3000步骤1：分别用带火星的木条和点燃的火柴靠近正极玻璃管尖嘴处，打开活塞，" +
            $"观察现象\n\u3000\u3000步骤2：分别用带火星的木条和点燃的火柴靠近负极玻璃管尖嘴处，打开活塞，观察现象" +
            $"\n\u3000\u3000\u3000实验后请归纳正负极气体性质";
        SetBlackboardShow(str);
        //左侧按钮隐藏
        Transform left = UI.Find("InprojectionIgnoreCanvas/Left");
        Transform btn = left.Find("UIButton (1)");
        btn.gameObject.SetActive(false);
        //ui隐藏        
        Transform waterUI = canvas.Find("water");
        Transform naohUI = canvas.Find("naoh");
        if (naohUI)
        {
            naohUI.gameObject.SetActive(false);
        }
        if (waterUI)
        {
            waterUI.gameObject.SetActive(false);
        }

        Transform tuopan = root.Find("desk/tuopan");
        tuopan.Find("water").gameObject.SetActive(false);
        tuopan.Find("naoh").gameObject.SetActive(false);
        tuopan.Find("bolibang").gameObject.SetActive(false);


        Transform huochai = canvas.Find("huochai");
        if (huochai != null)
        {
            huochai.gameObject.SetActive(true);
            huochai.localPosition = new Vector3(-149f, -383.6f, 0.512f);
        }
        else
        {
            TextCtrl huochai_text = new GameObject("huochai").GetScript<TextCtrl>();
            huochai_text.SetText("火柴");
            huochai_text.transform.SetParent(canvas, false);
            huochai_text.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 100);
            huochai_text.transform.localPosition = new Vector3(-307f, -483f, 2.027f);
            huochai_text.transform.SetAsFirstSibling();
        }
        Transform mutiao = canvas.Find("mutiao");
        if (mutiao != null)
        {
            mutiao.gameObject.SetActive(true);
            mutiao.localPosition = new Vector3(-323, -427f, 0.553f);
        }
        else
        {
            TextCtrl mutiao_text = new GameObject("mutiao").GetScript<TextCtrl>();
            mutiao_text.SetText("木条");
            mutiao_text.transform.SetParent(canvas, false);
            mutiao_text.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 100);
            mutiao_text.transform.localPosition = new Vector3(-573f, -483f, 2.027f);
            mutiao_text.transform.SetAsFirstSibling();
        }

        Transform sqPar = root.Find("desk/pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1");

        LiquidCtrl rightCtrl = sqPar.Find("right").gameObject.GetScript<LiquidCtrl>();
        rightCtrl.Level = 0.6f;

        LiquidCtrl leftCtrl = sqPar.Find("left").gameObject.GetScript<LiquidCtrl>();
        leftCtrl.Level = 0.75f;

        LiquidCtrl middleCtrl = root.Find("desk/pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1/middle").gameObject.GetScript<LiquidCtrl>();
        middleCtrl.Level = 0.76f;

        Transform mtUI = canvas.Find("mutiao");
        if (mtUI)
        {
            mtUI.localPosition = new Vector3(-323, -427, 0.521f);
        }

        mFireNum.Reset();

        DectectMoudle();
        //侧边按钮显示。
        Debug.Log("OnDetectionCallback:  " + "电解重置");
    }
    /// <summary>
    /// 检测模块
    /// </summary>
    void DectectMoudle()
    {
        //检测模块
        //火柴和木条显示       
        Transform tuopan = root.Find("desk/tuopan");
        tuopan.Find("water").gameObject.SetActive(false);
        tuopan.Find("naoh").gameObject.SetActive(false);
        tuopan.Find("bolibang").gameObject.SetActive(false);

        hcPar.localPosition = new Vector3(-0.069f, 0.131f, -0.1002f);

        Transform right = root.Find("desk/pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1/right");
        right.gameObject.layer = 11;
        BoxCollider bc = right.gameObject.GetBoxCollider();
        bc.center = new Vector3(-0.01f, 0.4f, 0);
        bc.size = new Vector3(0.1f, 0.3f, 0.5f);

        Transform left = root.Find("desk/pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1/left");
        left.gameObject.layer = 11;
        bc = left.gameObject.GetBoxCollider();
        bc.center = new Vector3(0.01f, 0.4f, 0);
        bc.size = new Vector3(0.1f, 0.3f, 0.5f);

        dragPar = tuopan.Find("dragPar");
        if (dragPar == null)
        {
            dragPar = new GameObject("dragPar").transform;
            dragPar.SetParent(tuopan);
            dragPar.localScale = Vector3.one;
            dragPar.localPosition = Vector3.zero;
        }
        hcPar.gameObject.SetActive(true);
        root.Find("desk/pour/hx_hxyq_sdjq/switchoff").GetComponent<BoxCollider>().enabled = false;

        Transform mt = hcPar.Find("mutiao");
        mt.GetComponent<FireCtrl>().SetPos(new Vector3(-0.17f, 0, 0));

        GlobalConfig.Instance.operationModel = OperationModel.Move;
        simpleDrag.canDrag = true;
        simpleDrag.ClickAction = (obj) =>//处理桌子上的点击物体
        {
            ClickDeskCallback(obj);
            return false;
        };
        simpleDrag.DragCallback = (record, trans) =>//处理拖拽过程中的物体
        {
            FireAir(record, trans);
        };
    }
    void ClickDeskCallback(GameObject obj)
    {
        if (obj.transform.parent != hcPar)
        {
            return;
        }
        string objName = obj.name;
        Transform deskObj = dragPar.Find(objName);
        if (deskObj != null)
        {
            FireCtrl fCtrl = deskObj.gameObject.GetScript<FireCtrl>();
            fCtrl.Reset();
        }
        //仪器上的物体重置
        Transform right = root.Find("desk/pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1/right");
        int count = right.childCount;
        if (count > 0)
        {
            Transform qingqi = right.Find("qingqi");
            if (qingqi != null)
            {
                qingqi.gameObject.SetActive(false);
            }
            Transform tr = right.GetChild(0);
            FireCtrl _fctrl = tr.GetComponent<FireCtrl>();
            if (_fctrl != null)
            {
                _fctrl.Reset();
            }
        }
        Transform left = root.Find("desk/pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1/left");
        count = left.childCount;
        if (count > 0)
        {
            Transform tr = left.GetChild(0);
            FireCtrl _fctrl = tr.GetComponent<FireCtrl>();
            if (_fctrl != null)
            {
                _fctrl.Reset();
            }
        }

        //当前拖拽物体重置
        obj.transform.SetParent(dragPar);
        //obj.GetBoxCollider().enabled = false;

        //桌子上的火柴或者木条
        deskObj = hcPar.Find(objName);
        if (deskObj == null)
        {
            GameObject mObj = ResManager.GetPrefab("SceneRes/" + objName);
            mObj.transform.SetParent(hcPar);
            mObj.name = objName;
            Vector3 pos = objName == FireEnum.huochai.ToString("g") ? new Vector3(0.06f, 0, 0) : new Vector3(-0.17f, 0, 0);
            mObj.transform.localPosition = pos;
            mObj.transform.localScale = new Vector3(1, 3, 1);
            mObj.GetScript<FireCtrl>();
        }
        //仪器按钮重置
        Transform niuRight = root.Find("desk/pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1/polySurface20/niu");
        niuRight.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
        Transform niuLeft = root.Find("desk/pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1/polySurface23/niu");
        niuLeft.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
    }
    /// <summary>
    /// 火柴和木条放到仪器上面回调。
    /// </summary>
    void FireAir(Transform record, Transform boxTr)
    {
        posNeg pn;
        if (boxTr.name == "left")//正极
        {
            //当前一起面物体删除         
            pn = posNeg.left;
            int a = record.name == "huochai" ? ++mFireNum.huochaiPosNum : ++mFireNum.mtPosNum;
            //Debug.Log(a);
        }
        else
        {
            pn = posNeg.right;
            int a = record.name == "huochai" ? ++mFireNum.huochaiNegNum : ++mFireNum.mtNegNum;
        }

        if (pn == posNeg.right)
        {
            Transform qingqi = root.Find("desk/pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1/qingqi");
            if (qingqi != null)
            {
                qingqi.gameObject.SetActive(false);
            }
        }

        //当前拖拽物体设置到仪器上。
        FireCtrl fctrl = record.gameObject.GetScript<FireCtrl>();
        fctrl.SetLeftRight(pn, boxTr);
        UI.transform.SetParent(MrSys.transform);

        Transform niuRight = root.Find("desk/pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1/polySurface20/niu");
        //niuRight.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
        Transform niuLeft = root.Find("desk/pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1/polySurface23/niu");
        //niuRight.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));

        DOTween.KillAll();
        DOTween.To(
            () =>
            {
                Transform _niu = pn == posNeg.left ? niuLeft : niuRight;
                Tweener twNiu = _niu.DOLocalRotate(Vector3.zero, 0.5f);
                return 0;
            },
            (a) =>
            {
            },
            1,
            3
        ).onComplete = () =>
        {
            if (mFireNum.AllOver())
            {
                //         string str = $"\n\u3000\u3000步骤:\n\u3000\u3000步骤1：分别用带火星的木条和点燃的火柴靠近正极玻璃管尖嘴处，打开活塞，" +
                //$"观察现象\n\u3000\u3000步骤2：分别用带火星的木条和点燃的火柴靠近负极玻璃管尖嘴处，打开活塞，观察现象" +
                //$"\n\u3000\u3000比单独电解蒸馏水速度快。说明水中加入少量氢氧化钠可增强导电性";
                //         SetBlackboardShow(str);

                //simpleDrag.ClickAction = null;
                //GlobalConfig.Instance.operationModel = OperationModel.Stay;
            }
        };
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
    /// <summary>
    /// 检测按钮
    /// </summary>
    void OnPrincipleCallback()
    {
        ResetInstruments();

        transform.Find("chemical").gameObject.SetActive(true);

        canvas.Find("Left/UIButton").gameObject.SetActive(false);
        Transform equTr = canvas.Find("equation");
        if (equTr == null)
        {
            equTr = ResManager.GetPrefab("SceneRes/equation").transform;
            equTr.SetParent(canvas, false);
            equTr.SetAsFirstSibling();
        }
        equTr.gameObject.SetActive(true);
        equTr.transform.localPosition = new Vector3(0, 266, 1.597f);
        /*Equation equ =*/
        equTr.gameObject.GetScript<Equation>();

        string str = string.Empty;
        SetBlackboardShow(str);

        Transform chequa = canvas.Find("chemicalEquation");
        if (chequa == null)
        {
            chequa = ResManager.GetPrefab("SceneRes/chemicalEquation").transform;
            chequa.SetParent(canvas, false);

        }
        chequa.transform.SetAsFirstSibling();
        chequa.gameObject.SetActive(true);
        chequa.localPosition = new Vector3(0, 120, 1.6f);
        chequa.transform.localScale = Vector3.one;

        //左侧按钮隐藏
        Transform left = UI.Find("InprojectionIgnoreCanvas/Left");
        Transform btn = left.Find("UIButton (1)");
        btn.gameObject.SetActive(false);

        //ui隐藏        
        Transform waterUI = canvas.Find("water");
        Transform naohUI = canvas.Find("naoh");
        if (naohUI)
        {
            naohUI.gameObject.SetActive(false);
        }
        if (waterUI)
        {
            waterUI.gameObject.SetActive(false);
        }
        Transform mutiaoUI = canvas.Find("mutiao");
        if (mutiaoUI != null)
        {
            mutiaoUI.gameObject.SetActive(false);
        }
        Transform huochaiUI = canvas.Find("huochai");
        if (huochaiUI != null)
        {
            huochaiUI.gameObject.SetActive(false);
        }
        //桌面盘子中水杯隐藏。
        Transform tuopan = root.Find("desk/tuopan");
        tuopan.gameObject.SetActive(false);

        Transform sqPar = root.Find("desk/pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1");
        LiquidCtrl rightCtrl = sqPar.Find("right").gameObject.GetScript<LiquidCtrl>();
        rightCtrl.Level = 0.94f;
        LiquidCtrl leftCtrl = sqPar.Find("left").gameObject.GetScript<LiquidCtrl>();
        leftCtrl.Level = 0.94f;
        LiquidCtrl middleCtrl = sqPar.Find("middle").gameObject.GetScript<LiquidCtrl>();
        middleCtrl.Level = 0.91f;

        PrincipleMoudle();
    }
    void PrincipleMoudle()
    {
        if (jiantouCtrl == null)
        {
            jiantouCtrl = ResManager.GetPrefab("SceneRes/jiantou").GetScript<JiantouCtrl>();
        }
        jiantouCtrl.SetJiantou(new Vector3(-0.171f, -0.126f, 0.108f));

        root.Find("desk/pour/hx_hxyq_sdjq/switchoff").GetComponent<BoxCollider>().enabled = true;
        //电源按钮点击        
        GlobalConfig.Instance.operationModel = OperationModel.Stay;
        simpleDrag.canDrag = true;
        simpleDrag.ClickAction = (obj) =>
        {
            bool is3D = true;
            if (obj.name == "switchoff")//电源开关管理
            {
                Debug.Log("电源开关处理");
                simpleDrag.ClickAction = null;
                jiantouCtrl.Show(false);
                SwitchOff(obj);
                PrincleMoudleCreatebubble();//生成气泡
                BottomContainer();
            }
            return is3D;
        };

    }
    void PrincleMoudleCreatebubble()
    {
        ParticleSystem qipao_right = root.Find("desk/qipao_right").GetComponent<ParticleSystem>();
        Transform right_trigger = qipao_right.transform.Find("right_Trigger");
        BoxCollider box;
        //BubbleCtrl bLeft, bRight;
        if (right_trigger == null)
        {
            box = new GameObject("right_Trigger").GetBoxCollider();
            right_trigger = box.transform;
            box.transform.SetParent(qipao_right.transform);
            box.transform.localPosition = new Vector3(-0.0008f, 0, 0.2941f);
            box.size = new Vector3(0.1f, 0.01f, 0.1f);
            ParticleSystem.TriggerModule trigger = qipao_right.trigger;
            trigger.inside = ParticleSystemOverlapAction.Kill;
            trigger.enabled = true;
            trigger.SetCollider(0, box);
        }
        /*bRight =*/
        right_trigger.gameObject.GetScript<BubbleCtrl>();
        ParticleSystem.VelocityOverLifetimeModule right_velocity = qipao_right.velocityOverLifetime;
        //rightbubbleSpeed = 0.15f;
        right_velocity.z = rightbubbleSpeed;
        qipao_right.Play();

        ParticleSystem qipao_left = root.Find("desk/qipao_left").GetComponent<ParticleSystem>();
        qipao_left.transform.localPosition = new Vector3(0.175f, -0.193f, -0.007f);
        Transform left_trigger = qipao_left.transform.Find("left_Trigger");
        if (left_trigger == null)
        {
            box = new GameObject("left_Trigger").GetBoxCollider();
            left_trigger = box.transform;
            box.transform.SetParent(qipao_left.transform);
            box.transform.localPosition = new Vector3(0, 0, 0.2918f);
            box.size = new Vector3(0.1f, 0.01f, 0.1f);
            ParticleSystem.TriggerModule trigger = qipao_left.trigger;
            trigger.inside = ParticleSystemOverlapAction.Kill;
            trigger.enabled = true;
            trigger.SetCollider(0, box);
        }
        /*bLeft = */
        left_trigger.gameObject.GetScript<BubbleCtrl>();

        ParticleSystem.VelocityOverLifetimeModule left_velocity = qipao_left.velocityOverLifetime;
        //leftbubbleSpeed = 0.12f;
        left_velocity.z = leftbubbleSpeed;
        qipao_left.Play();
    }
    //近距离观察溶液。
    void BottomContainer()
    {
        UI.SetParent(MrSys.transform, false);
        //加载微观场景

        ParticleSystem ps_right = root.Find("desk/qipao_right").GetComponent<ParticleSystem>();
        ParticleSystem.MainModule main = ps_right.main;
        main.startSpeed = 0.1f;
        ps_right.Play();

        ParticleSystem ps_left = root.Find("desk/qipao_left").GetComponent<ParticleSystem>();
        ParticleSystem.MainModule main_left = ps_left.main;
        main_left.startSpeed = 0.1f;
        ps_left.Play();

        //水分子创建       
        CreateH2O();

        Vector3 camPos = new Vector3(0.3f, -0.75f, 5.9f);
        Transform tr = MrSys.transform;
        Vector3 pos_3d = cam3D.transform.localPosition;
        if (Monitor23DMode.instance.is3D)
        {
            tr = cam3D.transform;
            camPos = new Vector3(0.18f, 0.16f, 0.4f);
            cam3D.enabled = false;
        }
        //cam3D.enabled = false;
        //cam3D.enabled = false;
        Tweener tw = tr.DOMove(camPos, 3f);
        tw.onComplete = () =>
        {
            //cam3D.enabled = true;
            //溶液震动                    
            LiquidCtrl lctl = root.Find("desk/shuidi").gameObject.GetScript<LiquidCtrl>();
            lctl.SolutionVibration(0.68f, 1f, 0.66f, 0.4f, 0.3f, 0.5f);
        };

        //return;

        DOTween.To(
            () =>
            {
                //相机拉近
                return 1;
            },
            (a) =>//每一帧都执行。
            {
                //溶液震动
                //Debug.LogError("溶液");
            },
            0,
            3.5f
            ).onComplete = () =>
            {
                //溶液震动            
                if (!Monitor23DMode.instance.is3D)
                {
                    MrSys.transform.localPosition = new Vector3(0, 0, 0.3f);
                }

                //微观场景加载                
                Transform weiguan = transform.Find("huaxue_weiguan");
                if (weiguan == null)
                {
                    weiguan = ResManager.GetPrefab("MicroScene/huaxue_weiguan").transform;
                    weiguan.SetParent(transform, true);
                    //curScene = weiguan.gameObject;
                }
                weiguan.gameObject.SetActive(true);
                //主场景取消
                Transform chemical = transform.Find("chemical");
                if (chemical != null)
                {
                    chemical.gameObject.SetActive(false);
                }
                //ui隐藏
                canvas.Find("chemicalEquation").gameObject.SetActive(false);
                canvas.Find("equation").gameObject.SetActive(false);

                canvas.Find("panelText").gameObject.SetActive(false);
                canvas.Find("posNeg").gameObject.SetActive(false);
                MicroScene();
            };
    }
    /// <summary>
    /// 生成水分子。
    /// </summary>
    void CreateH2O()
    {
        //生成水分子。
        //Transform microScene = transform.Find("huaxue_weiguan");
        Transform h2oPar = root.Find("desk/shuidi/H2oPar");
        if (h2oPar == null)
        {
            h2oPar = new GameObject("H2oPar").transform;
            h2oPar.SetParent(root.Find("desk/shuidi"));
            h2oPar.localPosition = Vector3.zero;
            h2oPar.localScale = Vector3.one;
            h2oPar.localRotation = Quaternion.Euler(Vector3.zero);
        }
        /* moveOfMolecular_ice mic = */
        h2oPar.gameObject.GetScript<moveOfMolecular_ice>();
        //
    }
    /// <summary>
    /// 微场景操作
    /// </summary>
    void MicroScene()
    {
        MrSys.transform.localPosition = new Vector3(0, 0, 0.3f);
        Transform uibtn = canvas.Find("Left/UIButton");
        uibtn.gameObject.SetActive(true);

        Transform cheE = canvas.Find("chemicalEquation");
        cheE?.gameObject.SetActive(true);
        cheE.transform.localPosition = new Vector3(0, -229, 0f);
        cheE.transform.localScale = Vector3.one;

        Transform micro_posNeg = canvas.Find("micro_posNeg");
        if (micro_posNeg == null)
        {
            micro_posNeg = ResManager.GetPrefab("SceneRes/posNeg").transform;
            micro_posNeg.SetParent(canvas, false);
            micro_posNeg.name = "micro_posNeg";
            micro_posNeg.localPosition = new Vector3(-122, 243, 1.395f);
            micro_posNeg.Find("pos").localPosition = new Vector3(-447, 275, 0);
            micro_posNeg.Find("neg").localPosition = new Vector3(847, 280, 0);
            micro_posNeg.Find("neg").GetComponentInChildren<Image>().color = new Color(1, 0, 0, 1);
        }
        micro_posNeg.gameObject.SetActive(true);

        MrSys.transform.localPosition = new Vector3(0, 0, 0.3f);
        //MrSys.transform.localPosition = new Vector3(0, 0, 0);
        cam3D.enabled = true;
        //播动画      
        Transform weiguan = transform.Find("huaxue_weiguan");
        Transform h2oElec = weiguan.Find("waterElectrolysis");
        if (h2oElec == null)
        {
            h2oElec = ResManager.GetPrefab("MicroScene/waterElectrolysis").transform;//水电解生成            
        }
        else
        {
            h2oElec.gameObject.SetActive(true);
        }
        h2oElec.name = "waterElectrolysis";
        h2oElec.SetParent(weiguan);
        h2oElec.transform.localPosition = new Vector3(0, 0.438f, -1.303f);
        h2oElec.localScale = Vector3.one;
        AnimationOper aop = h2oElec.transform.Find("effect").gameObject.GetAnimatorOper();
        aop.Complete += () =>
        {
            //返回主界面控制容器中液体高度         
            DOTween.To(
            () => { return 1; },
            (a) => { },
            0,
            3
            ).onComplete = () =>
            {
                //出现化学方程式
                //BackToMain();             
            };

        };
        aop.SetAnimSpeed(0.8f);
        aop.OnPause();
        DOTween.To(
            () => { return 1; },
            (a) => { },
            0,
            1
            ).onComplete = () =>
            {
                aop.OnContinue();
                aop.PlayForward("h2o");
            };
    }
    void ResetPrincple()
    {
        ResetInstruments();

        Transform equTr = canvas.Find("equation");
        if (equTr == null)
        {
            equTr = ResManager.GetPrefab("SceneRes/equation").transform;
            equTr.SetParent(canvas, false);
            equTr.SetAsFirstSibling();
        }
        equTr.gameObject.SetActive(true);
        equTr.gameObject.GetScript<Equation>();

        string str = string.Empty;
        SetBlackboardShow(str);

        Transform chequa = canvas.Find("chemicalEquation");
        if (chequa == null)
        {
            chequa = ResManager.GetPrefab("SceneRes/chemicalEquation").transform;
            chequa.SetParent(canvas, false);
            chequa.localPosition = new Vector3(0, 120, 6.6f);
        }
        chequa.transform.SetAsFirstSibling();
        chequa.gameObject.SetActive(true);

        //左侧按钮隐藏
        Transform left = UI.Find("InprojectionIgnoreCanvas/Left");
        Transform btn = left.Find("UIButton (1)");
        btn.gameObject.SetActive(false);

        //ui隐藏        
        Transform waterUI = canvas.Find("water");
        Transform naohUI = canvas.Find("naoh");
        if (naohUI)
        {
            naohUI.gameObject.SetActive(false);
        }
        if (waterUI)
        {
            waterUI.gameObject.SetActive(false);
        }

        //桌面盘子中水杯隐藏。
        Transform tuopan = root.Find("desk/tuopan");
        tuopan.gameObject.SetActive(false);

        Transform sqPar = root.Find("desk/pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1");
        LiquidCtrl rightCtrl = sqPar.Find("right").gameObject.GetScript<LiquidCtrl>();
        rightCtrl.Level = 0.94f;
        LiquidCtrl leftCtrl = sqPar.Find("left").gameObject.GetScript<LiquidCtrl>();
        leftCtrl.Level = 0.94f;
    }
    void BackToMain()
    {
        Debug.Log("backTomain");

        transform.Find("chemical").gameObject.SetActive(true);

        ResetPrincple();

        Transform micro_posNeg = canvas.Find("micro_posNeg");
        micro_posNeg.gameObject.SetActive(false);

        ParticleSystem qipao_right = root.Find("desk/qipao_right").GetComponent<ParticleSystem>();
        Transform right_trigger = qipao_right.transform.Find("right_Trigger");
        BoxCollider box;
        BubbleCtrl bLeft, bRight;
        if (right_trigger == null)
        {
            box = new GameObject("right_Trigger").GetBoxCollider();
            right_trigger = box.transform;
            box.transform.SetParent(qipao_right.transform);
            box.transform.localPosition = new Vector3(-0.0008f, 0, 0.2941f);
            box.size = new Vector3(0.1f, 0.01f, 0.1f);
            ParticleSystem.TriggerModule trigger = qipao_right.trigger;
            trigger.inside = ParticleSystemOverlapAction.Kill;
            trigger.enabled = true;
            trigger.SetCollider(0, box);
        }
        bRight = right_trigger.gameObject.GetScript<BubbleCtrl>();
        ParticleSystem.VelocityOverLifetimeModule right_velocity = qipao_right.velocityOverLifetime;
        //rightbubbleSpeed = 0.15f;
        right_velocity.z = rightbubbleSpeed;
        ParticleSystem.MainModule main = qipao_right.main;
        main.startSpeed = 0.1f;
        // qipao_right.Play();

        ParticleSystem qipao_left = root.Find("desk/qipao_left").GetComponent<ParticleSystem>();
        qipao_left.transform.localPosition = new Vector3(0.175f, -0.193f, -0.007f);
        Transform left_trigger = qipao_left.transform.Find("left_Trigger");
        if (left_trigger == null)
        {
            box = new GameObject("left_Trigger").GetBoxCollider();
            left_trigger = box.transform;
            box.transform.SetParent(qipao_left.transform);
            box.transform.localPosition = new Vector3(0, 0, 0.2918f);
            box.size = new Vector3(0.1f, 0.01f, 0.1f);
            ParticleSystem.TriggerModule trigger = qipao_left.trigger;
            trigger.inside = ParticleSystemOverlapAction.Kill;
            trigger.enabled = true;
            trigger.SetCollider(0, box);
        }
        bLeft = left_trigger.gameObject.GetScript<BubbleCtrl>();

        ParticleSystem.VelocityOverLifetimeModule left_velocity = qipao_left.velocityOverLifetime;
        //leftbubbleSpeed = 0.12f;
        left_velocity.z = leftbubbleSpeed;
        ParticleSystem.MainModule mainLeft = qipao_left.main;
        mainLeft.startSpeed = 0.06f;


        posNeg pn = posNeg.left;
        LiquidCtrl LCtrl = root.Find("desk/pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1/" + pn.ToString("g")).gameObject.GetScript<LiquidCtrl>();
        LCtrl.Level = 0.75f;

        pn = posNeg.right;
        LCtrl = root.Find("desk/pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1/" + pn.ToString("g")).gameObject.GetScript<LiquidCtrl>();
        LCtrl.Level = 0.6f;

        LiquidCtrl middleCtrl = root.Find("desk/pour/hx_hxyq_sdjq/hx_hxyq_sdjq 1/middle").gameObject.GetScript<LiquidCtrl>();
        middleCtrl.Level = 0.76f;

        simpleDrag.canDrag = false;
    }
    //
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
        StopAllCoroutines();
    }

    IEnumerator Scale(Transform tar)
    {
        yield return new WaitForSeconds(1);
        Vector3 scale = tar.localScale;
        WaitForSeconds wf = new WaitForSeconds(0.01f);
        for (int j = 0; j < 2; j++)
        {
            for (float i = 0; i < 1; i += 0.04f)
            {
                float t = Mathf.Abs(Mathf.Sin(Mathf.PI * 4 * i));
                tar.localScale = scale * (1 + 1f * t);
                //Debug.Log(t + ",   " + tar.localScale);
                yield return wf;
            }
        }
    }
}
public enum FireEnum
{
    huochai,//火柴
    mutiao,//木条
}

public class JiantouCtrl : MonoBehaviour
{
    private void Start()
    {
        this.name = "jiantou";
        Transform root = Tools.GetScenesObj("GameCtrl").transform.Find("chemical/root");
        transform.SetParent(root, false);
        transform.localScale = Vector3.one * 0.5f;
        transform.Find("GQ").localPosition = new Vector3(0, -0.024f, 0);
    }
    /// <summary>
    /// 设置箭头属性
    /// </summary>
    public void SetJiantou(Vector3 pos)
    {
        transform.localPosition = pos;
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

/// <summary>
/// 液面box控制
/// </summary>
public class BubbleCtrl : MonoBehaviour
{
    bool flow = false;
    float limit;
    float speed;
    public enum flowDirection//流动方向
    {
        none,
        up,
        down
    }
    flowDirection flowDir;

    public void SetFlow(flowDirection dir, float mSpeed, float mLimit)
    {
        flow = true;
        flowDir = dir;
        speed = mSpeed;
        limit = mLimit;
    }
    private void Update()
    {
        if (!flow)
        {
            return;
        }

        Vector3 pos = transform.localPosition;
        //Debug.Log(pos);
        if (flowDir == flowDirection.up)
        {
            if (pos.z <= limit)
            {
                pos.z += Time.deltaTime * speed;
                transform.localPosition = pos;
            }
            else
            {
                flow = false;
            }
        }
        else
        {
            if (pos.z > limit)
            {
                pos.z -= Time.deltaTime * speed;
                transform.localPosition = pos;
            }
            else
            {
                flow = false;
            }
        }
    }
}

public class FireCtrl : MonoBehaviour
{
    public Vector3 originPos;
    Transform originPar;
    Quaternion originQt;
    private void Awake()
    {
        originPos = transform.localPosition;
        originPar = transform.parent;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        originQt = transform.localRotation;
        //mPn = posNeg.middle;
        ////curState = FireState.stay;
    }
    private void Start()
    {
        ParticleSystem ps = transform.Find("hx_hxyq_hxmt/huo1").GetComponent<ParticleSystem>();
        if (this.name == "huochai")
        {
            ps.Play();
        }
    }
    public void Reset()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
        transform.parent = originPar;
        transform.localPosition = originPos;
        transform.localRotation = originQt;
        ParticleSystem[] ps = transform.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < ps.Length; i++)
        {
            ps[i].Stop();
        }
        transform.Find("hx_hxyq_hxmt/guang").GetComponent<ParticleSystem>().Play();
        if (this.name == "huochai")
        {
            transform.Find("hx_hxyq_hxmt/huo1").GetComponent<ParticleSystem>().Play();
        }
        gameObject.GetBoxCollider().enabled = true;
        transform.localScale = new Vector3(1, 3, 1);
    }
    void FireH2()
    {
        //TODO氢气点燃
        Transform qingqi = transform.parent.Find("qingqi");
        if (qingqi == null)
        {
            qingqi = ResManager.GetPrefab("SceneRes/qingqi").transform;
            qingqi.name = "qingqi";
            qingqi.SetParent(transform.parent);
            qingqi.localPosition = new Vector3(0, 0.3826f, 0);
        }
        qingqi.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
    /// 特效播放
    /// </summary>
    /// <param name="effectName"></param>
    void PlayEffect(posNeg pn)
    {
        //Debug.LogError(pn.ToString("g"));
        switch (pn)
        {
            case posNeg.right://负极
                if (this.name == "huochai")
                {
                    ParticleSystem[] ps = transform.GetComponentsInChildren<ParticleSystem>();
                    ps[0].Play();
                    //ps[1].Stop();
                    //ps[2].Stop();
                    //氢气点燃TODO    
                    CancelInvoke("FireH2");
                    Invoke("FireH2", 1);
                }
                else
                {
                    //木条熄灭
                    ParticleSystem[] ps = transform.GetComponentsInChildren<ParticleSystem>();
                    for (int i = 0; i < ps.Length; i++)
                    {
                        ps[i].Stop();
                    }
                }
                break;
            case posNeg.left://正极
                if (this.name == "huochai")//火柴燃烧更旺
                {
                    ParticleSystem psg = transform.Find("hx_hxyq_hxmt/guang").GetComponent<ParticleSystem>();
                    psg.Play();

                    ParticleSystem huo2 = transform.Find("hx_hxyq_hxmt/huo2").GetComponent<ParticleSystem>();
                    huo2.transform.localPosition = new Vector3(0, 0.015f, 0);
                    huo2.transform.localRotation = Quaternion.Euler(new Vector3(50, 90, 0));
                    huo2.Play();
                    //Debug.Log(huo2.name);
                    DOTween.To(
                        () => { return 0; },
                        (a) => { },
                        0,
                        1.5f
                        ).onComplete = () =>
                        {
                            ParticleSystem huo1 = transform.Find("hx_hxyq_hxmt/huo1").GetComponent<ParticleSystem>();
                            huo1.Stop();
                        };

                }
                else
                {
                    //木柴复燃。
                    ParticleSystem psg = transform.Find("hx_hxyq_hxmt/huo1").GetComponent<ParticleSystem>();
                    psg.transform.localPosition = new Vector3(0.0004f, 0.0155f, -0.00077f);
                    psg.Play();
                    ParticleSystem guang = transform.Find("hx_hxyq_hxmt/guang").GetComponent<ParticleSystem>();
                    guang.Play();

                }
                break;
            case posNeg.middle:
                break;
            default:
                break;
        }
    }
    public void SetLeftRight(posNeg pn, Transform par)
    {
        transform.SetParent(par);
        switch (pn)
        {
            case posNeg.right://负极
                transform.localPosition = new Vector3(0.0289f, 0.4202f, -0.0005f);
                transform.localRotation = Quaternion.Euler(0, 0, 140);
                break;
            case posNeg.left://正极
                transform.localPosition = new Vector3(0.0293f, 0.423f, 0.0006f);
                transform.localRotation = Quaternion.Euler(0, 0, 140);
                break;
            case posNeg.middle:
                break;
            default:
                break;
        }
        transform.localScale = new Vector3(0.6f, 0.6f, 1);
        gameObject.GetBoxCollider().enabled = false;
        transform.SetAsFirstSibling();
        DOTween.KillAll();
        PlayEffect(pn);
    }
    void OnDestroy()
    {
        CancelInvoke();
    }
    public void SetPos(Vector3 newPos)
    {
        transform.localPosition = newPos;
        originPos = newPos;
    }
}
public enum FireState
{
    stay,//非燃烧状态    
    move,
    fire,//燃烧状态
}
public enum posNeg
{
    right,//正极
    left,//负极
    middle,//中间水管
    //shaobei //烧杯水流控制。
}


//ui需要单独处理。
//函数模块化，委托化。

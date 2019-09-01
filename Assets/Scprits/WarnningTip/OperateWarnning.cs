using UnityEngine;
using UnityEngine.UI;

namespace liu
{
    /// <summary>
    /// 操作警示提示
    /// </summary>
    public class OperateWarnning : MonoBehaviour
    {

        public static OperateWarnning Instance;
        float showTime = 1f;
        GameObject panelRoot;
        GameObject blockPanel;

        Text _desText;

        private void Awake()
        {
            Instance = this;
            panelRoot = transform.Find("PanelRoot").gameObject;
            _desText = transform.Find("PanelRoot/Image/Text").GetComponent<Text>();
            blockPanel = panelRoot.transform.Find("BlockPanel").gameObject;
            UGUIEventListener.Get(blockPanel).onClick = OnClickBlockPanel;

            ControllerPanelState(false);
        }


        private void OnClickBlockPanel(GameObject go)
        {
            ClosePanel();
        }

        bool panelIsShow = false;
        float countDownTimer = 0;
        /// <summary>
        /// 显示不可操作的警示
        /// </summary>
        /// <param name="desText">警示的内容</param>
        public void ShowWarnningPanel(string desText)
        {
            ControllerPanelState(true);
            _desText = transform.Find("PanelRoot/Image/Text").GetComponent<Text>();
            _desText.text = desText;

            panelIsShow = true;
            countDownTimer = 0;

        }

        private void Update()
        {
            if (panelIsShow)
            {
                countDownTimer += Time.deltaTime;
                if (countDownTimer > showTime)
                {
                    ClosePanel();
                }
            }
        }

        void ClosePanel()
        {
            ControllerPanelState(false);
        }

        void ControllerPanelState(bool show)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(show);
            }
        }
    }
}
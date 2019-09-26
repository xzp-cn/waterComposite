using UnityEngine;
using UnityEngine.UI;
public class TextCtrl : MonoBehaviour
{
    Text text;
    RectTransform rt;
    private void Awake()
    {
        text = gameObject.GetScript<Text>();
        text.fontSize = 50;
        text.lineSpacing = 1;
        text.supportRichText = true;

        text.font = Font.CreateDynamicFontFromOSFont("Arial", 50);
        text.color = Color.white;
        text.alignment = TextAnchor.UpperLeft;

        rt = GetComponent<RectTransform>();
        //rt.anchorMax = new Vector2(1, 1);
        //rt.anchorMin = Vector2.zero;
        //rt.offsetMax = new Vector2(168.1f, 98.95f);
        //rt.offsetMin = new Vector2(308.3f, 394.35f);
        Vector3 pos = new Vector3(90, 156, 1.495f);
        rt.anchoredPosition3D = pos;
    }
    /// <summary>
    /// 控制字符串的显示
    /// </summary>
    /// <param name="textStr"></param>
    public void SetText(string textStr)
    {
        gameObject.SetActive(true);
        text.text = textStr;
        text.text.Replace("\\n", "\n");
    }
    public void SetFont(int size)
    {
        text.fontSize = size;
    }
    public void Show(bool isShow)
    {
        text.enabled = isShow;
    }
}

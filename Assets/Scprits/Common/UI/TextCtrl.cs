using UnityEngine;
using UnityEngine.UI;
public class TextCtrl : MonoBehaviour
{
    Text text;
    private void Awake()
    {
        text = gameObject.GetScript<Text>();
        text.fontSize = 60;
        text.lineSpacing = 1;
        text.supportRichText = true;

        text.font = Font.CreateDynamicFontFromOSFont("Arial", 50);
        text.color = Color.white;
        text.alignment = TextAnchor.UpperLeft;
    }
    /// <summary>
    /// 控制字符串的显示
    /// </summary>
    /// <param name="textStr"></param>
    public void SetText(string textStr)
    {
        text.text = textStr;
        text.text.Replace("\\n", "\n");
    }
    public void Show(bool isShow)
    {
        text.enabled = isShow;
    }
}

using UnityEngine;
using UnityEngine.UI;
public class Equation : MonoBehaviour
{
    private void Awake()
    {
        transform.localScale = Vector3.one * 2;
    }
    private void Start()
    {
        transform.localPosition = new Vector3(0, 266, 6.571f);
    }
    /// <summary>
    /// 左边字体表示
    /// </summary>
    public void SetTextLeft(string leftText)
    {
        transform.Find("left/Text").GetComponent<Text>().text = leftText;
    }
    /// <summary>
    /// 中间字体显示
    /// </summary>
    public void SetTextMiddle(string midText)
    {
        transform.Find("middle/Text").GetComponent<Text>().text = midText;
    }
    /// <summary>
    /// 设置右边字体显示
    /// </summary>
    public void SetTextRight(string rightText)
    {
        transform.Find("right/Text").GetComponent<Text>().text = rightText;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace liu
{
    [RequireComponent(typeof(LayoutGroup))]
    /// <summary>
    /// 根据LayoutGroup是VerticalLayoutGroup 还是HorizontalLayoutGroup
    /// 设置当前ScrollView的高度
    /// </summary>
    public class SetScrollViewHeight : MonoBehaviour
    {
        /// <summary>
        /// 获取当前的LayoutGroup
        /// 来判断是根据VerticalLayoutGroup 还是 HorizontalLayoutGroup
        /// </summary>
        LayoutGroup curLayoutGroup
        {
            get { return GetComponent<LayoutGroup>(); }
        }

        /// <summary>
        /// 当前ScrollView界面可以观察到的个数
        /// </summary>
        [MyRangeAttribute(1, 8, "纵向展示的个数")]
        public int showVerticalItemCount = 5;

        [MyRangeAttribute(6, 11, "横向展示的个数")]
        public int showHorizonItemCount = 6;

        /// <summary>
        /// 根据不同的布局和item个数，设置ScrollView的大小
        /// </summary>
        private void OnEnable()
        {
            //先判断是否是ScrollView ui组件，如果不是，下面的操作就没有意义
            if (transform.parent.parent.GetComponent<ScrollRect>())
            {
                //判断布局的类型，因为该脚本是需要layoutGroup才能挂载的
                if (curLayoutGroup is VerticalLayoutGroup)
                {
                    //转类型
                    VerticalLayoutGroup verticalLayoutGroup = curLayoutGroup as VerticalLayoutGroup;
                    //获取scrollView的RectTransform
                    RectTransform scrollViewRT = transform.parent.parent.transform as RectTransform;
                    //获取chil item 的height
                    RectTransform chilItemRT = transform.GetChild(0) as RectTransform;

                    float scrollViewRTWidth = chilItemRT.sizeDelta.x + verticalLayoutGroup.padding.left + verticalLayoutGroup.padding.right;

                    //计算scrollViewRTHeight  -7的原因是使用当前的buttonUI 尺寸在自适应的时候有边界的残留
                    float scrollViewRTHeight = verticalLayoutGroup.padding.top + verticalLayoutGroup.padding.bottom
                        + showVerticalItemCount * chilItemRT.sizeDelta.y + (showVerticalItemCount - 1) * verticalLayoutGroup.spacing - 7;
                    //对ScrollView sizeDelta赋值
                    scrollViewRT.sizeDelta = new Vector2(scrollViewRTWidth, scrollViewRTHeight);
                }
                else //HorizontalLayoutGroup
                {
                    //转类型
                    HorizontalLayoutGroup horizontalLayoutGroup = curLayoutGroup as HorizontalLayoutGroup;
                    //获取scrollView的RectTransform
                    RectTransform scrollViewRT = transform.parent.parent.transform as RectTransform;
                    //获取chil item 的height
                    RectTransform chilItemRT = transform.GetChild(0) as RectTransform;
                    //计算scrollView RTWidth
                    float scrollViewRTWidth = horizontalLayoutGroup.padding.left + horizontalLayoutGroup.padding.right
                        + showHorizonItemCount * chilItemRT.sizeDelta.x + (showHorizonItemCount - 1) * horizontalLayoutGroup.spacing + 7;

                    float scrollViewRTHeight = chilItemRT.sizeDelta.y + horizontalLayoutGroup.padding.top + horizontalLayoutGroup.padding.bottom;
                    //对ScrollView sizeDelta赋值
                    scrollViewRT.sizeDelta = new Vector2(scrollViewRTWidth, scrollViewRTHeight);
                }
            }
            else
            {
                Debug.LogWarning("SetScrollViewHeight.OnEnable() 该脚本不是使用在ScrollView组件下的Content上");
            }
        }


    }


    /// <summary>
    /// 实现一个在Inspector面板的滚动条控制int变量大小
    /// 和文字属性的功能
    /// </summary>
    public class MyRangeAttribute : PropertyAttribute
    {
        public int min;  //定义一个int的最小值
        public int max;  //定义一个int的最大值
        public string label;  //设置显示属性的文字

        public MyRangeAttribute(int min, int max, string label = "")
        {
            this.min = min;
            this.max = max;
            this.label = label;
        }
    }


    [CustomPropertyDrawer(typeof(MyRangeAttribute))]
    /// <summary>
    /// 对MyRangeAttribute的功能实现
    /// </summary>
    public class MyRangeAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //attribute 是 PropertyDrawer 中的一个PropertyAttribute属性
            //调用MyRangeAttribute的min，max，label用于绘制
            MyRangeAttribute myRangeAttribute = attribute as MyRangeAttribute;

            //拿到信息的值对界面进行绘制
            EditorGUI.IntSlider(position, property, myRangeAttribute.min, myRangeAttribute.max, myRangeAttribute.label);
        }
    }
}
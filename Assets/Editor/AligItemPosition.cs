using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace liu
{
    /// <summary>
    /// 根据限定的ScrollView item中的展示最大个数
    /// 然后根据当前显示的个数，动态调整显示区域
    /// </summary>
    [RequireComponent(typeof(LayoutGroup), typeof(SetScrollViewHeight))]
    public class AligItemPosition : MonoBehaviour
    {
        //记录第一个和当前显示最大个数的item位置
        Vector3 firstAnchorPosition, secondAnchorPosition = Vector3.zero;

        float offset = 0;

        SetScrollViewHeight setScrollViewHeight;
        SetScrollViewHeight SetScrollViewHeight
        {
            get
            {
                if (setScrollViewHeight == null) setScrollViewHeight = GetComponent<SetScrollViewHeight>();
                return setScrollViewHeight;
            }
        }

        LayoutGroup layoutGroup;

        private void OnEnable()
        {
            //根据不同的layout类型，获取对应的显示个数，设置比对锚点的数据信息
            layoutGroup = GetComponent<LayoutGroup>();

            layoutGroup.enabled = false;
            Invoke("DelaySetLayoutEnable", 0.1f);
        }
        void DelaySetLayoutEnable()
        {
            layoutGroup.enabled = true;
            
            Invoke("DelayCallBack", 0.1f);
        }
        void DelayCallBack()
        {

            //layoutGroup.enabled = false;
            int chilCount = transform.childCount;
            if (layoutGroup is HorizontalLayoutGroup)
            {
                //先判断SetScrollViewHeight设置的最大值会不会大于chilCount，会才做子物体点击的位置变换
                //会才给secondAnchorPosition 赋值
                if (chilCount > SetScrollViewHeight.showHorizonItemCount)
                {
                    
                    ////给两个记录点赋值
                    firstAnchorPosition = transform.GetChild(0).position;
                    secondAnchorPosition = transform.GetChild(SetScrollViewHeight.showHorizonItemCount - 1).position;
                    ////获取item与item之间的间隔
                    offset = Vector3.Distance(transform.GetChild(0).position, transform.GetChild(1).position);
                }
            }
            else
            {
                if (chilCount > SetScrollViewHeight.showVerticalItemCount)
                {
                    firstAnchorPosition = transform.GetChild(0).position;
                    secondAnchorPosition = transform.GetChild(SetScrollViewHeight.showVerticalItemCount - 1).position;
                    offset = Vector3.Distance(transform.GetChild(0).position, transform.GetChild(1).position);
                }
            }

            //给每个button的点击事件设置回调，判断点击的时候是否是在顶端或者底端
            //然后如果在顶端，则判断上面是否还有item，有的话往上移动一个offset的位置
            //如果在底端，判断往下还有item不，有的话往下移动一个offset的位置
            if (offset != 0) //说明现有的chilCout满足SetScrollViewHeight设定的最小展示值
            {
                for (int i = 0; i < chilCount; i++)
                {
                    UGUIEventListener.Get(transform.GetChild(i).gameObject).onClick += UIButtonOnClickHandler;
                }
            }
        }

        void UIButtonOnClickHandler(GameObject obj)
        {
            float disWithFirstAnchorPosition = Vector3.Distance(firstAnchorPosition, obj.transform.position);
            float disWithSecondAnchorPosition = Vector3.Distance(obj.transform.position, secondAnchorPosition);

            //说明是顶端的按钮,需要向上移动一个offset,没有满足的说明点在中间的UIButton上，不用移动
            if (disWithFirstAnchorPosition < disWithSecondAnchorPosition && disWithFirstAnchorPosition <= offset)
            {
                if (layoutGroup is HorizontalLayoutGroup)
                {
                    //点击的button与第一的button间还有多个button
                    if (Vector3.Distance(obj.transform.position, transform.GetChild(0).position) > disWithFirstAnchorPosition)
                    {
                        float curVerticalX = transform.position.x;
                        curVerticalX += offset;

                        //防止跳动，判断点击的物体是否是到左边，左边第一个content就设置为0
                        curVerticalX = curVerticalX > 0 ? 0 : curVerticalX;

                        transform.position = new Vector3(curVerticalX, transform.position.y,
                            transform.position.z);
                    }
                }
                else  //竖立
                {
                    //点击的button与第一的button间还有多个button
                    if (Vector3.Distance(obj.transform.position, transform.GetChild(0).position) > disWithFirstAnchorPosition)
                    {
                        float curVerticalY = transform.position.y;
                        curVerticalY -= offset;

                        curVerticalY = curVerticalY < 0 ? 0 : curVerticalY;
                        //重新赋值content的transform的位置
                        transform.position = new Vector3(transform.position.x, curVerticalY, transform.position.z);
                    }
                }
            }
            else if (disWithSecondAnchorPosition <= offset)
            {
                if (layoutGroup is HorizontalLayoutGroup)
                {
                    //点击的button与最后个的button间还有多个button
                    if (Vector3.Distance(obj.transform.position, transform.GetChild(transform.childCount - 1).position) > disWithSecondAnchorPosition)
                    {
                        float curVerticalX = transform.position.x;
                        curVerticalX -= offset;
                        
                        curVerticalX = curVerticalX > 0 ? 0 : curVerticalX;
                        transform.position = new Vector3(curVerticalX, transform.position.y,
                            transform.position.z);
                    }
                }
                else  //竖立
                {
                    //点击的button与最后个的button间还有多个button
                    if (Vector3.Distance(obj.transform.position, transform.GetChild(transform.childCount - 1).position) > disWithSecondAnchorPosition)
                    {
                        float curVerticalY = transform.position.y;
                        curVerticalY += offset;
                        
                        curVerticalY = curVerticalY < 0 ? 0 : curVerticalY;
                        //重新赋值content的transform的位置
                        transform.position = new Vector3(transform.position.x, curVerticalY, transform.position.z);
                    }
                }
            }

        }
    }
}
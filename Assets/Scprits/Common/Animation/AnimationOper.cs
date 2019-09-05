using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Animator 动画播放控制
/// </summary>
public class AnimationOper : MonoBehaviour
{
    public Animator anim;
    public string animName;
    public float frameRate;
    void Awake()
    {
        anim = GetComponent<Animator>();
        IsStart = false;
        IsComplete = false;
        frameRate = 24;
    }

    /// <summary>
    /// 当画是否开始
    /// </summary>
    public bool IsStart
    {
        get;
        set;
    }

    /// <summary>
    /// 动画是否完成
    /// </summary>
    public bool IsComplete
    {
        get;
        set;
    }

    /// <summary>
    /// 释放完成绑定事件
    /// </summary>
    private List<System.Action> _BindList = new List<System.Action>();
    private event System.Action _complete;
    public event System.Action Complete
    {
        add
        {
            ClearCompleteEvent();
            _complete += value;
            _BindList.Add(value);
        }

        remove
        {
            _complete -= value;
        }
    }

    public void ClearCompleteEvent()
    {
        for (int i = 0; i < _BindList.Count; i++)
        {
            _complete -= _BindList[i];
        }
        _BindList.Clear();
    }

    public System.Action<int> timePointEvent; //时间点事件,参数为当前时间
    float timeLength;
    float currLength;
    public float transitionTime = 0f;//过渡时间   
    int lastFrame = -1, curFrame = -1;
    bool isPlaying = false;
    /// <summary>
    /// 从头开始播放动画剪辑
    /// </summary>
    /// <param name="clipName"></param>
    public void PlayForward(string clipName, float offset = 0)
    {
        if (anim)
        {
            animName = clipName;
            //Debug.Log(animName);  
            currLength = 0;
            IsStart = false;
            curFrame = lastFrame = -1;
            //anim.CrossFade(clipName, transitionTime, 0, offset);          
            System.Array.FindIndex(anim.runtimeAnimatorController.animationClips, (ac) =>
            {
                if (ac.name == animName)
                {
                    timeLength = ac.length;
                    offset /= (timeLength * anim.GetCurrentAnimatorClipInfo(0)[0].clip.frameRate);
                    return true;
                }
                return false;
            });
            anim.Play(clipName, 0, offset);
            //timeLength = anim.GetCurrentAnimatorStateInfo(0).length;
            IsStart = true;
        }
        else
        {
            Debug.Log("没有找到动画");
        }

    }
    public void Update()
    {
        if (IsStart)
        {
            var asif = anim.GetCurrentAnimatorStateInfo(0);
            if (asif.IsName("Base." + animName))
            {
                //timeLength = anim.GetCurrentAnimatorStateInfo(0).length;
                if (currLength <= timeLength)
                {
                    if (timePointEvent != null)
                    {
                        //Debug.LogError(asif.normalizedTime);                       
                        float currentFrame = asif.length * asif.normalizedTime * frameRate;
                        //Debug.Log("---" + anim.GetCurrentAnimatorClipInfo(0)[0].clip.frameRate);
                        //Debug.Log("----------------------"+Mathf.RoundToInt(currentFrame));
                        curFrame = Mathf.RoundToInt(currentFrame);
                        if (lastFrame != curFrame)//作用：在该帧仅仅调用一次。（但是暂停时 ，要在调用处做特殊处理。因为下次启动时仍会在该帧调用）.         原因：动画播放和Update不一致，update会延迟捕捉当前贞数，update帧数会>=暂停时帧数,   有跳帧现象外部调用在几帧之间
                        {
                            timePointEvent(curFrame);
                        }
                        lastFrame = curFrame;
                    }
                    if (IsStart)
                    {
                        currLength += Time.deltaTime;
                    }
                }
                else
                {
                    //if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                    {
                        //播放完成
                        IsStart = false;
                        IsComplete = true;
                        currLength = 0;
                        lastFrame = curFrame = -1;
                        if (_complete != null)
                        {
                            _complete();
                            _complete = null;
                            timePointEvent = null;
                        }
                    }

                }
            }
        }
    }

    /// <summary>
    /// 暂停
    /// </summary>
    public void OnPause()
    {
        IsStart = false;
        if (anim != null)
        {
            anim.speed = 0;
        }
    }

    /// <summary>
    /// 继续
    /// </summary>
    public void OnContinue()
    {
        IsStart = true;
        if (anim != null)
        {
            anim.speed = 1;
        }
    }

    private void OnDisable()
    {
        anim.Update(0);
        anim.Rebind();
    }
    void OnDestroy()
    {
        IsStart = false;
        IsComplete = false;
        _complete = null;
        timePointEvent = null;
    }
}

using huang.common.screen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// 管理投屏窗口位置
/// </summary>
public class ManagerProjectWindow : MonoBehaviour
{

    //寻找当前目标窗口的进程
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName, CharSet charSet);

    [DllImport("user32.dll")]
    private static extern int GetWindowRect(IntPtr hwnd, out UnityEngine.Rect lpRect);

    /// <summary>
    /// 检查是否允许投屏
    /// 当屏幕 >= 两个的情况下，允许投屏
    /// </summary>
	bool CheckWindowsCanProjection()
    {
        int windowsNum = liu.GetScreenMode.GetSreenNum();
        if (windowsNum > 1)
            return true;
        else
            return false;
    }

    WindowsOnTop windowsOnTop;
    FView fView;
    IntPtr viewClientIntPtr;
    /// <summary>
    /// 判断是否找到 ViewClient 窗口
    /// </summary>
    /// <returns></returns>
    bool FindViewClientIntPtr()
    {
        viewClientIntPtr = IntPtr.Zero;
        viewClientIntPtr = FindWindow(null, "ViewClient", CharSet.Unicode);
        if (viewClientIntPtr == IntPtr.Zero)
        {
            Debug.LogWarning("ManagerProjectWindow.FindViewClientIntPtr() no found ViewClient");
            isHideSecondScreen = false;
            isShowSecondScreen = false;
            return false;
        }
        else
        {
            return true;
        }
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct RECT_WIN
    {
        public int Left; //最左坐标
        public int Top; //最上坐标
        public int Right; //最右坐标
        public int Bottom; //最下坐标
    }

    [DllImport("FView")]
    public static extern void ShowInExe(System.IntPtr hWnd, System.IntPtr textureHandle, int w, int h);

    [DllImport("FView")]
    public static extern void StopShow();


    //根据窗口句柄获取pid
    [DllImport("User32.dll")]
    public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);


    [DllImport("user32.dll", EntryPoint = "GetWindowLongA", SetLastError = true)]
    public static extern long GetWindowLong(IntPtr hwnd, int nIndex);

    public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong)
    {
        if (IntPtr.Size == 4)
        {
            return SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
        }
        return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
    }

    [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
    public static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
    public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern long SetWindowPos(IntPtr hwnd, long hWndInsertAfter, long x, long y, long cx, long cy, long wFlags);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);

    [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
    public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);


    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SetFocus(IntPtr hWnd);

    /// <summary>
    /// 得到屏幕个数，要注意只有在扩展模式屏幕个数才是2，复制模式屏幕个数仍是1.
    /// </summary>
    /// <returns></returns>
    [DllImport("oglwin")]
    public static extern uint GetMonitorCount();

    [DllImport("user32.dll")]
    public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip,
   MonitorEnumDelegate lpfnEnum, IntPtr dwData);


    public delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData);

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MonitorInfo
    {
        public uint size;
        public Rect rcMonitor;
        public Rect rcWork;
        public uint flags;
    }

    /// <summary>
    /// The struct that contains the display information
    /// </summary>
    public class DisplayInfo
    {
        public string Availability { get; set; }
        public string ScreenHeight { get; set; }
        public string ScreenWidth { get; set; }
        public Rect MonitorArea { get; set; }
        public Rect WorkArea { get; set; }
    }

    [DllImport("user32.dll")]
    public static extern bool GetMonitorInfoA(IntPtr monitorHandle, ref MonitorInfo mInfo);

    /// <summary>
    /// Collection of display information
    /// </summary>
    public class DisplayInfoCollection : List<DisplayInfo>
    {
    }

    /// <summary>
    /// Returns the number of Displays using the Win32 functions
    /// </summary>
    /// <returns>collection of Display Info</returns>
    public DisplayInfoCollection GetDisplays()
    {
        DisplayInfoCollection col = new DisplayInfoCollection();

        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
            delegate (IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData)
            {
                MonitorInfo mi = new MonitorInfo();
                mi.size = (uint)Marshal.SizeOf(mi);
                bool success = GetMonitorInfoA(hMonitor, ref mi);
                if (success)
                {
                    DisplayInfo di = new DisplayInfo();
                    di.ScreenWidth = (mi.rcMonitor.right - mi.rcMonitor.left).ToString();
                    di.ScreenHeight = (mi.rcMonitor.bottom - mi.rcMonitor.top).ToString();
                    di.MonitorArea = mi.rcMonitor;
                    di.WorkArea = mi.rcWork;
                    di.Availability = mi.flags.ToString();
                    col.Add(di);
                }
                return true;
            }, IntPtr.Zero);
        return col;
    }

    private const int SWP_NOOWNERZORDER = 0x200;
    private const int SWP_NOREDRAW = 0x8;
    private const int SWP_NOZORDER = 0x4;
    private const int SWP_SHOWWINDOW = 0x0040;
    private const int SWP_HIDEWINDOW = 0x0080;
    private const int WS_EX_MDICHILD = 0x40;
    private const int SWP_FRAMECHANGED = 0x20;
    private const int SWP_NOACTIVATE = 0x10;
    private const int SWP_ASYNCWINDOWPOS = 0x4000;
    private const int SWP_NOMOVE = 0x2;
    private const int SWP_NOSIZE = 0x1;
    private const int GWL_STYLE = -16;
    private const int WS_VISIBLE = 0x10000000;
    private const int WM_CLOSE = 0x10;
    private const int WS_CHILD = 0x40000000;
    private const int WS_CAPTION = 0x00C00000;
    private const int WS_SYSMENU = 0x00080000;
    private const int WS_SIZEBOX = 0x00040000;

    private const int SW_HIDE = 0; //{隐藏, 并且任务栏也没有最小化图标}
    private const int SW_SHOWNORMAL = 1; //{用最近的大小和位置显示, 激活}
    private const int SW_NORMAL = 1; //{同 SW_SHOWNORMAL}
    private const int SW_SHOWMINIMIZED = 2; //{最小化, 激活}
    private const int SW_SHOWMAXIMIZED = 3; //{最大化, 激活}
    private const int SW_MAXIMIZE = 3; //{同 SW_SHOWMAXIMIZED}
    private const int SW_SHOWNOACTIVATE = 4; //{用最近的大小和位置显示, 不激活}
    private const int SW_SHOW = 5; //{同 SW_SHOWNORMAL}
    private const int SW_MINIMIZE = 6; //{最小化, 不激活}
    private const int SW_SHOWMINNOACTIVE = 7; //{同 SW_MINIMIZE}
    private const int SW_SHOWNA = 8; //{同 SW_SHOWNOACTIVATE}
    private const int SW_RESTORE = 9; //{同 SW_SHOWNORMAL}
    private const int SW_SHOWDEFAULT = 10; //{同 SW_SHOWNORMAL}
    private const int SW_MAX = 10; //{同 SW_SHOWNORMAL}

    public void fullScreen(IntPtr hWnd)
    {
        long Style = GetWindowLong(hWnd, GWL_STYLE);
        Style = Style & ~WS_CAPTION & ~WS_SYSMENU & ~WS_SIZEBOX;

        SetWindowLong(hWnd, GWL_STYLE, (int)Style);
        int mCount = liu.GetScreenMode.GetSreenNum();
        //UnityEngine.Debug.Log("FView.fullScreen(): 显示器个数：" + mCount);
        if (mCount > 1)
        {
            var ds = GetDisplays();
            foreach (var d in ds)
            {
                if (d.Availability == "0")
                {
                    //UnityEngine.Debug.Log("FView.fullScreen():找到副屏");
                    //UnityEngine.Debug.Log("FView.fullScreen():Top = " + d.MonitorArea.top);
                    //UnityEngine.Debug.Log("FView.fullScreen():Left = " + d.MonitorArea.left);
                    //UnityEngine.Debug.Log("FView.fullScreen():Width = " + d.ScreenWidth);
                    //UnityEngine.Debug.Log("FView.fullScreen():Height = " + d.ScreenHeight);
                    //当前非主屏

                    SetWindowPos(hWnd, -1, d.MonitorArea.left, d.MonitorArea.top, d.MonitorArea.right - d.MonitorArea.left, d.MonitorArea.bottom - d.MonitorArea.top, 1 | 2);
                    MoveWindow(hWnd, d.MonitorArea.left, d.MonitorArea.top, d.MonitorArea.right - d.MonitorArea.left, d.MonitorArea.bottom - d.MonitorArea.top, true);
                    break;
                }
            }

        }
        else
        {
            SetWindowPos(hWnd, -1, 0, 0, 0, 0, 1 | 2);
        }
        ShowWindow(hWnd, 3);
    }


    UnityEngine.Rect rect1 = new UnityEngine.Rect();
    ScreenControlObj screenControlObj;
    bool isHideSecondScreen = false;
    bool isShowSecondScreen = false;

    private void OnApplicationFocus(bool focus)
    {
        //Debug.Log("获得焦点  focus  " + focus);
        if (focus)
        {
            //两个状态不一致时，说明出现情况
            //1、少屏幕
            //2、接上屏幕
            //3、默认状态更新
            //Debug.Log("获得焦点");
            //判断投屏窗口是否存在，存在则置于副屏
            if (FindViewClientIntPtr())
            {
                //UnityEngine.Debug.Log("有投屏窗口");
                GetWindowRect(viewClientIntPtr, out rect1);
                var num = Mathf.CeilToInt(rect1.height);
                //投到副屏去
                if (liu.GetScreenMode.GetSreenNum() > 1)
                {
                    //UnityEngine.Debug.Log("恢复两个屏幕  +  isShowSecondScreen  " + isShowSecondScreen);
                    if (num < 0) isShowSecondScreen = false;
                    if (!isShowSecondScreen)
                    {
                        //计算rect是否小于0，小于0才置于副屏
                        //GetWindowRect(viewClientIntPtr, out rect1);
                        //var num = Mathf.CeilToInt(rect1.height);
                        //说明是隐藏置底
                        //if (num < 0)
                        //{
                        //UnityEngine.Debug.Log("投屏投送到副屏");
                        //最大化
                        //ShowWindow(viewClientIntPtr, 3);
                        fullScreen(viewClientIntPtr);
                        isShowSecondScreen = true;
                        isHideSecondScreen = false;
                        //}
                    }
                }
                else
                {
                    if (num > 0) isHideSecondScreen = false;
                    if (!isHideSecondScreen)
                    {
                        //UnityEngine.Debug.Log("屏幕只有一个 副屏最小化");
                        //最小化
                        //ShowWindow(viewClientIntPtr, 2);
                        //if (screenControlObj == null) screenControlObj = FindObjectOfType<ScreenControlObj>();
                        //screenControlObj.curMode = ScreenManger.DualScreenMode.None;
                        //GlobalEntity.GetInstance().Dispatch(gEventEnum.CloseProjection);
                        CloseProjectionAction();
                        isHideSecondScreen = true;
                        isShowSecondScreen = false;
                    }
                }
            }
        }
    }

    public static Action CloseProjectionAction;

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.A))
        // {
            // Debug.Log("Update.KeyCode.A");
            // fullScreen(viewClientIntPtr);
            // isShowSecondScreen = true;
            // isHideSecondScreen = false;
        // }

        // if (Input.GetKeyDown(KeyCode.S))
        // {
            // Debug.Log("Update.KeyCode.S");
            // ShowWindow(viewClientIntPtr, 3);
            // fullScreen(viewClientIntPtr);
            // isShowSecondScreen = true;
            // isHideSecondScreen = false;
        // }
    }
}

﻿using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class TransparentWindow : MonoBehaviour
{
    static TransparentWindow _instance = new TransparentWindow();
    public static TransparentWindow Instance { get { return _instance; } }
    TransparentWindow() { }

    [SerializeField]
    private Material m_Material;

    [SerializeField]
    int _xOffset = 83;
    [SerializeField]
    int _yOffset = 0;

    #region 导入API

    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left; //最左坐标
        public int Top; //最上坐标
        public int Right; //最右坐标
        public int Bottom; //最下坐标
    }
    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
    [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
    public static extern long GetWindowLong(IntPtr hwnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    private static extern int SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int cx, int cy, int uFlags);

    [DllImport("user32.dll")]
    static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll", EntryPoint = "SetLayeredWindowAttributes")]
    static extern int SetLayeredWindowAttributes(IntPtr hwnd, int crKey, byte bAlpha, int dwFlags);

    [DllImport("User32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    const int GWL_STYLE = -16;
    const int GWL_EXSTYLE = -20;
    const uint WS_POPUP = 0x80000000;
    const uint WS_VISIBLE = 0x10000000;

    const uint WS_EX_TOPMOST = 0x00000008;
    const uint WS_EX_LAYERED = 0x00080000;
    const uint WS_EX_TRANSPARENT = 0x00000020;

    const int SWP_FRAMECHANGED = 0x0020;
    const int SWP_SHOWWINDOW = 0x0040;
    const int LWA_ALPHA = 2;


    private IntPtr HWND_TOPMOST = new IntPtr(-1);
    private IntPtr HWND_NOTOPMOST = new IntPtr(-2);

    #endregion

    public IntPtr windowHandle
    {
        get
        {
            if (_windowHandle == IntPtr.Zero)
            {
                _windowHandle = FindWindow(null, Application.productName);
            }
            return _windowHandle;
        }
    }

    // public int screenWidth = 600;
    // public int screenHeight = 1000;
    IntPtr _windowHandle = IntPtr.Zero;
    Vector2Int _offset = Vector2Int.zero;

    void Start()
    {
        Camera.main.depthTextureMode = DepthTextureMode.DepthNormals;
        if (Application.isEditor) return;

        MARGINS margins = new MARGINS() { cxLeftWidth = -1 };

        //1：忽略大小；2：忽略位置；4：忽略Z顺序
        SetWindowPos(windowHandle, HWND_TOPMOST, 0 + _xOffset, 0 + _yOffset,
        System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width,
        System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height, 4);

        // Set properties of the window
        // See: https://msdn.microsoft.com/en-us/library/windows/desktop/ms633591%28v=vs.85%29.aspx
        SetWindowLong(windowHandle, GWL_STYLE, WS_POPUP | WS_VISIBLE);
        SetWindowLong(windowHandle, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOPMOST); // 实现鼠标穿透

        // Extend the window into the client area
        //See: https://msdn.microsoft.com/en-us/library/windows/desktop/aa969512%28v=vs.85%29.aspx 
        DwmExtendFrameIntoClientArea(windowHandle, ref margins);

        ToggleTopMost(DataModel.Instance.Data.isTopMost);

        AddSystemTray();
    }

    private void LateUpdate()
    {
        if (Application.isEditor) return;

        // Drag();

        CursorPenetrate();
    }

    private void Drag()
    {
        // 拖拽开始
        if (Input.GetMouseButtonDown(2))
        {
            RECT rect = new RECT();
            GetWindowRect(windowHandle, ref rect);
            _offset.x = -System.Windows.Forms.Cursor.Position.X + rect.Left;
            _offset.y = -System.Windows.Forms.Cursor.Position.Y + rect.Top;
        }

        // 拖拽中
        if (Input.GetMouseButton(2))
        {
            SetWindowPos(windowHandle, HWND_TOPMOST, System.Windows.Forms.Cursor.Position.X + _offset.x,
            System.Windows.Forms.Cursor.Position.Y + _offset.y, 0, 0, 1 | 4);
        }

        // 结束拖拽
        if (Input.GetMouseButtonUp(2))
        {
            SavePos();
        }

        // 归位、删除所有设置并重启
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F9))
        {
            SetWindowPos(windowHandle, HWND_TOPMOST, 0, 0, 0, 0, 1 | 4);
            PlayerPrefs.DeleteAll();
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }

    void SavePos()
    {
        RECT rect = new RECT();
        GetWindowRect(windowHandle, ref rect);
        PlayerPrefs.SetInt(Application.productName + "_WindowRECT_Left", rect.Left);
        PlayerPrefs.SetInt(Application.productName + "_WindowRECT_Top", rect.Top);
    }

    Vector2Int LoadPos()
    {
        if (PlayerPrefs.HasKey(Application.productName + "_WindowRECT_Left"))
        {
            return new Vector2Int(PlayerPrefs.GetInt(Application.productName + "_WindowRECT_Left"),
             PlayerPrefs.GetInt(Application.productName + "_WindowRECT_Top"));
        }
        else
            return new Vector2Int(100, 100);
    }

    bool _isInRoleRect = false;
    Vector2Int _lastMousePos = Vector2Int.zero;

    void CursorPenetrate()
    {
        // 鼠标有位移时打射线，碰到角色则不穿透，否则窗口穿透
        var pos = GetMousePosW2U();
        if (GetMouseMove(_lastMousePos, pos) < 1) return;
        var posV3 = new Vector3(pos.x, pos.y, 0);
        RaycastHit hitInfo;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(posV3), out hitInfo, 100f, LayerMask.GetMask("WindowRect")))
        {
            // 鼠标进入角色范围
            if (!_isInRoleRect)
            {
                SetWindowLong(windowHandle, GWL_EXSTYLE, WS_EX_LAYERED);
                _isInRoleRect = true;
            }
        }
        else
        {
            // 鼠标移出
            if (_isInRoleRect)
            {
                SetWindowLong(windowHandle, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
                _isInRoleRect = false;
            }
        }
        _lastMousePos = pos;
    }

    // 获取从Windows桌面空间转换到Unity屏幕空间的鼠标位置
    public Vector2Int GetMousePosW2U()
    {
        RECT rect = new RECT();
        GetWindowRect(windowHandle, ref rect);
        Vector2Int leftBottom = new Vector2Int(rect.Left, rect.Bottom);
        var mousePos = new Vector2Int(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y);
        var screenHeight = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
        leftBottom.y = screenHeight - leftBottom.y;
        mousePos.y = screenHeight - mousePos.y;

        return mousePos - leftBottom;
    }

    float GetMouseMove(Vector2 last, Vector2 current)
    {
        return Mathf.Abs(current.x - last.x) + Mathf.Abs(current.y - last.y);
    }

    SystemTray _icon;
    // 创建托盘图标、添加选项
    void AddSystemTray()
    {
        _icon = Rainity.CreateSystemTrayIcon();
        _icon.AddItem("切换置顶显示", () =>
        {
            bool isTop = !DataModel.Instance.Data.isTopMost;
            DataModel.Instance.Data.isTopMost = isTop;
            DataModel.Instance.SaveData();
            DataModel.Instance.ReloadData();
            var style = (uint)GetWindowLong(windowHandle, GWL_EXSTYLE);
            ToggleTopMost(isTop);
        });
        _icon.AddItem("退出", Exit);
    }

    void ToggleTopMost(bool isTop)
    {
        if (isTop)
        {
            SetWindowPos(windowHandle, HWND_TOPMOST, 0, 0, 0, 0, 1 | 2);
        }
        else
        {
            SetWindowPos(windowHandle, HWND_NOTOPMOST, 0, 0, 0, 0, 1 | 2);
        }
    }

    void Exit()
    {
        _icon.Dispose();
        Application.Quit();
    }

    void OnRenderImage(RenderTexture from, RenderTexture to)
    {
        Graphics.Blit(from, to, m_Material);
    }
}
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace CS2BhopUI
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    public class MainForm : Form
    {
        // UI Colors
        private Color csLightGray = Color.FromArgb(220, 220, 220); // Background left
        private Color csWhite = Color.White;
        private Color csOrange = Color.FromArgb(235, 120, 15); // CS2 Logo Orange
        private Color csDarkOrange = Color.FromArgb(200, 95, 10);
        
        private Color darkPanelColor = Color.FromArgb(25, 25, 25);
        private Color textColor = Color.White;

        // UI Controls
        private Label titleLabel;
        private Label statusLabel;
        private Button toggleBhopBtn;
        private CheckBox duckJumpCheckbox;
        private Button btnGitHub;
        private Button btnDonate;
        private Button btnSteamGroup;
        private Button closeButton;
        private Button minimizeButton;
        private Panel contentPanel;
        
        // Dragging
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public MainForm()
        {
            InitializeComponent();
            BhopEngine.IsDuckJumpEnabled = true; // Default
            this.DoubleBuffered = true; // Prevent flicker
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.FormBorderStyle = FormBorderStyle.None;
            this.ClientSize = new Size(340, 270);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "CS2 AUTO-B-Hop by CS1337BOYS";
            this.BackColor = darkPanelColor;

            // Title Bar Panel for Dragging
            Panel topBar = new Panel();
            topBar.Height = 35;
            topBar.Dock = DockStyle.Top;
            topBar.BackColor = Color.FromArgb(20, 20, 20);
            topBar.MouseDown += TopBar_MouseDown;

            titleLabel = new Label();
            titleLabel.Text = "CS2 AUTO-B-Hop by CS1337BOYS";
            titleLabel.ForeColor = csOrange;
            titleLabel.BackColor = Color.Transparent;
            titleLabel.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            titleLabel.AutoSize = true;
            titleLabel.Location = new Point(10, 8);
            titleLabel.MouseDown += TopBar_MouseDown; // Allow drag clicking title
            topBar.Controls.Add(titleLabel);

            closeButton = new Button();
            closeButton.Text = "X";
            closeButton.ForeColor = Color.White;
            closeButton.BackColor = Color.Transparent;
            closeButton.FlatStyle = FlatStyle.Flat;
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Size = new Size(35, 35);
            closeButton.Dock = DockStyle.Right;
            closeButton.Cursor = Cursors.Hand;
            closeButton.Click += (s, e) => { BhopEngine.Stop(); Application.Exit(); };
            topBar.Controls.Add(closeButton);

            minimizeButton = new Button();
            minimizeButton.Text = "-";
            minimizeButton.ForeColor = Color.White;
            minimizeButton.BackColor = Color.Transparent;
            minimizeButton.FlatStyle = FlatStyle.Flat;
            minimizeButton.FlatAppearance.BorderSize = 0;
            minimizeButton.Size = new Size(35, 35);
            minimizeButton.Dock = DockStyle.Right;
            minimizeButton.Cursor = Cursors.Hand;
            minimizeButton.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
            topBar.Controls.Add(minimizeButton);

            this.Controls.Add(topBar);

            // Subtitle
            Label subTitle = new Label();
            subTitle.Text = "SETTINGS & STATUS";
            subTitle.ForeColor = csLightGray;
            subTitle.BackColor = Color.Transparent;
            subTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            subTitle.AutoSize = true;
            subTitle.Location = new Point(15, 50);
            this.Controls.Add(subTitle);

            // Checkbox
            duckJumpCheckbox = new CheckBox();
            duckJumpCheckbox.Text = "Enable Duck-Jump (Long Jump)";
            duckJumpCheckbox.ForeColor = textColor;
            duckJumpCheckbox.BackColor = Color.Transparent;
            duckJumpCheckbox.Font = new Font("Segoe UI", 10);
            duckJumpCheckbox.AutoSize = true;
            duckJumpCheckbox.Location = new Point(20, 80);
            duckJumpCheckbox.Checked = true;
            duckJumpCheckbox.CheckedChanged += (s, e) => BhopEngine.IsDuckJumpEnabled = duckJumpCheckbox.Checked;
            this.Controls.Add(duckJumpCheckbox);

            // Toggle Button
            toggleBhopBtn = new Button();
            toggleBhopBtn.Text = "START BHOP";
            toggleBhopBtn.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            toggleBhopBtn.BackColor = csOrange;
            toggleBhopBtn.ForeColor = Color.White;
            toggleBhopBtn.FlatStyle = FlatStyle.Flat;
            toggleBhopBtn.FlatAppearance.BorderSize = 0;
            toggleBhopBtn.Size = new Size(300, 45);
            toggleBhopBtn.Location = new Point(20, 120);
            toggleBhopBtn.Cursor = Cursors.Hand;
            toggleBhopBtn.Click += ToggleBhopBtn_Click;
            this.Controls.Add(toggleBhopBtn);

            // Status Label
            statusLabel = new Label();
            statusLabel.Text = "Status: INACTIVE";
            statusLabel.ForeColor = Color.LightGray;
            statusLabel.BackColor = Color.Transparent;
            statusLabel.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            statusLabel.AutoSize = true;
            statusLabel.Location = new Point(100, 175);
            this.Controls.Add(statusLabel);

            // Bottom Links row
            int btnY = 220;
            btnGitHub = CreateLinkButton("GitHub", 20, btnY, 90, "https://github.com/Adiru3");
            btnDonate = CreateLinkButton("Donate", 120, btnY, 90, "https://adiru3.github.io/Donate/");
            btnSteamGroup = CreateLinkButton("Steam Group", 220, btnY, 100, "https://steamcommunity.com/groups/CS1337BOYS");

            this.Controls.Add(btnGitHub);
            this.Controls.Add(btnDonate);
            this.Controls.Add(btnSteamGroup);

            this.ResumeLayout(false);
        }

        private Button CreateLinkButton(string text, int x, int y, int width, string url)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.ForeColor = csOrange;
            btn.BackColor = Color.FromArgb(30, 30, 30);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = csOrange;
            btn.FlatAppearance.BorderSize = 1;
            btn.Size = new Size(width, 30);
            btn.Location = new Point(x, y);
            btn.Cursor = Cursors.Hand;
            btn.Font = new Font("Segoe UI", 8, FontStyle.Regular);
            btn.Click += (s, e) => Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            return btn;
        }

        private void TopBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void ToggleBhopBtn_Click(object sender, EventArgs e)
        {
            if (BhopEngine.IsRunning)
            {
                BhopEngine.Stop();
                toggleBhopBtn.Text = "START BHOP";
                toggleBhopBtn.BackColor = csOrange;
                statusLabel.Text = "Status: INACTIVE";
                statusLabel.ForeColor = Color.LightGray;
            }
            else
            {
                BhopEngine.Start();
                toggleBhopBtn.Text = "STOP BHOP";
                toggleBhopBtn.BackColor = Color.IndianRed;
                statusLabel.Text = "Status: ACTIVE (Hold Space)";
                statusLabel.ForeColor = Color.LimeGreen;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Paint the compact CS2 themed background
            g.Clear(darkPanelColor);

            int w = this.Width;
            int h = this.Height;

            // Small subtle white/grey stripes in the background
            Point[] whiteStripe = { new Point(220, 0), new Point(260, 0), new Point(140, h), new Point(100, h) };
            g.FillPolygon(new SolidBrush(Color.FromArgb(15, csWhite)), whiteStripe);

            // Subtle Orange diagonal stripe
            Point[] orangeArea = { new Point(300, 0), new Point(340, 0), new Point(220, h), new Point(180, h) };
            using (SolidBrush orangeBrush = new SolidBrush(Color.FromArgb(20, csOrange)))
            {
                g.FillPolygon(orangeBrush, orangeArea);
            }
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, csOrange, ButtonBorderStyle.Solid);
        }
    }

    public static class BhopEngine
    {
        // P/Invokes
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x0100;
        const int WM_KEYUP = 0x0101;
        const int WM_SYSKEYDOWN = 0x0104;
        const int WM_SYSKEYUP = 0x0105;

        [StructLayout(LayoutKind.Sequential)]
        struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public int type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct InputUnion
        {
            [FieldOffset(0)] public MOUSEINPUT mi;
            [FieldOffset(0)] public KEYBDINPUT ki;
            [FieldOffset(0)] public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT { public int dx; public int dy; public uint mouseData; public uint dwFlags; public uint time; public IntPtr dwExtraInfo; }

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT { public ushort wVk; public ushort wScan; public uint dwFlags; public uint time; public IntPtr dwExtraInfo; }

        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT { public uint uMsg; public ushort wParamL; public ushort wParamH; }

        const int INPUT_KEYBOARD = 1;
        const uint KEYEVENTF_KEYUP = 0x0002;
        const uint KEYEVENTF_SCANCODE = 0x0008;
        const ushort SCANCODE_SPACE = 0x39;
        const ushort SCANCODE_LCONTROL = 0x1D;

        // State
        static IntPtr _hookID = IntPtr.Zero;
        static LowLevelKeyboardProc _proc = HookCallback;
        static volatile bool isSpaceDownPhysical = false;
        public static volatile bool IsRunning = false;
        public static volatile bool IsDuckJumpEnabled = true;

        private static Thread _bhopThread;
        private static Thread _msgLoopThread;
        
        // We install the Hook on the same thread that calls Start() (the UI thread)
        // because the UI thread already runs an Application message pump. 
        public static void Start()
        {
            if (IsRunning) return;
            IsRunning = true;

            // Apply GC Optimizations
            try {
                using (Process p = Process.GetCurrentProcess()) { p.PriorityClass = ProcessPriorityClass.High; }
                System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.SustainedLowLatency;
            } catch { }

            _bhopThread = new Thread(Loop);
            _bhopThread.Priority = ThreadPriority.Highest;
            _bhopThread.IsBackground = true;
            _bhopThread.Start();

            // Hook straight to the current UI message loop thread
            if (_hookID == IntPtr.Zero)
            {
                _hookID = SetHook(_proc);
            }
        }

        public static void Stop()
        {
            if (!IsRunning) return;
            IsRunning = false;
            
            if (_hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookID);
                _hookID = IntPtr.Zero;
            }

            try { System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.Interactive; } catch { }
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int msg = wParam.ToInt32();
                var kb = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                bool isInjected = (kb.flags & 0x10) != 0;

                if (!isInjected && kb.vkCode == 0x20) // VK_SPACE
                {
                    if (msg == WM_KEYDOWN || msg == WM_SYSKEYDOWN) isSpaceDownPhysical = true;
                    else if (msg == WM_KEYUP || msg == WM_SYSKEYUP) isSpaceDownPhysical = false;
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private static void Loop()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            // Standard Bhop Timer variables
            double subTickWindowMs = 7.812;
            double overlapRatioTimeMs = subTickWindowMs * 0.10;
            double staminaPenaltyBufferMs = 0.08; 

            // Long-Jump Timer variables
            double crouchPreDelayMs = 0.5; 
            double holdDownTimeMs = 7.03125;
            double crouchReleaseDelayMs = 15.0;
            double crouchResetDelayMs = 7.812; 

            while (IsRunning)
            {
                if (isSpaceDownPhysical)
                {
                    if (IsDuckJumpEnabled)
                    {
                        // DUCK-JUMP LOGIC (Max velocity)
                        SendKeyPress(SCANCODE_LCONTROL, false); 
                        SpinWait(crouchPreDelayMs, sw);
                        
                        SendKeyPress(SCANCODE_SPACE, false); 
                        SpinWait(holdDownTimeMs, sw);
                        
                        SendKeyPress(SCANCODE_SPACE, true);  
                        SpinWait(crouchReleaseDelayMs, sw); 
                        
                        SendKeyPress(SCANCODE_LCONTROL, true); 
                        SpinWait(crouchResetDelayMs, sw);
                    }
                    else
                    {
                        // STANDARD BHOP LOGIC
                        SendKeyPress(SCANCODE_SPACE, false);
                        SpinWait(subTickWindowMs - overlapRatioTimeMs, sw);
                        
                        SendKeyPress(SCANCODE_SPACE, true);
                        SpinWait(subTickWindowMs + overlapRatioTimeMs + staminaPenaltyBufferMs, sw); 
                    }
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }

        private static void SpinWait(double milliseconds, Stopwatch sw)
        {
            long start = sw.ElapsedTicks;
            long ticksToWait = (long)(milliseconds * Stopwatch.Frequency / 1000.0);
            while (sw.ElapsedTicks - start < ticksToWait) { /* ultra-busy wait */ }
        }

        private static void SendKeyPress(ushort scanCode, bool keyUp)
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = INPUT_KEYBOARD;
            inputs[0].u.ki.wVk = 0; 
            inputs[0].u.ki.wScan = scanCode;
            inputs[0].u.ki.dwFlags = KEYEVENTF_SCANCODE | (keyUp ? KEYEVENTF_KEYUP : 0u);
            inputs[0].u.ki.time = 0;
            inputs[0].u.ki.dwExtraInfo = IntPtr.Zero;

            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }
    }
}

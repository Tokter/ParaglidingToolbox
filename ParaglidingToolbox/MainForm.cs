using ParaglidingToolbox.Scenes;
using System.Runtime.InteropServices;

namespace ParaglidingToolbox
{
    public partial class MainForm : Form
    {
        private List<Scene> _scenes = new List<Scene>();
        private Scene _currentScene = null!;

        public MainForm()
        {
            InitializeComponent();
            skglControl.MouseWheel += SkglControl_MouseWheel;
            CurrentScene = new Scene_FluidSimulator();
            propertyGrid.SelectedObject = CurrentScene;

            //Setup render loop
            Application.Idle += Application_Idle;
        }

        private void Application_Idle(object? sender, EventArgs e)
        {
            while (IsApplicationIdle())
            {
                if (CurrentScene != null) CurrentScene.Update();
                skglControl.Invalidate();
            }
        }
        
        private bool IsApplicationIdle()
        {
            NativeMessage result;
            return PeekMessage(out result, IntPtr.Zero, (uint)0, (uint)0, (uint)0) == 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NativeMessage
        {
            public IntPtr Handle;
            public uint Message;
            public IntPtr WParameter;
            public IntPtr LParameter;
            public uint Time;
            public Point Location;
        }

        [DllImport("user32.dll")]
        public static extern int PeekMessage(out NativeMessage message, IntPtr window, uint filterMin, uint filterMax, uint remove);

        public Scene CurrentScene
        {
            get => _currentScene;
            set
            {
                if (_currentScene != null)
                {
                    _currentScene.ChangeCursor = null!;
                    _currentScene.ProcessFinished = null!;
                }                
                _currentScene = value;
                if (_currentScene != null)
                {
                    _currentScene.ChangeCursor = (cursor) => skglControl.Cursor = cursor;
                    _currentScene.SetScreenSize(skglControl.Width, skglControl.Height);
                    _currentScene.ProcessFinished = () => { propertyGrid.Refresh(); };
                    UpdateSceneTree();
                }
            }
        }

        private void UpdateSceneTree()
        {
            treeView.Nodes.Clear();

            var sceneNode = new TreeNode { Text = "Scene", Tag = CurrentScene };
            treeView.Nodes.Add(sceneNode);
            if (CurrentScene.Camera != null)
            {
                var cameraNodes = new TreeNode { Text = "Camera", Tag = CurrentScene.Camera };
                sceneNode.Nodes.Add(cameraNodes);
            }
            foreach (var node in CurrentScene.Root)
            {
                var treeNode = new TreeNode { Text = node.GetType().Name, Tag = node };
                sceneNode.Nodes.Add(treeNode);
            }
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null && e.Node.Tag != null)
            {
                propertyGrid.SelectedObject = e.Node.Tag;
                propertyGrid.Update();
            }
        }

        private void skglControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (_currentScene != null)
            {
                var inputEvent = InputEvent.MouseMove(e.X, e.Y, ToButton(e.Button), _shift, _control, _alt);
                _currentScene.ProcessEvent(inputEvent);
            }
        }

        private void skglControl_MouseDown(object sender, MouseEventArgs e)
        {                        
            if (_currentScene != null)
            {
                var inputEvent = InputEvent.MouseDown(e.X, e.Y, ToButton(e.Button), _shift, _control, _alt);
                _currentScene.ProcessEvent(inputEvent);
            }
        }

        private void skglControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (_currentScene != null)
            {
                var inputEvent = InputEvent.MouseUp(e.X, e.Y, ToButton(e.Button), _shift, _control, _alt);
                _currentScene.ProcessEvent(inputEvent);
            }
        }

        private void SkglControl_MouseWheel(object? sender, MouseEventArgs e)
        {
            if (_currentScene != null)
            {
                var inputEvent = InputEvent.MouseWheel(e.Delta, _shift, _control, _alt);
                _currentScene.ProcessEvent(inputEvent);
            }
        }

        private bool _shift = false;
        private bool _control = false;
        private bool _alt = false;

        private void skglControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (_currentScene != null)
            {
                _shift = e.Shift;
                _control = e.Control;
                _alt = e.Alt;
                var inputEvent = InputEvent.KeyUp(e.KeyCode, e.Shift, e.Control, e.Alt);
                _currentScene.ProcessEvent(inputEvent);
            }
        }

        private void skglControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (_currentScene != null)
            {
                _shift = e.Shift;
                _control = e.Control;
                _alt = e.Alt;
                var inputEvent = InputEvent.KeyDown(e.KeyCode, e.Shift, e.Control, e.Alt);
                _currentScene.ProcessEvent(inputEvent);
            }            
        }        

        private void skglControl_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs e)
        {
            if (_currentScene != null)
            {
                _currentScene.Draw(e.Surface);
            }
        }

        private ParaglidingToolbox.MouseButtons ToButton(System.Windows.Forms.MouseButtons mb)
        {
            var result = ParaglidingToolbox.MouseButtons.None;

            if (mb.HasFlag(System.Windows.Forms.MouseButtons.Left)) result |= ParaglidingToolbox.MouseButtons.Left;
            if (mb.HasFlag(System.Windows.Forms.MouseButtons.Right)) result |= ParaglidingToolbox.MouseButtons.Right;
            if (mb.HasFlag(System.Windows.Forms.MouseButtons.Middle)) result |= ParaglidingToolbox.MouseButtons.Middle;
            if (mb.HasFlag(System.Windows.Forms.MouseButtons.XButton1)) result |= ParaglidingToolbox.MouseButtons.XButton1;
            if (mb.HasFlag(System.Windows.Forms.MouseButtons.XButton1)) result |= ParaglidingToolbox.MouseButtons.XButton1;

            return result;
        }

        private void skglControl_SizeChanged(object sender, EventArgs e)
        {
            if (_currentScene != null)
            {
                _currentScene.SetScreenSize(skglControl.Width, skglControl.Height);
            }
        }
    }
}
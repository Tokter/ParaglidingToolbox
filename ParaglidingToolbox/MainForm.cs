using ParaglidingToolbox.Scenes;

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
        }

        public Scene CurrentScene
        {
            get => _currentScene;
            set
            {
                if (_currentScene != null) _currentScene.ChangeCursor = null!;
                _currentScene = value;
                if (_currentScene != null)
                {
                    _currentScene.ChangeCursor = (cursor) => skglControl.Cursor = cursor;
                    _currentScene.SetScreenSize(skglControl.Width, skglControl.Height);
                }
            }
        }
            
        public void InitializeCurrentScene()
        {
            
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
            skglControl.Invalidate();
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
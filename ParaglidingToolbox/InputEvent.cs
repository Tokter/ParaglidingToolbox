using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParaglidingToolbox
{
    public enum InputEventType
    {
        MouseWheel,
        MouseMove,
        MouseDown,
        MouseDoubleClick,
        MouseUp,
        KeyUp,
        KeyDown,
    }

    [Flags]
    public enum MouseButtons
    {
        None = 0,
        Left = 1,
        Right = 2,
        Middle = 4,
        XButton1 = 8,
        XButton2 = 16
    }

    public class InputEvent : EventArgs
    {
        public InputEventType InputEventType { get; private set; }
        public bool Processed { get; set; }
        public int MouseX { get; set; }
        public int MouseY { get; set; }
        public MouseButtons Button { get; set; }
        public int MouseDelta { get; set; }
        public bool Shift { get; set; }
        public bool Control { get; set; }
        public bool Alt { get; set; }
        public Keys Key { get; set; }

        public InputEvent(InputEventType inputEventType)
        {
            InputEventType = inputEventType;
            Processed = false;
        }

        public static InputEvent MouseMove(int x, int y, MouseButtons button, bool shift, bool control, bool alt)
        {
            return new InputEvent(InputEventType.MouseMove)
            {
                MouseX = x,
                MouseY = y,
                Button = button,
                Shift = shift,
                Control = control,
                Alt = alt
            };
        }

        public static InputEvent MouseWheel(int delta, bool shift, bool control, bool alt)
        {
            return new InputEvent(InputEventType.MouseWheel)
            {
                MouseDelta = delta,
                Shift = shift,
                Control = control,
                Alt = alt
            };
        }

        public static InputEvent MouseDown(int x, int y, MouseButtons button, bool shift, bool control, bool alt)
        {
            return new InputEvent(InputEventType.MouseDown)
            {
                MouseX = x,
                MouseY = y,
                Button = button,
                Shift = shift,
                Control = control,
                Alt = alt
            };
        }

        public static InputEvent MouseDoubleClick(int x, int y, MouseButtons button, bool shift, bool control, bool alt)
        {
            return new InputEvent(InputEventType.MouseDoubleClick)
            {
                MouseX = x,
                MouseY = y,
                Button = button,
                Shift = shift,
                Control = control,
                Alt = alt
            };
        }

        public static InputEvent MouseUp(int x, int y, MouseButtons button, bool shift, bool control, bool alt)
        {
            return new InputEvent(InputEventType.MouseUp)
            {
                MouseX = x,
                MouseY = y,
                Button = button,
                Shift = shift,
                Control = control,
                Alt = alt
            };
        }

        public static InputEvent KeyDown(Keys key, bool shift, bool control, bool alt)
        {
            return new InputEvent(InputEventType.KeyDown)
            {
                Key = key,
                Shift = shift,
                Control = control,
                Alt = alt
            };
        }

        public static InputEvent KeyUp(Keys key, bool shift, bool control, bool alt)
        {
            return new InputEvent(InputEventType.KeyUp)
            {
                Key = key,
                Shift = shift,
                Control = control,
                Alt = alt
            };
        }

    }
}

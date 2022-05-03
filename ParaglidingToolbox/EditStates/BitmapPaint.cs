using ParaglidingToolbox.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParaglidingToolbox.EditStates
{
    public class BitmapPaint : EditState
    {
        private Node_Bitmap? _selectedBitmapNode;
        private int _oldX, _oldY;


        public BitmapPaint(Scene scene) : base(scene) { }

        public override bool InspectEvent(InputEvent inputEvent)
        {
            if (Scene.ActiveState != this && inputEvent.InputEventType == InputEventType.MouseDown)
            {
                _selectedBitmapNode = Scene.Root.FindNodes(node => node is Node_Bitmap d && d.IntersectsWidth(Scene.CurrentAbsMousePos)).FirstOrDefault() as Node_Bitmap;
                if (_selectedBitmapNode != null)
                {
                    var localPos = _selectedBitmapNode.ToLocal(Scene.CurrentAbsMousePos);

                    _oldX = (int)Math.Round(localPos.X);
                    _oldY = (int)Math.Round(localPos.Y);
                    return true;
                }
            }
            return false;
        }

        public override bool ProcessEvent(InputEvent inputEvent)
        {
            switch (inputEvent.InputEventType)
            {
                case InputEventType.MouseMove:
                    if (_selectedBitmapNode != null)
                    {
                        var localPos = _selectedBitmapNode.ToLocal(Scene.CurrentAbsMousePos);

                        var x = (int)Math.Round(localPos.X);
                        var y = (int)Math.Round(localPos.Y);

                        if (_selectedBitmapNode.OnPaint != null && x >= 0 && y >= 0 && x < _selectedBitmapNode.Bitmap.Width && y < _selectedBitmapNode.Bitmap.Height)
                        {
                            _selectedBitmapNode.OnPaint(x, y, x - _oldX, y - _oldY, inputEvent.Button);
                        }
                    }
                    break;

                case InputEventType.MouseUp:
                    return true;
            }

            return false;
        }


    }
}
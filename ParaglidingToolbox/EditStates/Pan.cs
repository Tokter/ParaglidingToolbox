using ParaglidingToolbox.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParaglidingToolbox.EditStates
{
    partial class Pan : EditState
    {
        private Vector2 _cameraStartPos;
        private int _mouseDownScreenX;
        private int _mouseDownScreenY;

        public Pan(Scene scene) : base(scene) { }

        public override bool InspectEvent(InputEvent inputEvent)
        {
            if (Scene.ActiveState != this && inputEvent.InputEventType == InputEventType.MouseDown && inputEvent.Button == MouseButtons.Right)
            {
                _cameraStartPos = Scene.Camera.Position;
                _mouseDownScreenX = inputEvent.MouseX;
                _mouseDownScreenY = inputEvent.MouseY;
                return true;
            }
            return false;
        }

        public override bool ProcessEvent(InputEvent inputEvent)
        {
            switch (inputEvent.InputEventType)
            {
                case InputEventType.MouseMove:
                    var dx = _mouseDownScreenX - inputEvent.MouseX;
                    var dy = _mouseDownScreenY - inputEvent.MouseY;
                    var worldDelta = Scene.Camera.ToWorld(dx, dy) - Scene.Camera.ToWorld(0, 0);
                    Scene.Camera.Position = _cameraStartPos + worldDelta;
                    break;

                case InputEventType.MouseUp:
                    return true;
            }

            return false;
        }
    }
}

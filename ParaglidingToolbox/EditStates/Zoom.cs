using ParaglidingToolbox.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParaglidingToolbox.EditStates
{
    public class Zoom : EditState
    {
        public Zoom(Scene scene) : base(scene) { }

        public override bool InspectEvent(InputEvent inputEvent)
        {
            return inputEvent.InputEventType == InputEventType.MouseWheel;
        }

        public override bool ProcessEvent(InputEvent inputEvent)
        {
            if (inputEvent.InputEventType == InputEventType.MouseWheel)
            {
                var newScale = Scene.Camera.Scale * (float)Math.Pow(1.2d, inputEvent.MouseDelta / 120.0d);

                if (newScale < 0.05f) newScale = 0.05f;
                if (newScale > 1000) newScale = 1000;
                Scene.Camera.Scale = newScale;

                inputEvent.Processed = true;
            }

            return true;
        }
    }
}

using ParaglidingToolbox.Scenes;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParaglidingToolbox.EditStates
{
    public abstract class EditState
    {
        public Scene Scene { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public EditState(Scene scene)
        {
            Scene = scene;
        }

        public virtual bool Available()
        {
            return true;
        }

        public virtual void Activated()
        {
        }

        public virtual bool InspectEvent(InputEvent inputEvent)
        {
            return false;
        }

        public virtual bool ProcessEvent(InputEvent inputEvent)
        {
            return true;
        }

        public virtual Cursor? SetCursor()
        {
            return Cursor.Current;
        }

        public virtual void DrawUI(SKSurface surface, Camera camera)
        {
        }

        public Vector2 Snap(InputEvent inputEvent, Camera camera, Vector2 absPos)
        {
            if (inputEvent.Control)
            {
                absPos = Scene.GetClosestSnapPoint(camera, absPos);
            }
            else if (inputEvent.Shift)
            {
                //no snapping
            }
            else
            {
                absPos = Scene.GetGridSnapPoint(camera, absPos);
            }
            
            return absPos;
        }
    }
}

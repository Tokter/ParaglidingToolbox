using ParaglidingToolbox.EditStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParaglidingToolbox.Scenes
{
    public class Scene_FluidSimulator : Scene
    {
        public Scene_FluidSimulator()
        {
            RegisterEditState(new Pan(this));
            RegisterEditState(new Zoom(this));
            Root.Add(new Node_SimpleGrid());
        }
    }
}

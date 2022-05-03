using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParaglidingToolbox.Scenes
{
    public interface IIntersectable
    {
        bool IntersectsWidth(Vector2 pos);
    }
}

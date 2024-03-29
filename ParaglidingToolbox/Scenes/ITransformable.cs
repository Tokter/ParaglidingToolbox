﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParaglidingToolbox.Scenes
{
    public interface ITransformable
    {
        float Rotation { get; set; }
        Vector2 Position { get; set; }
        float Scale { get; set; }
        Matrix3x2 GetTransform();
        Matrix3x2 GetInvTransform();
        Vector2 ToAbs(Vector2 pos);
        Vector2 ToLocal(Vector2 pos);
        ITransformable Parent { get; }
    }
}

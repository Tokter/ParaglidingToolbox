using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParaglidingToolbox
{
    public static class FloatEx
    {
        public static float ToRad(this float angle)
        {
            return (float)(angle * Math.PI / 180.0f);
        }

        public static float ToDeg(this float angle)
        {
            return (float)(angle * 180.0f / Math.PI);
        }
    }
}

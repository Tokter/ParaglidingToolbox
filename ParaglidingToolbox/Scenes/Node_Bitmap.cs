using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParaglidingToolbox.Scenes
{
    public class Node_Bitmap : SceneNode, IIntersectable
    {
        private SKBitmap _bitmap;
        private SKRect _bitmapRect;

        public SKBitmap Bitmap => _bitmap;

        public Node_Bitmap(int size)
        {
            _bitmap = new SKBitmap(size, size, SKColorType.Rgba8888, SKAlphaType.Premul);
            _bitmapRect = new SKRect(0, 0, _bitmap.Width, _bitmap.Height);
        }

        public override void DrawScene(SKSurface surface, Camera camera)
        {
            surface.Canvas.DrawBitmap(_bitmap, _bitmapRect);
        }
        
        public bool IntersectsWidth(Vector2 pos)
        {
            var localPos = ToLocal(pos);
            return _bitmapRect.Contains(localPos.X, localPos.Y);
        }

        public Action<int, int, int, int, MouseButtons>? OnPaint { get; set; }

    }
}

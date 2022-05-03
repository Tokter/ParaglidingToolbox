using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParaglidingToolbox.Scenes
{
    public class Node_ForceField : SceneNode
    {
        private int _size;
        private double[,] _forceFieldX;
        private double[,] _forceFieldY;
        private SKPaint _linePaint = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Green,
            StrokeWidth = 0.1f
        };
        private SKPaint _dotPaint = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Red,
            StrokeWidth = 0.2f,
            StrokeCap = SKStrokeCap.Round,
        };

        public Node_ForceField(int size, double[,] forceFieldX, double[,] forceFieldY)
        {
            _size = size;
            _forceFieldX = forceFieldX;
            _forceFieldY = forceFieldY;
        }

        public override void DrawScene(SKSurface surface, Camera camera)
        {
            for (int y = 0; y < _size; y++)
            {
                for (int x = 0; x < _size; x++)
                {
                    surface.Canvas.DrawLine(0.5f + x, 0.5f + y, 0.5f + x + (float)_forceFieldX[x + 1, y + 1], 0.5f + y + (float)_forceFieldY[x + 1, y + 1], _linePaint);
                    surface.Canvas.DrawLine(0.5f + x, 0.5f + y, 0.5f + x, 0.5f + y, _dotPaint);
                }
            }

            //surface.Canvas.DrawBitmap(_bitmap, _bitmapRect);
        }

    }
}

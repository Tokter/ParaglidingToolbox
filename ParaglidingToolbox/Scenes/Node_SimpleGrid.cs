using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParaglidingToolbox.Scenes
{
    partial class Node_SimpleGrid : SceneNode
    {
        private SKColor _originColor;
        private SKPaint _originPaint;
        private SKColor _mayorColor;
        private SKPaint _mayorPaint;
        private SKColor _minorColor;
        private SKPaint _minorPaint;

        public SKColor BackgroundColor = new SKColor(30, 30, 30, 255);

        public Node_SimpleGrid()
        {
            OriginColor = new SKColor(0x60, 0x60, 0x60, 255);
            MayorColor = new SKColor(0x50, 0x50, 0x50, 255);
            MinorColor = new SKColor(0x40, 0x40, 0x40, 255);
        }

        public SKColor OriginColor
        {
            get { return _originColor; }
            set
            {
                _originColor = value;
                if (_originPaint != null) _originPaint.Dispose();
                _originPaint = new SKPaint
                {
                    Color = _originColor,
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 1.0f,
                    StrokeCap = SKStrokeCap.Butt,
                };
            }
        }

        public SKColor MayorColor
        {
            get { return _mayorColor; }
            set
            {
                _mayorColor = value;
                if (_mayorPaint != null) _mayorPaint.Dispose();
                _mayorPaint = new SKPaint
                {
                    Color = _mayorColor,
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 1.0f,
                    StrokeCap = SKStrokeCap.Butt,
                };
            }
        }

        public SKColor MinorColor
        {
            get { return _originColor; }
            set
            {
                _minorColor = value;
                if (_minorPaint != null) _minorPaint.Dispose();
                _minorPaint = new SKPaint
                {
                    Color = _minorColor,
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 1.0f,
                    StrokeCap = SKStrokeCap.Butt,
                };
            }
        }

        public override void DrawScene(SKSurface surface, Camera camera)
        {
            surface.Canvas.Clear(BackgroundColor);

            var topLeft = camera.ToWorld(0, 0);
            var tenPixels = (camera.ToWorld(10, 0) - topLeft).Length();

            var bottomRight = camera.ToWorld(camera.ScreenWidth - 1, camera.ScreenHeight - 1);
            int left = ((int)Math.Round(topLeft.X / 100.0f) - 1) * 100;
            int top = ((int)Math.Round(topLeft.Y / 100.0f) - 1) * 100;
            int bottom = ((int)Math.Round(bottomRight.Y / 100.0f) + 1) * 100;
            int right = ((int)Math.Round(bottomRight.X / 100.0f) + 1) * 100;

            int step = 10;
            if (tenPixels > 10) step = 100;

            for (int x = left; x <= right; x += step)
            {
                surface.Canvas.DrawLine(x, top, x, bottom, GetGridPaint(x, camera));
            }

            for (int y = top; y <= bottom; y += step)
            {
                surface.Canvas.DrawLine(left, y, right, y, GetGridPaint(y, camera));
            }
        }

        private SKPaint GetGridPaint(int pos, Camera camera)
        {
            if (pos == 0)
            {
                _originPaint.StrokeWidth = 2.0f / camera.Scale;
                return _originPaint;
            }
            else if (pos % 100 == 0)
            {
                _mayorPaint.StrokeWidth = 1.5f / camera.Scale;
                return _mayorPaint;
            }
            else
            {
                _minorPaint.StrokeWidth = 1.0f / camera.Scale;
                return _minorPaint;
            }
        }

        public override void DrawUI(SKSurface surface, Camera camera)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_originPaint != null) _originPaint.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

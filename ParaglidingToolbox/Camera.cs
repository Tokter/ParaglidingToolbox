using ParaglidingToolbox.PropertyGridConverters;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParaglidingToolbox
{
    public class Camera
    {
        private float _rotation = 0.0f;
        private Vector2 _position = new Vector2(0, 0);
        private float _scale = 1.0f;
        private bool _viewTransformIsDirty = true;

        private Matrix3x2 _modelViewtransform;
        private Matrix3x2 _modelViewInvTransform;

        private Matrix3x2 _viewTransform;
        private Matrix3x2 _viewInvTransform;

        public float Rotation
        {
            get => _rotation;
            set { _rotation = value; _viewTransformIsDirty = true; }
        }

        [TypeConverter(typeof(Vector2Converter))]
        public Vector2 Position
        {
            get => _position;
            set { _position = value; _viewTransformIsDirty = true; }
        }

        public float Scale
        {
            get => _scale;
            set { _scale = value; _viewTransformIsDirty = true; }
        }

        private int _screenWidth;
        private int _screenHeight;

        [Browsable(false)]
        public int ScreenWidth
        {
            get => _screenWidth;
            set { _screenWidth = value; _viewTransformIsDirty = true; }
        }

        [Browsable(false)]
        public int ScreenHeight
        {
            get => _screenHeight;
            set { _screenHeight = value; _viewTransformIsDirty = true; }
        }

        public void ApplyModelViewTransformToSurface(SKSurface surface, Matrix3x2 modelTransform, Matrix3x2 invModelTransform)
        {
            if (_viewTransformIsDirty)
            {
                _viewTransform = CalculateViewTransform();
                Matrix3x2.Invert(_viewTransform, out _viewInvTransform);
                _viewTransformIsDirty = false;
            }

            _modelViewtransform = modelTransform * _viewTransform;
            _modelViewInvTransform = _viewInvTransform * invModelTransform;
            surface.Canvas.SetMatrix(new SKMatrix(_modelViewtransform.M11, _modelViewtransform.M21, _modelViewtransform.M31, _modelViewtransform.M12, _modelViewtransform.M22, _modelViewtransform.M32, 0, 0, 1));
        }

        public Vector2 ToWorld(int screenX, int screenY)
        {
            return Vector2.Transform(new Vector2(screenX, screenY), _viewInvTransform);
        }

        public Vector2 ToScreen(Vector2 worldPos)
        {
            return Vector2.Transform(worldPos, _viewTransform);
        }

        protected virtual Matrix3x2 CalculateViewTransform()
        {
            return Matrix3x2.CreateTranslation(-Position.X, -Position.Y)
                * Matrix3x2.CreateRotation(Rotation)
                * Matrix3x2.CreateScale(Scale);
        }
    }

    public class ScreenCenterCamera : Camera
    {
        protected override Matrix3x2 CalculateViewTransform()
        {
            var screenCenter = new Vector2(ScreenWidth / 2.0f, ScreenHeight / 2.0f);

            return
                Matrix3x2.CreateTranslation(-Position.X, -Position.Y)
                * Matrix3x2.CreateRotation(Rotation)
                * Matrix3x2.CreateScale(Scale)
                * Matrix3x2.CreateTranslation(screenCenter);

        }
    }

    public class UICamera : Camera
    {
        protected override Matrix3x2 CalculateViewTransform()
        {
            return Matrix3x2.Identity;
        }
    }
}

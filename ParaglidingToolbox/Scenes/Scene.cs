using ParaglidingToolbox.EditStates;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParaglidingToolbox.Scenes
{
    public class Scene : IDisposable
    {
        private ScreenCenterCamera _camera;
        private UICamera _uiCamera;
        private SceneNode _root;

        [Browsable(false)]
        public Camera Camera => _camera;

        [Browsable(false)]
        public Camera UICamera => _uiCamera;

        [Browsable(false)]
        public SceneNode Root => _root;

        [Browsable(false)]
        public Vector2 CurrentAbsMousePos { get; private set; }

        [Browsable(false)]
        public int CurrentMouseScreenX { get; private set; }

        [Browsable(false)]
        public int CurrentMouseScreenY { get; private set; }

        public Scene()
        {
            _camera = new ScreenCenterCamera();
            _uiCamera = new UICamera();
            _root = null!;
            _root = new SceneNode();
        }

        public void SetScreenSize(int width, int height)
        {
            _camera.ScreenWidth = width;
            _camera.ScreenHeight = height;
            _uiCamera.ScreenWidth = width;
            _uiCamera.ScreenHeight = height;
        }

        protected void RegisterEditState(EditState state)
        {
            _availableStates.Add(state);
        }

        #region Event handling

        [Browsable(false)]
        public Action ProcessFinished { get; set; }

        private List<EditState> _availableStates = new List<EditState>();
        private Stack<EditState> _activeState = new Stack<EditState>();
        private bool _cursorChanged = false;

        [Browsable(false)]
        public EditState? ActiveState
        {
            get
            {
                if (_activeState.Count == 0) return null;
                return _activeState.Peek();
            }
        }

        public void Activate(string name)
        {
            if (ActiveState == null)
            {
                var newState = _availableStates.FirstOrDefault(s => s?.Name?.ToLower().Trim() == name.ToLower().Trim());
                if (newState != null)
                {
                    newState.Activated();
                    _activeState.Push(newState);
                }
            }
        }

        public bool ProcessEvent(InputEvent inputEvent)
        {
            bool handled = false;
            _cursorChanged = false;

            //Store current mouse position
            if (inputEvent.InputEventType == InputEventType.MouseMove || inputEvent.InputEventType == InputEventType.MouseUp || inputEvent.InputEventType == InputEventType.MouseDown)
            {
                CurrentAbsMousePos = Camera.ToWorld(inputEvent.MouseX, inputEvent.MouseY);
                CurrentMouseScreenX = inputEvent.MouseX;
                CurrentMouseScreenY = inputEvent.MouseY;
            }

            //Inspect Event
            //Check if one of the available edit states wants to be activated
            foreach (var s in _availableStates.Where(a => a.Available()))
            {
                if (s.InspectEvent(inputEvent))
                {
                    if (ActiveState != s)
                    {
                        s.Activated();
                        _activeState.Push(s);
                    }
                }
            }

            if (!_cursorChanged && ActiveState == null)
            {
                SetCursor(Cursors.Arrow);
            }

            if (ActiveState == null) return false;

            //Process Event
            var done = false;
            while (!done && ActiveState != null)
            {
                handled = true;
                var removeState = ActiveState.ProcessEvent(inputEvent);
                if (removeState)
                {
                    _activeState.Pop();
                    done = false;
                }
                else
                {
                    done = true;
                }
            }

            if (ActiveState == null && ProcessFinished != null) ProcessFinished();
            
            return handled;
        }

        public void SetCursor(Cursor cursor)
        {
            _cursorChanged = true;
            if (ChangeCursor != null) ChangeCursor(cursor);
        }

        [Browsable(false)]
        public Action<Cursor>? ChangeCursor { get; set; }

        #endregion

        #region Drawing

        private Matrix3x2 _modelTransform = Matrix3x2.Identity;
        private Matrix3x2 _modelInvTransform = Matrix3x2.Identity;
        private Stack<Matrix3x2> _modelTransforms = new Stack<Matrix3x2>();
        private Stack<Matrix3x2> _modelInvTransforms = new Stack<Matrix3x2>();
        private SKPaint _stateText = new SKPaint { Color = SKColors.Orange, TextSize = 16.0f };

        private void ClearModelTransform()
        {
            _modelTransform = Matrix3x2.Identity;
            _modelInvTransform = Matrix3x2.Identity;
        }

        private void PushModelTransform()
        {
            _modelTransforms.Push(_modelTransform);
            _modelInvTransforms.Push(_modelInvTransform);
        }

        public void SetModelTransform(Matrix3x2 matrix)
        {
            _modelTransform = matrix * _modelTransform;
            Matrix3x2.Invert(_modelTransform, out _modelInvTransform);
        }

        private void PopModelTransform()
        {
            _modelTransform = _modelTransforms.Pop();
            _modelInvTransform = _modelInvTransforms.Pop();
        }

        public void Draw(SKSurface surface)
        {
            //Draw in object space
            ClearModelTransform();
            surface.Canvas.Save();
            DrawSceneNode(surface, Root, Camera);
            surface.Canvas.Restore();

            //Draw in screen space
            ClearModelTransform();
            surface.Canvas.Save();
            DrawUISceneNode(surface, Root, UICamera);

            if (!string.IsNullOrEmpty(ActiveState?.Description))
            {
                surface.Canvas.DrawText($"{ActiveState.Description} - [Esc] to abort", 30, Camera.ScreenHeight - 30, _stateText);
            }

            surface.Canvas.Restore();
        }

        private void DrawSceneNode(SKSurface surface, SceneNode node, Camera camera)
        {
            PushModelTransform();
            SetModelTransform(node.GetTransform());
            camera.ApplyModelViewTransformToSurface(surface, _modelTransform, _modelInvTransform);
            node.DrawScene(surface, camera);
            foreach (var child in node.Where(n => n.Visible))
            {
                DrawSceneNode(surface, child, camera);
            }
            PopModelTransform();
        }

        private void DrawUISceneNode(SKSurface surface, SceneNode node, Camera camera)
        {
            node.DrawUI(surface, camera);

            for (int i = 0; i < node.Count; i++)
            {
                if (node[i].Visible)
                {
                    DrawUISceneNode(surface, node[i], camera);
                }
            }

            ActiveState?.DrawUI(surface, camera);
        }

        #endregion

        #region Snapping

        private List<SnapHint> _snapHints = new List<SnapHint>();
        private List<Vector2> _intersectionPoints = new List<Vector2>();
        private List<Vector2> _linePoints = new List<Vector2>();
        private List<Vector2> _snapPoints = new List<Vector2>();
        private SKPaint _snapLine = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.0f,
            StrokeCap = SKStrokeCap.Round,
            BlendMode = SKBlendMode.SrcOver,
            Color = new SKColor(100, 255, 0, 20)
        };

        public void CaptureSnapHints()
        {
            _snapHints.Clear();

            //Gather all snapping hints
            foreach (IProvideSnapHints snapHintProvider in Root.FindNodes(n => n is IProvideSnapHints))
            {
                _snapHints.AddRange(snapHintProvider.GetSnapHints());
            }

            //Calculate all intersection points of hint lines
            _intersectionPoints.Clear();
            for (int i = 0; i < _snapHints.Count; i++)
            {
                if (_snapHints[i].Type == SnapType.Line)
                {
                    for (int j = i + 1; j < _snapHints.Count; j++)
                    {
                        if (_snapHints[j].Type == SnapType.Line)
                        {
                            var intersectionPoint = _snapHints[i].GetIntersectionPoint(_snapHints[j]);
                            if (intersectionPoint != null)
                            {
                                _intersectionPoints.Add(intersectionPoint.Value);
                            }
                        }
                    }
                }
            }
        }

        public Vector2 GetGridSnapPoint(Camera camera, Vector2 absPosition)
        {
            var gridHint = _snapHints.FirstOrDefault(s => s.Type == SnapType.Grid);
            if (gridHint != null && gridHint.SnapFunction != null)
            {
                return gridHint.SnapFunction(absPosition);
            }
            else return absPosition;
        }

        public Vector2 GetClosestSnapPoint(Camera camera, Vector2 absPosition)
        {
            float closestDistance = float.MaxValue;
            Vector2 closestPoint = Vector2.Zero;

            //Check all intersection points of hint lines
            //We want them to have the strongest snapping force
            foreach (var intersectionPoint in _intersectionPoints)
            {
                var distance = (intersectionPoint - absPosition).Length();
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPoint = intersectionPoint;
                }
            }
            if (DoesSnap(camera, closestPoint, absPosition)) return closestPoint;

            //Check all points that can be snapped to
            _snapPoints.Clear();
            closestDistance = float.MaxValue;
            closestPoint = Vector2.Zero;

            bool snappedToPoint = false;
            for (int i = 0; i < _snapHints.Count; i++)
            {
                if (_snapHints[i].Type == SnapType.Point)
                {
                    _snapPoints.Add(_snapHints[i].PosA);
                    var distance = (_snapHints[i].PosA - absPosition).Length();
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPoint = _snapHints[i].PosA;
                        snappedToPoint = true;
                    }
                }
            }

            //Now we also snap it to a close by line
            var startPoint = absPosition;
            if (snappedToPoint && DoesSnap(camera, closestPoint, absPosition))
            {
                startPoint = closestPoint;
            }

            _linePoints.Clear();
            closestDistance = float.MaxValue;
            foreach (var lineHint in _snapHints.Where(h => h.Type == SnapType.Line))
            {
                var linePoint = lineHint.GetClosestPointOnLine(startPoint);
                _linePoints.Add(linePoint);
                var distance = (linePoint - startPoint).Length();
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPoint = linePoint;
                }
            }

            return Snap(camera, closestPoint, startPoint);
        }

        private bool DoesSnap(Camera camera, Vector2 snapPoint, Vector2 startPoint)
        {
            var screenSnapPoint = camera.ToScreen(snapPoint);
            var screenStartPoint = camera.ToScreen(startPoint);

            return ((screenSnapPoint - screenStartPoint).Length() <= 20);
        }

        private Vector2 Snap(Camera camera, Vector2 snapPoint, Vector2 startPoint)
        {
            if (DoesSnap(camera, snapPoint, startPoint)) return snapPoint; else return startPoint;
        }

        public void DrawSnapHints(SKSurface surface, Camera camera)
        {
            foreach (var lineHint in _snapHints.Where(h => h.Type == SnapType.Line))
            {
                //A->B dir;
                var dir = lineHint.PosB - lineHint.PosA;
                var from = camera.ToScreen(lineHint.PosA - 5 * dir);
                var to = camera.ToScreen(lineHint.PosB + 5 * dir);

                surface.Canvas.DrawLine(from.X, from.Y, to.X, to.Y, _snapLine);
            }


            foreach (var point in _snapPoints)
            {
                var sp = camera.ToScreen(point);

                surface.Canvas.DrawLine(sp.X - 5, sp.Y - 5, sp.X + 5, sp.Y + 5, _snapLine);
                surface.Canvas.DrawLine(sp.X - 5, sp.Y + 5, sp.X + 5, sp.Y - 5, _snapLine);
            }
        }

        #endregion

        public void Dispose()
        {
            if (_snapLine != null)
            {
                _snapLine.Dispose();
                _snapLine = null!;
            }
        }
    }
}

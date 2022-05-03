using ParaglidingToolbox.EditStates;
using ParaglidingToolbox.FluidSimulator;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParaglidingToolbox.Scenes
{
    public class Scene_FluidSimulator : Scene
    {
        private const int SIZE = 64;
        
        private FluidSimulator2D _simulator;
        private Node_Bitmap _bitmapNode;

        public float WindStrength { get; set; } = 0.1f;
        public float ThermalStrength { get; set; } = 0.1f;
        public double Viscosity { get => _simulator.Viscosity; set => _simulator.Viscosity = value; }

        public Scene_FluidSimulator()
        {
            //Setup editing tools
            RegisterEditState(new Pan(this));
            RegisterEditState(new Zoom(this));
            RegisterEditState(new BitmapPaint(this));

            _simulator = new FluidSimulator2D(SIZE, 0.001d, 0.0000001d);

            //Setup scene
            Root.Add(new Node_SimpleGrid());
            _bitmapNode = new Node_Bitmap(SIZE);
            _bitmapNode.OnPaint = OnPaint;
            Root.Add(_bitmapNode);

            var forceField = new Node_ForceField(SIZE, _simulator.ForceX, _simulator.ForceY);
            Root.Add(forceField);
        }

        public void OnPaint(int x, int y, int dx, int dy, MouseButtons button)
        {
            if (button == MouseButtons.Right)
            {
                _simulator.SetDensity(x - 1, y, 2000);
                _simulator.SetDensity(x, y - 1, 2000);
                _simulator.SetDensity(x + 1, y, 2000);
                _simulator.SetDensity(x, y + 1, 2000);
            }
            if (button == MouseButtons.Left)
            {
                _simulator.SetForce(x, y, dx * 5, dy * 5);
            }
        }

        public override void Update()
        {
            //Wind gradient
            for (int y = 0; y < SIZE; y++)
            {
                _simulator.SetForce(0, y, (SIZE - y - 1) * WindStrength, 0);
            }

            //Thermal
            for (int y = SIZE - 1; y >= SIZE / 2; y--)
            {
                _simulator.SetForce(SIZE / 2, y, 0, -1 * ThermalStrength);
            }


            _simulator.Velocity_Step(0.1f);            
            _simulator.Desnity_Step(0.1f);
            DensityToBitmap(_bitmapNode.Bitmap);
            _simulator.Clear(0);
        }

        public void DensityToBitmap(SKBitmap bmp)
        {
            double min = 0.0f;
            double max = 40.0f;

            for (int y = 1; y <= SIZE; y++)
            {
                for (int x = 1; x <= SIZE; x++)
                {
                    var d = Math.Min(max, _simulator.Density[x, y]);
                    byte b = (byte)((d - min) * 255.0d / (max - min));
                    bmp.SetPixel(x - 1, y - 1, new SKColor(b, b, b, 255));
                }
            }
        }
    }    
}

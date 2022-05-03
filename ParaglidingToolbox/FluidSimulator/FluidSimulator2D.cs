using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParaglidingToolbox.FluidSimulator
{
    public class FluidSimulator2D
    {
        private int _n;
        private double _visc;
        private double _diff;
        private double[,] _density;
        private double[,] _density_prev;
        private double[,] _forceX;
        private double[,] _forceX_prev;
        private double[,] _forceY;
        private double[,] _forceY_prev;

        public double[,] Density => _density;
        public double[,] ForceX => _forceX;
        public double[,] ForceY => _forceY;
        public double Viscosity { get => _visc; set => _visc = value; }

        public FluidSimulator2D(int n, double visc, double diff)
        {
            _n = n;
            _visc = visc;
            _diff = diff;
            _density = new double[_n + 2, _n + 2];
            _density_prev = new double[_n + 2, _n + 2];
            _forceX = new double[_n + 2, _n + 2];
            _forceX_prev = new double[_n + 2, _n + 2];
            _forceY = new double[_n + 2, _n + 2];
            _forceY_prev = new double[_n + 2, _n + 2];
        }

        public void SetForce(int x, int y, double fx, double fy)
        {
            _forceX_prev[x + 1, y + 1] = fx;
            _forceY_prev[x + 1, y + 1] = fy;
        }

        public void SetDensity(int x, int y, double d)
        {
            _density_prev[x + 1, y + 1] = d;
        }

        public void Clear(double baseDensity)
        {
            for (int y = 0; y < (_n + 2); y++)
            {
                for (int x = 0; x < (_n + 2); x++)
                {
                    _forceX_prev[x, y] = 0.0d;
                    _forceY_prev[x, y] = 0.0d;
                    _density_prev[x, y] = baseDensity;
                }
            }
        }

        public void Desnity_Step(double dt)
        {
            Add_Source(_density, _density_prev, dt);
            Swap(ref _density_prev, ref _density); Diffuse(0, _density, _density_prev, _diff, dt);
            Swap(ref _density_prev, ref _density); Advect(0, _density, _density_prev, dt);
        }

        public void Velocity_Step(double dt)
        {
            Add_Source(_forceX, _forceX_prev, dt);
            Add_Source(_forceY, _forceY_prev, dt);
            Swap(ref _forceX_prev, ref _forceX); Diffuse(0, _forceX, _forceX_prev, _visc, dt);  //1
            Swap(ref _forceY_prev, ref _forceY); Diffuse(0, _forceY, _forceY_prev, _visc, dt);  //2
            Project();
            Swap(ref _forceX_prev, ref _forceX);
            Swap(ref _forceY_prev, ref _forceY);
            Advect(0, _forceX, _forceX_prev, dt); //1
            Advect(0, _forceY, _forceY_prev, dt); //2
            Project();
        }

        private void Add_Source(double[,] target, double[,] source, double dt)
        {
            for (int x = 0; x < (_n + 2); x++)
            {
                for (int y = 0; y < (_n + 2); y++)
                {
                    target[x, y] += dt * source[x, y];
                }
            }
        }

        private void Swap(ref double[,] a, ref double[,] b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        private void Diffuse(int b, double[,] field, double[,] field_prev, double alpha, double dt)
        {
            double a = dt * alpha * _n * _n;
            Lin_Solve(b, field, field_prev, a, 1 + 4 * a);
        }

        private void Lin_Solve(int b, double[,] field, double[,] field_prev, double a, double c)
        {
            for (int k = 0; k < 20; k++)
            {
                Parallel.For(1, _n + 1, (x) =>
                //for (int x = 1; x <= _n; x++) 
                {
                    for (int y = 1; y <= _n; y++)
                    {
                        field[x, y] = (field_prev[x, y] + a * (field[x - 1, y] + field[x + 1, y] + field[x, y - 1] + field[x, y + 1])) / c;
                    }
                });
                Set_Boundary(b, field);
            }
        }

        private void Set_Boundary(int b, double[,] field)
        {
            if (b > 0)
            {
                for (int i = 1; i <= _n; i++)
                {
                    field[0, i] = b == 1 ? -field[1, i] : field[1, i];
                    field[_n + 1, i] = b == 1 ? -field[_n, i] : field[_n, i];
                    field[i, 0] = b == 2 ? -field[i, 1] : field[i, 1];
                    field[i, _n + 1] = b == 2 ? -field[i, _n] : field[i, _n];
                }
                field[0, 0] = 0.5d * (field[1, 0] + field[0, 1]);
                field[0, _n + 1] = 0.5d * (field[1, _n + 1] + field[0, _n]);
                field[_n + 1, 0] = 0.5d * (field[_n, 0] + field[_n + 1, 1]);
                field[_n + 1, _n + 1] = 0.5d * (field[_n, _n + 1] + field[_n + 1, _n]);
            }
            else
            {
                for (int i = 1; i <= _n; i++)
                {
                    field[0, i] = 0;
                    field[_n + 1, i] = 0;
                    field[i, 0] = 0;
                    field[i, _n + 1] = 0;
                }
                field[0, 0] = 0;
                field[0, _n + 1] = 0;
                field[_n + 1, 0] = 0;
                field[_n + 1, _n + 1] = 0;
            }
        }

        private void Advect(int b, double[,] field, double[,] field_prev, double dt)
        {
            int i0, j0, i1, j1;
            double fx, fy, s0, t0, s1, t1, dt0;
            dt0 = dt * _n;

            for (int x = 1; x <= _n; x++)
            {
                for (int y = 1; y <= _n; y++)
                {
                    fx = x - dt0 * _forceX[x, y]; fy = y - dt0 * _forceY[x, y];
                    if (fx < 0.5d) fx = 0.5d; if (fx > _n + 0.5d) fx = _n + 0.5d; i0 = (int)fx; i1 = i0 + 1;
                    if (fy < 0.5d) fy = 0.5d; if (fy > _n + 0.5d) fy = _n + 0.5d; j0 = (int)fy; j1 = j0 + 1;
                    s1 = fx - i0; s0 = 1 - s1; t1 = fy - j0; t0 = 1 - t1;
                    field[x, y] = s0 * (t0 * field_prev[i0, j0] + t1 * field_prev[i0, j1]) + s1 * (t0 * field_prev[i1, j0] + t1 * field_prev[i1, j1]);
                }
            }

            Set_Boundary(b, field);
        }

        private void Project()
        {
            for (int x = 1; x <= _n; x++)
            {
                for (int y = 1; y <= _n; y++)
                {
                    _forceY_prev[x, y] = -0.5d * (_forceX[x + 1, y] - _forceX[x - 1, y] + _forceY[x, y + 1] - _forceY[x, y - 1]) / _n;
                    //_forceY_prev[x, y] = -0.5f * (_forceX[x, y] - _forceX[x - 1, y] + _forceY[x, y] - _forceY[x, y - 1]) / _n;
                    _forceX_prev[x, y] = 0;
                }
            }

            Set_Boundary(0, _forceX_prev);
            Set_Boundary(0, _forceY_prev);

            Lin_Solve(0, _forceX_prev, _forceY_prev, 1, 4);

            for (int x = 1; x <= _n; x++)
            {
                for (int y = 1; y <= _n; y++)
                {
                    _forceX[x, y] -= 0.5d * (_forceX_prev[x + 1, y] - _forceX_prev[x - 1, y]) * _n;
                    _forceY[x, y] -= 0.5d * (_forceX_prev[x, y + 1] - _forceX_prev[x, y - 1]) * _n;
                    //_forceX[x, y] -= 0.5f * (_forceX_prev[x + 1, y] - _forceX_prev[x, y]) * _n;
                    //_forceY[x, y] -= 0.5f * (_forceX_prev[x, y + 1] - _forceX_prev[x, y]) * _n;
                }
            }

            Set_Boundary(1, _forceX);
            Set_Boundary(2, _forceY);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParaglidingToolbox
{
    public enum SnapType
    {
        Line,
        Point,
        Grid
    }

    public class SnapHint
    {
        public SnapType Type { get; set; }
        public Vector2 PosA { get; set; }
        public Vector2 PosB { get; set; }
        public Func<Vector2, Vector2>? SnapFunction { get; set; }

        public Vector2? GetIntersectionPoint(SnapHint b)
        {
            // Line AB represented as a1x + b1y = c1 
            double a1 = PosB.Y - PosA.Y;
            double b1 = PosA.X - PosB.X;
            double c1 = a1 * (PosA.X) + b1 * (PosA.Y);

            // Line CD represented as a2x + b2y = c2 
            double a2 = b.PosB.Y - b.PosA.Y;
            double b2 = b.PosA.X - b.PosB.X;
            double c2 = a2 * (b.PosA.X) + b2 * (b.PosA.Y);

            double determinant = a1 * b2 - a2 * b1;

            if (determinant == 0)
            {
                // The lines are parallel.
                return null;
            }
            else
            {
                double x = (b2 * c1 - b1 * c2) / determinant;
                double y = (a1 * c2 - a2 * c1) / determinant;
                return new Vector2((float)x, (float)y);
            }
        }

        public Vector2 GetClosestPointOnLine(Vector2 pos)
        {
            Vector2 AP = pos - PosA;       //Vector from A to P   
            Vector2 AB = PosB - PosA;      //Vector from A to B  

            float magnitudeAB = AB.LengthSquared();     //Magnitude of AB vector (it's length squared)     
            float ABAPproduct = Vector2.Dot(AP, AB);    //The DOT product of a_to_p and a_to_b     
            float distance = ABAPproduct / magnitudeAB; //The normalized "distance" from a to your closest point  

            return PosA + AB * distance;
        }
    }

    public interface IProvideSnapHints
    {
        IEnumerable<SnapHint> GetSnapHints();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;
using static solidworks_addins_3.BraeClasses;


namespace solidworks_addins_3
{
    public class Functions
    {
        public static double InToM(double inches)
        {
            return inches * 0.0254;
        }

        public static double MToIn(double meters)
        {
            return meters / 0.0254;
        }

        public static double degreeToRadian(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        public static double radianToDegree(double radian)
        {
            return radian * 180.0 / Math.PI;
        }

        public static double getAngleBetween2DPointsInRadians(Point point1, Point point2)
        {
            // Calculate the direction vectors
            double directionX = point2.x - point1.x;
            double directionY = point2.y - point1.y;

            // Calculate the angle in radians
            double angle = Math.Atan2(directionY, directionX);

            return angle;
        }
    }
}

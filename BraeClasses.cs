using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace solidworks_addins_3
{
    public class BraeClasses
    {
        public class Point
        {
            public double x;
            public double y;
            public double z;
            public Point(double x, double y, double z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }
            public (double, double, double) ListValues()
            {
                return (x, y, z);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static solidworks_addins_3.BraeClasses;
using static solidworks_addins_3.Functions;
using SolidWorks.Interop.sldworks;

namespace solidworks_addins_3
{
    public static class SkMgrOverloads
    {
        public static void CreateLineFromPoints(this ISketchManager skMgr, Point point1, Point point2)
        {
            skMgr.CreateLine(point1.x, point1.y, point1.z, point2.x, point2.y, point2.z);
        }

        public static void CreateCenterLineFromPoints(this ISketchManager skMgr, Point point1, Point point2)
        {
            skMgr.CreateCenterLine(point1.x, point1.y, point1.z, point2.x, point2.y, point2.z);
        }

        public static void Create3PointArcFromPoints(this ISketchManager skMgr, Point point1, Point point2, Point point3)
        {
            skMgr.Create3PointArc(point1.x, point1.y, point1.z, point2.x, point2.y, point2.z, point3.x, point3.y, point3.z);
        }

        public static void DrawFilledTriangleInDirectionWithOrientation(this ISketchManager skMgr, Point center, double MatThickness, double HthDistance, double HoleDiameter, double arcRadius, double direction, double triangleAngle)
        {
            double distanceFromTriangleCenterToCircleCenter = ((HthDistance / 2) / Math.Cos(degreeToRadian(30)));
            Point triangleCenter = new Point(center.x + distanceFromTriangleCenterToCircleCenter * Math.Cos(degreeToRadian(direction)), center.y + distanceFromTriangleCenterToCircleCenter * Math.Sin(degreeToRadian(direction)),0);
            skMgr.DrawFilletedTriangle(triangleCenter, MatThickness, HthDistance, HoleDiameter, arcRadius, triangleAngle);
        }
        public static void DrawFilletedTriangle(this ISketchManager skMgr, Point center, double MatThickness, double HthDistance, double HoleDiameter, double arcRadius, double triangleAngle)
        {
            //the defining dimension could either be the hold to the arc thickness being 2*matThickness or it could be arc to arc so check both and pick which one is right
            double distanceIfHoleToArc = ((HthDistance / 2) / Math.Cos(degreeToRadian(30))) - (HoleDiameter / 2 + 2 * MatThickness + arcRadius);
            double distanceIfArcToArc = ((HthDistance / 2) / Math.Cos(degreeToRadian(30))) - (2 * arcRadius + 2 * MatThickness);

            double distanceFromTriangleCenterToArcCenter;
            if (distanceIfHoleToArc > distanceIfArcToArc) { distanceFromTriangleCenterToArcCenter = distanceIfArcToArc; }
            else { distanceFromTriangleCenterToArcCenter = distanceIfHoleToArc; }

            Point arcCenter1 = new Point(center.x + distanceFromTriangleCenterToArcCenter * Math.Cos(degreeToRadian(triangleAngle)), center.y + distanceFromTriangleCenterToArcCenter * Math.Sin(degreeToRadian(triangleAngle)), 0);
            Point arcCenter2 = new Point(center.x + distanceFromTriangleCenterToArcCenter * Math.Cos(degreeToRadian(triangleAngle + 120)), center.y + distanceFromTriangleCenterToArcCenter * Math.Sin(degreeToRadian(triangleAngle + 120)), 0);
            Point arcCenter3 = new Point(center.x + distanceFromTriangleCenterToArcCenter * Math.Cos(degreeToRadian(triangleAngle + 240)), center.y + distanceFromTriangleCenterToArcCenter * Math.Sin(degreeToRadian(triangleAngle + 240)), 0);

            Point arcBottom1 = new Point(arcCenter1.x + arcRadius * Math.Cos(degreeToRadian(triangleAngle)), arcCenter1.y + arcRadius * Math.Sin(degreeToRadian(triangleAngle)), 0);
            Point arcBottom2 = new Point(arcCenter2.x + arcRadius * Math.Cos(degreeToRadian(triangleAngle + 120)), arcCenter2.y + arcRadius * Math.Sin(degreeToRadian(triangleAngle + 120)), 0);
            Point arcBottom3 = new Point(arcCenter3.x + arcRadius * Math.Cos(degreeToRadian(triangleAngle + 240)), arcCenter3.y + arcRadius * Math.Sin(degreeToRadian(triangleAngle + 240)), 0);

            Point arcRight1 = new Point(arcCenter1.x + arcRadius * Math.Cos(degreeToRadian(triangleAngle + 60)), arcCenter1.y + arcRadius * Math.Sin(degreeToRadian(triangleAngle + 60)), 0);
            Point arcRight2 = new Point(arcCenter2.x + arcRadius * Math.Cos(degreeToRadian(triangleAngle + 180)), arcCenter2.y + arcRadius * Math.Sin(degreeToRadian(triangleAngle + 180)), 0);
            Point arcRight3 = new Point(arcCenter3.x + arcRadius * Math.Cos(degreeToRadian(triangleAngle + 300)), arcCenter3.y + arcRadius * Math.Sin(degreeToRadian(triangleAngle + 300)), 0);

            Point arcLeft1 = new Point(arcCenter1.x + arcRadius * Math.Cos(degreeToRadian(triangleAngle - 60)), arcCenter1.y + arcRadius * Math.Sin(degreeToRadian(triangleAngle - 60)), 0);
            Point arcLeft2 = new Point(arcCenter2.x + arcRadius * Math.Cos(degreeToRadian(triangleAngle + 60)), arcCenter2.y + arcRadius * Math.Sin(degreeToRadian(triangleAngle + 60)), 0);
            Point arcLeft3 = new Point(arcCenter3.x + arcRadius * Math.Cos(degreeToRadian(triangleAngle + 180)), arcCenter3.y + arcRadius * Math.Sin(degreeToRadian(triangleAngle + 180)), 0);

            skMgr.Create3PointArcFromPoints(arcLeft1, arcRight1, arcBottom1);
            skMgr.Create3PointArcFromPoints(arcLeft2, arcRight2, arcBottom2);
            skMgr.Create3PointArcFromPoints(arcLeft3, arcRight3, arcBottom3);

            skMgr.CreateLineFromPoints(arcLeft1, arcRight3);
            skMgr.CreateLineFromPoints(arcLeft2, arcRight1);
            skMgr.CreateLineFromPoints(arcLeft3, arcRight2);
        }
    }
}

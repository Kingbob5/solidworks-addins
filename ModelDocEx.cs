using System;
using SolidWorks.Interop.sldworks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.swconst;
using static solidworks_addins_3.BraeClasses;
using static solidworks_addins_3.Functions;
using static solidworks_addins_3.SelectingManager;

using static solidworks_addins_3.isoSketchTypeOptions_e;
using System;
using System.Diagnostics;

namespace solidworks_addins_3
{
    public static class ModelDocEx //public functions for the class to access
    {
        public static void CreateBox(this IModelDoc2 model, IEntity reference, double width, double length, double height)
        {
            CreateExtrusion(model, reference, skMgr => skMgr.CreateCenterRectangle(0, 0, 0, width / 2, length / 2, height / 2) != null, height);
        }

        public static void CreateCylinder(this IModelDoc2 model, IEntity reference, double diameter, double height)
        {
            CreateExtrusion(model, reference, skMgr => skMgr.CreateCircleByRadius(0, 0, 0, diameter / 2) != null, height);
        }

        public static void CreateIsogrid(this IModelDoc2 model, IEntity reference, ISketchPoint StartingPoint, IEdge PatternDirection, double MatThickness, double HthDistance, double HoleDiameter, double ArcLength, double CutDepth)
        {
            ISketchManager skMgr = model.SketchManager;

            //create the bounding box
            reference.Select(false); //select only the reference
            ISketch boundSketch = CreateIsoBoundSketch(skMgr, model, reference);

            //get the angle to propogate to
            reference.Select(false);
            double angleOfEdge = getAngleEdge(skMgr, model, PatternDirection); //in radians

            //create the isogrid cutout
            reference.Select(false);
            ISketch holeSketch = CreateIsoSketch((int)isoSketchTypeOptions_e.isoSketchHole, skMgr, reference, StartingPoint, angleOfEdge, MatThickness, HthDistance, HoleDiameter, ArcLength);
            Feature holeCut = CreateCutExtrusionFromSketch(model, reference, holeSketch, CutDepth);

            reference.Select(false);
            ISketch upTriangle = CreateIsoSketch((int)isoSketchTypeOptions_e.isoSketchUpTriangle, skMgr, reference, StartingPoint, angleOfEdge, MatThickness, HthDistance, HoleDiameter, ArcLength);
            Feature upTriangleCut = CreateCutExtrusionFromSketch(model, reference, upTriangle, CutDepth);

            reference.Select(false);
            ISketch downTriangle = CreateIsoSketch((int)isoSketchTypeOptions_e.isoSketchDownTriangle, skMgr, reference, StartingPoint, angleOfEdge, MatThickness, HthDistance, HoleDiameter, ArcLength);
            Feature downTriangleCut = CreateCutExtrusionFromSketch(model, reference, downTriangle, CutDepth);

            //rename some stuff
            Feature boundSketchFeature = (Feature)boundSketch;
            boundSketchFeature.Name = "bounding-sketch";
            holeCut.Name = "hole-cut";
            upTriangleCut.Name = "up-triangle-cut";
            downTriangleCut.Name = "down-triangle-cut";

            SelectionMgr swSelMgr = (SelectionMgr)model.SelectionManager;
            SelectData selData = swSelMgr.CreateSelectData();
            model.ClearSelection2(true);

            //select the features with the right marks for the fill pattern to work
            swSelMgr.AddOjectToSelectionListAndMark(PatternDirection, selData, 1);
            swSelMgr.AddOjectToSelectionListAndMark(boundSketch, selData, 16384);
            swSelMgr.AddOjectToSelectionListAndMark(holeCut, selData, 4);

            //create the hole fill pattern
            Feature holeFillPattern = model.FeatureManager.CreateIsogridFillPattern(HthDistance, MatThickness);
            if (holeFillPattern == null) { throw new Exception("hole fill pattern not created"); }

            //select the features with the right marks for the fill pattern to work
            //TODO, swSelMgr.DeSelect2(3, 4); didnt work but maybe later
            model.ClearSelection2(true);
            swSelMgr.AddOjectToSelectionListAndMark(PatternDirection, selData, 1);
            swSelMgr.AddOjectToSelectionListAndMark(boundSketch, selData, 16384);
            swSelMgr.AddOjectToSelectionListAndMark(upTriangleCut, selData, 4);

            //create the hole fill pattern
            Feature upTriangleFillPattern = model.FeatureManager.CreateIsogridFillPattern(HthDistance, MatThickness);
            if (upTriangleFillPattern == null) { throw new Exception("up triangle fill pattern not created"); }

            //select the features with the right marks for the fill pattern to work
            model.ClearSelection2(true);
            swSelMgr.AddOjectToSelectionListAndMark(PatternDirection, selData, 1);
            swSelMgr.AddOjectToSelectionListAndMark(boundSketch, selData, 16384);
            swSelMgr.AddOjectToSelectionListAndMark(downTriangleCut, selData, 4);

            //create the hole fill pattern
            Feature downTriangleFillPattern = model.FeatureManager.CreateIsogridFillPattern(HthDistance, MatThickness);
            if (downTriangleFillPattern == null) { throw new Exception("down triangle fill pattern not created"); }
            
            holeFillPattern.Name = "Isogrid-Hole-FillPattern";
            upTriangleFillPattern.Name = "Isogrid-UpTriangle-Fill";
            downTriangleFillPattern.Name = "Isogrid-DownTriangle-Fill";
            
            model.ClearSelection2(true);
            swSelMgr.AddOjectToSelectionListAndMark(boundSketch, selData, 0);
            model.BlankSketch();

            model.ClearSelection2(true);
            swSelMgr.AddOjectToSelectionListAndMark(boundSketch, selData, 0);
            swSelMgr.AddOjectToSelectionListAndMark(downTriangleCut, selData, 0);
            swSelMgr.AddOjectToSelectionListAndMark(upTriangleCut, selData, 0);
            swSelMgr.AddOjectToSelectionListAndMark(holeCut, selData, 0);
            swSelMgr.AddOjectToSelectionListAndMark(holeFillPattern, selData, 0);
            swSelMgr.AddOjectToSelectionListAndMark(upTriangleFillPattern, selData, 0);
            swSelMgr.AddOjectToSelectionListAndMark(downTriangleFillPattern, selData, 0);

            Feature folder = model.FeatureManager.InsertFeatureTreeFolder2((int)swFeatureTreeFolderType_e.swFeatureTreeFolder_Containing);
            folder.Name = "isogrid";

            

            model.ClearSelection2(true);
        }

        private static double getAngleEdge(ISketchManager skMgr, IModelDoc2 model, IEdge patternDirection)
        {
            skMgr.InsertSketch(true);
            skMgr.AddToDB = true; //dont go through UI

            model.ClearSelection2(true);
            model.GraphicsRedraw2();

            SelectionMgr swSelMgr = (SelectionMgr)model.SelectionManager;
            SelectData selData = swSelMgr.CreateSelectData();
            swSelMgr.AddSelectionListObject(patternDirection, selData);


            model.GraphicsRedraw2();

            skMgr.SketchUseEdge3(false, false);

            model.GraphicsRedraw2();

            model.ClearSelection2(true);

            double[] lines = (double[])model.GetLines();

            if (lines == null)
            {
                throw new Exception("edge not converted successfully");
            }

            Point sketchLine1 = new Point(lines[1], lines[2], lines[3]);
            Point sketchLine2 = new Point(lines[4], lines[5], lines[6]);

            double angle = getAngleBetween2DPointsInRadians(sketchLine1, sketchLine2);
            angle += degreeToRadian(90);
            //model.SelectFeature(patternDirection, 0);

            skMgr.AddToDB = false;
            skMgr.InsertSketch(true);

            model.EditDelete();

            return angle;
        }



        private static ISketch CreateIsoBoundSketch(ISketchManager skMgr, IModelDoc2 model, IEntity reference)
        {
            skMgr.InsertSketch(true);
            skMgr.AddToDB = true; //dont go through UI

            ISketch sketch = skMgr.ActiveSketch;
            
            if (sketch == null)
            {
                throw new NullReferenceException("Failed to create sketch segment");
            }

            SelectData swSelData = default(SelectData);

            reference.Select4(true, swSelData);
            skMgr.SketchUseEdge3(false, false);

            reference.Select4(true, swSelData);
            skMgr.SketchUseEdge3(false, true);
            //skMgr.SketchUseEdge3(false, false);

            skMgr.AddToDB = false;
            skMgr.InsertSketch(true);

            return sketch;
        }

        private static ISketch CreateIsoSketch(int sketchType, ISketchManager skMgr, IEntity reference, ISketchPoint StartingPoint, double angleOfLine, double MatThickness, double HthDistance, double HoleDiameter, double ArcLength)
        {
            skMgr.InsertSketch(true);
            skMgr.AddToDB = true; //dont go through UI

            var sketch = skMgr.ActiveSketch;

            if (sketch == null)
            {
                throw new NullReferenceException("Failed to create sketch segment");
            }

            Point holeCenter = new Point(StartingPoint.X, StartingPoint.Y, 0);

            if (sketchType != 0 && sketchType != 1 && sketchType != 2)
            {
                throw new Exception("invalid isogrid sketch type");
            }

            angleOfLine = radianToDegree(angleOfLine);

            switch (sketchType)
            {
                case (int)isoSketchTypeOptions_e.isoSketchHole:
                    skMgr.CreateCircleByRadius(holeCenter.x, holeCenter.y, holeCenter.z, HoleDiameter / 2);
                    break;

                case (int)isoSketchTypeOptions_e.isoSketchUpTriangle:
                    skMgr.DrawFilledTriangleInDirectionWithOrientation(holeCenter, MatThickness, HthDistance, HoleDiameter, ArcLength, angleOfLine + 180, angleOfLine);
                    break;

                case (int)isoSketchTypeOptions_e.isoSketchDownTriangle:
                    skMgr.DrawFilledTriangleInDirectionWithOrientation(holeCenter, MatThickness, HthDistance, HoleDiameter, ArcLength, angleOfLine + 120, angleOfLine - 180);
                    break;
            }
            
            skMgr.AddToDB = false;
            skMgr.InsertSketch(true);

            return sketch;
        }

        private static Feature CreateCutExtrusionFromSketch(IModelDoc2 model, IEntity reference, ISketch sketch, double height)
        {
            if (!reference.SelectByMark(false, 0))
            {
                throw new Exception("Failed to select reference");
            }
            model.IActiveView.EnableGraphicsUpdate = false; //stops the graphics from updating until the code finishes running
            model.FeatureManager.EnableFeatureTree = false; //stops the feature tree from flickering until code finishes
            //NOTE: very important that these are renabled after the code finishes running

            try
            {
                if ((sketch as IFeature).Select2(false, 0))
                {
                    //model.GraphicsRedraw2();
                    //var feat = model.FeatureManager.FeatureCut4(true, false, false, (int)swEndConditions_e.swEndCondBlind,
                    //    (int)swEndConditions_e.swEndCondBlind, height, 0, false, false, false, false, 0, 0,
                    //    false, false, false, false, false, false, false, 0, 0, false)
                    Feature feat = model.FeatureManager.FeatureCut4(true, false, false, (int)swEndConditions_e.swEndCondBlind, 0, height, 0, false, false, false,
                        false, 0, 0, false, false, false, false, false, false, true,
                        true, true, false, 0, 0, false, true);

                    if (feat == null)
                    {
                        throw new NullReferenceException("Failed to create extrusion");
                    }

                    model.IActiveView.EnableGraphicsUpdate = true;
                    model.FeatureManager.EnableFeatureTree = true;

                    return feat;
                }
                else
                {
                    throw new Exception("base sketch plane not selected");
                }
            }
            finally //regardless of if there is an exeption or not I want to make sure this code runs
            {
                model.IActiveView.EnableGraphicsUpdate = true;
                model.FeatureManager.EnableFeatureTree = true;
            }
        }

        private static void CreateExtrusion(IModelDoc2 model, IEntity reference, Func<ISketchManager, bool> creator, double height)
        {
            if (!reference.SelectByMark(false, 0))
            {
                throw new Exception("Failed to select reference");
            }
            model.IActiveView.EnableGraphicsUpdate = false; //stops the graphics from updating until the code finishes running
            model.FeatureManager.EnableFeatureTree = false; //stops the feature tree from flickering until code finishes
            //NOTE: very important that these are renabled after the code finishes running

            try
            {
                var sketch = CreateSketch(model, creator);

                if ((sketch as IFeature).Select2(false, 0))
                {
                    var feat = model.FeatureManager.FeatureExtrusion3(true, false, false, (int)swEndConditions_e.swEndCondBlind,
                        (int)swEndConditions_e.swEndCondBlind, height, 0, false, false, false, false, 0, 0,
                        false, false, false, false, false, false, false, 0, 0, false);
                    if (feat == null)
                    {
                        throw new NullReferenceException("Failed to create extrusion");
                    }
                }
                else
                {
                    throw new Exception("base sketch plane not selected");
                }
            }
            finally //regardless of if there is an exeption or not I want to make sure this code runs
            {
                model.IActiveView.EnableGraphicsUpdate = true;
                model.FeatureManager.EnableFeatureTree = true;
            }
        }

        private static ISketch CreateSketch(IModelDoc2 model, Func<ISketchManager, bool> creator)
        {
            var skMgr = model.SketchManager;

            skMgr.InsertSketch(true);
            skMgr.AddToDB = true; //dont go through UI

            var sketch = skMgr.ActiveSketch;

            if (!creator.Invoke(skMgr))
            {
                throw new NullReferenceException("Failed to create sketch segment");
            }

            skMgr.AddToDB = false;
            skMgr.InsertSketch(true);

            return sketch;
        }
    }
}

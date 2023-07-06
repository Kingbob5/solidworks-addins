using System;
using SolidWorks.Interop.sldworks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.swconst;

namespace solidworks_addins_3
{
    public static class ModelDocEx //public functions for the class to access
    {
        public static void CreateBox(this IModelDoc2 model, IEntity reference, double width, double length, double height)
        {
            CreateExtrusion(model, reference, skMgr => skMgr.CreateCenterRectangle(0, 0, 0, width/2, length/2, height/2) != null, height);
        }

        public static void CreateCylinder(this IModelDoc2 model, IEntity reference, double diameter, double height)
        {
            CreateExtrusion(model, reference, skMgr => skMgr.CreateCircleByRadius(0, 0, 0, diameter / 2) != null, height);
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

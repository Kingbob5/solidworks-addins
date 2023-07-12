using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using static solidworks_addins_3.Functions;

namespace solidworks_addins_3
{
    public static class FeatureManagerHandler
    {
        public static Feature CreateIsogridFillPattern(this FeatureManager swFeatMgr, double HthDistance, double MatThickness)
        {
            FillPatternFeatureData swFillPatternData = (FillPatternFeatureData)swFeatMgr.CreateDefinition((int)swFeatureNameID_e.swFmFillPattern);
            swFillPatternData.FeaturesToPatternType = (int)swFeaturesToPatternType_e.swFeaturesToPatternSelectedFeatures;
            swFillPatternData.GeometryPattern = true;
            swFillPatternData.LayoutSpacingType = (int)swPatternLayoutSpacingType_e.swPatternLayoutTargetSpacing;
            swFillPatternData.InstanceSpacing = HthDistance;
            swFillPatternData.PatternLayoutType = (int)swPatternLayoutType_e.swPatternLayoutPerforation;
            swFillPatternData.StaggerAngle = degreeToRadian(60);
            swFillPatternData.Margins = 2 * MatThickness;
            swFillPatternData.Rotation = 0;
            swFillPatternData.SeedCutFlipShapeDirection = false;
            return swFeatMgr.CreateFeature(swFillPatternData);
        }
    }
}

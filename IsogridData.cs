using CodeStack.SwEx.Common.Attributes;
using CodeStack.SwEx.PMPage.Attributes;
using SolidWorks.Interop.swconst;
using solidworks_addins_3.Properties;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidworks_addins_3
{
    [PageOptions(swPropertyManagerPageOptions_e.swPropertyManagerOptions_CancelButton | swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton)]
    [Title(typeof(Resources), nameof(Resources.CommandTitleCreateIsogrid))] //do it this way so if you change the title it changes in all locations its used
    [Icon(typeof(Resources), nameof(Resources.isogrid_icon))]
    public class IsogridData
    {
        public IsogridVariables Variables { get; set; }

        public FaceReference FaceReference { get; set; }

        public PointReference PointReference { get; set; }

        public EdgeReference EdgeReference { get; set; }

    }

    public class EdgeReference
    {
        [SelectionBox(typeof(ReferenceSelectionEdgeFilter), swSelectType_e.swSelDATUMAXES, swSelectType_e.swSelEDGES)]
        [Description("direction to propogate")]
        [ControlOptions(height: 12)]
        [ControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_SelectFace)]
        public IEdge Edge { get; set; }
    }

    public class PointReference
    {
        [SelectionBox(typeof(ReferenceSelectionSketchPointFilter), swSelectType_e.swSelSKETCHPOINTS, swSelectType_e.swSelEXTSKETCHPOINTS)]
        [Description("starting point for hole")]
        [ControlOptions(height: 12)]
        [ControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_SelectFace)]
        public ISketchPoint Point { get; set; }
    }

    public class FaceReference
    {
        [SelectionBox(typeof(ReferenceSelectionFeatureFilter), swSelectType_e.swSelDATUMPLANES, swSelectType_e.swSelFACES)]
        [Description("Base face or plane")]
        [ControlOptions(height: 12)]
        [ControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_SelectFace)]
        public IEntity Face { get; set; }
    }

    public class IsogridVariables
    {
        [Description("Material Thickness")]
        [ControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_Width)]
        [NumberBoxOptions(swNumberboxUnitType_e.swNumberBox_Length, 0, 1000, 0.01, false, 0.1, 0.001, swPropMgrPageNumberBoxStyle_e.swPropMgrPageNumberBoxStyle_NoScrollArrows)]
        public double MatThickness { get; set; }

        [Description("Hole to Hole Distance")]
        [ControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_LinearDistance)]
        [NumberBoxOptions(swNumberboxUnitType_e.swNumberBox_Length, 0, 1000, 0.01, false, 0.1, 0.001, swPropMgrPageNumberBoxStyle_e.swPropMgrPageNumberBoxStyle_NoScrollArrows)]
        public double HthDistance { get; set; }

        [Description("Hole Diameter")]
        [ControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_LinearDistance)]
        [NumberBoxOptions(swNumberboxUnitType_e.swNumberBox_Length, 0, 1000, 0.01, false, 0.1, 0.001, swPropMgrPageNumberBoxStyle_e.swPropMgrPageNumberBoxStyle_NoScrollArrows)]
        public double HoleDiameter { get; set; }

        [Description("Arc Radius")]
        [ControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_LinearDistance)]
        [NumberBoxOptions(swNumberboxUnitType_e.swNumberBox_Length, 0, 1000, 0.01, false, 0.1, 0.001, swPropMgrPageNumberBoxStyle_e.swPropMgrPageNumberBoxStyle_NoScrollArrows)]
        public double ArcRadius { get; set; }

        [Description("Cut Depth")]
        [ControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_LinearDistance)]
        [NumberBoxOptions(swNumberboxUnitType_e.swNumberBox_Length, 0, 1000, 0.01, false, 0.1, 0.001, swPropMgrPageNumberBoxStyle_e.swPropMgrPageNumberBoxStyle_NoScrollArrows)]
        public double CutDepth { get; set; }
    }
}

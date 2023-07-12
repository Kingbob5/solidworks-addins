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
    [Title(typeof(Resources), nameof(Resources.CommandTitleCreateBox))] //do it this way so if you change the title it changes in all locations its used
    [Icon(typeof(Resources), nameof(Resources.cylinder_icon))]
    public class BoxData
    {
        [SelectionBox(typeof(ReferenceSelectionFeatureFilter), swSelectType_e.swSelDATUMPLANES, swSelectType_e.swSelFACES)]
        [Description("Base face or plane")]
        [ControlOptions(height: 12)]
        [ControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_SelectFace)]
        public IEntity Reference { get; set; }

        [Description("Width of Box")]
        [ControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_Width)]
        [NumberBoxOptions(swNumberboxUnitType_e.swNumberBox_Length, 0, 1000, 0.01, false, 0.1, 0.001, swPropMgrPageNumberBoxStyle_e.swPropMgrPageNumberBoxStyle_NoScrollArrows)]
        public double Width { get; set; }

        [Description("Length of Box")]
        [ControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_LinearDistance)]
        [NumberBoxOptions(swNumberboxUnitType_e.swNumberBox_Length, 0, 1000, 0.01, false, 0.1, 0.001, swPropMgrPageNumberBoxStyle_e.swPropMgrPageNumberBoxStyle_NoScrollArrows)]
        public double Length { get; set; }

        [Description("Height of Box")]
        [ControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_LinearDistance)]
        [NumberBoxOptions(swNumberboxUnitType_e.swNumberBox_Length, 0, 1000, 0.01, false, 0.1, 0.001, swPropMgrPageNumberBoxStyle_e.swPropMgrPageNumberBoxStyle_Thumbwheel)]
        public double Height { get; set; }
    }
}

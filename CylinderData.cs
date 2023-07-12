using CodeStack.SwEx.PMPage.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.sldworks;
using CodeStack.SwEx.Common.Attributes;
using solidworks_addins_3.Properties;
using System.ComponentModel;

namespace solidworks_addins_3
{
    [PageOptions(swPropertyManagerPageOptions_e.swPropertyManagerOptions_CancelButton | swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton)]
    [Title(typeof(Resources), nameof(Resources.CommandTitleCreateCylinder))] //do it this way so if you change the title it changes in all locations its used
    [Icon(typeof(Resources), nameof(Resources.cylinder_icon))]
    
    public class CylinderData
    {
        [SelectionBox(typeof(ReferenceSelectionFeatureFilter), swSelectType_e.swSelDATUMPLANES, swSelectType_e.swSelFACES)]
        [Description("Base face or plane")]
        [ControlOptions(height: 12)]
        [ControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_SelectFace)]
        public IEntity Reference { get; set; }

        [Description("Diameter of Cylinder")]
        [ControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_Diameter)]
        [NumberBoxOptions(swNumberboxUnitType_e.swNumberBox_Length, 0,1000,0.01, false, 0.1,0.001, swPropMgrPageNumberBoxStyle_e.swPropMgrPageNumberBoxStyle_NoScrollArrows)]
        public double Diameter { get; set; }

        [Description("Height of Cylinder")]
        [Icon(typeof(Resources), nameof(Resources.toolbar_icon))]
        [NumberBoxOptions(swNumberboxUnitType_e.swNumberBox_Length, 0, 1000, 0.01, false, 0.1, 0.001, swPropMgrPageNumberBoxStyle_e.swPropMgrPageNumberBoxStyle_NoScrollArrows)]
        public double Height { get; set; }
    }
}

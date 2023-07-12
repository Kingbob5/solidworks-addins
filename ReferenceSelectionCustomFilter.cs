using CodeStack.SwEx.PMPage.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.sldworks;
using CodeStack.SwEx.PMPage.Controls;

namespace solidworks_addins_3
{
    public class ReferenceSelectionFeatureFilter : SelectionCustomFilter<IEntity>
    {

        protected override bool Filter(IPropertyManagerPageControlEx selBox, IEntity selection, swSelectType_e selType, ref string itemText)
        {

            if (selType == swSelectType_e.swSelFACES)
            {
                var face = selection as IFace2;

                if (!face.IGetSurface().IsPlane())
                {
                    return false;
                }
            }
            return true;
        }
    }

    public class ReferenceSelectionSketchPointFilter : SelectionCustomFilter<ISketchPoint>
    {

    }

    public class ReferenceSelectionEdgeFilter : SelectionCustomFilter<IEdge>
    {
        
    }
}

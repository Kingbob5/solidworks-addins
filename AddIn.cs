using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.ComponentModel;
using solidworks_addins_3.Properties;
using CodeStack.SwEx.AddIn;
using CodeStack.SwEx;
using CodeStack.SwEx.AddIn.Attributes;
using CodeStack.SwEx.Common.Attributes;
using CodeStack.SwEx.AddIn.Enums;
using CodeStack.SwEx.PMPage;
using static solidworks_addins_3.Functions;

namespace solidworks_addins_3
{
    [ComVisible(true), Guid("D150233B-2429-4E92-98C8-7075950DA690")]
    [AutoRegister("Brae's Features", "extra description")]


    public class AddIn : SwAddInEx
    {
        [Title("Brae's Features")]
        [Description("Features by Brae to save you time and make you happy")]
        [Icon(typeof(Resources), nameof(Resources.toolbar_icon))]
        private enum Commands_e //this makes it very easy to add and remove entities
        {
            [Title("Isogrid")]
            [Description("makes isogrid")]
            [Icon(typeof(Resources), nameof(Resources.isogrid_icon))]
            [CommandItemInfo(true, true, swWorkspaceTypes_e.Part, true)]
            Isogrid,

            [Title(typeof(Resources), nameof(Resources.CommandTitleCreateCylinder))] //do it this way so if you change the title it changes in all locations its used
            [Description("makes a cylinder where you tell it to")]
            [Icon(typeof(Resources), nameof(Resources.cylinder_icon))]
            [CommandItemInfo(true, true, swWorkspaceTypes_e.Part, true)]
            CreateCylinder,


            [Title(typeof(Resources), nameof(Resources.CommandTitleCreateBox))] //do it this way so if you change the title it changes in all locations its used
            [Icon(typeof(Resources), nameof(Resources.cylinder_icon))]
            [CommandItemInfo(true, true, swWorkspaceTypes_e.Part, true)]
            CreateBox,

            OtherFeatures
        }

        private PropertyManagerPageEx<PropertyPageHandler, CylinderData> m_CylPmPage;
        private PropertyManagerPageEx<PropertyPageHandler, BoxData> m_BoxPmPage;
        private PropertyManagerPageEx<PropertyPageHandler, IsogridData> m_IsoPmPage;

        private CylinderData m_CylData;
        private BoxData m_BoxData;
        private IsogridData m_IsoData;

        public override bool OnConnect() //this registers these features with solidworks when it is opened
        {
            AddCommandGroup<Commands_e>(OnButtonClick, OnButtonEnable);

            m_CylPmPage = new PropertyManagerPageEx<PropertyPageHandler, CylinderData>(App);
            m_CylPmPage.Handler.Closing += OnCylPageClosing;
            m_CylPmPage.Handler.Closed += OnCylPageClose;

            m_CylData = new CylinderData() //add some defauly cylinder data
            {
                Diameter = 0.01,
                Height = 0.02
            };

            m_BoxPmPage = new PropertyManagerPageEx<PropertyPageHandler, BoxData>(App);
            m_BoxPmPage.Handler.Closing += OnBoxPageClosing;
            m_BoxPmPage.Handler.Closed += OnBoxPageClose;

            m_BoxData = new BoxData() //add some defauly cylinder data
            {
                Width = Functions.InToM(10),
                Length = Functions.InToM(10),
                Height = Functions.InToM(1)
            };

            m_IsoPmPage = new PropertyManagerPageEx<PropertyPageHandler, IsogridData>(App);
            m_IsoPmPage.Handler.Closing += OnIsoPageClosing;
            m_IsoPmPage.Handler.Closed += OnIsoPageClose;

            m_IsoData = new IsogridData() //add some defauly cylinder data
            {
                Variables = new IsogridVariables()
                {
                    MatThickness = Functions.InToM(0.09),
                    HoleDiameter = Functions.InToM(0.18),
                    HthDistance = Functions.InToM(1),
                    ArcRadius = Functions.InToM(0.09),
                    CutDepth = Functions.InToM(.1)
                }
            };

            return true;
        }

        private void OnCylPageClosing(swPropertyManagerPageCloseReasons_e reason, 
            CodeStack.SwEx.PMPage.Base.ClosingArg arg)
        {
            //ValidateReferences(m_CylData.Reference, reason, arg);
        }

        private void OnCylPageClose(swPropertyManagerPageCloseReasons_e reason)
        {
            if (reason == swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Okay)
            {
                try
                {
                    //CreateCylinder()
                    App.IActiveDoc2.CreateCylinder(m_CylData.Reference, m_CylData.Diameter, m_CylData.Height);
                    //called when the property manager page is closed (because the check is pressed would be the reason)
                }
                catch (Exception ex)
                {
                    App.SendMsgToUser2(ex.Message, (int)swMessageBoxIcon_e.swMbStop, (int)swMessageBoxBtn_e.swMbOk);
                }
            }
        }

        private void OnBoxPageClosing(swPropertyManagerPageCloseReasons_e reason,
            CodeStack.SwEx.PMPage.Base.ClosingArg arg)
        {
            //ValidateReferences(m_BoxData.Reference, reason, arg);
        }

        private void OnBoxPageClose(swPropertyManagerPageCloseReasons_e reason)
        {
            if (reason == swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Okay)
            {
                try
                {
                    //CreateCylinder()
                    App.IActiveDoc2.CreateBox(m_BoxData.Reference, m_BoxData.Width, m_BoxData.Length, m_BoxData.Height);
                    //called when the property manager page is closed (because the check is pressed would be the reason)
                }
                catch (Exception ex)
                {
                    App.SendMsgToUser2(ex.Message, (int)swMessageBoxIcon_e.swMbStop, (int)swMessageBoxBtn_e.swMbOk);
                }
            }
        }

        private void OnIsoPageClosing(swPropertyManagerPageCloseReasons_e reason,
            CodeStack.SwEx.PMPage.Base.ClosingArg arg)
        {
            ValidateReferences(m_IsoData, reason, arg);
        }

        private void OnIsoPageClose(swPropertyManagerPageCloseReasons_e reason)
        {
            if (reason == swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Okay)
            {
                try
                {
                    //CreateCylinder()
                    App.IActiveDoc2.CreateIsogrid(m_IsoData.FaceReference.Face, m_IsoData.PointReference.Point, m_IsoData.EdgeReference.Edge, m_IsoData.Variables.MatThickness, m_IsoData.Variables.HthDistance, m_IsoData.Variables.HoleDiameter, m_IsoData.Variables.ArcRadius, m_IsoData.Variables.CutDepth);
                    //called when the property manager page is closed (because the check is pressed would be the reason)
                }
                catch (Exception ex)
                {
                    App.SendMsgToUser2(ex.Message, (int)swMessageBoxIcon_e.swMbStop, (int)swMessageBoxBtn_e.swMbOk);
                }
            }
        }

       

        private void OnButtonClick(Commands_e cmd) //when a button is clicked it will try to run these features and if they fail for some reason the exeption is caught
        {
            try
            {
                switch (cmd)
                {
                    case Commands_e.Isogrid:
                        m_IsoPmPage.Show(m_IsoData);
                        break;

                    case Commands_e.CreateCylinder:
                        m_CylPmPage.Show(m_CylData); //open the property manager
                        //ModelDocEx.CreateCylinder(App.IActiveDoc2, 0.01, 0.01);
                        break;

                    case Commands_e.CreateBox:
                        m_BoxPmPage.Show(m_BoxData);
                        //App.IActiveDoc2.CreateBox(0.01, 0.01, 0.01);
                        break;

                    case Commands_e.OtherFeatures:
                        //TODO: add more features...
                        break;
                }
            }
            catch (Exception ex)
            {
                App.SendMsgToUser2(ex.Message, (int)swMessageBoxIcon_e.swMbStop, (int)swMessageBoxBtn_e.swMbOk);
            }
        }

        private void OnButtonEnable(Commands_e cmd, ref CommandItemEnableState_e state) //this checks for if the button for each feature should be enabled or not
        {
            switch(cmd)
            {
                case Commands_e.Isogrid:
                case Commands_e.CreateCylinder:
                case Commands_e.CreateBox:
                    //nesting them like this means the same code runs for all of them
                    //case Commands_e.OtherFeatures:

                    var model = App.IActiveDoc2;

                    state = CommandItemEnableState_e.DeselectDisable; //diabled by default

                    if (model is PartDoc)
                    {
                        state = CommandItemEnableState_e.DeselectEnable;
                        /*var selType = (swSelectType_e)model.ISelectionManager.GetSelectedObjectType3(1, -1);

                        if (selType == swSelectType_e.swSelDATUMPLANES)
                        {
                            state = CommandItemEnableState_e.DeselectEnable;
                        }
                        else if (selType == swSelectType_e.swSelFACES)
                        {
                            var face = model.ISelectionManager.GetSelectedObject6(1, -1) as IFace2;

                            if (face.IGetSurface().IsPlane())
                            {
                                state = CommandItemEnableState_e.DeselectEnable;
                            }
                        }*/
                    }
                    break;
                
            }
        }

        private void ValidateReferences(IsogridData data, swPropertyManagerPageCloseReasons_e reason, CodeStack.SwEx.PMPage.Base.ClosingArg arg)
        {
            if (reason == swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Okay)
            {
                if (data.FaceReference.Face == null)
                {
                    arg.ErrorMessage = "select face reference";
                    arg.Cancel = true;
                }
                if (data.EdgeReference.Edge == null)
                {
                    arg.ErrorMessage = "select edge reference";
                    arg.Cancel = true;
                }
                if (data.PointReference.Point == null)
                {
                    arg.ErrorMessage = "select point reference";
                    arg.Cancel = true;
                }
                if (InvalidGeometry(data))
                {
                    arg.ErrorMessage = "invalid geometry (arc lengths too long or hth distance too short)";
                    arg.Cancel = true;
                }
            }
        }

        public bool InvalidGeometry(IsogridData data)
        {
            //the defining dimension could either be the hold to the arc thickness being 2*matThickness or it could be arc to arc so check both and pick which one is right
            double distanceIfHoleToArc = ((data.Variables.HthDistance / 2) / Math.Cos(degreeToRadian(30))) - (data.Variables.HoleDiameter / 2 + 2 * data.Variables.MatThickness + data.Variables.ArcRadius);
            double distanceIfArcToArc = ((data.Variables.HthDistance / 2) / Math.Cos(degreeToRadian(30))) - (2 * data.Variables.ArcRadius + 2 * data.Variables.MatThickness);

            double distanceFromTriangleCenterToArcCenter;
            if (distanceIfHoleToArc > distanceIfArcToArc) { distanceFromTriangleCenterToArcCenter = distanceIfArcToArc; }
            else { distanceFromTriangleCenterToArcCenter = distanceIfHoleToArc; }

            if (distanceFromTriangleCenterToArcCenter <=0)
            { 
                return true; 
            }
            return false;
        }
    }
}

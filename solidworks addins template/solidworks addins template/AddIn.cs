using CodeStack.SwEx.AddIn.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CodeStack.SwEx.AddIn;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using CodeStack.SwEx.AddIn.Enums;
using CodeStack.SwEx.Common.Attributes;
using System.ComponentModel;
using solidworks_addins_3.Properties;
using CodeStack.SwEx.PMPage;
using CodeStack.SwEx.PMPage.Base;

namespace solidworks_addins_3
{
    [ComVisible(true), Guid("D150233B-2429-4E92-98C8-7075950DA690")]
    [AutoRegister("Brae's Features", "Custom Features by Brae to save you time and convice you to buy him snacks")]

    public class AddIn : SwAddInEx
    {
        [Title("Brae's Features")]
        [Description("Features by Brae to save you time and make you happy")]
        [Icon(typeof(Resources), nameof(Resources.toolbar_icon))]
        private enum Commands_e //this makes it very easy to add and remove entities
        {
            [Title("Isogrid")]
            [Description("makes isogrid")]
            [Icon(typeof(Resources), nameof(Resources.isogrid))]
            [CommandItemInfo(true, true, swWorkspaceTypes_e.Part, true)]
            Isogrid,

            [Title(typeof(Resources), nameof(Resources.CommandTitleCreateCylinder))] //do it this way so if you change the title it changes in all locations its used
            [Description("makes a cylinder where you tell it to")]
            [Icon(typeof(Resources), nameof(Resources.cylinder))]
            [CommandItemInfo(true, true, swWorkspaceTypes_e.Part, true)]
            CreateCylinder,


            [Title(typeof(Resources), nameof(Resources.CommandTitleCreateBox))] //do it this way so if you change the title it changes in all locations its used
            [Icon(typeof(Resources), nameof(Resources.cylinder))]
            [CommandItemInfo(true, true, swWorkspaceTypes_e.Part, true)]
            CreateBox,

            OtherFeatures
        }

        private PropertyManagerPageEx<PropertyPageHandler, CylinderData> m_CylPmPage;
        private PropertyManagerPageEx<PropertyPageHandler, BoxData> m_BoxPmPage;
        private CylinderData m_CylData;
        private BoxData m_BoxData;

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
                Width = 0.01,
                Length = 0.01,
                Height = 0.01
            };

            return true;
        }

        private void OnCylPageClosing(swPropertyManagerPageCloseReasons_e reason, 
            CodeStack.SwEx.PMPage.Base.ClosingArg arg)
        {
            ValidateReference(m_CylData.Reference, reason, arg);
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
            ValidateReference(m_BoxData.Reference, reason, arg);
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

        private void OnButtonClick(Commands_e cmd) //when a button is clicked it will try to run these features and if they fail for some reason the exeption is caught
        {
            try
            {
                switch (cmd)
                {
                    case Commands_e.Isogrid:
                        //create isogrid
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

        private void ValidateReference(IEntity reference, swPropertyManagerPageCloseReasons_e reason, CodeStack.SwEx.PMPage.Base.ClosingArg arg)
        {
            if (reason == swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Okay)
            {
                if (m_CylData.Reference == null)
                {
                    arg.ErrorMessage = "select reference";
                    arg.Cancel = true;
                }
            }
        }
    }
}

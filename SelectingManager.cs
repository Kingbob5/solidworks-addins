using System;
using SolidWorks.Interop.sldworks;
using System.Diagnostics;
using SolidWorks.Interop.swconst;


namespace solidworks_addins_3
{
    public static class SelectingManager
    {
        public static void AddOjectToSelectionListAndMark(this SelectionMgr swSelMgr, object ObjectToAdd, SelectData selData, int mark = 0)
        {
            swSelMgr.AddSelectionListObject(ObjectToAdd, selData); //add to sel data
            swSelMgr.SetSelectedObjectMark(swSelMgr.GetSelectedObjectCount2(-1), mark, (int)swSelectionMarkAction_e.swSelectionMarkSet); //mark it with the mark
        }
        public static void SelectFeature(this IModelDoc2 model, IFeature feature, int mark = 0)
        {
            string featType = null;
            string featName = feature.GetNameForSelection(out featType);
            model.Extension.SelectByID2(featName, featType, 0, 0, 0, true, mark, null, 0);
        }


        private static double[] GetFaceNormalAtMidCoEdge(CoEdge swCoEdge)
        {
            Face2 swFace = default(Face2);
            Surface swSurface = default(Surface);
            Loop2 swLoop = default(Loop2);
            double[] varParams = null;
            double[] varPoint = null;
            double dblMidParam = 0;
            double[] dblNormal = new double[3];
            bool bFaceSenseReversed = false;

            varParams = (double[])swCoEdge.GetCurveParams();
            if (varParams[6] > varParams[7])
            {
                dblMidParam = (varParams[6] - varParams[7]) / 2 + varParams[7];
            }
            else
            {
                dblMidParam = (varParams[7] - varParams[6]) / 2 + varParams[6];
            }
            varPoint = (double[])swCoEdge.Evaluate(dblMidParam);

            // Get the face of the given coedge
            // Check for the sense of the face
            swLoop = (Loop2)swCoEdge.GetLoop();
            swFace = (Face2)swLoop.GetFace();
            swSurface = (Surface)swFace.GetSurface();
            bFaceSenseReversed = swFace.FaceInSurfaceSense();
            varParams = (double[])swSurface.EvaluateAtPoint(varPoint[0], varPoint[1], varPoint[2]);
            if (bFaceSenseReversed)
            {
                // Negate the surface normal as it is opposite from the face normal
                dblNormal[0] = -varParams[0];
                dblNormal[1] = -varParams[1];
                dblNormal[2] = -varParams[2];
            }
            else
            {
                dblNormal[0] = varParams[0];
                dblNormal[1] = varParams[1];
                dblNormal[2] = varParams[2];
            }
            return dblNormal;

        }

        private static double[] GetTangentAtMidCoEdge(CoEdge swCoEdge)
        {
            double[] varParams = null;
            double dblMidParam = 0;
            double[] dblTangent = new double[3];
            varParams = (double[])swCoEdge.GetCurveParams();
            if (varParams[6] > varParams[7])
            {
                dblMidParam = (varParams[6] - varParams[7]) / 2.0 + varParams[7];
            }
            else
            {
                dblMidParam = (varParams[7] - varParams[6]) / 2.0 + varParams[6];
            }
            varParams = (double[])swCoEdge.Evaluate(dblMidParam);
            dblTangent[0] = varParams[3];
            dblTangent[1] = varParams[4];
            dblTangent[2] = varParams[5];
            return dblTangent;

        }

        private static double[] GetCrossProduct(double[] varVec1, double[] varVec2)
        {
            double[] dblCross = new double[3];

            dblCross[0] = varVec1[1] * varVec2[2] - varVec1[2] * varVec2[1];
            dblCross[1] = varVec1[2] * varVec2[0] - varVec1[0] * varVec2[2];
            dblCross[2] = varVec1[0] * varVec2[1] - varVec1[1] * varVec2[0];
            return dblCross;

        }

        private static bool VectorsAreEqual(double[] varVec1, double[] varVec2)
        {
            bool functionReturnValue = false;
            double dblMag = 0;
            double dblDot = 0;
            double[] dblUnit1 = new double[3];
            double[] dblUnit2 = new double[3];
            dblMag = Math.Pow((varVec1[0] * varVec1[0] + varVec1[1] * varVec1[1] + varVec1[2] * varVec1[2]), 0.5);
            dblUnit1[0] = varVec1[0] / dblMag;
            dblUnit1[1] = varVec1[1] / dblMag;
            dblUnit1[2] = varVec1[2] / dblMag;
            dblMag = Math.Pow((varVec2[0] * varVec2[0] + varVec2[1] * varVec2[1] + varVec2[2] * varVec2[2]), 0.5);
            dblUnit2[0] = varVec2[0] / dblMag;
            dblUnit2[1] = varVec2[1] / dblMag;
            dblUnit2[2] = varVec2[2] / dblMag;
            dblDot = dblUnit1[0] * dblUnit2[0] + dblUnit1[1] * dblUnit2[1] + dblUnit1[2] * dblUnit2[2];
            dblDot = Math.Abs(dblDot - 1.0);
            // Compare within a tolerance
            //1.0e-10
            if (dblDot < 1E-10)
            {
                functionReturnValue = true;
            }
            else
            {
                functionReturnValue = false;
            }
            return functionReturnValue;

        }

        public static void SelectHoleEdges(Face2 swFace, SelectData swSelData)
        {
            Loop2 swThisLoop = default(Loop2);
            CoEdge swThisCoEdge = default(CoEdge);
            CoEdge swPartnerCoEdge = default(CoEdge);
            Entity swEntity = default(Entity);
            double[] varThisNormal = null;
            double[] varPartnerNormal = null;
            double[] varCrossProduct = null;
            double[] varTangent = null;
            object[] vEdgeArr = null;
            Edge swEdge = default(Edge);
            Curve swCurve = default(Curve);
            bool bRet = true;
            int count = 0;
            bool bCount = true;

            swThisLoop = (Loop2)swFace.GetFirstLoop();
            while ((swThisLoop != null))
            {
                // Hole is inner loop
                // Circular or elliptical hole has only one edge
                bRet = swThisLoop.IsOuter();
                count = swThisLoop.GetEdgeCount();
                if (count != 1)
                {
                    bCount = false;
                }
                else
                {
                    bCount = true;
                }
                if ((bRet == false) && (bCount == true))
                {
                    swThisCoEdge = (CoEdge)swThisLoop.GetFirstCoEdge();
                    swPartnerCoEdge = (CoEdge)swThisCoEdge.GetPartner();
                    varThisNormal = (double[])GetFaceNormalAtMidCoEdge(swThisCoEdge);
                    varPartnerNormal = (double[])GetFaceNormalAtMidCoEdge(swPartnerCoEdge);
                    if (!VectorsAreEqual(varThisNormal, varPartnerNormal))
                    {
                        // There is a sufficient change between the two faces to determine
                        // what kind of transition is being made
                        varCrossProduct = (double[])GetCrossProduct(varThisNormal, varPartnerNormal);
                        varTangent = (double[])GetTangentAtMidCoEdge(swThisCoEdge);
                        if (VectorsAreEqual(varCrossProduct, varTangent))
                        {
                            // Hole
                            vEdgeArr = (object[])swThisLoop.GetEdges();
                            Debug.Assert(0 < vEdgeArr.Length);
                            swEdge = (Edge)vEdgeArr[0];
                            swCurve = (Curve)swEdge.GetCurve();
                            // Ignore elliptical holes
                            if (swCurve.IsCircle())
                            {
                                swEntity = (Entity)swEdge;
                                bRet = swEntity.Select4(true, swSelData);
                                Debug.Assert(bRet);
                            }
                        }
                    }
                }


                // Move on to the next
                swThisLoop = (Loop2)swThisLoop.GetNext();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace UnityEngine.UI
{
    /// <summary>
    /// Dynamic material class makes it possible to create custom materials on the fly on a per-Graphic basis,
    /// and still have them get cleaned up correctly.
    /// </summary>
    public static class StencilMaterial
    {
        private class MatEntry
        {
            public Material baseMat;
            public Material customMat;
            public int count;

            public int stencilId;
            public StencilOp operation = StencilOp.Keep;
            public CompareFunction compareFunction = CompareFunction.Always;
            public int readMask;
            public int writeMask;
            public bool useAlphaClip;
            public ColorWriteMask colorMask;
        }

        private static List<MatEntry> m_List = new List<MatEntry>();

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [Obsolete("Use Material.Add instead.", true)]
        public static Material Add(Material baseMat, int stencilID) { return null; }

        /// <summary>
        /// Add a new material using the specified base and stencil ID.
        /// </summary>
        //! UnityEngine.UI.Mask.GetModifiedMaterial 中的调用都是传的 UnityEngine.Rendering.CompareFunction.Always
        public static Material Add(Material baseMat, int stencilID, StencilOp operation, CompareFunction compareFunction, ColorWriteMask colorWriteMask)
        {
            return Add(baseMat, stencilID, operation, compareFunction, colorWriteMask, 255, 255);
        }

        /// <summary>
        /// Add a new material using the specified base and stencil ID.
        /// </summary>
        //! 核心方法
        //! UnityEngine.UI.Mask.GetModifiedMaterial 中的调用都是传的 UnityEngine.Rendering.CompareFunction.Equal
        //! 在UnityEngine.UI.MaskableGraphic.GetModifiedMaterial:  StencilOp.Keep, CompareFunction.Equal, ColorWriteMask.All
        public static Material Add(Material baseMat, int stencilID, StencilOp operation, CompareFunction compareFunction, ColorWriteMask colorWriteMask, int readMask, int writeMask)
        {
            if ((stencilID <= 0 && colorWriteMask == ColorWriteMask.All) || baseMat == null)
                return baseMat;

            #region 关于 baseMat(material) 的检查

            if (!baseMat.HasProperty("_Stencil"))
            {
                Debug.LogWarning("Material " + baseMat.name + " doesn't have _Stencil property", baseMat);
                return baseMat;
            }
            if (!baseMat.HasProperty("_StencilOp"))
            {
                Debug.LogWarning("Material " + baseMat.name + " doesn't have _StencilOp property", baseMat);
                return baseMat;
            }
            if (!baseMat.HasProperty("_StencilComp"))
            {
                Debug.LogWarning("Material " + baseMat.name + " doesn't have _StencilComp property", baseMat);
                return baseMat;
            }
            if (!baseMat.HasProperty("_StencilReadMask"))
            {
                Debug.LogWarning("Material " + baseMat.name + " doesn't have _StencilReadMask property", baseMat);
                return baseMat;
            }
            if (!baseMat.HasProperty("_StencilWriteMask"))
            {
                Debug.LogWarning("Material " + baseMat.name + " doesn't have _StencilWriteMask property", baseMat);
                return baseMat;
            }
            if (!baseMat.HasProperty("_ColorMask"))
            {
                Debug.LogWarning("Material " + baseMat.name + " doesn't have _ColorMask property", baseMat);
                return baseMat;
            }

            #endregion

            #region  m_List 已经有了 计数+1,返回

            for (int i = 0; i < m_List.Count; ++i)
            {
                MatEntry ent = m_List[i];

                if (ent.baseMat == baseMat
                    && ent.stencilId == stencilID
                    && ent.operation == operation
                    && ent.compareFunction == compareFunction
                    && ent.readMask == readMask
                    && ent.writeMask == writeMask
                    && ent.colorMask == colorWriteMask)
                {
                    ++ent.count;
                    return ent.customMat;
                }
            }

            #endregion

            var newEnt = new MatEntry();
            newEnt.count = 1;
            newEnt.baseMat = baseMat;
            newEnt.customMat = new Material(baseMat);
            newEnt.customMat.hideFlags = HideFlags.HideAndDontSave;
            newEnt.stencilId = stencilID;
            newEnt.operation = operation;
            newEnt.compareFunction = compareFunction;
            newEnt.readMask = readMask;
            newEnt.writeMask = writeMask;
            newEnt.colorMask = colorWriteMask;
            newEnt.useAlphaClip = operation != StencilOp.Keep && writeMask > 0;

            //! 魔改 customMat 里面的值
            newEnt.customMat.name = string.Format("Stencil Id:{0}, Op:{1}, Comp:{2}, WriteMask:{3}, ReadMask:{4}, ColorMask:{5} AlphaClip:{6} ({7})", stencilID, operation, compareFunction, writeMask, readMask, colorWriteMask, newEnt.useAlphaClip, baseMat.name);

            newEnt.customMat.SetInt("_Stencil", stencilID);
            newEnt.customMat.SetInt("_StencilOp", (int)operation);
            newEnt.customMat.SetInt("_StencilComp", (int)compareFunction);
            newEnt.customMat.SetInt("_StencilReadMask", readMask);
            newEnt.customMat.SetInt("_StencilWriteMask", writeMask);
            newEnt.customMat.SetInt("_ColorMask", (int)colorWriteMask);
            newEnt.customMat.SetInt("_UseUIAlphaClip", newEnt.useAlphaClip ? 1 : 0);

            if (newEnt.useAlphaClip)
                newEnt.customMat.EnableKeyword("UNITY_UI_ALPHACLIP");
            else
                newEnt.customMat.DisableKeyword("UNITY_UI_ALPHACLIP");

            m_List.Add(newEnt);
            return newEnt.customMat;
        }

        /// <summary>
        /// Remove an existing material, automatically cleaning it up if it's no longer in use.
        /// </summary>
        public static void Remove(Material customMat)
        {
            if (customMat == null)
                return;

            for (int i = 0; i < m_List.Count; ++i)
            {
                MatEntry ent = m_List[i];

                if (ent.customMat != customMat)
                    continue;

                if (--ent.count == 0)
                {
                    Misc.DestroyImmediate(ent.customMat);
                    ent.baseMat = null;
                    m_List.RemoveAt(i);
                }
                return;
            }
        }

        public static void ClearAll()
        {
            for (int i = 0; i < m_List.Count; ++i)
            {
                MatEntry ent = m_List[i];

                Misc.DestroyImmediate(ent.customMat);
                ent.baseMat = null;
            }
            m_List.Clear();
        }
    }
}

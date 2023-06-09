using System;

namespace UnityEngine.UI
{
    /// <summary>
    ///   This element is capable of being masked out.
    /// </summary>
    public interface IMaskable //! 配合 UnityEngine.UI.MaskUtilities.NotifyStencilStateChanged 使用
    {
        /// <summary>
        /// Recalculate masking for this element and all children elements.
        /// </summary>
        /// <remarks>
        /// Use this to update the internal state (recreate materials etc).
        /// </remarks>
        void RecalculateMasking();
    }
}

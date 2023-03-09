namespace UnityEngine.UI
{
    /// <summary>
    ///   Interface which allows for the modification of the Material used to render a Graphic before they are passed to the CanvasRenderer.
    /// </summary>
    /// <remarks>
    /// When a Graphic sets a material is is passed (in order) to any components on the GameObject that implement IMaterialModifier. This component can modify the material to be used for rendering.
    /// </remarks>
    public interface IMaterialModifier
    {
        /// <summary>
        /// Perform material modification in this function.
        /// </summary>
        /// <param name="baseMaterial">The material that is to be modified</param>
        /// <returns>The modified material.</returns>
        //! UnityEngine.CanvasRenderer.SetMaterial 调用传参的时候会被调用
        //! 最终生效处: UnityEngine.UI.Graphic.UpdateMaterial
        Material GetModifiedMaterial(Material baseMaterial);
    }
}

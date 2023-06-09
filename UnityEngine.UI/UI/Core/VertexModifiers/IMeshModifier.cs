using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [Obsolete("Use IMeshModifier instead", true)]
    public interface IVertexModifier
    {
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [Obsolete("use IMeshModifier.ModifyMesh (VertexHelper verts)  instead", true)]
        void ModifyVertices(List<UIVertex> verts);
    }

    public interface IMeshModifier
    {
        [Obsolete("use IMeshModifier.ModifyMesh (VertexHelper verts) instead", false)]
        void ModifyMesh(Mesh mesh);
        //! 调用处是 UnityEngine.UI.Graphic.DoMeshGeneration
        //! 此时可以修改 mesh, 做特效
        void ModifyMesh(VertexHelper verts);
    }
}

using Newtonsoft.Json;
using Striped.Engine.Serialization;

namespace Striped.Engine.Rendering.TemplateRenderers;

[Serializable]
public class GLMesh : SerializeableObject
{
    public string name = "new mesh";
    public GLMeshData renderMesh { get; private set; }

    public GLMesh() => renderMesh = new GLMeshData();
    
    public void SetVertices(float[] vertices)
    {
        GLMeshData data = renderMesh;
        data.vertices = vertices;
        renderMesh = data;
    }
    
    public void SetIndices(int[] indices)
    {
        GLMeshData data = renderMesh;
        data.indices = indices;
        renderMesh = data;
    }
    
    public void SetUVS(float[] uvs)
    {
        GLMeshData data = renderMesh;
        data.uvs = uvs;
        renderMesh = data;
    }
    
    public void SetNormals(float[] normals)
    {
        GLMeshData data = renderMesh;
        data.normals = normals;
        renderMesh = data;
    }
    
    public void SetVertexColors(float[] colors)
    {
        GLMeshData data = renderMesh;
        data.vertexColors = colors;
        renderMesh = data;
    }

    public override string Serialize()
    {
        return JsonConvert.SerializeObject(this,Formatting.Indented);
    }

    public override void Deserialize(string data)
    {
        GLMesh deserialized = JsonConvert.DeserializeObject<GLMesh>(data);
        renderMesh = deserialized.renderMesh;
        name = deserialized.name;
    }
}
using Newtonsoft.Json;
using Striped.Engine.Serialization;

namespace Striped.Engine.Rendering.TemplateRenderers;

[Serializable]
public class GLMesh : SerializeableObject
{
    public string name = "new mesh";
    [JsonIgnore]public GLMeshData RenderMesh { get => renderMesh; }
    private GLMeshData renderMesh;
    
    public GLMesh() => renderMesh = new GLMeshData();
    
    public void SetVertices(float[] vertices)
    {
        renderMesh.vertices = vertices;
    }
    
    public void SetIndices(int[] indices)
    {
        renderMesh.indices = indices;
    }
    
    public void SetUVS(float[] uvs)
    {
        renderMesh.uvs = uvs;
    }
    
    public void SetNormals(float[] normals)
    {
        renderMesh.normals = normals;
    }
    
    public void SetVertexColors(float[] colors)
    {
        renderMesh.vertexColors = colors;
    }

    public override string Serialize()
    {
        return JsonConvert.SerializeObject(renderMesh,Formatting.Indented);
    }

    public override void Deserialize(string data)
    {
        GLMeshData deserialized = JsonConvert.DeserializeObject<GLMeshData>(data);
        renderMesh = deserialized;
        name = "Deserialized Mesh";
    }
}
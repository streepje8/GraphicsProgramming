namespace Striped.Engine.Rendering.TemplateRenderers;

public enum GLMeshDataInfo
{
    HasVertecies = 1,
    HasIndices = 2,
    HasUVs = 4,
    HasNormals = 8,
    HasVertexColors = 16
}

public struct GLMeshData
{
    public float[] vertices;
    public int[] indices;
    public float[] vertexColors;
    public float[] normals;
    public float[] uvs;
    
    public GLMeshData(float[]? vertices = null, int[]? indices = null, float[]? uvs = null, float[]? normals = null, float[]? vertexColors = null)
    {
        this.vertices = vertices ?? Array.Empty<float>();
        this.indices = indices ?? Array.Empty<int>();
        this.uvs = uvs ?? Array.Empty<float>();
        this.normals = normals ?? Array.Empty<float>();
        this.vertexColors = vertexColors ?? Array.Empty<float>();
    }

    public byte GetMeshDataInfo()
    {
        byte flags = 0x0;
        if (vertices.Length > 0) flags |= (byte) GLMeshDataInfo.HasVertecies;
        if (indices.Length > 0) flags |= (byte) GLMeshDataInfo.HasIndices;
        if (uvs.Length > 0) flags |= (byte) GLMeshDataInfo.HasUVs;
        if (normals.Length > 0) flags |= (byte) GLMeshDataInfo.HasNormals;
        if (vertexColors.Length > 0) flags |= (byte) GLMeshDataInfo.HasVertexColors;
        return flags;
    }

    public int GetStride()
    {
        int floats = 0;
        byte meshDataInfo = GetMeshDataInfo();
        if ((meshDataInfo & (byte) GLMeshDataInfo.HasVertecies) != 0) floats += 3;
        if ((meshDataInfo & (byte) GLMeshDataInfo.HasUVs) != 0) floats += 2;
        if ((meshDataInfo & (byte) GLMeshDataInfo.HasNormals) != 0) floats += 3;
        if ((meshDataInfo & (byte) GLMeshDataInfo.HasVertexColors) != 0) floats += 3;
        return floats * sizeof(float);
    }
    
    public float[] GetMeshData()
    {
        List<float> data = new List<float>();
        for (int i = 0; i < vertices.Length / 3; i++)
        {
            int iii = i * 3;
            data.Add(vertices[iii]);
            data.Add(vertices[iii + 1]);
            data.Add(vertices[iii + 2]);
            
            if (uvs.Length > 0)
            {
                data.Add(uvs[i*2]);
                data.Add(uvs[i*2 + 1]);
            }

            if (normals.Length > 0)
            {
                data.Add(normals[iii]);
                data.Add(normals[iii + 1]);
                data.Add(normals[iii + 2]);
            }

            if (vertexColors.Length > 0)
            {
                data.Add(vertexColors[iii]);
                data.Add(vertexColors[iii + 1]);
                data.Add(vertexColors[iii + 2]);
            }
        }
        return data.ToArray();
    }
}
using Assimp;
using Assimp.Configs;
using OpenTK.Mathematics;
using Striped.Engine.Rendering.TemplateRenderers;
using Striped.Engine.Serialization;

namespace Striped.Engine.Util;

public enum DefaultMesh
{
    Cube,
    Plane,
    Sphere,
    Capsule,
    Torus
}

public class ModelLoader
{
    public static GLMesh LoadDefaultMesh(DefaultMesh mesh)
    {
        return AssetImporter.ImportAsset<GLMesh>(Application.AssetsFolder + "/Meshes/Standard/" + mesh.ToString().ToLower() + ".glmesh");
    }

    public static GLMesh LoadModel(string path)
    {
        AssimpContext importer = new AssimpContext();
        importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
        Scene model = importer.ImportFile(path, PostProcessPreset.TargetRealTimeMaximumQuality);
        if (model.Meshes.Count > 0)
        {
            Mesh mesh = model.Meshes[0];
            GLMesh glMesh = new GLMesh();
            glMesh.SetVertices(Vector3ToFloatArray(mesh.Vertices.ToArray()));
            glMesh.SetNormals(Vector3ToFloatArray(mesh.Normals.ToArray()));
            glMesh.SetIndices(mesh.GetIndices());
            glMesh.SetUVS(Vector2ToFloatArray(mesh.TextureCoordinateChannels[0].ToArray()));
            glMesh.SetVertexColors(Array.Empty<float>());
            return glMesh;
        }
        return null;
    }

    private static float[] Vector3ToFloatArray(Vector3D[] vectors)
    {
        float[] floats = new float[vectors.Length * 3];
        for (int i = 0; i < floats.Length; i++)
        {
            floats[i] = vectors[i / 3][i % 3];
        }
        return floats;
    }
    
    private static float[] Vector2ToFloatArray(Vector3D[] vectors)
    {
        float[] floats = new float[vectors.Length * 2];
        for (int i = 0; i < floats.Length; i++)
        {
            floats[i] = vectors[i / 2][i % 2];
        }
        return floats;
    }
}
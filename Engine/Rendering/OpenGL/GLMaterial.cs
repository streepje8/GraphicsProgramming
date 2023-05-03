using Newtonsoft.Json;
using Striped.Engine.Rendering.TemplateRenderers.Shaders;
using Striped.Engine.Serialization;
using Striped.Engine.Util;

namespace Striped.Engine.Rendering.TemplateRenderers;

[Serializable]
public struct SerializedMaterial
{
    public string shaderName;
    public string shader;
    public string[] textures;
}

public class GLMaterial : SerializeableObject
{
    public OpenGLShader shader;
    public List<Texture2D> textures {get; private set;} = new List<Texture2D>();

    public GLMaterial(string shaderName)
    {
        OpenGLShader glShader = OpenGLRenderer.GetShader(shaderName);
        if (glShader == null) Logger.Err("A shader with the name " + shaderName + " could not be found!");
        else shader = glShader;
    }
    

    public void Enable()
    {
        shader.Enable();
    }

    public override string Serialize()
    {
        SerializedMaterial material = new SerializedMaterial();
        material.shader = shader.Serialize();
        material.shaderName = shader.name;
        List<string> serializedTextures = new List<string>();
        foreach (var texture2D in textures)
        {
            serializedTextures.Add(texture2D.Serialize());
        }
        material.textures = serializedTextures.ToArray();
        return JsonConvert.SerializeObject(material, Formatting.Indented);
    }

    public override void Deserialize(string data)
    {
        SerializedMaterial material = JsonConvert.DeserializeObject<SerializedMaterial>(data);
        shader = OpenGLRenderer.GetShader(material.shaderName);
        if (shader == null)
        {
            shader = Deserializer.Deserialize<OpenGLShader>(material.shader);
            OpenGLRenderer.AddShaderInternal(shader);
        }
        textures = new List<Texture2D>();
        foreach (var materialTexture in material.textures)
        {
            textures.Add(Deserializer.Deserialize<Texture2D>(materialTexture));
        }
    }
}
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Striped.Engine.Core;
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
    public OpenGLShader? shader;
    public List<GLTexture2D> textures {get; private set;} = new List<GLTexture2D>();

    public GLMaterial(string shaderName)
    {
        OpenGLShader? glShader = null;
        if (!(GameSession.ActiveSession.Window == null || GameSession.ActiveSession.Window.ActiveRenderer == null))
        {
            if (!typeof(OpenGLRenderer).IsAssignableFrom(GameSession.ActiveSession.Window.ActiveRenderer.GetType()))
            {
                Logger.Except(new Exception("GL Material only supports the OpenGLRenderer"), new LoggingContext(2));
            }
            glShader = ((OpenGLRenderer)GameSession.ActiveSession.Window.ActiveRenderer).GetShader(shaderName); ;
        }
        else
        {
            glShader = OpenGLRenderer.GetShaderStatic(shaderName); //Fallback for materials made before the renderer is initialized
        }
        if (glShader == null) Logger.Err("A shader with the name " + shaderName + " could not be found!");
        else shader = glShader;
    }
    
    [Obsolete("Warning you are creating an OpenGL Material with the unsafe constructor! Only do this when you know what you are doing")]
    public GLMaterial(OpenGLShader shader)
    {
        this.shader = shader;
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
        shader = ((OpenGLRenderer)GameSession.ActiveSession.Window.ActiveRenderer).GetShader(material.shaderName);
        if (shader == null)
        {
            shader = Deserializer.Deserialize<OpenGLShader>(material.shader);
            OpenGLRenderer.AddShaderInternal(shader);
        }
        textures = new List<GLTexture2D>();
        foreach (var materialTexture in material.textures)
        {
            textures.Add(Deserializer.Deserialize<GLTexture2D>(materialTexture));
        }
    }

    private readonly Dictionary<string, int> uniformCache = new Dictionary<string, int>();

    public void SetMatrix4(string name, Matrix4 value, bool supressWarnings = true)
    {
        Enable();
        int endLocation = -1;
        if (!uniformCache.TryGetValue(name, out int location))
        {
            endLocation = shader.GetUniformLocation(name);
            uniformCache.Add(name, endLocation);
        }
        else endLocation = location;
        if(!supressWarnings && endLocation == -1) Logger.Warn("Uniform " + name + " not found!");
        GL.UniformMatrix4(endLocation, true, ref value);
    }
    
    public void SetVector3(string name, Vector3 value, bool supressWarnings = true)
    {
        Enable();
        int endLocation = -1;
        if (!uniformCache.TryGetValue(name, out int location))
        {
            endLocation = shader.GetUniformLocation(name);
            uniformCache.Add(name, endLocation);
        }
        else endLocation = location;
        if(!supressWarnings && endLocation == -1) Logger.Warn("Uniform " + name + " not found!");
        GL.Uniform3(endLocation, ref value);
    }
    
    public void SetVector2(string name, Vector2 value)
    {
        Enable();
        int endLocation = -1;
        if (!uniformCache.TryGetValue(name, out int location))
        {
            endLocation = shader.GetUniformLocation(name);
            uniformCache.Add(name, endLocation);
        }
        else endLocation = location;
        GL.Uniform2(endLocation, ref value);
    }
    
    public void SetFloat(string name, float value, bool supressWarnings  = true)
    {
        Enable();
        int endLocation = -1;
        if (!uniformCache.TryGetValue(name, out int location))
        {
            endLocation = shader.GetUniformLocation(name);
            uniformCache.Add(name, endLocation);
        }
        else endLocation = location;
        if(!supressWarnings && endLocation == -1) Logger.Warn("Uniform " + name + " not found!");
        GL.Uniform1(endLocation, value);
    }
    
    public void SetInt(string name, int value, bool supressWarnings  = true)
    {
        Enable();
        int endLocation = -1;
        if (!uniformCache.TryGetValue(name, out int location))
        {
            endLocation = shader.GetUniformLocation(name);
            uniformCache.Add(name, endLocation);
        }
        else endLocation = location;
        if(!supressWarnings && endLocation == -1) Logger.Warn("Uniform " + name + " not found!");
        GL.Uniform1(endLocation, value);
    }
}
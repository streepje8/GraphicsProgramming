using Striped.Engine.Rendering.TemplateRenderers.Shaders;
using Striped.Engine.Util;

namespace Striped.Engine.Rendering.TemplateRenderers;

public class GLMaterial
{
    public OpenGLShader shader;
    public List<Texture2D> textures {get; private set;} = new List<Texture2D>() ;

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
}
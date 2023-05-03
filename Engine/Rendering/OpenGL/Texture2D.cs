using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace Striped.Engine.Rendering.TemplateRenderers;

public class Texture2D
{
    private int handle = -1;
    
    public Texture2D(string path)
    {
        handle = GL.GenTexture();
        StbImage.stbi_set_flip_vertically_on_load(1);
        
        GL.ActiveTexture(TextureUnit.Texture0);
        Bind();
        SetFilterMode(TextureMinFilter.Nearest,false);
        SetWrapMode(TextureWrapMode.Repeat,false);
        
        using (Stream s = File.OpenRead(path))
        {
            ImageResult image = ImageResult.FromStream(s, ColorComponents.RedGreenBlueAlpha);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
        }
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
    }

    public void SetFilterMode(TextureMinFilter mode,bool bind = true)
    {
        if(bind) Bind();
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)mode);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)mode);
    }
    
    public void SetWrapMode(TextureWrapMode mode,bool bind = true)
    {
        if(bind) Bind();
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)mode);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)mode);
    }
    
    public void Bind()
    {
        GL.BindTexture(TextureTarget.Texture2D, handle);
    }
}
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Striped.Engine.BuildinComponents;
using Striped.Engine.Rendering.TemplateRenderers;
using Striped.Engine.Util;

namespace GraphicsProgramming.Engine.BuildinComponents.Renderers.Abstract;

public class QuadRenderer : RenderComponent
{
    public GLMaterial materal = new GLMaterial("Default/Error");
    int vertexBufferObject;
    int vertexArrayObject;

    float[] triangle = {
        -0.5f, -0.5f, 0.0f, 1f, 0.0f, 0.0f, //Bottom-left vertex 
        0.5f, -0.5f, 0.0f, 0.0f, 1f, 0.0f, //Bottom-right vertex
        0.0f,  0.5f, 0.0f, 0.0f, 0f, 1.0f, //Top vertex
    };
    
    float[] quad = {
        -0.5f, -0.5f, 0.0f, 1f, 0.0f, 0.0f, //Bottom-left vertex 
        0.5f, -0.5f, 0.0f, 0.0f, 1f, 0.0f, //Bottom-right vertex
        -0.5f, 0.5f, 0.0f, 0f, 0.0f, 1.0f, //Bottom-left vertex 
        0.5f, 0.5f, 0.0f, 1.0f, 1f, 0.0f, //Bottom-right vertex
    };

    public override void OnCreate()
    {
        if (transform == null)
        {
            entity.AddComponent<Transform>();
            Logger.Warn("The " + GetType().Name + " component required a transform. Automatically added one!");
        }
        vertexBufferObject = GL.GenBuffer();
        vertexArrayObject = GL.GenVertexArray();
        
        GL.BindVertexArray(vertexArrayObject);

        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, quad.Length * sizeof(float), quad, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(0);
        GL.EnableVertexAttribArray(1);
    }

    private Matrix4 transformReference;
    
    public override void OnRender(Camera cam)
    {
        transformReference = transform.TRS();
        GL.UniformMatrix4(materal.shader.GetUniformLocation("transform"), true, ref transformReference);
        GL.BindVertexArray(vertexArrayObject);
        materal.Enable();
        GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
    }

    public override void OnDestroy()
    {
        GL.DeleteBuffer(vertexBufferObject);
    }
}
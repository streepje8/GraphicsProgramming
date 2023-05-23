using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Striped.Engine.BuildinComponents;
using Striped.Engine.Rendering.TemplateRenderers;
using Striped.Engine.Rendering.TemplateRenderers.Shaders;
using Striped.Engine.Util;

namespace Striped.Engine.BuildinComponents.Renderers.Abstract;

public class GLMeshRenderer : RenderComponent
{
    public GLMaterial materal { get; private set; } = new GLMaterial("Default/Error");
    private int vertexBufferObject = -1;
    private int vertexArrayObject = -1;
    private int elementBufferObject = -1;

    public GLMesh Mesh { get; private set; } = new GLMesh();
    private GLMeshData renderMesh;

    public void SetMesh(GLMesh mesh)
    {
        Mesh = mesh;
        renderMesh = mesh.RenderMesh;
        ReloadMesh();
    }
    
    public override void OnCreate()
    {
        renderMesh = new GLMeshData(Array.Empty<float>(),Array.Empty<int>(), Array.Empty<float>(), Array.Empty<float>(), Array.Empty<float>());
        if (transform == null)
        {
            entity.AddComponent<Transform>();
            Logger.Warn("The " + GetType().Name + " component required a transform. Automatically added one!");
        }
        ReloadMesh();
    }

    public void ReloadMesh()
    {
        if((renderMesh.GetMeshDataInfo() & (byte)GLMeshDataInfo.HasVertecies) == 0)
        {
            return;
        }
        //Regenerate buffers
        if (vertexBufferObject != -1) GL.DeleteBuffer(vertexBufferObject);
        if(elementBufferObject != -1) GL.DeleteBuffer(elementBufferObject);
        vertexBufferObject = GL.GenBuffer();
        elementBufferObject = GL.GenBuffer();
        vertexArrayObject = GL.GenVertexArray();

        //Bind the stuff and populate it
        byte meshDataInfo = renderMesh.GetMeshDataInfo();
        int stride = renderMesh.GetStride();
        float[] data = renderMesh.GetMeshData();
        GL.BindVertexArray(vertexArrayObject);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, renderMesh.indices.Length * sizeof(int), renderMesh.indices, BufferUsageHint.StaticDraw);

        //Tell the gpu about what the data is
        //Get the locations in the shader
        OpenGLShader? shader = materal.shader;
        int pos = -1, uv = -1, normal = -1, color = -1;
        
        int offset = 0;
        if ((meshDataInfo & (byte)GLMeshDataInfo.HasVertecies) != 0)
        {
            pos = shader.GetAttribLocation("vPosition");
            GL.VertexAttribPointer(pos, 3, VertexAttribPointerType.Float, false, stride, offset);
            GL.EnableVertexAttribArray(0);
            offset += 3 * sizeof(float);
        }
        if ((meshDataInfo & (byte)GLMeshDataInfo.HasUVs) != 0)
        {
            uv = shader.GetAttribLocation("vTexCoord");
            GL.VertexAttribPointer(uv, 2, VertexAttribPointerType.Float, false, stride, offset);
            GL.EnableVertexAttribArray(1);
            offset += 2 * sizeof(float);
        }
        
        if ((meshDataInfo & (byte)GLMeshDataInfo.HasNormals) != 0)
        {
            normal = shader.GetAttribLocation("vNormal");
            GL.VertexAttribPointer(normal, 3, VertexAttribPointerType.Float, false, stride, offset);
            GL.EnableVertexAttribArray(2);
            offset += 3 * sizeof(float);
        }
        
        if ((meshDataInfo & (byte)GLMeshDataInfo.HasNormals) != 0)
        {
            color = shader.GetAttribLocation("vColor");
            GL.VertexAttribPointer(color, 3, VertexAttribPointerType.Float, false, stride, offset);
            GL.EnableVertexAttribArray(3);
            offset += 3 * sizeof(float);
        }
    }

    public void SetMaterial(GLMaterial mat)
    {
        materal = mat;
        ReloadMesh();
    }

    private Matrix4 transformReference;
    private Matrix4 viewReference;
    private Matrix4 projectionReference;
    
    public override void OnRender(Camera cam)
    {
        if((renderMesh.GetMeshDataInfo() & (byte)GLMeshDataInfo.HasVertecies) == 0)
        {
            return;
        }
        transformReference = transform.TRS();
        viewReference = Camera.Active.ViewMatrix;
        projectionReference = Camera.Active.ProjectionMatrix;
        int transformLocation = materal.shader.GetUniformLocation("model");
        int viewLocation = materal.shader.GetUniformLocation("view");
        int projectionLocation = materal.shader.GetUniformLocation("projection");
        materal.Enable();
        GL.UniformMatrix4(transformLocation, true, ref transformReference);
        GL.UniformMatrix4(viewLocation, true, ref viewReference);
        GL.UniformMatrix4(projectionLocation, true, ref projectionReference);
        int i = 0;
        foreach (var texture in materal.textures)
        {
            Enum.TryParse("Texture" + i, out TextureUnit unit);
            GL.ActiveTexture(unit);
            texture.Bind();
            i++;
        }
        GL.BindVertexArray(vertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, renderMesh.indices.Length, DrawElementsType.UnsignedInt, 0);
    }

    public override void OnDestroy()
    {
        GL.DeleteBuffer(vertexBufferObject);
        GL.DeleteBuffer(elementBufferObject);
    }
}
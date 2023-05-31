using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Striped.Engine.BuildinComponents;
using Striped.Engine.Core;
using Striped.Engine.Rendering.Core;
using Striped.Engine.Rendering.TemplateRenderers;
using Striped.Engine.Rendering.TemplateRenderers.Shaders;
using Striped.Engine.Util;

namespace GraphicsProgramming.Engine.Rendering.RayTracing;

public class RTRenderer : Renderer
{
    private static Vector4 clearColor = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
    public static GLMaterial SkyboxMaterial { get; set; }
    public static Vector4 ClearColor
    {
        get => clearColor;
        set
        {
            clearColor = value;
            GL.ClearColor(clearColor.X,clearColor.Y,clearColor.Z,clearColor.W);
        }
    }

    public static GLMaterial MainRenderer { get; private set; }

    private static Dictionary<string, OpenGLShader?> shaders = new Dictionary<string, OpenGLShader?>();
    
    
    //Data for the main quad
    private float[] vertecies = {
        -1f, -1f, 0.5f, 
        1f, -1f, 0.5f, 
        -1f, 1f, 0.5f,
        1f, 1f, 0.5f
    };

    private int[] indicies = new int[]
    {
        0, 1, 2,
        2, 1, 3
    };
    
    private float[] vUVS = {
        0.0f,1.0f, //Bottom-left vertex 
        1.0f,1.0f, //Bottom-right vertex
        0.0f,0.0f, //Top-left vertex 
        1.0f,0.0f, //Top-right vertex
    };
    
    private float[] vNormals = {
        0.0f, 0.0f, 0.0f, //Bottom-left vertex 
        0.0f, 0.0f, 0.0f, //Bottom-left vertex 
        0.0f, 0.0f, 0.0f, //Bottom-left vertex 
        0.0f, 0.0f, 0.0f, //Bottom-left vertex 
    };
    
    private float[] vColors = {
        1f, 0.0f, 0.0f, //Bottom-left vertex 
        0.0f, 1f, 0.0f, //Bottom-right vertex
        0f, 0.0f, 1.0f, //Top-left vertex 
        1.0f, 1f, 0.0f //Top-right vertex
    };
    private GLMeshData quad;
    private int vertexBufferObject = -1;
    private int vertexArrayObject = -1;
    private int elementBufferObject = -1;
    
    

    public override void OnResize(EngineWindow engineWindow, int width, int height)
    {
        GL.Viewport(0,0,width,height);
        foreach (var camera in Component<Camera>.instances.Span)
        {
            camera.UpdateProjectionMatrix(new Vector2i(width,height));
        }
    }
    
    public OpenGLShader? CreateShader(string filePath)
    {
        OpenGLShader? shader = new OpenGLShader(filePath);
        shaders.Add(shader.name,shader);
        shader.BindSource();
        shader.CompileAndLoad();
        return shader;
    }
    
    public static void AddShaderInternal(OpenGLShader shader)
    {
        shaders.Add(shader.name,shader);
    }

    public static OpenGLShader? GetShader(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        if (shaders.TryGetValue(name, out OpenGLShader? shader)) return shader;
        return null;
    }
    
    public override void OnLoad()
    {
        CreateShader(Application.AssetsFolder + "/Shaders/Standard/errorShader.shader");
        OpenGLShader RTShader = CreateShader(Application.AssetsFolder + "/Shaders/Standard/defaultRayTraced.shader");
        if(RTShader == null) Logger.Except(new FileNotFoundException("The raytracing shader could not be located! Are you sure your assets folder is set correctly?"));
#pragma warning disable CS0618
        MainRenderer = new GLMaterial(RTShader);
#pragma warning restore CS0618
        GL.ClearColor(clearColor.X,clearColor.Y,clearColor.Z,clearColor.W);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(CullFaceMode.Back);
        
        //Create the main quad
        quad = new GLMeshData(vertecies,indicies, vUVS, vNormals, vColors);
        //Regenerate buffers
        if (vertexBufferObject != -1) GL.DeleteBuffer(vertexBufferObject);
        if(elementBufferObject != -1) GL.DeleteBuffer(elementBufferObject);
        vertexBufferObject = GL.GenBuffer();
        elementBufferObject = GL.GenBuffer();
        vertexArrayObject = GL.GenVertexArray();

        //Bind the stuff and populate it
        byte meshDataInfo = quad.GetMeshDataInfo();
        int stride = quad.GetStride();
        float[] data = quad.GetMeshData();
        GL.BindVertexArray(vertexArrayObject);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, quad.indices.Length * sizeof(int), quad.indices, BufferUsageHint.StaticDraw);

        //Tell the gpu about what the data is
        //Get the locations in the shader
        OpenGLShader? shader = MainRenderer.shader;
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

    private Matrix4 mat;
    private Matrix4 transformReference;
    private Matrix4 viewReference;
    private Matrix4 projectionReference;
    private int frame = 0;
    
    
    public override void OnRenderFrame()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.Clear(ClearBufferMask.DepthBufferBit);
        foreach (Camera cam in Camera.instances.Span)
        {
            if (cam.isActive)
            {
                RenderCamera(cam);
            }
        }
        Camera.SetNoneActive();
    }

    private void RenderCamera(Camera cam)
    {
        cam.SetActive();
        foreach (RenderComponent rend in cam.entity.GetEnvironment().GetAllComponentsInScene<RenderComponent>())
        {
            rend.OnRender(cam);
        }
        transformReference = Matrix4.Identity;
        int transformLocation = MainRenderer.shader.GetUniformLocation("model");
        GL.UniformMatrix4(transformLocation, true, ref transformReference);
        MainRenderer.SetInt("_Frame", frame++);
        MainRenderer.SetVector3("_CamPos", cam.transform.position);
        float planeHeight = cam.Near * MathF.Tan(MathHelper.DegreesToRadians(cam.FOV * 0.5f)) * 2;
        MainRenderer.SetVector3("_ViewParams", new Vector3(planeHeight * cam.Aspect,planeHeight,cam.Near));
        MainRenderer.SetMatrix4("_CamLocalToWorldMatrix", cam.transform.TRS());
        int i = 0;
        foreach (var texture in MainRenderer.textures)
        {
            Enum.TryParse("Texture" + i, out TextureUnit unit);
            GL.ActiveTexture(unit);
            texture.Bind();
            i++;
        }
        GL.BindVertexArray(vertexArrayObject);
        MainRenderer.Enable();
        GL.DrawElements(PrimitiveType.Triangles, quad.indices.Length, DrawElementsType.UnsignedInt, 0);
    }
    
    public override void OnUnLoad()
    {
        GL.DeleteBuffer(vertexBufferObject);
        GL.DeleteBuffer(elementBufferObject);
        foreach (var openGLShader in shaders) openGLShader.Value.CleanUp();
    }
}
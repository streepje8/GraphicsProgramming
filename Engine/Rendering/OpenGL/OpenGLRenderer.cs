using Assimp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Striped.Engine.BuildinComponents;
using Striped.Engine.Core;
using Striped.Engine.Rendering.Core;
using Striped.Engine.Rendering.TemplateRenderers.Shaders;
using Striped.Engine.Serialization;
using Striped.Engine.Util;
using Camera = Striped.Engine.BuildinComponents.Camera;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

namespace Striped.Engine.Rendering.TemplateRenderers;

public class OpenGLRenderer : Renderer
{

    private static Vector4 clearColor = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
    public static bool RenderSkyBox { get; set; } = true;
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

    private static Dictionary<string, OpenGLShader?> shaders = new Dictionary<string, OpenGLShader?>();

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
        CreateShader(Application.AssetsFolder + "/Shaders/Standard/defaultDiffuse.shader");
        CreateShader(Application.AssetsFolder + "/Shaders/Standard/defaultSkybox.shader");
        SetupSkybox();
        GL.ClearColor(clearColor.X,clearColor.Y,clearColor.Z,clearColor.W);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(CullFaceMode.Back);
    }

    private Matrix4 mat;

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
        if (RenderSkyBox) RenderSkybox();
        foreach (RenderComponent rend in cam.entity.GetEnvironment().GetAllComponentsInScene<RenderComponent>())
        {
            rend.OnRender(cam);
        }
    }
    
    public override void OnUnLoad()
    {
        foreach (var openGLShader in shaders) openGLShader.Value.CleanUp();
        CleanUpSkybox();
    }
    

    private int skyboxVBO;
    private int skyboxVAO;
    private int skyboxEBO;
    private int skyboxIndiciesCount;
    
    private void SetupSkybox()
    {
        GLMesh box = ModelLoader.LoadDefaultMesh(DefaultMesh.Cube);
        GLMeshData renderMesh = box.RenderMesh;
        SkyboxMaterial = new GLMaterial("Default/Sky");
        if((renderMesh.GetMeshDataInfo() & (byte)GLMeshDataInfo.HasVertecies) == 0)
        {
            return;
        }

        Texture2D rgbNoise = new Texture2D(Application.AssetsFolder + "/Textures/Standard/RGBNoise.png");
        SkyboxMaterial.textures.Add(rgbNoise);

        skyboxIndiciesCount = renderMesh.indices.Length;
        //Regenerate buffers
        if (skyboxVBO != -1) GL.DeleteBuffer(skyboxVBO);
        if(skyboxEBO != -1) GL.DeleteBuffer(skyboxEBO);
        skyboxVBO = GL.GenBuffer();
        skyboxEBO = GL.GenBuffer();
        skyboxVAO = GL.GenVertexArray();

        //Bind the stuff and populate it
        byte meshDataInfo = renderMesh.GetMeshDataInfo();
        int stride = renderMesh.GetStride();
        float[] data = renderMesh.GetMeshData();
        GL.BindVertexArray(skyboxVAO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, skyboxVBO);
        GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, skyboxEBO);
        GL.BufferData(BufferTarget.ElementArrayBuffer, renderMesh.indices.Length * sizeof(int), renderMesh.indices, BufferUsageHint.StaticDraw);

        //Tell the gpu about what the data is
        //Get the locations in the shader
        OpenGLShader shader = SkyboxMaterial.shader;
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
    
    private void RenderSkybox()
    {
        GL.CullFace(CullFaceMode.Front);
        SkyboxMaterial.Enable();
        SkyboxMaterial.SetMatrix4("view", Camera.Active.ViewMatrix);
        SkyboxMaterial.SetMatrix4("projection", Camera.Active.ProjectionMatrix);
        SkyboxMaterial.SetMatrix4("model", Matrix4.CreateScale(500.0f) * Matrix4.CreateTranslation(Camera.Active.transform!.position));
        SkyboxMaterial.SetFloat("iTime", Time.time);
        SkyboxMaterial.SetVector2("iResolution",GameSession.ActiveSession.Window.Size);
        SkyboxMaterial.SetVector3("cameraPosition", Camera.Active.transform.position);
        int i = 0;
        foreach (var texture in SkyboxMaterial.textures)
        {
            Enum.TryParse("Texture" + i, out TextureUnit unit);
            GL.ActiveTexture(unit);
            texture.Bind();
            i++;
        }
        GL.BindVertexArray(skyboxVAO);
        GL.DrawElements(PrimitiveType.Triangles, skyboxIndiciesCount, DrawElementsType.UnsignedInt, 0);
        GL.CullFace(CullFaceMode.Back);
    }

    private void CleanUpSkybox()
    {
        GL.DeleteBuffer(skyboxVBO);
        GL.DeleteBuffer(skyboxEBO);
    }
}
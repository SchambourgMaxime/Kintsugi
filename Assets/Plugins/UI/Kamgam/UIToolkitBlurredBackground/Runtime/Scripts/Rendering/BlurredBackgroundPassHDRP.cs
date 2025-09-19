#if !KAMGAM_RENDER_PIPELINE_URP && KAMGAM_RENDER_PIPELINE_HDRP
// Based on: https://github.com/alelievr/HDRP-Custom-Passes/blob/2021.2/Assets/CustomPasses/CopyPass/CopyPass.cs#L67
// as recommended by antoinel_unity in https://forum.unity.com/threads/custom-pass-into-render-texture-into-custom-aov.1146872/#post-7362314

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Kamgam.UIToolkitBlurredBackground
{
    public class BlurredBackgroundPassHDRP : CustomPass
    {
        public const string ShaderName = "Kamgam/UI Toolkit/HDRP/Blur Shader";

        [System.NonSerialized]
        protected Material _material;
        public Material Material
        {
            get
            {
                if (_material == null)
                {
                    // Create material with shader
                    var shader = Shader.Find(ShaderName);
                    if (shader != null)
                    {
                        _material = CoreUtils.CreateEngineMaterial(shader);
                        _material.color = Color.white;

                        switch (_shaderQuality)
                        {
                            case ShaderQuality.Low:
                                _material.SetKeyword(new LocalKeyword(shader, "_SAMPLES_LOW"), true);
                                _material.SetKeyword(new LocalKeyword(shader, "_SAMPLES_MEDIUM"), false);
                                _material.SetKeyword(new LocalKeyword(shader, "_SAMPLES_HIGH"), false);
                                break;

                            case ShaderQuality.Medium:
                                _material.SetKeyword(new LocalKeyword(shader, "_SAMPLES_LOW"), false);
                                _material.SetKeyword(new LocalKeyword(shader, "_SAMPLES_MEDIUM"), true);
                                _material.SetKeyword(new LocalKeyword(shader, "_SAMPLES_HIGH"), false);
                                break;

                            case ShaderQuality.High:
                                _material.SetKeyword(new LocalKeyword(shader, "_SAMPLES_LOW"), false);
                                _material.SetKeyword(new LocalKeyword(shader, "_SAMPLES_MEDIUM"), false);
                                _material.SetKeyword(new LocalKeyword(shader, "_SAMPLES_HIGH"), true);
                                break;

                            default:
                                break;
                        }

                        setOffset(Offset);
                    }
                }
                return _material;
            }

            set
            {
                _material = value;
            }
        }

        void setOffset(float value)
        {
            if (_material != null)
                _material.SetVector("_BlurOffset", new Vector4(value, value, 0f, 0f));
        }

        [System.NonSerialized]
        protected int _blurIterations = 0;
        public int BlurIterations
        {
            get => _blurIterations;
            set
            {
                if (_blurIterations != value)
                {
                    _blurIterations = value;
                    enabled = _blurIterations > 0;
                }
            }
        }

        protected float _offset = 1.5f;
        /// <summary>
        /// This is only used in the performance shader. Default is 1.5f. You can increase this AND reduce the blur strength to imporve performance. However, the quality will start to degrade rapidly.
        /// </summary>
        public float Offset
        {
            get => _offset;
            set
            {
                _offset = value;
                setOffset(value);
            }
        }

        protected ShaderQuality _shaderQuality = ShaderQuality.Medium;

        /// <summary>
        /// The used shader quality. The higher the more performance it will cost.
        /// </summary>
        public ShaderQuality ShaderQuality
        {
            get => _shaderQuality;
            set
            {
                _shaderQuality = value;
                _material = null;
            }
        }

        /// <summary>
        /// The used resolution of the render texture.
        /// </summary>
        [System.NonSerialized]
        public Vector2Int Resolution = new Vector2Int(512, 512);

        public void UpdateRenderTextureResolutions()
        {
            if (_renderTargetBlurredA != null)
            {
                _renderTargetBlurredA.Release();
                _renderTargetBlurredA.width = Resolution.x;
                _renderTargetBlurredA.height = Resolution.y;
                _renderTargetBlurredA.Create();
            }

            if (_renderTargetBlurredB != null)
            {
                _renderTargetBlurredB.Release();
                _renderTargetBlurredB.width = Resolution.x;
                _renderTargetBlurredB.height = Resolution.y;
                _renderTargetBlurredB.Create();
            }
        }

        [System.NonSerialized]
        protected RenderTexture _renderTargetBlurredA;
        public RenderTexture RenderTargetBlurredA
        {
            get
            {
                if (_renderTargetBlurredA == null)
                {
                    _renderTargetBlurredA = createRenderTexture();

                    if (_renderTargetHandleA != null)
                    {
                        _renderTargetHandleA.Release();
                        _renderTargetHandleA = null;
                    }
                }

                return _renderTargetBlurredA;
            }
        }

        [System.NonSerialized]
        protected RenderTexture _renderTargetBlurredB;
        public RenderTexture RenderTargetBlurredB
        {
            get
            {
                if (_renderTargetBlurredB == null)
                {
                    _renderTargetBlurredB = createRenderTexture();

                    if (_renderTargetHandleB != null)
                    {
                        _renderTargetHandleB.Release();
                        _renderTargetHandleB = null;
                    }
                }

                return _renderTargetBlurredB;
            }
        }

        [System.NonSerialized]
        protected RTHandle _renderTargetHandleA;
        public RTHandle RenderTargetHandleA
        {
            get
            {
                if (_renderTargetHandleA == null)
                    _renderTargetHandleA = RTHandles.Alloc(RenderTargetBlurredA);

                return _renderTargetHandleA;
            }
        }

        [System.NonSerialized]
        protected RTHandle _renderTargetHandleB;
        public RTHandle RenderTargetHandleB
        {
            get
            {
                if (_renderTargetHandleB == null)
                    _renderTargetHandleB = RTHandles.Alloc(RenderTargetBlurredB);

                return _renderTargetHandleB;
            }
        }

        RenderTexture createRenderTexture()
        {
            var texture = new RenderTexture(Resolution.x, Resolution.y, 16);
            texture.filterMode = FilterMode.Bilinear;

            return texture;
        }

        [System.NonSerialized]
        public bool AreTexturesSwapped;

        public Texture GetBlurredTexture()
        {
            return AreTexturesSwapped ? RenderTargetBlurredB : RenderTargetBlurredA;
        }

        protected override bool executeInSceneView => false;

        protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
        {
            name = "UITK Blurred Background";
        }

        protected override void Execute(CustomPassContext ctx)
        {
            if (Material == null || BlurIterations == 0 || Offset == 0)
                return;

            var source = ctx.cameraColorBuffer;

            // First pass is just a copy with the right scale (plus downsampling).
            // From: ctx.cmd.Blit(RenderTargetBlurredB, RenderTargetBlurredA, Material);
            //
            // Sadly the API for copying, scaling AND using a material is not exposed.
            //
            // TODO: Investigate if this breaks XR compatibility.
            //   Solution leads: Use 2DArray and SAMPLE_TEXTURE2D_X in the shader and maybe use Blit_Texture() or Blit_Identifier to pass the material.
            //   See: https://github.com/Unity-Technologies/UnityCsReference/blob/master/Runtime/Export/Graphics/RenderingCommandBuffer.cs#L901
            //   and: https://github.com/Unity-Technologies/UnityCsReference/blob/master/Runtime/Export/Graphics/RenderingCommandBuffer.bindings.cs#L614
            var scale = RTHandles.rtHandleProperties.rtHandleScale;
            ctx.cmd.Blit(source, RenderTargetBlurredA, new Vector2(scale.x, scale.y), Vector2.zero, 0, 0);
            AreTexturesSwapped = false;

            // All other blur passes play ping pong between A and B
            for (int i = 0; i < BlurIterations; i++)
            {
                if (AreTexturesSwapped)
                {
                    ctx.cmd.Blit(RenderTargetBlurredB, RenderTargetBlurredA, Material, 0);
                    ctx.cmd.Blit(RenderTargetBlurredA, RenderTargetBlurredB, Material, 1);
                }
                else
                {
                    ctx.cmd.Blit(RenderTargetBlurredA, RenderTargetBlurredB, Material, 0);
                    ctx.cmd.Blit(RenderTargetBlurredB, RenderTargetBlurredA, Material, 1);
                }
            }
        }

        protected override void Cleanup()
        {
            CoreUtils.Destroy(_material);

            if (_renderTargetBlurredA != null)
            {
                _renderTargetBlurredA.Release();
                _renderTargetBlurredA = null;
            }

            if (_renderTargetBlurredB != null)
            {
                _renderTargetBlurredB.Release();
                _renderTargetBlurredB = null;
            }

            if (_renderTargetHandleA != null)
            {
                _renderTargetHandleA.Release();
                _renderTargetHandleA = null;
            }

            if (_renderTargetHandleB != null)
            {
                _renderTargetHandleB.Release();
                _renderTargetHandleB = null;
            }

            base.Cleanup();
        }
    }
}
#endif
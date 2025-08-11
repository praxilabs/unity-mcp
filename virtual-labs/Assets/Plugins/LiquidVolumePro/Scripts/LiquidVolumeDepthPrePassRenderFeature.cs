using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace LiquidVolumeFX {

    public class LiquidVolumeDepthPrePassRenderFeature : ScriptableRendererFeature {

        static class ShaderParams {
            public const string RTBackBufferName = "_VLBackBufferTexture";
            public static int RTBackBuffer = Shader.PropertyToID(RTBackBufferName);
            public const string RTFrontBufferName = "_VLFrontBufferTexture";
            public static int RTFrontBuffer = Shader.PropertyToID(RTFrontBufferName);
            public static int FlaskThickness = Shader.PropertyToID("_FlaskThickness");
            public static int ForcedInvisible = Shader.PropertyToID("_LVForcedInvisible");
            public const string SKW_FP_RENDER_TEXTURE = "LIQUID_VOLUME_FP_RENDER_TEXTURES";
        }

        enum Pass {
            BackBuffer = 0,
            FrontBuffer = 1
        }

        public readonly static List<LiquidVolume> lvBackRenderers = new List<LiquidVolume>();
        public readonly static List<LiquidVolume> lvFrontRenderers = new List<LiquidVolume>();

        public static void AddLiquidToBackRenderers(LiquidVolume lv) {
            if (lv == null || lv.topology != TOPOLOGY.Irregular || lvBackRenderers.Contains(lv)) return;
            lvBackRenderers.Add(lv);
        }

        public static void RemoveLiquidFromBackRenderers(LiquidVolume lv) {
            if (lv == null || !lvBackRenderers.Contains(lv)) return;
            lvBackRenderers.Remove(lv);
        }

        public static void AddLiquidToFrontRenderers(LiquidVolume lv) {
            if (lv == null || lv.topology != TOPOLOGY.Irregular || lvFrontRenderers.Contains(lv)) return;
            lvFrontRenderers.Add(lv);
        }

        public static void RemoveLiquidFromFrontRenderers(LiquidVolume lv) {
            if (lv == null || !lvFrontRenderers.Contains(lv)) return;
            lvFrontRenderers.Remove(lv);
        }

        class DepthPass : ScriptableRenderPass {

            const string profilerTag = "LiquidVolumeDepthPrePass";

            Material mat;
            int targetNameId;
            RTHandle targetRT;
            int passId;
            List<LiquidVolume> lvRenderers;
            public ScriptableRenderer renderer;
            public bool interleavedRendering;
            Vector3 currentCameraPosition;

            public DepthPass(Material mat, Pass pass, RenderPassEvent renderPassEvent) {
                this.renderPassEvent = renderPassEvent;
                this.mat = mat;
                switch (pass) {
                    case Pass.BackBuffer: {
                            targetNameId = ShaderParams.RTBackBuffer;
                            RenderTargetIdentifier rt = new RenderTargetIdentifier(targetNameId, 0, CubemapFace.Unknown, -1);
                            targetRT = RTHandles.Alloc(rt, name: ShaderParams.RTBackBufferName);
                            passId = (int)Pass.BackBuffer;
                            lvRenderers = lvBackRenderers;
                            break;
                        }
                    case Pass.FrontBuffer: {
                            targetNameId = ShaderParams.RTFrontBuffer;
                            RenderTargetIdentifier rt = new RenderTargetIdentifier(targetNameId, 0, CubemapFace.Unknown, -1);
                            targetRT = RTHandles.Alloc(rt, name: ShaderParams.RTFrontBufferName);
                            passId = (int)Pass.FrontBuffer;
                            lvRenderers = lvFrontRenderers;
                            break;
                        }
                }
            }

            public void Setup(LiquidVolumeDepthPrePassRenderFeature feature, ScriptableRenderer renderer) {
                this.renderer = renderer;
                this.interleavedRendering = feature.interleavedRendering;
            }
			

            int SortByDistanceToCamera(LiquidVolume lv1, LiquidVolume lv2) {
                bool isNull1 = lv1 == null;
                bool isNull2 = lv2 == null;
                if (isNull1 && isNull2) return 0;
                if (isNull2) return 1;
                if (isNull1) return -1;
                float dist1 = Vector3.Distance(lv1.transform.position, currentCameraPosition);
                float dist2 = Vector3.Distance(lv2.transform.position, currentCameraPosition);
                if (dist1 < dist2) return 1;
                if (dist1 > dist2) return -1;
                return 0;

            }			

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
                cameraTextureDescriptor.colorFormat = LiquidVolume.useFPRenderTextures ? RenderTextureFormat.RHalf : RenderTextureFormat.ARGB32;
                cameraTextureDescriptor.sRGB = false;
                cameraTextureDescriptor.depthBufferBits = 16;
                cameraTextureDescriptor.msaaSamples = 1;
                cmd.GetTemporaryRT(targetNameId, cameraTextureDescriptor);
                if (!interleavedRendering) {
                    ConfigureTarget(targetRT);
                }
                ConfigureInput(ScriptableRenderPassInput.Depth);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {

                if (lvRenderers == null) return;

                CommandBuffer cmd = CommandBufferPool.Get(profilerTag);
                cmd.Clear();
                cmd.SetGlobalFloat(ShaderParams.ForcedInvisible, 0);

                Camera cam = renderingData.cameraData.camera;
                if (interleavedRendering) {
                    RenderTargetIdentifier destination = new RenderTargetIdentifier(targetNameId, 0, CubemapFace.Unknown, -1);
                    currentCameraPosition = cam.transform.position;
                    lvRenderers.Sort(SortByDistanceToCamera);
                    lvRenderers.ForEach((LiquidVolume lv) => {
                        if (lv != null && lv.isActiveAndEnabled) {
                            if (lv.topology == TOPOLOGY.Irregular) {
                                cmd.SetRenderTarget(destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
                                if (LiquidVolume.useFPRenderTextures) {
                                    cmd.ClearRenderTarget(true, true, new Color(cam.farClipPlane, 0, 0, 0), 1f);
                                    cmd.EnableShaderKeyword(ShaderParams.SKW_FP_RENDER_TEXTURE);
                                } else {
                                    cmd.ClearRenderTarget(true, true, new Color(0.9882353f, 0.4470558f, 0.75f, 0f), 1f);
                                    cmd.DisableShaderKeyword(ShaderParams.SKW_FP_RENDER_TEXTURE);
                                }
                                cmd.SetGlobalFloat(ShaderParams.FlaskThickness, 1.0f - lv.flaskThickness);
                                // draw back face
                                cmd.DrawRenderer(lv.mr, mat, lv.subMeshIndex >= 0 ? lv.subMeshIndex : 0, passId);
                            }
                            // draw liquid
#if UNITY_2022_1_OR_NEWER
                            cmd.SetRenderTarget(renderer.cameraColorTargetHandle, renderer.cameraDepthTargetHandle);
#else
                            cmd.SetRenderTarget(renderer.cameraColorTarget, renderer.cameraDepthTarget);
#endif
                            cmd.DrawRenderer(lv.mr, lv.liqMat, lv.subMeshIndex >= 0 ? lv.subMeshIndex : 0, shaderPass: 1);
                        }
                    });
                    cmd.SetGlobalFloat(ShaderParams.ForcedInvisible, 1);
                } else {
                    // accumulate back face depths into custom rt
                    if (LiquidVolume.useFPRenderTextures) {
                        cmd.ClearRenderTarget(true, true, new Color(cam.farClipPlane, 0, 0, 0), 1f);
                        cmd.EnableShaderKeyword(ShaderParams.SKW_FP_RENDER_TEXTURE);
                    } else {
                        cmd.ClearRenderTarget(true, true, new Color(0.9882353f, 0.4470558f, 0.75f, 0f), 1f);
                        cmd.DisableShaderKeyword(ShaderParams.SKW_FP_RENDER_TEXTURE);
                    }

                    lvRenderers.ForEach((LiquidVolume lv) => {
                        if (lv != null && lv.isActiveAndEnabled) {
                            cmd.SetGlobalFloat(ShaderParams.FlaskThickness, 1.0f - lv.flaskThickness);
                            cmd.DrawRenderer(lv.mr, mat, lv.subMeshIndex >= 0 ? lv.subMeshIndex : 0, passId);
                        }
                    });
                }

                context.ExecuteCommandBuffer(cmd);

                CommandBufferPool.Release(cmd);
            }

            public override void FrameCleanup(CommandBuffer cmd) {
                cmd.ReleaseTemporaryRT(targetNameId);
            }

            public void CleanUp() {
                RTHandles.Release(targetRT);
            }
        }


        [SerializeField, HideInInspector]
        Shader shader;

        public static bool installed;
        Material mat;
        DepthPass backPass, frontPass;

        [Tooltip("Renders each irregular liquid volume completely before rendering the next one.")]
        public bool interleavedRendering;

        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;

        private void OnDestroy() { 
            Shader.SetGlobalFloat(ShaderParams.ForcedInvisible, 0);
            CoreUtils.Destroy(mat);
            if (backPass != null) {
                backPass.CleanUp();
            }
            if (frontPass != null) {
                frontPass.CleanUp();
            }
        }

        public override void Create() {
            name = "Liquid Volume Depth PrePass";
            shader = Shader.Find("LiquidVolume/DepthPrePass");
            if (shader == null) {
                return;
            }
            mat = CoreUtils.CreateEngineMaterial(shader);
            backPass = new DepthPass(mat, Pass.BackBuffer, renderPassEvent);
            frontPass = new DepthPass(mat, Pass.FrontBuffer, renderPassEvent);
        }

        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
            installed = true;
            if (backPass != null && lvBackRenderers.Count > 0) {
                backPass.Setup(this, renderer);
                renderer.EnqueuePass(backPass);
            }
            if (frontPass != null && lvFrontRenderers.Count > 0) {
                frontPass.Setup(this, renderer);
                frontPass.renderer = renderer;
                renderer.EnqueuePass(frontPass);
            }
        }
    }
}

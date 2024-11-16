using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace MVoxelizer.MVRuntime
{
    public class MeshVoxelizerRuntime : MeshVoxelizer
    {
        public bool showProgressBar = true;
        public bool compactOutput = true;
        public bool exportVoxelizedTexture = false;

        public void ApplySetting(MeshVoxelizerSetting setting)
        {
            generationType = setting.generationType;
            voxelSizeType = setting.voxelSizeType;
            subdivisionLevel = setting.subdivisionLevel;
            absoluteVoxelSize = setting.absoluteVoxelSize;
            precision = setting.precision;
            uvConversion = setting.uvConversion;
            edgeSmoothing = setting.edgeSmoothing;
            applyScaling = setting.applyScaling;
            alphaCutoff = setting.alphaCutoff;
            cutoffThreshold = setting.cutoffThreshold;
            modifyVoxel = setting.modifyVoxel;
            voxelMesh = setting.voxelMesh;
            voxelScale = setting.voxelScale;
            voxelRotation = setting.voxelRotation;
            boneWeightConversion = setting.boneWeightConversion;
            innerfaceCulling = setting.innerfaceCulling;
            backfaceCulling = setting.backfaceCulling;
            optimization = setting.optimization;
            fillCenter = setting.fillCenter;
            fillMethod = setting.fillMethod;
            centerMaterial = setting.centerMaterial;
            compactOutput = setting.compactOutput;
            showProgressBar = setting.showProgressBar;
        }

        public override GameObject VoxelizeMesh()
        {
            GameObject go = base.VoxelizeMesh();
            if (go != null)
            {
                go.transform.SetSiblingIndex(sourceGameObject.transform.GetSiblingIndex() + 1);
                go.transform.localPosition = sourceGameObject.transform.localPosition;
                go.transform.localRotation = sourceGameObject.transform.localRotation;
                if (!applyScaling) go.transform.localScale = sourceGameObject.transform.localScale;
            }

            // Nếu bạn muốn hiển thị progress bar tùy chỉnh, bạn có thể implement nó ở đây
            // Ví dụ: UpdateProgressBar("Voxelizing Mesh...", progress);

            return go;
        }

       /* protected override bool GenerateMeshMaterialsOpt()
        {
            // Thay thế việc tạo material và texture ở runtime
            m_result.voxelizedMaterials = new Material[m_source.materials.Length];
            for (int i = 0; i < m_source.materials.Length; ++i)
            {
                m_result.voxelizedMaterials[i] = new Material(m_source.materials[i]);
                m_result.voxelizedMaterials[i].name = m_source.materials[i].name + "_Voxelized";
            }

            Dictionary<Texture, Texture> texDict = new Dictionary<Texture, Texture>();
            foreach (var mat in m_result.voxelizedMaterials)
            {
                Shader shader = mat.shader;
                int propertyCount = ShaderUtil.GetPropertyCount(shader);
                for (int i = 0; i < propertyCount; i++)
                {
                    if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                    {
                        string tName = ShaderUtil.GetPropertyName(shader, i);
                        Texture tex = mat.GetTexture(tName);
                        if (tex != null && uvConversion == UVConversion.SourceMesh)
                        {
                            if (texDict.ContainsKey(tex))
                            {
                                mat.SetTexture(tName, texDict[tex]);
                            }
                            else
                            {
                                Texture newTex = m_opt.tInfo.CreateTexture(tex);
                                mat.SetTexture(tName, newTex);
                                texDict.Add(tex, newTex);
                            }
                        }
                    }
                }
            }

            return true;
        }*/

        protected override GameObject GenerateResult()
        {
            m_result.voxelizedMesh = new Mesh();
            m_result.voxelizedMesh.name = m_source.mesh.name + " Voxelized";
#if UNITY_2017_3_OR_NEWER
            if (m_result.vertices.Count > 65535) m_result.voxelizedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
#endif
            m_result.voxelizedMesh.SetVertices(m_result.vertices);
            m_result.voxelizedMesh.SetNormals(m_result.normals);
            m_result.voxelizedMesh.subMeshCount = m_result.triangles.Count;
            for (int i = 0; i < m_result.triangles.Count; ++i)
            {
                m_result.voxelizedMesh.SetTriangles(m_result.triangles[i], i);
            }
            m_result.voxelizedMesh.SetUVs(0, m_result.uv);
            if (m_result.uv2.Count != 0) m_result.voxelizedMesh.SetUVs(1, m_result.uv2);
            if (m_result.uv3.Count != 0) m_result.voxelizedMesh.SetUVs(2, m_result.uv3);
            if (m_result.uv4.Count != 0) m_result.voxelizedMesh.SetUVs(3, m_result.uv4);
            if (m_result.boneWeights.Count != 0)
            {
                m_result.voxelizedMesh.boneWeights = m_result.boneWeights.ToArray();
                m_result.voxelizedMesh.bindposes = m_source.mesh.bindposes;
            }

            GameObject go = new GameObject(m_result.voxelizedMesh.name);

            if (m_source.skinnedMeshRenderer != null)
            {
                SkinnedMeshRenderer skinnedMeshRenderer = go.AddComponent<SkinnedMeshRenderer>();
                skinnedMeshRenderer.sharedMesh = m_result.voxelizedMesh;
                skinnedMeshRenderer.sharedMaterials = m_result.voxelizedMaterials;
                skinnedMeshRenderer.bones = m_source.skinnedMeshRenderer.bones;
            }
            else
            {
                MeshFilter meshFilter = go.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = m_result.voxelizedMesh;

                MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterials = m_result.voxelizedMaterials;
            }

            return go;
        }

        protected override bool CancelProgress(string msg, float value)
        {
            // Implement hệ thống cancel progress tùy chỉnh nếu cần
            // Ví dụ: return CustomProgressBar.Cancel(msg, value);
            return false;
        }
    }
}

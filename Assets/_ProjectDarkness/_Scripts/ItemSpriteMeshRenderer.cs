using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectDarkness
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class ItemSpriteMeshRenderer : MonoBehaviour
    {
        [Header("Source")]
        [SerializeField] private Sprite _sprite;
        [SerializeField] private Material _materialOverride;

        [Header("Shape")]
        [SerializeField] [Min(0.01f)] private float _depthInPixels = 1f;
        [SerializeField] [Min(0.01f)] private float _scaleMultiplier = 1f;
        [SerializeField] [Range(0f, 1f)] private float _alphaCutoff = 0.1f;
        
        [Header("Build")]
        [SerializeField] private bool _buildOnStart = false;

        private const string RuntimeShaderName = "ProjectDarkness/ItemSpriteCutout";

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private Material _runtimeMaterial;
        private Material _runtimeOverrideMaterial;

        private static readonly Vector3 FrontNormal = Vector3.forward;
        private static readonly Vector3 BackNormal = Vector3.back;
        private static readonly Vector3 LeftNormal = Vector3.left;
        private static readonly Vector3 RightNormal = Vector3.right;
        private static readonly Vector3 TopNormal = Vector3.up;
        private static readonly Vector3 BottomNormal = Vector3.down;

        private void Awake()
        {
            EnsureComponents();
        }
        
        private void Start()
        {
            if (_buildOnStart)
            {
                Rebuild();
            }
        }

        [Button("Rebuild")]
        public void Rebuild()
        {
            EnsureComponents();

            if (_sprite == null)
            {
                ClearMesh();
                return;
            }

            Texture2D texture = _sprite.texture;
            if (texture == null)
            {
                ClearMesh();
                return;
            }

            try
            {
                texture.GetPixel(Mathf.RoundToInt(_sprite.rect.x), Mathf.RoundToInt(_sprite.rect.y));
            }
            catch (UnityException)
            {
                Debug.LogError($"Sprite texture '{texture.name}' must have Read/Write enabled for {nameof(ItemSpriteMeshRenderer)}.", this);
                ClearMesh();
                return;
            }

            Mesh mesh = BuildMesh(_sprite, texture);
            if (_meshFilter.sharedMesh != null)
            {
                Destroy(_meshFilter.sharedMesh);
            }

            _meshFilter.sharedMesh = mesh;
            _meshRenderer.sharedMaterial = GetOrCreateMaterial(texture);
        }

        private void OnDestroy()
        {
            if (_runtimeMaterial != null)
            {
                Destroy(_runtimeMaterial);
            }

            if (_runtimeOverrideMaterial != null)
            {
                Destroy(_runtimeOverrideMaterial);
            }

            if (_meshFilter != null && _meshFilter.sharedMesh != null)
            {
                Destroy(_meshFilter.sharedMesh);
            }
        }

        private void EnsureComponents()
        {
            if (!TryGetComponent(out _meshFilter))
            {
                _meshFilter = gameObject.AddComponent<MeshFilter>();
            }

            if (!TryGetComponent(out _meshRenderer))
            {
                _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            }

            _meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _meshRenderer.receiveShadows = false;
            _meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            _meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        }

        private void ClearMesh()
        {
            if (_meshFilter != null && _meshFilter.sharedMesh != null)
            {
                Destroy(_meshFilter.sharedMesh);
                _meshFilter.sharedMesh = null;
            }
        }

        private Mesh BuildMesh(Sprite sprite, Texture2D texture)
        {
            Rect rect = sprite.rect;
            int rectX = Mathf.RoundToInt(rect.x);
            int rectY = Mathf.RoundToInt(rect.y);
            int width = Mathf.RoundToInt(rect.width);
            int height = Mathf.RoundToInt(rect.height);

            Color[] spritePixels = texture.GetPixels(rectX, rectY, width, height);
            bool[] solidPixels = new bool[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    solidPixels[(y * width) + x] = spritePixels[(y * width) + x].a > _alphaCutoff;
                }
            }

            float pixelSize = (_scaleMultiplier / sprite.pixelsPerUnit);
            float halfDepth = pixelSize * _depthInPixels * 0.5f;

            Vector2 pivotPixels = sprite.pivot;
            float originX = pivotPixels.x * pixelSize;
            float originY = pivotPixels.y * pixelSize;

            List<Vector3> vertices = new();
            List<Vector2> uvs = new();
            List<Vector3> normals = new();
            List<int> triangles = new();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (!solidPixels[(y * width) + x])
                    {
                        continue;
                    }

                    float minX = (x * pixelSize) - originX;
                    float maxX = ((x + 1) * pixelSize) - originX;
                    float minY = (y * pixelSize) - originY;
                    float maxY = ((y + 1) * pixelSize) - originY;

                    AddQuad(
                        vertices,
                        uvs,
                        normals,
                        triangles,
                        new Vector3(minX, minY, halfDepth),
                        new Vector3(maxX, minY, halfDepth),
                        new Vector3(maxX, maxY, halfDepth),
                        new Vector3(minX, maxY, halfDepth),
                        FrontNormal,
                        rectX + x,
                        rectY + y,
                        texture);

                    AddQuad(
                        vertices,
                        uvs,
                        normals,
                        triangles,
                        new Vector3(maxX, minY, -halfDepth),
                        new Vector3(minX, minY, -halfDepth),
                        new Vector3(minX, maxY, -halfDepth),
                        new Vector3(maxX, maxY, -halfDepth),
                        BackNormal,
                        rectX + x,
                        rectY + y,
                        texture);

                    if (!IsSolid(solidPixels, width, height, x - 1, y))
                    {
                        AddQuad(
                            vertices,
                            uvs,
                            normals,
                            triangles,
                            new Vector3(minX, minY, -halfDepth),
                            new Vector3(minX, minY, halfDepth),
                            new Vector3(minX, maxY, halfDepth),
                            new Vector3(minX, maxY, -halfDepth),
                            LeftNormal,
                            rectX + x,
                            rectY + y,
                            texture);
                    }

                    if (!IsSolid(solidPixels, width, height, x + 1, y))
                    {
                        AddQuad(
                            vertices,
                            uvs,
                            normals,
                            triangles,
                            new Vector3(maxX, minY, halfDepth),
                            new Vector3(maxX, minY, -halfDepth),
                            new Vector3(maxX, maxY, -halfDepth),
                            new Vector3(maxX, maxY, halfDepth),
                            RightNormal,
                            rectX + x,
                            rectY + y,
                            texture);
                    }

                    if (!IsSolid(solidPixels, width, height, x, y - 1))
                    {
                        AddQuad(
                            vertices,
                            uvs,
                            normals,
                            triangles,
                            new Vector3(minX, minY, -halfDepth),
                            new Vector3(maxX, minY, -halfDepth),
                            new Vector3(maxX, minY, halfDepth),
                            new Vector3(minX, minY, halfDepth),
                            BottomNormal,
                            rectX + x,
                            rectY + y,
                            texture);
                    }

                    if (!IsSolid(solidPixels, width, height, x, y + 1))
                    {
                        AddQuad(
                            vertices,
                            uvs,
                            normals,
                            triangles,
                            new Vector3(minX, maxY, halfDepth),
                            new Vector3(maxX, maxY, halfDepth),
                            new Vector3(maxX, maxY, -halfDepth),
                            new Vector3(minX, maxY, -halfDepth),
                            TopNormal,
                            rectX + x,
                            rectY + y,
                            texture);
                    }
                }
            }

            Mesh mesh = new()
            {
                name = $"{sprite.name}_ItemSpriteMesh"
            };
            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uvs);
            mesh.SetNormals(normals);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        private Material GetOrCreateMaterial(Texture2D texture)
        {
            if (_materialOverride != null)
            {
                if (_runtimeOverrideMaterial == null || _runtimeOverrideMaterial.shader != _materialOverride.shader)
                {
                    if (_runtimeOverrideMaterial != null)
                    {
                        Destroy(_runtimeOverrideMaterial);
                    }

                    _runtimeOverrideMaterial = new Material(_materialOverride)
                    {
                        name = $"{_materialOverride.name} (Runtime)"
                    };
                }

                ApplyTextureToMaterial(_runtimeOverrideMaterial, texture);
                return _runtimeOverrideMaterial;
            }

            if (_runtimeMaterial == null)
            {
                Shader shader = Shader.Find(RuntimeShaderName);
                if (shader == null)
                {
                    Debug.LogError($"Could not find required shader '{RuntimeShaderName}' for {nameof(ItemSpriteMeshRenderer)}.", this);
                    return null;
                }

                _runtimeMaterial = new Material(shader)
                {
                    name = "Runtime Item Sprite Material"
                };
            }

            ApplyTextureToMaterial(_runtimeMaterial, texture);
            return _runtimeMaterial;
        }

        private void ApplyTextureToMaterial(Material material, Texture2D texture)
        {
            if (material == null)
            {
                return;
            }

            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;

            if (material.HasProperty("_BaseMap"))
            {
                material.SetTexture("_BaseMap", texture);
            }
            else
            {
                material.mainTexture = texture;
            }

            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", Color.white);
            }

            if (material.HasProperty("_Cutoff"))
            {
                material.SetFloat("_Cutoff", _alphaCutoff);
            }

            if (material.HasProperty("_Cull"))
            {
                material.SetFloat("_Cull", (float)UnityEngine.Rendering.CullMode.Back);
            }
        }

        private static bool IsSolid(bool[] solidPixels, int width, int height, int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
            {
                return false;
            }

            return solidPixels[(y * width) + x];
        }

        private static void AddQuad(
            List<Vector3> vertices,
            List<Vector2> uvs,
            List<Vector3> normals,
            List<int> triangles,
            Vector3 bottomLeft,
            Vector3 bottomRight,
            Vector3 topRight,
            Vector3 topLeft,
            Vector3 normal,
            int pixelX,
            int pixelY,
            Texture2D texture)
        {
            Vector2 uvMin = GetUvMin(pixelX, pixelY, texture);
            Vector2 uvMax = GetUvMax(pixelX, pixelY, texture);

            int start = vertices.Count;
            vertices.Add(bottomLeft);
            vertices.Add(bottomRight);
            vertices.Add(topRight);
            vertices.Add(topLeft);

            uvs.Add(new Vector2(uvMin.x, uvMin.y));
            uvs.Add(new Vector2(uvMax.x, uvMin.y));
            uvs.Add(new Vector2(uvMax.x, uvMax.y));
            uvs.Add(new Vector2(uvMin.x, uvMax.y));

            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);

            triangles.Add(start + 0);
            triangles.Add(start + 1);
            triangles.Add(start + 2);
            triangles.Add(start + 0);
            triangles.Add(start + 2);
            triangles.Add(start + 3);
        }

        private static Vector2 GetUvMin(int pixelX, int pixelY, Texture2D texture)
        {
            float u = pixelX / (float)texture.width;
            float v = pixelY / (float)texture.height;
            return new Vector2(u, v);
        }

        private static Vector2 GetUvMax(int pixelX, int pixelY, Texture2D texture)
        {
            float u = (pixelX + 1) / (float)texture.width;
            float v = (pixelY + 1) / (float)texture.height;
            return new Vector2(u, v);
        }
    }
}

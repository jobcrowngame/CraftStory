using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CombineMeshObj : MonoBehaviour
{
    Dictionary<int, Dictionary<Vector3, CombineInstance>> combine = new Dictionary<int, Dictionary<Vector3, CombineInstance>>();
    Dictionary<int, Material> materials = new Dictionary<int, Material>();
    List<Mesh> meshObjs = new List<Mesh>();

    public void AddObj(int key, Mesh mesh, Material material, Vector3 pos)
    {
        if (!materials.ContainsKey(key))
        {
            materials[key] = material;
        }

        Matrix4x4 matri = new Matrix4x4(new Vector4(1, 0, 0, 0), new Vector4(0, 1, 0, 0), new Vector4(0, 0, 1, 0), new Vector4(pos.x, pos.y, pos.z, 0));
        CombineInstance instance = new CombineInstance();
        instance.transform = matri;
        instance.mesh = mesh;

        if (!combine.ContainsKey(key))
            combine[key] = new Dictionary<Vector3, CombineInstance>();
        
        combine[key][pos] = instance;
    }

    public void RemoveMesh(int key, Vector3 pos)
    {
        combine[key].Remove(pos);
    }

    public void Combine()
    {
        foreach (var item in meshObjs)
        {
            Destroy(item);
        }
        meshObjs.Clear();

        foreach (var k in combine.Keys)
        {
            var obj = CreateMeshObj(k.ToString());
            var mesh = obj.AddComponent<MeshFilter>();
            mesh.mesh = new Mesh();
            mesh.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.mesh.CombineMeshes(combine[k].Values.ToArray());
            meshObjs.Add(mesh.mesh);

            var renderer = obj.AddComponent<MeshRenderer>();
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.material = materials[k];
        }
    }

    public void Clear()
    {
        combine.Clear();
        materials.Clear();
        meshObjs.Clear();
    }
  
    /// <Summary>
    /// 結合したメッシュを表示するGameObjectを作成します。
    /// </Summary>
    GameObject CreateMeshObj(string matName)
    {
        GameObject obj = new GameObject();
        obj.name = $"CombinedMesh_{matName}";
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;
        return obj;
    }
}
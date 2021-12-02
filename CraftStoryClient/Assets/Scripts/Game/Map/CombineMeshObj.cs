using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CombineMeshObj : MonoBehaviour
{
    Dictionary<int, Dictionary<Vector3, CombineInstance>> combine = new Dictionary<int, Dictionary<Vector3, CombineInstance>>();
    Dictionary<int, Material> materials = new Dictionary<int, Material>();
    Dictionary<int, GameObject> meshObjs = new Dictionary<int, GameObject>();

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

    public void RemoveObj(int key, Vector3 pos)
    {
        combine[key].Remove(pos);
        Combine();
    }

    public void Combine()
    {
        foreach (var k in combine.Keys)
        {
            if (meshObjs.ContainsKey(k))
            {
                Destroy(meshObjs[k].gameObject);
                meshObjs.Remove(k);
            }

            var obj = CreateMeshObj(k.ToString());
            meshObjs[k] = obj;

            var mesh = obj.AddComponent<MeshFilter>();
            var renderer = obj.AddComponent<MeshRenderer>();

            mesh.mesh = new Mesh();
            mesh.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.mesh.CombineMeshes(combine[k].Values.ToArray());

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
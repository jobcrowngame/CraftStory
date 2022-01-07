using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CombineMeshCtl : MonoBehaviour
{
    Dictionary<int, Dictionary<Vector3Int, CombineInstance>> combine = new Dictionary<int, Dictionary<Vector3Int, CombineInstance>>();
    Dictionary<int, Material> materials = new Dictionary<int, Material>();
    List<Mesh> meshList = new List<Mesh>();
    List<GameObject> meshObj = new List<GameObject>();

    public void AddObj(int key, Mesh mesh, Material material, Vector3Int pos, Direction direction)
    {
        if (!materials.ContainsKey(key))
        {
            materials[key] = material;
        }

        Matrix4x4 matri = GetMatrix4x4ByDir(direction, pos);
        CombineInstance instance = new CombineInstance();
        instance.transform = matri;
        instance.mesh = mesh;

        if (!combine.ContainsKey(key))
            combine[key] = new Dictionary<Vector3Int, CombineInstance>();
        
        combine[key][pos] = instance;
    }

    public void RemoveMesh(int key, Vector3Int pos)
    {
        if (combine.ContainsKey(key))
        {
            combine[key].Remove(pos);
        }
    }

    public void Combine()
    {
        foreach (var item in meshList)
        {
            Destroy(item);
        }
        meshList.Clear();
        DestroyMeshObj();

        foreach (var k in combine.Keys)
        {
            var CombinedMeshObj = CreateMeshObj(k.ToString());
            var mesh = CombinedMeshObj.AddComponent<MeshFilter>();
            mesh.mesh = new Mesh();
            mesh.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.mesh.CombineMeshes(combine[k].Values.ToArray());
            meshList.Add(mesh.mesh);

            var renderer = CombinedMeshObj.AddComponent<MeshRenderer>();
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            renderer.receiveShadows = true;
            renderer.material = materials[k];

            meshObj.Add(CombinedMeshObj);
        }
    }

    public void Clear()
    {
        combine.Clear();
        materials.Clear();
        meshList.Clear();
    }
    private void DestroyMeshObj()
    {
        foreach (var item in meshObj)
        {
            Destroy(item);
        }
        meshObj.Clear();
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

    private Matrix4x4 GetMatrix4x4ByDir(Direction direction, Vector3 pos)
    {
        Vector4 v1 = Vector4.zero;
        Vector4 v2 = Vector4.zero;
        Vector4 v3 = Vector4.zero;
        Vector4 v4 = new Vector4(pos.x, pos.y, pos.z, 0);

        switch (direction)
        {
            case Direction.back:
                v1 = new Vector4(-1, 0, 0, 0);
                v2 = new Vector4(0, 1, 0, 0);
                v3 = new Vector4(0, 0, -1, 0);
                break;

            case Direction.right:
                v1 = new Vector4(0, 0, -1, 0);
                v2 = new Vector4(0, 1, 0, 0);
                v3 = new Vector4(1, 0, 0, 0);
                break;

            case Direction.left:
                v1 = new Vector4(0, 0, 1, 0);
                v2 = new Vector4(0, 1, 0, 0);
                v3 = new Vector4(-1, 0, 0, 0);
                break;

            default:
                v1 = new Vector4(1, 0, 0, 0);
                v2 = new Vector4(0, 1, 0, 0);
                v3 = new Vector4(0, 0, 1, 0);
                break;
        }

        return new Matrix4x4(v1, v2, v3, v4);
    }
}
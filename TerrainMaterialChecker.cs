using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class TerrainMaterialChecker : MonoBehaviour
{
    public Material placeholderMaterial; // 拖入你创建的灰色材质

    void Start()
    {
        Terrain terrain = GetComponent<Terrain>();
        if (terrain.materialTemplate == null)
        {
            Debug.LogWarning("Terrain material is missing! Applying placeholder material.");
            terrain.materialTemplate = placeholderMaterial;
        }
    }
}
using UnityEngine;

public class GroupManager : MonoBehaviour
{
    // Colors for different statuses
    private Material defaultMaterial;
    private Material completeMaterial;

    public Renderer[] voxelRenderers;

    public void Init(Material defaultMaterial, Material completeMaterial)
    {
        this.defaultMaterial = defaultMaterial;
        this.completeMaterial = completeMaterial;
        voxelRenderers = GetComponentsInChildren<Renderer>();
        SetDefaultMaterial();
    }

    // Method to set the default Material
    public void SetDefaultMaterial()
    {
        SetGroupMaterial(defaultMaterial);
    }

    // Method to set the Complete Material
    public void SetCompleteMaterial()
    {
        SetGroupMaterial(completeMaterial);
    }

    // Method to set the Hide Material
    public void SetHideMaterial()
    {
        SetGroupMaterial(null);
    }

    // Helper method to change Material of all voxels in the group
    private void SetGroupMaterial(Material material)
    {
        foreach (Renderer renderer in voxelRenderers)
        {
            if(material == null)
            {
                renderer.enabled = false;
            }
            else
            {
                renderer.enabled = true;
                renderer.material = material;
            }
        }
    }
}

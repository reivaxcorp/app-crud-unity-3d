using System.Threading.Tasks;
using UnityEngine;


public class BuildItem: MonoBehaviour
{

    // Get Texture saved in local device
    public Texture2D GetSavedTexture(string imageName)
    {
        return MyApplication.repository.LoadTextureAsPNG(imageName);
    }

    public async Task AsignMaterialAsync(string imageName, GameObject gameObject)
    {
       
        Texture2D texture2D = GetSavedTexture(imageName);

        if(texture2D == null)
        {
            ManageMaterialRemote createMaterial = new ManageMaterialRemote(imageName);
            texture2D = await createMaterial.DownloadImage();
            MyApplication.repository.SaveTextureAsPNG(texture2D, imageName);
        }

        Material newMaterial = new Material(Shader.Find("Standard"));
        newMaterial.mainTexture = texture2D;
        newMaterial.SetTexture("_MainTex", texture2D);

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        Material[] currentMaterials = meshRenderer.materials;
        currentMaterials[0] = newMaterial;
        meshRenderer.materials = currentMaterials;
        gameObject.SetActive(true);
    }
}

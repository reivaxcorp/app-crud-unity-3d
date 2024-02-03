using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class BuildItem: MonoBehaviour
{

    // Get Texture saved in local device
    public Texture2D GetSavedTexture(string idProduct)
    {
        return MyApplication.repository.LoadTextureAsPNG(idProduct);
    }

    public async Task AsignMaterialAsync(string idProduct, GameObject gameObject, string uri)
    {
        ManageMaterialRemote createMaterial = new ManageMaterialRemote(uri);

        Texture2D texture2D = GetSavedTexture(idProduct);

        if(texture2D == null)
        {
            texture2D = await createMaterial.DownloadImage();
            MyApplication.repository.SaveTextureAsPNG(texture2D, idProduct);
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

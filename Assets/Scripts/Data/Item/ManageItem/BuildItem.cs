using System.Threading.Tasks;
using UnityEngine;


public class BuildItem: MonoBehaviour
{

    // Get Texture saved in local device
    public Texture2D GetSavedTexture(string imageName)
    {
        FileManager fileManager = new FileManager(FirebaseSDK.GetInstance().auth.CurrentUser.UserId);
        return fileManager.LoadFileAsTexture2D(imageName);
    }

    public async Task AsignMaterialAsync(string imageName, GameObject gameObject)
    {
       
        Texture2D texture2D = GetSavedTexture(imageName + ".png");

        // si no esta la imagen, es que se actualizo anteriormente en otro dispositivo
        if(texture2D == null)
        {
            ManageTextureRemote createMaterial = new ManageTextureRemote(imageName);
            texture2D = await createMaterial.DownloadImage();
            FileManager fileManager = new FileManager(FirebaseSDK.GetInstance().auth.CurrentUser.UserId);
            fileManager.SaveFileInternalExtorage(texture2D, imageName);
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

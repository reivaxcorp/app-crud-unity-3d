using System;
using System.Threading.Tasks;
using UnityEngine;


public class BuildItem: MonoBehaviour
{
    public ButtonBoxUiManager buttonBoxUiManager;
    
    public async Task AsignMaterialAsync(string imageName, GameObject cubo)
    {
       
        Texture2D texture2D = GetSavedTexture(imageName);

        // si no esta la imagen, es que se actualizo anteriormente en otro dispositivo
        if(texture2D == null)
        {
            texture2D = await MyApplication.repository.DowloadImageStorage(imageName);

            if(texture2D != null)
            {
                FileManager fileManager = new FileManager(FirebaseSDK.GetInstance().auth.CurrentUser.UserId);
                fileManager.SaveFileInternalExtorage(texture2D, imageName);
            } else
            {
                Debug.LogWarning("Error al bajar la imagen de Firebase Storage");
            }
        }

        try
        {

            Material newMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            newMaterial.mainTexture = texture2D;
            newMaterial.SetTexture("_MainTex", texture2D);
        
            MeshRenderer meshRenderer = cubo.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                Material[] currentMaterials = meshRenderer.materials;
                currentMaterials[0] = newMaterial;
                meshRenderer.materials = currentMaterials;
            }

            FillBoxUi(texture2D, cubo.name);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Error al aplicar la textura " + ex.Message);
        }

    }
 
    // Obtiene la textura desde nuestro dispositivo interno
    private Texture2D GetSavedTexture(string imageName)
    {
        FileManager fileManager = new FileManager(FirebaseSDK.GetInstance().auth.CurrentUser.UserId);
        return fileManager.LoadFileAsTexture2D(imageName);
    }

    public void FillBoxUi(Texture2D textureSlot, string slotNumber)
    {
        if(buttonBoxUiManager == null) { 
            Debug.LogWarning("Referencia no colocada");
            return; 
        }
        buttonBoxUiManager.FillBoxUi(textureSlot, slotNumber);
    }
}

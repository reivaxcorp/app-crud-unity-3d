using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BuildItem : MonoBehaviour
{
    public ButtonBoxUiManager buttonBoxUiManager;

    // Diccionario para persistencia en memoria: Key = slotId, Value = imageName
    private Dictionary<string, string> _slotImageMap = new Dictionary<string, string>();

    // Get y Set para acceder desde otras clases (ManageItems, etc.)
    public Dictionary<string, string> SlotImageMap
    {
        get => _slotImageMap;
        set => _slotImageMap = value;
    }

    public async Task AsignMaterialAsync(string imageName, GameObject cubo)
    {
        Texture2D texture2D = GetSavedTexture(imageName);
        Debug.Log("Image name " + imageName);
        if (texture2D == null)
        {
            texture2D = await MyApplication.repository.DowloadImageStorage(imageName);

            if (texture2D != null)
            {
                FileManager fileManager = new FileManager(FirebaseSDK.GetInstance().auth.CurrentUser.UserId);
                fileManager.SaveFileInternalExtorage(texture2D, imageName);
            }
            else
            {
                Debug.LogWarning("Error al bajar la imagen de Firebase Storage");
                return;
            }
        }

        try
        {
            // Aplicar al cubo 3D siempre (por si es una instancia nueva)
            Material newMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            newMaterial.mainTexture = texture2D;
            newMaterial.SetTexture("_BaseMap", texture2D); // URP estándar

            MeshRenderer meshRenderer = cubo.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material = newMaterial;
            }

            // --- LÓGICA DE OPTIMIZACIÓN DE UI ---
            string slotId = cubo.name;

            // Solo actualizamos la UI si es la primera vez o si la imagen cambió
            if (!_slotImageMap.ContainsKey(slotId) || _slotImageMap[slotId] != imageName)
            {
                // Actualizamos el mapa con el nuevo nombre
                _slotImageMap[slotId] = imageName;

                // Refrescamos el slot de la UI
                FillBoxUi(texture2D, slotId);
                Debug.Log($"UI Actualizada: Slot {slotId} ahora tiene {imageName}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Error al aplicar la textura " + ex.Message);
        }
    }

    private Texture2D GetSavedTexture(string imageName)
    {
        FileManager fileManager = new FileManager(FirebaseSDK.GetInstance().auth.CurrentUser.UserId);
        return fileManager.LoadFileAsTexture2D(imageName);
    }

    public void FillBoxUi(Texture2D textureSlot, string slotNumber)
    {
        if (buttonBoxUiManager == null) return;
        buttonBoxUiManager.FillBoxUi(textureSlot, slotNumber);
    }

    // Método extra para obtener el nombre de imagen de un slot desde afuera
    public string GetImageNameBySlot(string slotId)
    {
        return _slotImageMap.ContainsKey(slotId) ? _slotImageMap[slotId] : null;
    }
}
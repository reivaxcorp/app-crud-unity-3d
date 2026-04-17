using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Text.RegularExpressions;

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
        //Debug.Log("Image name " + imageName);
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

            BuildItemUi(imageName, cubo.name, texture2D);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Error al aplicar la textura " + ex.Message);
        }
    }

    /// <param name="imageName">Nombre del archivo de imagen (ej: "foto1.jpg")</param>
    /// <param name="mainItemId">El ID que viene del nombre del objeto (debe ser "1" al "10")</param>
    /// <param name="texture2D">La textura ya cargada en memoria</param>
    private void BuildItemUi(string imageName, string mainItemId, Texture2D texture2D)
    {
        // --- LÓGICA DE OPTIMIZACIÓN DE UI (ESTRICTA) ---

        // 1. Validamos que el string sea EXACTAMENTE un número y nada más.
        // int.TryParse fallará si el string contiene "slot_" o cualquier letra.
        if (int.TryParse(mainItemId, out int slotNumber))
        {
            // 2. Verificamos que esté en el rango de tus 10 slots base
            if (slotNumber >= 1 && slotNumber <= 10)
            {
                // Solo llegamos aquí si mainItemId es exactamente "1", "2", etc.

                // 3. Comprobamos si la imagen cambió para no estresar la UI
                if (!_slotImageMap.ContainsKey(mainItemId) || _slotImageMap[mainItemId] != imageName)
                {
                    // Actualizamos el mapa de memoria
                    _slotImageMap[mainItemId] = imageName;

                    // Refrescamos el componente Image de la UI
                    FillBoxUi(texture2D, mainItemId);

                    Debug.Log($"<color=green>SISTEMA:</color> UI Slot {mainItemId} actualizado con {imageName}");
                }
            }
        }
        else
        {
            // Esto se ejecutará para "slot_1", "copy_1", "Suelo", etc.
            // Debug.Log($"Ignorado para UI: {mainItemId}"); 
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
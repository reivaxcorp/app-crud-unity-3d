using UnityEngine;
using UnityEngine.UI; // Indispensable para el componente Image
using System.Linq; // Para buscar fácil en la lista

public class ButtonBoxUiManager : MonoBehaviour
{
    [SerializeField] private BuildManager buildManager;
    public void OnClickBox(string slotName, Texture2D selectedTexture)
    {
        Debug.Log("Slot seleccionado: " + slotName);

        if (buildManager != null)
        {
            // Le decimos al BuildManager que prepare el cubo con esta textura
            // slotName será "1", "2", etc., según el nombre del objeto UI
            buildManager.PrepareCube(slotName, selectedTexture);
        }
    }

    public void FillBoxUi(Texture2D textureSlot, string slotNumber)
    {
        // 1. Buscamos el objeto del slot por nombre (ej: "Slot1", "Slot2" o solo "1")
        // Asumo que tus botones de la UI tienen nombres que contienen el número
        Transform slotTransform = transform.Find("slot_" + slotNumber);

        if (slotTransform != null)
        {
            // 2. Buscamos el componente Image en ese slot (o en sus hijos)
            Image imgComponent = slotTransform.GetComponentInChildren<Image>();

            if (imgComponent != null && textureSlot != null)
            {
                // 3. ¡LA CLAVE!: Convertimos Texture2D a Sprite
                // Rect define qué parte de la textura usar (toda), y el Pivot el centro (0.5f)
                Sprite newSprite = Sprite.Create(
                    textureSlot,
                    new Rect(0, 0, textureSlot.width, textureSlot.height),
                    new Vector2(0.5f, 0.5f)
                );

                imgComponent.sprite = newSprite;
                imgComponent.color = Color.white; // Aseguramos que sea visible
                Debug.Log($"SISTEMA: Slot {slotNumber} actualizado con éxito.");
            }
        }
        else
        {
            Debug.LogWarning($"SISTEMA: No se encontró el objeto de UI para el slot: {slotNumber}");
        }

    }
}

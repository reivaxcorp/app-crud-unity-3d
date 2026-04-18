using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBoxUi : MonoBehaviour
{
    [Header("Construcción")]
    public float castDelay = 0.5f; // Tiempo de espera entre cubos (puedes subirlo a 1.0f)
    private float _nextCastTime = 0f;

    void Update()
    {
        // Solo escuchamos el teclado si ya pasó el tiempo de espera
        if (Time.time >= _nextCastTime)
        {
            HandleKeyboardInput();
        }
    }

    private void HandleKeyboardInput()
    {

        string keyPress = gameObject.name.Split('_')[1];
        if (int.TryParse(keyPress, out int slotNumber))
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + slotNumber)) // Alpha1, Alpha2, etc.
            {
                ExecuteBuild(slotNumber);
            }
        }
        
        // Caso especial para el 0 (que mapea al slot 10)
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ExecuteBuild(10);
        }

        // --- Lógica de Borrado (Tecla | / º / ª) ---
        // Usamos BackQuote que es la tecla a la izquierda del 1
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            ExecuteDelete();
        }
    }

    private void ExecuteBuild(int index)
    {
        // 1. Buscamos y disparamos el botón
        if (TriggerSlotButton(index))
        {
            // 2. Si tuvo éxito (había un slot válido), seteamos el próximo tiempo permitido
            // Time.time es el tiempo transcurrido desde que inició el juego
            _nextCastTime = Time.time + castDelay;

            Debug.Log($"<color=yellow>Cooldown:</color> Esperando {castDelay}s para el próximo cubo.");
        }
    }

    private void ExecuteDelete()
    {
        // 1. Buscamos el BuildManager en la escena
        // (Podrías tener una referencia directa por Inspector para que sea más rápido)
        BuildManager buildManager = FindAnyObjectByType<BuildManager>();

        if (buildManager != null)
        {
            buildManager.ActionDelete();

            // Aplicamos el mismo cooldown que para construir
            _nextCastTime = Time.time + castDelay;

            Debug.Log("<color=red>Acción:</color> Borrar bloque (Teclado)");
        }
    }

    private bool TriggerSlotButton(int index)
    {
        string targetName = "slot_" + index;
        Transform slotTransform = gameObject.transform.parent.transform.Find(targetName);

        if (slotTransform != null)
        {
            ButtonBoxUi script = slotTransform.GetComponent<ButtonBoxUi>();
            if (script != null)
            {
                script.OnButtonClick();
                return true;
            }
        }
        return false;
    }

    public void OnButtonClick()
    {
        Image _myImage = GetComponent<Image>();
        // 1. Verificamos que haya un sprite asignado
        if (_myImage != null && _myImage.sprite != null)
        {
            // 2. Extraemos la Texture2D del sprite
            Texture2D texture2d = _myImage.sprite.texture;

            // 3. Enviamos el nombre y la textura al Manager
            GetComponentInParent<ButtonBoxUiManager>()
                 .OnClickBox(this.gameObject.name, texture2d);
        }
        else
        {
            Debug.LogWarning($"El slot {gameObject.name} no tiene una imagen cargada aún.");
        }
    }
}

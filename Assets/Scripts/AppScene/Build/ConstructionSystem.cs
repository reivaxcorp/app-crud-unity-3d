using UnityEngine;

public class ConstructionSystem : MonoBehaviour
{
    [Header("Configuración")]
    public float buildDistance = 10f;
    public LayerMask buildableLayers; // Incluye el suelo y los propios cubos
    public GameObject cubePrefab;

    public void PlaceBlock(string slotId, Texture2D texture)
    {
        // 1. Lanzamos el rayo desde el centro de la cámara
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, buildDistance, buildableLayers))
        {
            // 2. Calculamos la posición adyacente basada en la "Normal" de la cara impactada
            // La normal es la dirección hacia donde apunta la cara del cubo o suelo tocado
            Vector3 spawnPosition = hit.point + (hit.normal * 0.5f);

            // 3. ¡EL TRUCO DEL SNAPPING!: Redondeamos los ejes Y y Z (y X también para ser precisos)
            // Esto asegura que el primer bloque en el suelo y los siguientes queden en la rejilla
            float snappedX = Mathf.Round(spawnPosition.x);
            float snappedY = Mathf.Round(spawnPosition.y);
            float snappedZ = Mathf.Round(spawnPosition.z);

            Vector3 finalPos = new Vector3(snappedX, snappedY, snappedZ);

            // 4. Instanciamos el cubo real
            GameObject newCube = Instantiate(cubePrefab, finalPos, Quaternion.identity);

            // 5. Aplicamos la textura del slot
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            mat.mainTexture = texture;
            newCube.GetComponent<MeshRenderer>().material = mat;

            // Asignamos el nombre para el sistema de guardado local
            newCube.name = slotId;

            Debug.Log($"Bloque {slotId} colocado en {finalPos}");
        }
    }
}
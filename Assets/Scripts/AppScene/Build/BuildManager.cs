using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class BuildManager : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject cubePrefab; // El prefab genérico del cubo
    public Transform spawnPoint;  // El punto arriba del PJ
    public LayerMask buildLayer;  // Capa de los cubos y suelo

    private GameObject _previewCube;
    private string _currentSlotId;
    private Texture2D _currentTexture;

    // Lista local de cubos para el JSON
    private List<GameObject> _instantiatedCubes = new List<GameObject>();

    public void PrepareCube(string slotId, Texture2D tex)
    {
        if (_previewCube != null) Destroy(_previewCube);

        _currentSlotId = slotId;
        _currentTexture = tex;

        // Creamos la "vista previa" arriba del PJ
        _previewCube = Instantiate(cubePrefab, spawnPoint.position, Quaternion.identity);
        _previewCube.transform.SetParent(spawnPoint);

        // Aplicamos la textura al preview
        _previewCube.GetComponent<MeshRenderer>().material.mainTexture = tex;

        // Deshabilitamos colisiones del preview para que no empuje al PJ
        _previewCube.GetComponent<Collider>().enabled = false;
    }

    public void ActionPlace()
    {
        if (_previewCube == null) return;

        // Lanzamos Raycast desde el centro de la cámara
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f, buildLayer))
        {
            // Lógica de Snapping (Ajuste a la rejilla)
            Vector3 spawnPos = hit.point + hit.normal * 0.5f;
            spawnPos = new Vector3(Mathf.Round(spawnPos.x), Mathf.Round(spawnPos.y), Mathf.Round(spawnPos.z));

            GameObject newCube = Instantiate(cubePrefab, spawnPos, Quaternion.identity);
            newCube.GetComponent<MeshRenderer>().material.mainTexture = _currentTexture;
            newCube.name = _currentSlotId; // Guardamos el ID en el nombre para saber cuál es

            newCube.GetComponent<Collider>().enabled = true;

            _instantiatedCubes.Add(newCube);
            Debug.Log("Cubo colocado en: " + spawnPos);
        }
    }

    public void ActionDelete()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f, buildLayer))
        {
            // Si tocamos un cubo que no es el suelo
            if (hit.collider.gameObject != null && hit.collider.gameObject.CompareTag("Player") == false)
            {
                GameObject target = hit.collider.gameObject;
                _instantiatedCubes.Remove(target);
                Destroy(target);
            }
        }
    }

    // Al cerrar o guardar, generaríamos el JSON recorriendo _instantiatedCubes
    public void SaveWorld()
    {
        WorldSaveData data = new WorldSaveData();

        foreach (GameObject cube in _instantiatedCubes)
        {
            data.placedCubes.Add(new CubeData
            {
                slotId = cube.name, // El nombre que le pusimos al instanciarlo
                position = cube.transform.position,
                rotation = cube.transform.rotation
            });
        }

        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, "world_save.json");
        File.WriteAllText(path, json);

        Debug.Log("Mundo guardado localmente en: " + path);
    }

    public void LoadWorld()
    {
        string path = Path.Combine(Application.persistentDataPath, "world_save.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            WorldSaveData data = JsonUtility.FromJson<WorldSaveData>(json);

            // Limpiar mundo actual antes de cargar
            foreach (GameObject cube in _instantiatedCubes) Destroy(cube);
            _instantiatedCubes.Clear();

            foreach (CubeData cubeData in data.placedCubes)
            {
                // Aquí necesitarías una referencia a tus texturas descargadas
                // para volver a aplicarlas según el slotId
                GameObject newCube = Instantiate(cubePrefab, cubeData.position, cubeData.rotation);
                newCube.name = cubeData.slotId;

                // Re-aplicar textura (tendrías que llamar a tu lógica de BuildItem aquí)
                _instantiatedCubes.Add(newCube);
            }
            Debug.Log("Mundo cargado con éxito.");
        }

    }
}
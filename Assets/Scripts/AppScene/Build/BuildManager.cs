using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    [SerializeField] MenuDialogConfirm menuDialog;
    [Header("Configuración")]
    public GameObject cubePrefab; // El prefab genérico del cubo
    public Transform spawnPoint;  // El punto arriba del PJ
    public LayerMask buildLayer;  // Capa de los cubos y suelo
    public GameObject parentLocalItemWorld;
    private GameObject _previewCube;
    private string _currentSlotId;
    private Texture2D _currentTexture;
    private BuildItem _buildItem;

    // Lista local de cubos para el JSON
    private List<GameObject> _instantiatedCubes = new List<GameObject>();
    // Cambiamos la lista a pública para que ManageItems la vea o creamos un método
    public List<GameObject> GetInstantiatedCubes() => _instantiatedCubes;

    private void Start()
    {
        _buildItem = GetComponent<BuildItem>();
        if (parentLocalItemWorld == null) Debug.LogWarning("Coloca la referencia de los items copia del mundo");
        if (menuDialog == null) Debug.LogWarning("Coloca la referencia del dialogo");
    }

    public void PrepareCube(string slotId, Texture2D tex)
    {
        if (_previewCube != null && slotId == _currentSlotId)
        {
            ActionPlace();
        } else
        {

            if(_previewCube != null) 
                Destroy(_previewCube);

            _currentSlotId = slotId;
            _currentTexture = tex;

            // Creamos la "vista previa" arriba del PJ
            _previewCube = Instantiate(cubePrefab, spawnPoint.position, Quaternion.identity);
            _previewCube.transform.SetParent(spawnPoint);

            // Aplicamos la textura al preview
            _previewCube.GetComponent<MeshRenderer>().material.mainTexture = tex;

            // Deshabilitamos colisiones del preview para que no empuje al PJ
            _previewCube.GetComponent<Collider>().enabled = false;
            _previewCube.GetComponent<Rigidbody>().isKinematic = true;

        }
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
           
            newCube.GetComponent<Rigidbody>().isKinematic = true;
            newCube.GetComponent<Collider>().enabled = true;
            _instantiatedCubes.Add(newCube);
           
            newCube.transform.SetParent(parentLocalItemWorld.transform);
            Debug.Log("Cubo colocado en: " + spawnPos);
        }
    }

    public void ActionDelete()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f, buildLayer))
        {
            GameObject target = hit.collider.gameObject;

            // no eliminemos el terreno 
            if (target.GetComponent<Terrain>() != null) return;

            // ¿Es un Main Item? (Tienen el Tag o están en itemSceneConfig)
            if (target.transform.parent != null && target.transform.parent.name == "ItemSceneConfig")
            {
                menuDialog.ShowDialog("Shoot to Edit", "You can’t delete the original—shoot it to remove it.");
            }
            else
            {
                // Es una copia local
                _instantiatedCubes.Remove(target);
                Destroy(target);

                // podria ir SaveWorld() 
                // pero para optimizar agregamos un boton
                // SaveWorld(); // Guardamos el cambio en el JSON
            }
        }
    }

    // Función para borrar todos los clones de un ID específico
    // podria ir SaveWorld() al final, pero para optimizar agregamos un boton
    // save world
    public void DeleteAllClonesOfId(string slotId)
    {
        // Buscamos de atrás para adelante para no romper el índice al borrar
        for (int i = _instantiatedCubes.Count - 1; i >= 0; i--)
        {
            if (_instantiatedCubes[i].name == "copy_slot_"+slotId)
            {
                GameObject toDestroy = _instantiatedCubes[i];
                _instantiatedCubes.RemoveAt(i);
                Destroy(toDestroy);
            }
        }
        // SaveWorld();
    }

    // Función para actualizar la textura de todos los clones cuando el original cambia en Firebase
    public async Task UpdateAllClonesTexture(string itemId, string imageName)
    {
        foreach (GameObject cube in _instantiatedCubes)
        {
            // item id se repeta el 1, 2. 3, ...
            // item copia posee slot_1, slot_2, slot_..
            if (cube.name == "copy_slot_"+ itemId)
            {
                // Usamos el componente BuildItem que ya sabe manejar la descarga
                await GetComponent<BuildItem>().AsignMaterialAsync(imageName, cube);
            }
        }
    }

    public void StartSaveWorld()
    {
        StartCoroutine(SaveWorldCoroutine());
    }

    // Al cerrar o guardar, generaríamos el JSON recorriendo _instantiatedCubes
    private IEnumerator SaveWorldCoroutine()
    {
        // 1. Mostramos el mensaje inicial
        menuDialog.ShowDialog("Save World", "Save world...");

        // 2. Esperamos al final del frame para que Unity dibuje el texto en pantalla
        yield return new WaitForEndOfFrame();

        // 3. Procesamos los datos
        WorldSaveData data = new WorldSaveData();
        foreach (GameObject cube in _instantiatedCubes)
        {
            data.placedCubes.Add(new CubeData
            {
                slotId = cube.name,
                position = cube.transform.position,
                rotation = cube.transform.rotation
            });
        }

        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, "world_save.json");

        // 4. Escribimos el archivo
        File.WriteAllText(path, json);

        Debug.Log("Mundo guardado en: " + path);

        // 5. Mostramos el mensaje final
        menuDialog.SetBodyText("World saved");
    }

    public async Task<bool> LoadLocalWorld()
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
                newCube.GetComponent<Rigidbody>().isKinematic = true;
                newCube.GetComponent<BoxCollider>().enabled = true;

                // Re-aplicar textura (tendrías que llamar a tu lógica de BuildItem aquí)
                _instantiatedCubes.Add(newCube);
                string imageName = _buildItem.GetImageNameBySlot(cubeData.slotId.Split('_')[1]);
                await _buildItem.AsignMaterialAsync(imageName, newCube);
                newCube.transform.SetParent(parentLocalItemWorld.transform);
              //  _manageItems.getItemConfig.EnablePhysicsItem(newCube);
            }

            Debug.Log("Mundo cargado con éxito.");
            return true;
        } else
        {
            Debug.Log("No hay mundo previo.");
            return false;
        }

    }
}
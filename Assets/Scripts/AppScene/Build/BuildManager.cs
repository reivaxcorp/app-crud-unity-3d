using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class BuildManager : MonoBehaviour, IItemManager
{

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buildSound;

    [SerializeField] MenuDialogConfirm menuDialog;
    [SerializeField] GameObject buttonDelete;
    [Header("Configuración")]
    public GameObject cubePrefab; // El prefab genérico del cubo
    public LayerMask buildLayer;  // Capa de los cubos y suelo
    public GameObject parentLocalItemWorld;
    private string _currentSlotId;
    private bool _actionDeleteEnable;
    private Texture2D _currentTexture;
    private BuildItem _buildItem;

    private GameObject _ghostPreview;    // El cubo verde que sigue el puntero

    // Lista local de cubos para el JSON
    private List<GameObject> _instantiatedCubes = new List<GameObject>();
    // Cambiamos la lista a pública para que ManageItems la vea o creamos un método
    public List<GameObject> GetInstantiatedCubes() => _instantiatedCubes;

    private void Start()
    {
        _actionDeleteEnable = false;
        _buildItem = GetComponent<BuildItem>();
        if (parentLocalItemWorld == null) Debug.LogWarning("Coloca la referencia de los items copia del mundo");
        if (menuDialog == null) Debug.LogWarning("Coloca la referencia del dialogo");
        if (buttonDelete == null) Debug.LogWarning("Coloca la referencia del boton borrar, para borrar los items caja");
    }

    private void Update()
    {
        if (_ghostPreview != null)
        {
            UpdateGhostPreview();
        }
    }

    public void PrepareCube(string slotId, Texture2D tex)
    {
        // Si tocamos el mismo slot, construimos
        if (_ghostPreview != null && slotId == _currentSlotId)
        {
            ActionPlace();
        }
        else
        {
            // Limpiamos previos si existen
            if (_ghostPreview != null) Destroy(_ghostPreview);

            _currentSlotId = slotId;
            _currentTexture = tex;

            // 2. Creador del Ghost Preview (El verde transparente)
            _ghostPreview = Instantiate(cubePrefab);
            ApplyPreviewSettings(_ghostPreview, tex, 0.4f); // Transparente

            // Le ponemos un color verde suave
            _ghostPreview.GetComponent<MeshRenderer>().material.color = new Color(0, 1, 0, 0.4f);
            
            // IMPORTANTE: Asegurarnos que empiece desactivado hasta que el Raycast toque algo
            _ghostPreview.SetActive(false);
        }
    }

    private void ApplyPreviewSettings(GameObject obj, Texture2D tex, float alpha)
    {
        obj.GetComponent<Collider>().enabled = false;
        if (obj.GetComponent<Rigidbody>()) obj.GetComponent<Rigidbody>().isKinematic = true;

        Material mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mat.mainTexture = tex;
        // Si el shader soporta transparencia, seteamos el alpha
        if (alpha < 1.0f)
        {
            mat.SetInt("_Surface", 1); // 1 es Transparent en URP Unlit
            mat.renderQueue = 3000;
        }
        obj.GetComponent<MeshRenderer>().material = mat;
    }

    private void UpdateGhostPreview()
    {
        if (_actionDeleteEnable) return;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 12f, buildLayer))
        {
            if (!_ghostPreview.activeSelf) _ghostPreview.SetActive(true);

            // Lógica de Snapping
            Vector3 spawnPos = hit.point + hit.normal * 0.5f;
            spawnPos = new Vector3(Mathf.Round(spawnPos.x), Mathf.Round(spawnPos.y), Mathf.Round(spawnPos.z));

            _ghostPreview.transform.position = spawnPos;
        }
        else
        {
            // Si no apuntamos a nada construible, ocultamos el fantasma
            _ghostPreview.SetActive(false);
        }
    }

    public void ActionPlace()
    {
        if (_ghostPreview == null || !_ghostPreview.activeSelf) return;

        // Usamos la posición exacta donde ya está el Ghost Preview
        Vector3 finalPos = _ghostPreview.transform.position;

        GameObject newCube = Instantiate(cubePrefab, finalPos, Quaternion.identity);
        newCube.GetComponent<ItemScript>().SetReferenceManager(this);

        // Aplicamos la textura real
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mat.mainTexture = _currentTexture;
        newCube.GetComponent<MeshRenderer>().material = mat;

        newCube.name = _currentSlotId;
        newCube.GetComponent<Rigidbody>().isKinematic = true;
        newCube.GetComponent<Collider>().enabled = true;

        newCube.transform.SetParent(parentLocalItemWorld.transform);
        _instantiatedCubes.Add(newCube);
     
        PlaySoundPutBox();
    }

    private void PlaySoundPutBox()
    {
        // EFECTO ADICTIVO DE SONIDO
        if (audioSource != null && buildSound != null)
        {
            // Variamos el pitch entre 0.9 y 1.1 (un 10% arriba o abajo)
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(buildSound);
        }
    }

    public void SetActionDeleteEnable(bool enable)
    {
        _actionDeleteEnable = enable;
    }

    public void ActionDelete()
    {
        SetActionDeleteEnable(true);
        _buildItem.buttonBoxUiManager.EnableHighlight(buttonDelete);

        if (_ghostPreview != null)
            _ghostPreview.SetActive(false);

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
                RemoveCubeCopy(target);

                // podria ir SaveWorld() 
                // pero para optimizar agregamos un boton
                // SaveWorld(); // Guardamos el cambio en el JSON
            }
        }
    }

    // Es una copia local
    private void RemoveCubeCopy(GameObject cube)
    {
        // Es una copia local
        _instantiatedCubes.Remove(cube);
        Destroy(cube);
    }

    // Función para borrar todos los clones de un ID específico
    // podria ir SaveWorld() al final, pero para optimizar agregamos un boton
    // save world
    public void DeleteAllClonesOfId(string slotId)
    {
        // Buscamos de atrás para adelante para no romper el índice al borrar
        for (int i = _instantiatedCubes.Count - 1; i >= 0; i--)
        {
            if (_instantiatedCubes[i].name == "slot_"+slotId)
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
        int count = 0;
        int batchSize = 5; // Procesamos de a 5 cubos por vez
        int delayMs = 30;  // Pausa de 30ms para dejar que el celular respire

        // Filtramos primero los cubos que coinciden para no iterar de más adentro del loop
        string targetName = "slot_" + itemId;

        foreach (GameObject cube in _instantiatedCubes)
        {
            if (cube != null && cube.name == targetName)
            {
                // Usamos el componente BuildItem
                await GetComponent<BuildItem>().AsignMaterialAsync(imageName, cube);

                count++;

                // Cada vez que procesamos un lote (batchSize), hacemos una pausa
                if (count % batchSize == 0)
                {
                    // Task.Delay no bloquea el hilo principal, permite que el juego siga corriendo
                    await Task.Delay(delayMs);
                }
            }
        }

        Debug.Log($"<color=cyan>Update:</color> Se actualizaron {count} clones de {itemId} con pausas.");
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
                newCube.GetComponent<ItemScript>().SetReferenceManager(this);

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

    public void OnDelete(GameObject itemToDelete)
    {
        RemoveCubeCopy(itemToDelete);
    }
}
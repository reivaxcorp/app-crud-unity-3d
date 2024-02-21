using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

public class LocalDb : IRepositoryLocal
{
    private const string SAVE_FILE_NAME = "items.crud";
    private string folderUidUser;

    public LocalDb()
    {
        if (FirebaseSDK.GetInstance().auth != null)
        {
            this.folderUidUser = FirebaseSDK.GetInstance().user.UserId;
        }
        else
        {
            Debug.LogWarning("Firebase auth no esta inicializado");
        }
    }

    public async Task DeleteLocalItemById(string id)
    {
        List<ItemLocal> localItemsList = await GetLocalItemsAsync();

        // Buscar si el item ya existe en la lista
        int existingIndex = localItemsList.FindIndex(x => x.Id == id);

        if (existingIndex != -1)
        {
            // Si el item existe, eliminarlo de la lista
            localItemsList.RemoveAt(existingIndex);
        }
        await SaveLocalItemsAsync(localItemsList);
    }

    public async Task<ItemLocal> GetLocalItemById(string id)
    {
        List<ItemLocal> localItemsList = await GetLocalItemsAsync();

        int existingIndex = localItemsList.FindIndex(x => x.Id == id);

        if (existingIndex != -1)
        {
            return localItemsList[existingIndex];
        }
        throw new Exception("El item local fue borrado o no existe");
    }

    public async Task<List<ItemLocal>> GetLocalItemsAsync()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, folderUidUser);
        string filePath = Path.Combine(folderPath, SAVE_FILE_NAME);

        // Verificar si la carpeta existe, si no, crearla
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        if (File.Exists(filePath))
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                // Deserializar de manera asíncrona
                BinaryFormatter formatter = new BinaryFormatter();
                return await Task.FromResult(formatter.Deserialize(stream) as List<ItemLocal>);
            }
        }
        else
        {
            Debug.Log("Archivos locales no encontrados");
            return new List<ItemLocal>();
        }
    }

    public async Task SaveLocalItemsAsync(List<ItemLocal> listItemsLocal)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, folderUidUser);
        string filePath = Path.Combine(folderPath, SAVE_FILE_NAME);

        // Verificar si la carpeta existe, si no, crearla
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        await Task.Run(() =>
        {
            // Serializar y guardar de manera asíncrona
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, listItemsLocal);
            }
        });
    }

}

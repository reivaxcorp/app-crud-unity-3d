using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserLocalData
{
    public string localUserUi; // El uid del usuario
}

[System.Serializable]
public class CubeData
{
    public string slotId; // El ID del 1 al 10 que viene de Firebase
    public Vector3 position;
    public Quaternion rotation;
}

[System.Serializable]
public class WorldSaveData
{
    public List<CubeData> placedCubes = new List<CubeData>();
}
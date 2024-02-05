using UnityEngine;

public interface IDataTextureLocalSavedTest
{
    void SaveTextureAsPNG(Texture2D textureToSave, string imageId);
    void RemoveTexture(string imageId);
    Texture2D LoadTextureAsPNG(string imageId);
}


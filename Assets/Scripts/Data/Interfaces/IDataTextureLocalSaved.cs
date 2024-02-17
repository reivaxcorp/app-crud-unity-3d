using UnityEngine;

public interface IDataTextureLocalSaved
{
    void SaveTextureAsPNG(Texture2D textureToSave, string imageName);
    void RemoveLocalTexture(string imageName);
    Texture2D LoadTextureAsPNG(string imageName);
}


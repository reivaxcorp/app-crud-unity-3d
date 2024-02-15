using UnityEngine;

public interface IDataTextureLocalSaved
{
    void SaveTextureAsPNG(Texture2D textureToSave, string imageName);
    void RemoveTexture(string imageName);
    Texture2D LoadTextureAsPNG(string imageName);
}


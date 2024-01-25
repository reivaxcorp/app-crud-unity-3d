/*
 * TextureScaler.cs
 * Script for resizing textures in Unity
 * Original Author: OpenAI GPT-3.5
 * Modified by [ReivaxCorp.]
 */

using UnityEngine;


public class TextureScaler
{
    public static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Rect texR = new Rect(0, 0, targetWidth, targetHeight);
        int width = targetWidth;
        int height = targetHeight;

        // Get the source colors.
        Color[] pix = source.GetPixels(0, 0, source.width, source.height);

        Color[] newPix = new Color[width * height];

        float ratioX = (float)source.width / width;
        float ratioY = (float)source.height / height;

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int x = Mathf.FloorToInt(j * ratioX);
                int y = Mathf.FloorToInt(i * ratioY);
                newPix[i * width + j] = pix[y * source.width + x];
            }
        }

        // Create a new texture and set its pixels to the modified colors.
        Texture2D newTex = new Texture2D(width, height);
        newTex.SetPixels(newPix);
        newTex.Apply();

        return newTex;
    }
}


using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;



public class AssetProcessEditor : AssetPostprocessor
{
    private void OnPostprocessTexture(Texture2D texture)
    {
        var textureImp = assetImporter as TextureImporter;

        string path = assetImporter.assetPath;
        string extend = Path.GetExtension(path);

        if (!extend.Equals(".png") && !extend.Equals(".jpg") && !extend.Equals(".psd")) return;
        if (!path.StartsWith("Assets/ArtAssets")) return;

        if (path.Contains("Sprite"))
        {
            SetSprite(textureImp);
        }

        if (path.Contains("TileMap"))
        {
            SetSprite(textureImp);
        }
    }

    private void SetSprite(TextureImporter textrueImp)
    {
        textrueImp.textureType = TextureImporterType.Sprite;
        textrueImp.GetSourceTextureWidthAndHeight(out int width, out int height);
        int size = Mathf.Max(width, height);

        if (size <= 32)
            size = 32;

        else if (size > 32 && size <= 64)
            size = 64;

        else if (size > 64 && size <= 128)
            size = 128;

        else if (size > 128 && size <= 256)
            size = 256;

        else if (size > 256 && size <= 512)
            size = 512;

        else if (size > 512 && size <= 1024)
            size = 1024;

        else if (size > 1024 && size <= 2048)
            size = 2048;

        else if (size > 2048 && size <= 4096)
            size = 4096;

        else if (size > 4096 && size <= 8192)
            size = 8192;

        else if (size > 8192 && size <= 16384)
            size = 16384;

        textrueImp.maxTextureSize = size;
    }
}

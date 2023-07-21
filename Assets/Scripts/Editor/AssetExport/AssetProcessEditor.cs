using System.IO;
using UnityEditor;
using UnityEngine;



public class AssetProcessEditor : AssetPostprocessor
{
    private void OnPostprocessTexture(Texture2D texture)
    {
        var textrueImp = assetImporter as TextureImporter;

        string path = assetImporter.assetPath;
        string extend = Path.GetExtension(path);

        if (!extend.Equals(".png") && !extend.Equals(".jpg") && !extend.Equals(".psd")) return;
        if (!path.StartsWith("Assets/ArtAssets")) return;

        if (path.Contains("Sprite"))
        {
            textrueImp.textureType = TextureImporterType.Sprite;
        }

        if (path.Contains("TileMap"))
        {
            textrueImp.textureType = TextureImporterType.Sprite;
            
        }
    }
}

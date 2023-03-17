#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;


public static class Check
{
    private static readonly string[] ResPath = { "Assets/Resources/Sprite" };

    struct PathMap
    {
        public string BasePath;
        public string ComparePath;
        public float Degree;
    }

    [MenuItem ("AssetDatabase/ImportExample")]
    public static void CheckAll()
    { 
        List<Sprite> allSprites =new List<Sprite>();
        allSprites.Clear();
        var textures = GetTexturesPath(ResPath);
        foreach (var element in textures)
        {
            var sprites = LoadSlicedSprites(element).ToList();
            foreach (var sprite in sprites)
            {
                allSprites.Add(sprite);
            }
        }
        Debug.Log(allSprites.Count);
        List<PathMap> pathMaps = CheckAllSprites(allSprites);
        foreach (var element in pathMaps)
        {
            Debug.Log("相似度" + element.Degree + "-----" + element.BasePath + "-----" + element.ComparePath);
        }
    }

    /// <summary>
    /// 获取指定路径下全部Texture
    /// </summary>
    /// <param name="path">路径</param>
    /// <returns>每一张texture的路径组成的list</returns>
    private static List<string> GetTexturesPath(string[] path)
    {
        List<string> paths = new List<string>();
        string[] spriteGUIDs = AssetDatabase.FindAssets("t:Sprite", path);
        foreach (string spriteGuid in spriteGUIDs) 
        {
            string spritePath = AssetDatabase.GUIDToAssetPath(spriteGuid);
            paths.Add(spritePath);
        }
        return paths;
    }
    
    private static Sprite[] LoadSlicedSprites(string filePath)
    {
        // 获取纹理
        Texture2D texture = LoadTexture(filePath);
        if (texture == null) return null;

        // 获取切割后的sprites
        Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(filePath);
        if (sprites == null || sprites.Length == 0) return null;

        // 将Object类型转换为Sprite类型
        Sprite[] subSprites = new Sprite[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            subSprites[i] = (Sprite)sprites[i];
        }

        return subSprites;
    }
    
    /// <summary>
    /// 根据文件夹路径加载texture
    /// </summary>
    /// <param name="fileDictionaryPath">文件夹路径</param>
    /// <returns></returns>
    private static Texture2D LoadTexture(string fileDictionaryPath)
    {
        if (File.Exists(fileDictionaryPath))
        {
            byte[] fileData = File.ReadAllBytes(fileDictionaryPath);
            Texture2D tex2D = new Texture2D(2, 2);
            if (tex2D.LoadImage(fileData))
                return tex2D;
        }
        return null;
    }
    
    /// <summary>
    /// 根据图片列表对比全部图片
    /// </summary>
    /// <param name="sprites">图片列表</param>
    /// <returns>全部的路径以及相似度数值</returns>
    private static List<PathMap> CheckAllSprites(List<Sprite> sprites)
    {
        List<PathMap> pathMaps = new List<PathMap>();
        for (int i = 0; i < sprites.Count; i++)
        {
            for (int j = 0; j < sprites.Count; j++)
            {
                if (sprites[i].name == sprites[j].name)
                {
                    continue;
                }
                var pathMap = CheckSprites(sprites[i], sprites[j]);
                pathMaps.Add(pathMap);
            }
        }
        return pathMaps;
    }

    /// <summary>
    /// 对比Sprite
    /// </summary>
    /// <param name="baseSprite">对比的第一张Sprite</param>
    /// <param name="compareSprite">对比的第二张Sprite</param>
    /// <returns>两张图的路径以及相似度数值</returns>
    private static PathMap CheckSprites(Sprite baseSprite,Sprite compareSprite)
    {
        PathMap pathMap = new PathMap();
        pathMap.Degree = (float)SpriteCompare.IsSimilar(baseSprite, compareSprite);
        pathMap.BasePath = baseSprite.name;
        pathMap.ComparePath = compareSprite.name;
        return pathMap;
    }

}
#endif
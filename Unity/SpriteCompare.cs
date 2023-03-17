using UnityEngine;

public class SpriteCompare
{
    public static double IsSimilar(Sprite sprite1, Sprite sprite2)
    {
        Texture2D texture1 = new Texture2D((int)sprite1.rect.width, (int)sprite1.rect.height,
            sprite1.texture.format, false);
        texture1.SetPixels(sprite1.texture.GetPixels((int)sprite1.rect.xMin, (int)sprite1.rect.yMin,
            (int)sprite1.rect.width, (int)sprite1.rect.height));
        texture1.Apply();
        
        Texture2D texture2 = new Texture2D((int)sprite2.rect.width, (int)sprite2.rect.height,
            sprite2.texture.format, false);
        texture2.SetPixels(sprite2.texture.GetPixels((int)sprite2.rect.xMin, (int)sprite2.rect.yMin,
            (int)sprite2.rect.width, (int)sprite2.rect.height));
        texture2.Apply();

        if (texture1.width != texture2.width || texture1.height != texture2.height)
        {
            // 如果两个纹理的大小不同，则将它们的大小统一
            texture2.Resize(texture1.width, texture1.height);
        }
        
        Color32[] textureData1 = texture1.GetPixels32();
        Color32[] textureData2 = texture2.GetPixels32();

        double[] vector1 = new double[textureData1.Length];
        double[] vector2 = new double[textureData2.Length];

        for (int i = 0; i < textureData1.Length; i++)
        {
            vector1[i] = (textureData1[i].r + textureData1[i].g + textureData1[i].b) / 3.0;
        }

        for (int i = 0; i < textureData2.Length; i++)
        {
            vector2[i] = (textureData2[i].r + textureData2[i].g + textureData2[i].b) / 3.0;
        }

        double similarity = CalculateCosineSimilarity(vector1, vector2);

        return similarity ;
    }

    private static double CalculateCosineSimilarity(double[] vector1, double[] vector2)
    {
        double dotProduct = 0;
        double magnitude1 = 0;
        double magnitude2 = 0;

        for (int i = 0; i < vector1.Length; i++)
        {
            dotProduct += vector1[i] * vector2[i];
            magnitude1 += Mathf.Pow((float)vector1[i], 2);
            magnitude2 += Mathf.Pow((float)vector2[i], 2);
        }

        magnitude1 = Mathf.Sqrt((float)magnitude1);
        magnitude2 = Mathf.Sqrt((float)magnitude2);

        if (magnitude1 == 0 || magnitude2 == 0)
        {
            return 0;
        }

        double similarity = dotProduct / (magnitude1 * magnitude2);
        return similarity;
    }
    
}

using System.Linq;
using UnityEngine;

namespace Forester
{
     public class TerrainPainting  {
        private TerrainData terrainData;
        private Vector3 terrainPos;

        private float[] GetTextureBlend(Vector3 targetPos)
        {
            int xpos = (int)(((targetPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
            int zpos = (int)(((targetPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);
            float[,,] splatmapData = terrainData.GetAlphamaps(xpos, zpos, 1, 1);
            float[] blendValues = new float[splatmapData.GetUpperBound(0) + terrainData.terrainLayers.Length];
            for (int i = 0; i < blendValues.Length; i++)
            {
                blendValues[i] = splatmapData[0, 0, i];
            }
            return blendValues;
        }

        private float[] GetHeightBlend(Vector3 targetPos,int width, int height)
        {
            int xpos = (int)(((targetPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
            int zpos = (int)(((targetPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);
            float[,] heightData = terrainData.GetHeights(xpos, zpos, width, height);
            float[] heightValues = new float[(int)Mathf.Pow(heightData.Length,2)];
            int i = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    heightValues[i] = heightData[x, y];
                    i++;
                }
            }
            return heightValues;
        }

        private float[] GetSpecificTextureBlend(Vector2 targetPos)
        {
            int xpos = (int)targetPos.x;
            int zpos = (int)targetPos.y;
            zpos = Mathf.Clamp(zpos, 0, terrainData.alphamapHeight);
            xpos = Mathf.Clamp(xpos, 0, terrainData.alphamapWidth);
            float[,,] splatmapData = terrainData.GetAlphamaps(xpos, zpos, 1, 1);
            float[] blendValues = new float[splatmapData.GetUpperBound(0) + terrainData.terrainLayers.Length];
            for (int i = 0; i < blendValues.Length; i++)
            {
                blendValues[i] = splatmapData[0, 0, i];
            }
            return blendValues;
        }

        public int TerrainTargetTex(Terrain terrain,Vector3 targetPos)
        {
            terrainData = terrain.terrainData;
            terrainPos = terrain.transform.position;

            float[] blendedValues = GetTextureBlend(targetPos);
            int index = 0;
            for (int i = 0; i < blendedValues.Length; i++)
            {
                if (blendedValues[i] > index)
                {
                    index = i;
                }
            }
            return index;
        }

        float SumArray (float[] array)
        {
            float sum = 0;
            foreach(float f in array)
            {
                sum += f;
            }
            return sum;
        }

        public static Color32[] FlipPixels(Color32[] color, bool flipX, bool flipY,float texWidth, float texHeight)
        {
            Color32[] originalPixels = color;
            var flippedPixels =
                Enumerable.Range(0, (int)texWidth * (int)texHeight)
                .Select(index => {
                    int x = index % (int)texWidth;
                    int y = index / (int)texWidth;
                    if (flipX)
                        x = (int)texWidth - 1 - x;
                    if (flipY)
                        y = (int)texHeight - 1 - y;
                    return originalPixels[y * (int)texWidth + x];
                }
                );
            return flippedPixels.ToArray();
        }

        public static Texture2D Resize(Texture2D source, int newWidth, int newHeight)
        {
            source.filterMode = FilterMode.Point;
            RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
            rt.filterMode = FilterMode.Point;
            RenderTexture.active = rt;
            Graphics.Blit(source, rt);
            Texture2D nTex = new Texture2D(newWidth, newHeight);
            nTex.ReadPixels(new Rect(0, 0, newWidth, newWidth), 0, 0);
            nTex.Apply();
            RenderTexture.active = null;
            return nTex;

        }

        public float[,,] SetTerrainColor(Terrain terrain, int targetLayer, int size, float opacity,bool opacityOverlap, Vector3 targetPos, Color32[] stampPixels, bool clearLayer = false)
        {
            terrainData = terrain.terrainData;
            terrainPos = terrain.transform.position;

            int stampResolution = (int)Mathf.Sqrt(stampPixels.Length);
            int adjResolution = Mathf.RoundToInt((stampResolution / 100) * size);
            stampPixels = FlipPixels(stampPixels, true, true, stampResolution, stampResolution);
            Texture2D adjStamp = new Texture2D(stampResolution, stampResolution, TextureFormat.ARGB32, false, false);
            adjStamp.SetPixels32(stampPixels);
            adjStamp.Apply();
            adjStamp = Resize(adjStamp, adjResolution, adjResolution);
            adjStamp.Apply();

            Color32[] colors = adjStamp.GetPixels32();

            int xpos = (int)(((targetPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
            int zpos = (int)(((targetPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);

            int dominantTex = TerrainTargetTex(terrain, targetPos);
            int baseTex = dominantTex == targetLayer ? 0 : dominantTex;

            float[,,] originalSplatData = null;

            for (int x = 0; x < adjResolution; x++)
            {
                if (zpos - (adjResolution / 2) < 1 || zpos + (adjResolution / 2) > terrainData.alphamapWidth - 1 || xpos + (adjResolution / 2) > terrainData.alphamapHeight - 1 || xpos - (adjResolution / 2) < 1)
                {
                    return null;
                }

                originalSplatData = terrainData.GetAlphamaps(xpos - (adjResolution / 2), zpos - (adjResolution / 2), adjResolution, adjResolution);
                float[,,] splatmapData = terrainData.GetAlphamaps(xpos - (adjResolution / 2), zpos - (adjResolution / 2), adjResolution, adjResolution);

                for (int z = 0; z < adjResolution; z++)
                {
                    float[] splatWeights = GetSpecificTextureBlend(new Vector2((xpos - adjResolution / 2) + z, (zpos - adjResolution / 2) + x));
                    targetLayer = clearLayer ? baseTex : targetLayer;
                    float overlap = opacityOverlap ? splatWeights[targetLayer] : 0;
                    float colorData = (colors[(z + (adjResolution * x))].r);
                    colorData /= 255;
                    float stampOp = Mathf.Lerp(overlap, Mathf.Max(splatWeights[targetLayer], colorData), opacity/100);
                    splatWeights[targetLayer] = stampOp*2;
                    for (int n = 0; n < terrainData.alphamapLayers; n++)
                    {
                        if (splatWeights[targetLayer] >= 0.9F && n != targetLayer) splatWeights[n] = 0;
                    }
                    for (int n = 0; n < terrainData.alphamapLayers; n++)
                    {
                        splatWeights[n] /= SumArray(splatWeights);
                        splatmapData[x, z, n] = splatWeights[n];
                    }
                }
                terrainData.SetAlphamaps(xpos - (adjResolution / 2), zpos - (adjResolution / 2), splatmapData);
            }
            terrain.Flush();
            return originalSplatData;
        }


        public void ResetSplatColors (Terrain terrain, int size, Vector3 targetPos, Color32[] stampPixels, TerrainData rawTerrainData)
        {
            terrainData = terrain.terrainData;
            terrainPos = terrain.transform.position;

            int stampResolution = (int)Mathf.Sqrt(stampPixels.Length);
            int adjResolution = Mathf.RoundToInt((stampResolution / 100) * (size * 2));
            stampPixels = FlipPixels(stampPixels, true, true, stampResolution, stampResolution);
            Texture2D adjStamp = new Texture2D(stampResolution, stampResolution, TextureFormat.ARGB32, false, false);
            adjStamp.SetPixels32(stampPixels);
            adjStamp.Apply();
            adjStamp = Resize(adjStamp, adjResolution, adjResolution);
            adjStamp.Apply();

            Color32[] colors = adjStamp.GetPixels32();

            int xpos = (int)(((targetPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
            int zpos = (int)(((targetPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);

            if (zpos - (adjResolution / 2) < 1 || zpos + (adjResolution / 2) > terrainData.alphamapWidth - 1 || xpos + (adjResolution / 2) > terrainData.alphamapHeight - 1 || xpos - (adjResolution / 2) < 1)
            {
                return;
            }

            float[,,] splatmapData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
            float[,,] rawSplatmapData = rawTerrainData.GetAlphamaps(0, 0, rawTerrainData.alphamapWidth, rawTerrainData.alphamapHeight);
            for (int x = zpos - (adjResolution / 2); x < zpos + (adjResolution / 2); x++)
            {
                for (int z = xpos - (adjResolution / 2); z < xpos + (adjResolution/2); z++)
                {
                    for (int n = 0; n < terrainData.alphamapLayers; n++)
                    {
                        int valx = x - (zpos - (adjResolution / 2));
                        int valz = z - (xpos - (adjResolution / 2));
                        float colorData = (colors[(valz + (adjResolution * valx))].r);
                        colorData /= 255;
                        splatmapData[x, z, n] = colorData > 0 ? rawSplatmapData[x,z,n] : splatmapData[x, z, n];
                    }
                }
            }
            terrainData.SetAlphamaps(0,0, splatmapData);
            terrain.Flush();
        }

        public float[,] SetTerrainHeight(Terrain terrain, int size, float strength, Vector3 targetPos, Color32[] stampPixels, TerrainData rawTerrainData)
        {
            terrainData = terrain.terrainData;
            terrainPos = terrain.transform.position;

            int stampResolution = (int)Mathf.Sqrt(stampPixels.Length);
            int adjResolution = Mathf.RoundToInt((stampResolution / 100) * size);
            stampPixels = FlipPixels(stampPixels, true, true, stampResolution, stampResolution);
            Texture2D adjStamp = new Texture2D(stampResolution, stampResolution, TextureFormat.ARGB32, false, false);
            adjStamp.SetPixels32(stampPixels);
            adjStamp.Apply();
            adjStamp = Resize(adjStamp, adjResolution, adjResolution);
            adjStamp.Apply();

            Color32[] colors = adjStamp.GetPixels32();

            int xpos = (int)(((targetPos.x - terrainPos.x) / terrainData.size.x) * terrainData.heightmapResolution);
            int zpos = (int)(((targetPos.z - terrainPos.z) / terrainData.size.z) * terrainData.heightmapResolution);

            if (zpos - (adjResolution) < 1 || zpos + (adjResolution) > terrainData.heightmapResolution - 1 || xpos + (adjResolution) > terrainData.heightmapResolution - 1 || xpos - (adjResolution) < 1)
            {
                return null;
            }

            float[,] heightData = terrainData.GetHeights(xpos - (adjResolution / 2), zpos - (adjResolution / 2), adjResolution, adjResolution);
            float[,] rawHeightData = rawTerrainData.GetHeights(xpos - (adjResolution / 2), zpos - (adjResolution / 2), adjResolution, adjResolution);

            for (int x = 0; x < adjResolution; x++)
            {
                for (int z = 0; z < adjResolution; z++)
                {
                    float pointData = terrainData.GetHeight((xpos - adjResolution / 2) + z, (zpos - adjResolution / 2) + x);
                    float colorData = (colors[(z + (adjResolution * x))].r);
                    colorData /= 255;
                    float stampStrength = Mathf.Lerp(pointData, Mathf.Clamp(pointData += strength, pointData, terrainData.size.y / 2), colorData);
                    stampStrength /= terrainData.size.y;
                    rawHeightData[x, z] = heightData[x, z];
                    heightData[x, z] = stampStrength;
                }
            }

            terrainData.SetHeights(xpos - (adjResolution / 2), zpos - (adjResolution / 2), heightData);
            rawTerrainData.SetHeights(xpos - (adjResolution / 2), zpos - (adjResolution / 2), rawHeightData);
            terrain.Flush();
            return rawHeightData;
        }

        public void ResetHeight(Terrain terrain, int size, Vector3 targetPos, Color32[] stampPixels, TerrainData rawTerrainData)
        {
            if (rawTerrainData == null) return;
            terrainData = terrain.terrainData;
            terrainPos = terrain.transform.position;

            int stampResolution = (int)Mathf.Sqrt(stampPixels.Length);
            int adjResolution = Mathf.RoundToInt((stampResolution / 100) * (size/1.5f));

            int xpos = (int)(((targetPos.x - terrainPos.x) / terrainData.size.x) * terrainData.heightmapResolution);
            int zpos = (int)(((targetPos.z - terrainPos.z) / terrainData.size.z) * terrainData.heightmapResolution);

            if (zpos - (adjResolution) < 1 || zpos + (adjResolution) > terrainData.heightmapResolution - 1 || xpos + (adjResolution) > terrainData.heightmapResolution - 1 || xpos - (adjResolution) < 1)
            {
                return;
            }

            float[,] newHeightData = terrainData.GetHeights(xpos - (adjResolution / 2), zpos - (adjResolution / 2), adjResolution, adjResolution);
            float[,] rawHeightData = rawTerrainData.GetHeights(xpos - (adjResolution / 2), zpos - (adjResolution / 2), adjResolution, adjResolution);

            for (int x = 0; x < adjResolution;x++)
            {
                for(int y = 0; y < adjResolution;y++)
                {
                    newHeightData[x, y] = Mathf.Clamp(rawHeightData[x, y], 0, (newHeightData[x, y] + rawHeightData[x, y]) / 2);
                }
            }

            terrainData.SetHeights(xpos - (adjResolution / 2), zpos - (adjResolution / 2), newHeightData);
            terrain.terrainData.RefreshPrototypes();
            terrain.Flush();
        }
    }
}

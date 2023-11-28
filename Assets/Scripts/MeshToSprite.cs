using System.Collections.Generic;
using UnityEngine;

public static class MeshToSprite
{
    public static void ConvertMeshsToSprite(Vector3[] meshVertices, int[] meshTriangles, ref Sprite sprite)
    {
        List<Vector2> verticies = new();
        List<ushort> triangles = new();
        List<(int, int)> vericiesNewIndexes = new();

        Vector2 shiftValues = Vector2.zero;
        for (int v = 0; v < meshVertices.Length; v++)
        {
            if (meshVertices[v].x < shiftValues.x)
            {
                shiftValues.x = meshVertices[v].x;
            }
            if (meshVertices[v].y < shiftValues.y)
            {
                shiftValues.y = meshVertices[v].y;
            }
        }

        float maxValue = meshVertices[0].x - shiftValues.x;
        float minValue = meshVertices[0].y - shiftValues.y;
        for (int v = 0; v < meshVertices.Length; v++)
        {
            if (meshVertices[v].x - shiftValues.x < minValue)
            {
                minValue = meshVertices[v].x - shiftValues.x;
            }
            if (meshVertices[v].y - shiftValues.y < minValue)
            {
                minValue = meshVertices[v].y - shiftValues.y;
            }

            if (meshVertices[v].x - shiftValues.x > maxValue)
            {
                maxValue = meshVertices[v].x - shiftValues.x;
            }
            if (meshVertices[v].y - shiftValues.y > maxValue)
            {
                maxValue = meshVertices[v].y - shiftValues.y;
            }
        }

        for (int v = 0; v < meshVertices.Length; v++)
        {
            // SHIFT TO POSITIVE VALUES
            Vector2 shiftedVertices = (Vector2)meshVertices[v] - shiftValues;
            Vector2 normalizedVertices = new((shiftedVertices.x - minValue) / (maxValue - minValue), (shiftedVertices.y - minValue) / (maxValue - minValue));
            Vector2 convertedVertice = new(normalizedVertices.x * (sprite.rect.xMax - sprite.rect.xMin) + sprite.rect.xMin, normalizedVertices.y * (sprite.rect.yMax - sprite.rect.yMin) + sprite.rect.yMin);

            if (!verticies.Exists((vert) => vert.x == convertedVertice.x && vert.y == convertedVertice.y))
            {
                verticies.Add(convertedVertice);
                if (verticies.Count - 1 != v)
                {
                    vericiesNewIndexes.Add((v, verticies.Count - 1));
                }
            }
            else
            {
                vericiesNewIndexes.Add((v, verticies.FindIndex((vert) => vert.x == convertedVertice.x && vert.y == convertedVertice.y)));
            }
        }

        for (int t = 0; t < meshTriangles.Length; t++)
        {
            if (!vericiesNewIndexes.Exists((d) => d.Item1.Equals(meshTriangles[t])))
            {
                triangles.Add((ushort)meshTriangles[t]);
            }
            else
            {
                triangles.Add((ushort)vericiesNewIndexes.Find((d) => d.Item1.Equals(meshTriangles[t])).Item2);
            }
        }

        sprite.OverrideGeometry(verticies.ToArray(), triangles.ToArray());
    }
}

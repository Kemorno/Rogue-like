using System.Collections.Generic;
using UnityEngine;
using Resources;
using Enums;

public static class TextureGenerator
{
    public static Texture2D textureFromSprite(Sprite _sprite)
    {
        Texture2D texture = new Texture2D((int)_sprite.rect.width, (int)_sprite.rect.height);
        Color[] spritePixels = _sprite.texture.GetPixels((int)_sprite.textureRect.x,
                                                (int)_sprite.textureRect.y,
                                                (int)_sprite.textureRect.width,
                                                (int)_sprite.textureRect.height);
        texture.SetPixels(spritePixels);
        texture.Apply();
        return texture;
    }
}
public static class Grid
{
    public static Texture2D ChunkGrid(int ChunkSize, int cellSize = 64)
    {
        Texture2D ChunkGrid = new Texture2D(cellSize * ChunkSize, cellSize * ChunkSize)
        {
            alphaIsTransparency = true,
            filterMode = FilterMode.Point,
            anisoLevel = 5,
            wrapMode = TextureWrapMode.Clamp,
            
        };

        Texture2D cell = new Texture2D(cellSize, cellSize);

        for (int x = 0; x < cell.width; x++)
        {
            for (int y = 0; y < cell.height; y++)
            {
                if (x == 0 || x == cell.width - 1 || y == 0 || y == cell.height - 1)
                    cell.SetPixel(x, y, Color.grey);
                else
                    cell.SetPixel(x, y, Color.clear);
            }
        }
        cell.Apply();
        for (int x = 0; x < ChunkGrid.width; x += cellSize)
        {
            for (int y = 0; y < ChunkGrid.height; y += cellSize)
            {
                ChunkGrid.SetPixels(x,y, cellSize, cellSize, cell.GetPixels());
            }
        }
        for (int x = 0; x < ChunkGrid.width; x++)
        {
            for (int y = 0; y < ChunkGrid.height; y++)
            {
                if (x == 0 || x == ChunkGrid.width - 1 || y == 0 || y == ChunkGrid.height - 1)
                {
                    ChunkGrid.SetPixel(x, y, Color.white);
                }

            }
        }
        ChunkGrid.Apply();
        return ChunkGrid;
    }
}
public static class Overlay
{
    public static Texture2D TileType(Chunk chunk, int cellSize = 64)
    {
        Texture2D texture = new Texture2D(chunk.Size * cellSize, chunk.Size * cellSize)
        {
            filterMode = FilterMode.Point,
            alphaIsTransparency = true
        };

        for (int x = 0; x < chunk.Size; x++)
        {
            for (int y = 0; y < chunk.Size; y++)
            {
                if (chunk.Tiles[new CoordInt(x, y)].Type == tileType.Wall)
                    texture.SetPixels(x * cellSize, y * cellSize, cellSize, cellSize, Opaque(cellSize, Color.black).GetPixels());
                if (chunk.Tiles[new CoordInt(x, y)].Type == tileType.Floor)
                    texture.SetPixels(x * cellSize, y * cellSize, cellSize, cellSize, Opaque(cellSize, Color.white).GetPixels());
            }
        }

        texture.SetPixels(Grid.ChunkGrid(chunk.Size).GetPixels());
        texture.Apply();

        return texture;
    }

    public static Texture2D Opaque(int Size, Color color)
    {
        Texture2D texture = new Texture2D(Size, Size);
        
        for(int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
        return texture;
    }
}

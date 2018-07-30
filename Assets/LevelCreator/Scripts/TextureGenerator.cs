using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LevelGenerator))]
public static class TextureGenerator {

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
    public static Texture2D gridTexture(LevelGenerator.Tile[,] _Map, Texture2D _gridCell)
    {
        int mapWidht = _Map.GetLength(0);
        int mapHeight = _Map.GetLength(1);

        int cellWidht = _gridCell.width;
        int cellHeight = _gridCell.height;

        Texture2D texture = new Texture2D(mapWidht * cellWidht, mapHeight * cellHeight, TextureFormat.ARGB32, false);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;

        for(int x = 0; x < texture.width; x+=cellWidht)
        {
            for (int y = 0; y < texture.height; y+=cellHeight)
            {
                texture.SetPixels(x, y, cellWidht, cellHeight, _gridCell.GetPixels());
            }
        }
        texture.Apply();
        return texture;
    }

}
public static class Overlay
{
    public static Texture2D Tiles(LevelGenerator.Tile[,] _Map)
    {
        Texture2D texture = new Texture2D(_Map.GetLength(0), _Map.GetLength(1), TextureFormat.ARGB32, false);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;

        for (int x = 0; x < _Map.GetLength(0); x++)
        {
            for (int y = 0; y < _Map.GetLength(1); y++)
            {
                if (_Map[x, y].tileType == Enums.tileType.Wall)
                    texture.SetPixel(x, y, new Color32(0, 0, 0, 255 / 2));
                if (_Map[x, y].tileType == Enums.tileType.Floor)
                    texture.SetPixel(x, y, new Color32(255, 255, 255, 255 / 2));
            }
        }
        texture.Apply();
        return texture;
    }
    public static Texture2D RoomTiles(LevelGenerator.Tile[,] _Map, List<LevelGenerator.Room> Rooms)
    {
        Texture2D texture = new Texture2D(_Map.GetLength(0), _Map.GetLength(1), TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        {
            Color fillColor = Color.clear;
            Color[] fillPixels = new Color[texture.width * texture.height];

            for (int i = 0; i < fillPixels.Length; i++)
            {
                fillPixels[i] = fillColor;
            }

            texture.SetPixels(fillPixels);
        }//Set all transparent

        for (int i = 0; i < Rooms.Count; i++)
        {
            System.Random prng = new System.Random(Rooms[i].RoomID);
            Color32 roomTileColor = new Color32((byte)prng.Next(0, 255), (byte)prng.Next(0, 255), (byte)prng.Next(0, 255), 255 / 2);
            Color32 roomWallColor = roomTileColor + (Color.white * 0.2f);
            roomWallColor.a = roomTileColor.a;

            for (int f = 0; f < Rooms[i].roomTiles.Count; f++)
            {
                texture.SetPixel(Rooms[i].roomTiles[f].RawCoord.coords.x, Rooms[i].roomTiles[f].RawCoord.coords.y, roomTileColor);
            }
            for (int w = 0; w < Rooms[i].wallTiles.Count; w++)
            {
                texture.SetPixel(Rooms[i].wallTiles[w].RawCoord.coords.x, Rooms[i].wallTiles[w].RawCoord.coords.y, roomWallColor);
            }
        }
        texture.Apply();
        return texture;
    }
    public static Texture2D RoomClass(LevelGenerator.Tile[,] _Map, List<LevelGenerator.Room> Rooms)
    {
        Texture2D texture = new Texture2D(_Map.GetLength(0), _Map.GetLength(1), TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        {
            Color fillColor = Color.clear;
            Color[] fillPixels = new Color[texture.width * texture.height];

            for (int i = 0; i < fillPixels.Length; i++)
            {
                fillPixels[i] = fillColor;
            }

            texture.SetPixels(fillPixels);
        }//Set all transparent

        for (int i = 0; i < Rooms.Count; i++)
        {
            System.Random prng = new System.Random(Rooms[i].RoomID);

            Color color = Color.clear;

            switch (Rooms[i].roomClass)
            {
                case Enums.roomClass.Neutral:
                    color = Color.white;
                    break;
                case Enums.roomClass.Physical:
                    color = Color.red;
                    break;
                case Enums.roomClass.Scientific:
                    color = Color.blue;
                    break;
                case Enums.roomClass.Social:
                    color = Color.green;
                    break;
                case Enums.roomClass.Spiritual:
                    color = Color.yellow;

                    break;
            }

            for (int f = 0; f < Rooms[i].roomTiles.Count; f++)
            {
                texture.SetPixel(Rooms[i].roomTiles[f].RawCoord.coords.x, Rooms[i].roomTiles[f].RawCoord.coords.y, color);
            }
            for (int w = 0; w < Rooms[i].wallTiles.Count; w++)
            {
                texture.SetPixel(Rooms[i].wallTiles[w].RawCoord.coords.x, Rooms[i].wallTiles[w].RawCoord.coords.y, color);
            }
        }
        texture.Apply();
        return texture;
    }
    public static Texture2D RoomSize(LevelGenerator.Tile[,] _Map, List<LevelGenerator.Room> Rooms)
    {
        Texture2D texture = new Texture2D(_Map.GetLength(0), _Map.GetLength(1), TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        {
            Color fillColor = Color.clear;
            Color[] fillPixels = new Color[texture.width * texture.height];

            for (int i = 0; i < fillPixels.Length; i++)
            {
                fillPixels[i] = fillColor;
            }

            texture.SetPixels(fillPixels);
        }//Set all transparent

        for (int i = 0; i < Rooms.Count; i++)
        {
            System.Random prng = new System.Random(Rooms[i].RoomID);

            Color color = Color.clear;

            float biggestRoom = (float)Enums.roomSize.Big;

            switch (Rooms[i].roomSize)
            {
                case Enums.roomSize.Tiny:
                    color = Color.Lerp(Color.white, Color.blue, (float)Enums.roomSize.Tiny / biggestRoom);
                    break;
                case Enums.roomSize.Small:
                    color = Color.Lerp(Color.white, Color.blue, (float)Enums.roomSize.Small / biggestRoom);
                    break;
                case Enums.roomSize.Medium:
                    color = Color.Lerp(Color.white, Color.blue, (float)Enums.roomSize.Medium / biggestRoom);
                    break;
                case Enums.roomSize.Large:
                    color = Color.Lerp(Color.white, Color.blue, (float)Enums.roomSize.Large / biggestRoom);
                    break;
                case Enums.roomSize.Big:
                    color = Color.Lerp(Color.white, Color.blue, (float)Enums.roomSize.Big / biggestRoom);
                    break;
            }

            for (int f = 0; f < Rooms[i].roomTiles.Count; f++)
            {
                texture.SetPixel(Rooms[i].roomTiles[f].RawCoord.coords.x, Rooms[i].roomTiles[f].RawCoord.coords.y, color);
            }
            for (int w = 0; w < Rooms[i].wallTiles.Count; w++)
            {
                texture.SetPixel(Rooms[i].wallTiles[w].RawCoord.coords.x, Rooms[i].wallTiles[w].RawCoord.coords.y, color);
            }
        }
        texture.Apply();
        return texture;
    }
    public static Texture2D RoomType(LevelGenerator.Tile[,] _Map, List<LevelGenerator.Room> Rooms)
    {
        Texture2D texture = new Texture2D(_Map.GetLength(0), _Map.GetLength(1), TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        {
            Color fillColor = Color.clear;
            Color[] fillPixels = new Color[texture.width * texture.height];

            for (int i = 0; i < fillPixels.Length; i++)
            {
                fillPixels[i] = fillColor;
            }

            texture.SetPixels(fillPixels);
        }//Set all transparent

        for (int i = 0; i < Rooms.Count; i++)
        {
            System.Random prng = new System.Random(Rooms[i].RoomID);

            Color color = Color.clear;

            switch (Rooms[i].roomType)
            {
                case Enums.roomType.None:
                    color = Color.grey;
                    break;
                case Enums.roomType.Spawn:
                    color = Color.white;
                    break;
                case Enums.roomType.Shop:
                    color = Color.yellow;
                    break;
                case Enums.roomType.Secret:
                    color = Color.blue;
                    break;
                case Enums.roomType.Enemy:
                    color = Color.magenta;
                    break;
                case Enums.roomType.MiniBoss:
                    color = Color.red;
                    break;
                case Enums.roomType.Boss:
                    color = Color.black;
                    break;
            }

            for (int f = 0; f < Rooms[i].roomTiles.Count; f++)
            {
                texture.SetPixel(Rooms[i].roomTiles[f].RawCoord.coords.x, Rooms[i].roomTiles[f].RawCoord.coords.y, color);
            }
            for (int w = 0; w < Rooms[i].wallTiles.Count; w++)
            {
                texture.SetPixel(Rooms[i].wallTiles[w].RawCoord.coords.x, Rooms[i].wallTiles[w].RawCoord.coords.y, color);
            }
        }
        texture.Apply();
        return texture;
    }
}

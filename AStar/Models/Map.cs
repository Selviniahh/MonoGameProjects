using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AStar.Models;
//do not forget this class makes Tile array and updates those. Tile class is just tile class and this class makes 2d tile array 
public class Map 
{
    public readonly Point Size = new Point(100, 100);
    public Tile[,] Tiles { get; }
    public Point TileSize { get; }

    public Vector2 MapToScreen(int x, int y) => new Vector2(x * TileSize.X, y * TileSize.Y); //Take int as argument and return Vector2
    public (int x, int y) ScreenToMap(Vector2 pos) => ((int)pos.X / TileSize.X, (int)pos.Y / TileSize.Y); //Take Vector as argument and return tuple (two integer) 

    public Map()
    {
        Tiles = new Tile[Size.X, Size.Y];
        var texture = Globals.Content.Load<Texture2D>("tile");
        TileSize = new Point(texture.Width, texture.Height); //64,64

        for (int y = 0; y < Size.Y; y++)
        {
            for (int x = 0; x < Size.X; x++)
            {
                Tiles[x, y] = new Tile(texture, MapToScreen(x, y), x,y);
            }
        }
    }

    public void Update()
    {
        for (int y = 0; y < Size.Y; y++)
        {
            for (int x = 0; x < Size.X; x++)
            {
                Tiles[x, y].Update();
            }
        }
    }

    public void Draw()
    {
        for (int y = 0; y < Size.Y; y++)
        {
            for (int x = 0; x < Size.X; x++)
            {
                Tiles[x,y].Draw();
            }
        }
    }

}
using System.Diagnostics;
using BFSPathFinding.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BFSPathFinding.Models;

public class Tile : Sprite
{
    public bool Blocked { get; set; }
    public bool Path { get; set; }
    public bool Visited { get; set; }
    private readonly int _mapX;
    private readonly int _mapY;
    public Color VisitColor; 
    
    public Tile(Texture2D texture, Vector2 position, int mapX, int mapY) : base(texture, position)
    {
        _mapX = mapX;
        _mapY = mapY;
    }

    public void Update()
    {
        if (Rectangle.Contains(InputManager.MouseRectangle))
        {
            if (InputManager.MouseClicked)
            {
                Blocked = !Blocked;
            }

            if (InputManager.MouseRightClicked)
            {
                PathFinder.BFSearch(_mapX, _mapY);
            }
        }

        if (Blocked)
        {
            Color = Color.Red;
        }
        else if (Path)
        {
            Color = Color.Green;
        }
        else if (Visited)
        {
            Color = Color.Aquamarine;
        }
        else
        {
            Color = Color.White;
        }

    }

    public override void Draw()
    {
        base.Draw();
        Globals.SpriteBatch.Draw(_texture,Position,null,Color,0f,Origin,1f,SpriteEffects.None,0f);
    }
}
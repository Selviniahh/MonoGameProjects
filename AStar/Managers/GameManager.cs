using AStar.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AStar.Managers;

public class GameManager
{
    private readonly Map _map;

    public GameManager()
    {
        _map = new Map();
        PathFinder.Init(_map);
        //pathfinder init
    }

    public void Update()
    {
        InputManager.Update();
        _map.Update();
    }

    public void Draw()
    {
        Globals.SpriteBatch.Begin();
        _map.Draw();
        Globals.SpriteBatch.End();
    }
}
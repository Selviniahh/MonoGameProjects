using System;
using Animation2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ETG;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private readonly GameManager _gameManager = new GameManager();

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Globals.Content = Content;
        Globals.GraphicsDevice = GraphicsDevice;
        Globals.ScreenWidth = _graphics.PreferredBackBufferWidth;
        Globals.ScreenHeight = _graphics.PreferredBackBufferHeight;

    }

    protected override void Initialize()
    {
        _gameManager.Initialize();
        base.Initialize();
    }
    
    

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Globals.GraphicsDevice = GraphicsDevice;
        Globals.SpriteBatch = _spriteBatch;
        _gameManager.LoadContent();

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        Globals.Update(gameTime);
        _gameManager.Update();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _gameManager.Draw();


        base.Draw(gameTime);
    }
}
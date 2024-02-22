using System.Collections.Generic;
using System.Diagnostics;
using Animation2;
using ETG.Enemies;
using ETG.GameLogics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;

namespace ETG;

public class GameManager
{
    private Hero _hero;
    private UserInterface _userInterface;
    // private BulletMan _bulletMan;
    private SpawnRandomBulletMan _spawnRandomBulletMan;
    public void Initialize()
    {
        Globals.Font = Globals.Content.Load<SpriteFont>("font");
        _hero = new Hero();
        _userInterface = new UserInterface();
        // _bulletMan = new BulletMan(new Vector2(250,250));
        _spawnRandomBulletMan = new SpawnRandomBulletMan();
        _spawnRandomBulletMan.Initialize();
    }

    public void LoadContent()
    {
        // _bulletMan.LoadContent();
    }

    public void Update()
    {
        InputManager.Update();
        _userInterface.Update();
        _hero.Update();
        _spawnRandomBulletMan.Update();
        // _bulletMan.Update();
        

     
    }

    public void Draw()
    {
        Globals.SpriteBatch.Begin(SpriteSortMode.BackToFront,BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null);
        InputManager.Draw();
        _hero.Draw();
        
        _spawnRandomBulletMan.LoadContent();
        _spawnRandomBulletMan.Draw();
        // _bulletMan.Draw();
        _userInterface.Gun = _hero._gun.AnimManagerDict[_hero._gun.CurrentState].GetCurrentFrameAsTexture(); 
        
        _userInterface.Draw();
        Globals.SpriteBatch.End();
        // //This is the drawn Animation will be 
    }

    
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Security;
using Animation2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ETG.Guns;

public abstract class GunBase
{
    public readonly Dictionary<string, AnimationManager> AnimManagerDict = new Dictionary<string, AnimationManager>();
    public readonly Dictionary<string, AnimationManager> VFXManagerDict = new Dictionary<string, AnimationManager>();
    public  Vector2 GunPosition;
    public Vector2 GunOrigin;
    public float GunRotation;
    public SpriteEffects GunSpriteEffects = SpriteEffects.None;
    public Vector2 InitialGunOrigin;
    protected float PressedTimer;
    public static int AmmoCount;
    protected Vector2 BarrelTipPosition;

    private readonly Texture2D _reloadTextureString;
    private Texture2D _reloadSliderTexture, _reloadSliderValueTex;
    private Vector2 ReloadTextPosition => new Vector2(InputManager.HeroPosition.X - (InputManager.HeroOrigin.X/2)  - (float)_reloadTextureString.Width /2, InputManager.HeroPosition.Y  - InputManager.HeroOrigin.Y - (float)_reloadTextureString.Height - 50);
    private Vector2 _SliderXBegin => new Vector2(ReloadTextPosition.X + 20, InputManager.HeroPosition.Y - InputManager.HeroOrigin.Y - (float)_reloadTextureString.Height - 50);
    private Vector2 _sliderXEnd => new Vector2(ReloadTextPosition.X + _reloadSliderTexture.Width - 20, InputManager.HeroPosition.Y - InputManager.HeroOrigin.Y - (float)_reloadTextureString.Height - 50);
    private Vector2 _reloadSliderValue;
    public static bool Isreloading; //Display Slider as well as make reload
    protected bool ShowReloadText; //Should we show the reload text string blinking
    public string CurrentState = "Fire"; //Current state of the gun just like animation. EnemyIdle, Fire, Reload.
    public string VFXCurrentState = "EnemyIdle"; //Current state of the gun just like animation. EnemyIdle, Fire, Reload.
    
    private readonly Effect _fadeEffect;
    private float _fadeTimer;
    protected float ClickTimer; //Timer for click. If clicked it will reset
    protected float ReloadTime; //How long should it take to reload 1.5f
    protected float ReloadTimer; //Timer for ReloadTime. Starts from 0 ends at ReloadTime
    protected int ProjectileDistanceTotal; //How far should the magazine travel before it disappears
    public float ShootTimer; 
    public bool IsOnCooldown = false;



    public GunBase( Vector2 gunPosition, float gunRotation)
    {
        GunPosition = gunPosition;
        GunRotation = gunRotation;
        _reloadTextureString = Globals.Content.Load<Texture2D>("Texts/ReloadText");
        _reloadSliderTexture = Globals.Content.Load<Texture2D>("UI/Slider");
        _reloadSliderValueTex = Globals.Content.Load<Texture2D>("UI/SliderValue");
        _fadeEffect = Globals.Content.Load<Effect>("Effects/effect01");

    }

    public abstract void Update(Vector2 gunPosition);
    public abstract void Shoot(Vector2 direction);

    public abstract void Reload();

    public virtual void Draw(Vector2 position, float scale)
    {
        //Draw Animation
        AnimManagerDict[CurrentState].Draw(AnimManagerDict[CurrentState].LastTexture,position, Color.White, GunRotation, GunOrigin, scale, GunSpriteEffects, 0.01f);
        

        //Reload VFX Animation
        if (VFXCurrentState == "Reload")
        {
            VFXManagerDict[VFXCurrentState].Draw(VFXManagerDict[VFXCurrentState].LastTexture,BarrelTipPosition, Color.White, GunRotation, GunOrigin,2f, GunSpriteEffects, 0.2f);
        }
        
        //Draw Shading Reload String Blinking
        Globals.SpriteBatch.End();
        _fadeTimer += Globals.TotalSeconds + 0.1f;
        _fadeEffect.Parameters["Time"].SetValue(_fadeTimer); 
        Globals.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, _fadeEffect);
        if (ShowReloadText) Globals.SpriteBatch.Draw(_reloadTextureString, ReloadTextPosition, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.2f);
        Globals.SpriteBatch.End();
        Globals.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null);
        
        //Slide Animation
        if (Isreloading)
        {
            float progress = ReloadTimer / ReloadTime;
            _reloadSliderValue = Vector2.Lerp(_SliderXBegin, _sliderXEnd, progress);
        }

        //Draw Sliders
        if (Isreloading) Globals.SpriteBatch.Draw(_reloadSliderTexture, ReloadTextPosition, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.2f);
        if (Isreloading) Globals.SpriteBatch.Draw(_reloadSliderValueTex, _reloadSliderValue, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.2f);
    }
}
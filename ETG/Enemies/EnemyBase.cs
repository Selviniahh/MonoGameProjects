using System;
using System.Collections.Generic;
using Animation2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ETG.Enemies;

public abstract class EnemyBase
{
    protected Dictionary<string, AnimationManager> AnimationManagerDict = new Dictionary<string, AnimationManager>();
    protected string CurrentState = "EnemyIdle";
    protected Vector2 Position;
    protected float Speed;
    protected float Health;
    protected float DirectionAngle;
    protected Rectangle EnemyBounds;
    protected Texture2D BoundsTexture;
    public bool StopEverything, CanDrawGun = true;
    private Effect _fadeGray;
    private float _fadeTimer;
    protected int DeathLeapX, DeathLeapY;
    protected int LeapAmount; 
    protected float LeapTimer;
    protected float DistanceToHero;


    public virtual void LoadContent()
    { 
        _fadeGray = Globals.Content.Load<Effect>("Effects/FadeGray");
    }

    public virtual void Update()
    {
    }


    public virtual void Draw()
    {
        //Make Death Animation Gray and Faded out. 
        if (StopEverything)
        {
            Globals.SpriteBatch.End();
            _fadeTimer = Math.Min(_fadeTimer + Globals.TotalSeconds / 10, 1);
            _fadeGray.Parameters["Time"].SetValue(_fadeTimer);
            Globals.SpriteBatch.Begin(SpriteSortMode.FrontToBack,BlendState.NonPremultiplied,SamplerState.PointClamp, null, null, _fadeGray);
            AnimationManagerDict[CurrentState].Draw(AnimationManagerDict[CurrentState].LastTexture, Position, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.4f);
            Globals.SpriteBatch.End();
            Globals.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null);
        }
        else
        {
            AnimationManagerDict[CurrentState].Draw(AnimationManagerDict[CurrentState].LastTexture, Position, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.4f);
        }
        // AnimationManagerDict[CurrentState].Draw(Position);
    }
    
    protected float GetDirectionAngle()
    {
        Vector2 diff = Position - Hero.Position;
        float angle = (float)Math.Atan2(diff.Y, diff.X);
        angle = MathHelper.ToDegrees(angle);
        if (angle < 0) angle += 360;
        return angle;
    }
    
    protected float GetDistanceFromHero()
    {
        return Vector2.Distance(Position, Hero.Position);
    }
}
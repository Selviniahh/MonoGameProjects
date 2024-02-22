using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ETG.Guns;

public abstract class ProjectileBase : ICloneable
{
    protected Texture2D Texture;
    public Vector2 Position;
    protected Rectangle ProjectileBounds;
    protected Vector2 Velocity;
    protected Vector2 Direction;
    protected float Speed;
    protected float FireSpeed;
    protected float Rotation;
    protected float Damage;
    protected float Lifetime;
    protected Vector2 Origin;
    protected SpriteEffects SpriteEffects = SpriteEffects.None;
    protected float Timer;
    public float DistanceTraveled;
    public Vector2 StartVectorPoint;
    

    public abstract void Update();
    
    public object Clone()
    {
        return MemberwiseClone();
    }

    public abstract void Draw();
}
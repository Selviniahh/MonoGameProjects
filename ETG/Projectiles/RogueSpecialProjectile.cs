using System;
using System.Collections.Generic;
using System.Timers;
using Animation2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ETG.Guns;

public class RogueSpecialProjectile : ProjectileBase
{
    public  Rectangle RogueSpecialBounds;
    public static float RogueSpecialDamage = 1f;
    public RogueSpecialProjectile(Vector2 position, Vector2 velocity, Vector2 origin, float speed, float fireSpeed, float rotation, float lifetimeDistance)
    {
        Texture = Globals.Content.Load<Texture2D>("Projectiles/Player_Projectile");
        Position = position;
        Velocity = velocity;
        Speed = speed;
        FireSpeed = fireSpeed;
        Rotation = rotation;
        Lifetime = lifetimeDistance;
        Origin = origin;
        Direction = Velocity; // Set the direction based on the initial velocity and speed
        RogueSpecialBounds = ProjectileBounds;
    }

    public override void Update()
    {
        RogueSpecialBounds = new Rectangle((int)Math.Round(Position.X), (int)Math.Round(Position.Y), (Texture.Width *4), (Texture.Height*4));
        Position += Direction * Speed;
    }

    public override void Draw()
    {
        Globals.SpriteBatch.Draw(Texture,Position,null,Color.White,Rotation,Origin,4f,SpriteEffects,0.2f);
    }
    
}
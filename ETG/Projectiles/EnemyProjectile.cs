using Animation2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ETG.Guns;

public class EnemyProjectile : ProjectileBase
{
    public Rectangle EnemyProjectileBounds;
    public EnemyProjectile(Vector2 position, Vector2 velocity, Vector2 origin, float speed, float fireSpeed, float rotation, float lifetimeDistance)
    {
        Texture = Globals.Content.Load<Texture2D>("Projectiles/Enemy_Projectile");
        Position = position;
        Velocity = velocity;
        Speed = speed;
        FireSpeed = fireSpeed;
        Rotation = rotation;
        Lifetime = lifetimeDistance;
        Origin = origin;
        Direction = Velocity; // Set the direction based on the initial velocity and speed
    }
    public override void Update()
    {
        EnemyProjectileBounds = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width*4, Texture.Height*4);
        Position += Direction * Speed;
    }

    public override void Draw()
    {
        Globals.SpriteBatch.Draw(Texture, Position, null, Color.White, Rotation, Origin, 4f, SpriteEffects.None, 0.2f);
    }
}
using System;
using System.Collections.Generic;
using Animation2;
using ETG.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ETG.Guns;

public class Revolver : GunBase
{
    private List<EnemyProjectile> _projectiles = new List<EnemyProjectile>();
    public float Angle;
    public int GunOriginX;
    public int GunOriginY;
    public bool IsFiring { get; private set; }
    private float _animationTimer = 0f;
    private const float _fireAnimationDuration = 2.1f;

    public Revolver(Vector2 gunPosition, float gunRotation) : base(gunPosition, gunRotation)
    {
        //Add Animations
        var idleAnimTexture = Globals.Content.Load<Texture2D>("Enemy/BulletMan/Gun/big_gun_idle_001");
        var idleAnimManager = new AnimationManager();
        idleAnimManager.AddAnimation("Idle",new Animation(idleAnimTexture,0.15f,1,1));
        AnimManagerDict.Add("Idle", idleAnimManager);

        var fireAnimTexture = Globals.Content.Load<Texture2D>("Enemy/BulletMan/Gun/Revolver_Fire");
        var fireAnimManager = new AnimationManager();
        fireAnimManager.AddAnimation("Fire",new Animation(fireAnimTexture,0.30f,7,1));
        AnimManagerDict.Add("Fire", fireAnimManager);
        
        //Origin
        GunOriginX = 2;
        GunOriginY = 15;
        
    }

    public override void Update(Vector2 gunPosition)
    {
        Vector2 direction = new Vector2((float)Math.Cos(GunRotation), (float)Math.Sin(GunRotation));
        GunPosition = gunPosition;
        GunOrigin = new Vector2(GunOriginX, GunOriginY);
        BarrelTipPosition = GunPosition + GunOrigin + (direction * 100);
        
        //Play animations
        if (IsFiring)
        {
            _animationTimer += Globals.TotalSeconds;

            // Check if the "Fire" animation is done
            if (_animationTimer >= _fireAnimationDuration)
            {
                IsFiring = false;
                _animationTimer = 0f;  // reset the timer
                CurrentState = "Idle";
            }
        }
        _projectiles.ForEach(x => x.Update());
    }

    public override void Draw(Vector2 position, float scale)
    {
        base.Draw(position, scale);
        _projectiles.ForEach(x => x.Draw());

    }

    public override void Shoot(Vector2 direction)
    {
        // if (_shootOnce)
        // {
        //     EnemyProjectile enemyProjectile = new EnemyProjectile(BarrelTipPosition,direction,GunOrigin,5f,5f,GunRotation,ProjectileDistanceTotal);
        //     enemyProjectile.StartVectorPoint = BarrelTipPosition;
        //     _projectiles.Add(enemyProjectile);
        //     _shootOnce = false;
        //     CurrentState = "Fire";
        //     _fireCooldown = 0f;
        // }
        
        EnemyProjectile enemyProjectile = new EnemyProjectile(BarrelTipPosition,direction,GunOrigin,5f,5f,GunRotation,ProjectileDistanceTotal);
        enemyProjectile.StartVectorPoint = BarrelTipPosition; 
        _projectiles.Add(enemyProjectile);
        CurrentState = "Fire";
        IsFiring = true;


    }

    public override void Reload()
    {
        
    }
    
    
    public float GetDirectionAngle()
    {
        Vector2 diff = Hero.Position - GunPosition;
        var angle = (float)Math.Atan2(diff.Y, diff.X);
        Angle = MathHelper.ToDegrees(angle);
        if (Angle < 0)
        {
            Angle += 360;
        }
        {
            
        }
        return angle;
    }
}
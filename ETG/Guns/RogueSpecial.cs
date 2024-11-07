using System;
using System.Collections.Generic;
using System.Diagnostics;
using Animation2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ETG.Guns;

public class RogueSpecial : GunBase
{
    public static List<RogueSpecialProjectile> _projectiles = new List<RogueSpecialProjectile>();
    private bool _shootFirstBullet = true;
    private Texture2D _VFXTexture;
    public static bool ReloadFinished;

    private float _projectileFireSpeed = 3f;
    

    public RogueSpecial(Vector2 gunPosition, float gunRotation) : base(gunPosition, gunRotation)
    {
        ReloadTime = 1.5f; //reload time in seconds
        ProjectileDistanceTotal = 500; //projectile can go 200 units as much 
        // GunIdleTexture = Globals.Content.Load<Texture2D>("Guns/knav3_idle_001");
        GunOrigin = new Vector2(1, 9);
        AmmoCount = 8;
        
        var idleAnimTexture = Globals.Content.Load<Texture2D>("Guns/knav3_idle_001");
        var idleAnimManager = new AnimationManager();
        idleAnimManager.AddAnimation("Idle", new Animation(idleAnimTexture,0.15f,1,1));
        AnimManagerDict.Add("Idle", idleAnimManager);
        
        var gunFireAnimTexture = Globals.Content.Load<Texture2D>("Guns/RogueSpecial_Fire");
        var fireAnimManager = new AnimationManager();
        fireAnimManager.AddAnimation("Fire", new Animation(gunFireAnimTexture,0.08f,5,1));
        AnimManagerDict.Add("Fire", fireAnimManager);
        
        var reloadAnimTexture = Globals.Content.Load<Texture2D>("Guns/knav3_reload_001");
        var reloadAnimationManager = new AnimationManager();
        reloadAnimationManager.AddAnimation("Reload", new Animation(reloadAnimTexture,0.15f,1,1));
        AnimManagerDict.Add("Reload", reloadAnimationManager);

        _VFXTexture = Globals.Content.Load<Texture2D>("VFX/RogueSpecialReload/ReloadVFX");
        var reloadVFXAnimationManager = new AnimationManager();
        reloadVFXAnimationManager.AddAnimation("Reload", new Animation(_VFXTexture,0.15f,5,1));
        VFXManagerDict.Add("Reload", reloadVFXAnimationManager);

    }

    public override void Update(Vector2 gunPosition)
    {
        GunPosition = gunPosition;
        Vector2 direction = new Vector2((float)Math.Cos(GunRotation), (float)Math.Sin(GunRotation));
        GunOrigin = CalculateOrigin(direction);
        direction *= 50;
        
         // Adjust the multiplier based on your sprite's dimensions
            float barrelOffset = 5550f; // Adjust this value as needed
            Vector2 barrelOffsetVector = direction * barrelOffset;
            BarrelTipPosition = GunPosition + barrelOffsetVector;
            Vector2 ExtraOffset = new Vector2(0, -30);
            
        BarrelTipPosition = GunPosition + direction + ExtraOffset;
        ClickTimer += Globals.TotalSeconds;

        //Set CurrentState
        if (InputManager.CurrentMouse.LeftButton == ButtonState.Pressed)
            CurrentState = "Fire";
            
        if (InputManager.CurrentMouse.LeftButton == ButtonState.Released)
            CurrentState = "Idle";
        
        //Handle VFXCurrentState for VFX animations
        if (Isreloading)
        {
            VFXCurrentState = "Reload";
        }
        else if (!Isreloading)
        {
            VFXCurrentState = "Idle";

        }
       

        //Shoot
        if (InputManager.CurrentMouse.LeftButton == ButtonState.Pressed && ClickTimer > 0.4f && !Isreloading && AnimManagerDict["Fire"].IsAnimationFinished())
        {
            ReloadFinished = false;
            Vector2 bulletDirection = new Vector2((float)Math.Cos(GunRotation), (float)Math.Sin(GunRotation));
            bulletDirection *= _projectileFireSpeed;
            if (AmmoCount <= 0)
            {
                ShowReloadText = true;
                return;
            }

            if (_shootFirstBullet)
            {
                Shoot(bulletDirection);
                _shootFirstBullet = false;
            }
            
            PressedTimer += Globals.TotalSeconds;
            if (PressedTimer > 0.5f)
            {
                PressedTimer = 0;
                Shoot(bulletDirection);
            }

            ClickTimer = 0;
        }
        
        
        else
        {
            _shootFirstBullet = true;
        }

        foreach (var projectile in _projectiles)
        {
            projectile.Update();
        }

        if (Isreloading)
        {
            CurrentState = "Reload";
            ReloadTimer += Globals.TotalSeconds;
            Reload();
        }
        
        if (!Isreloading && InputManager.CurrentKeyboard.IsKeyDown(Keys.R) && InputManager.PreviousKeyboard.IsKeyUp(Keys.R))
        {
            CurrentState = "Reload";
            Isreloading = true;
        }

        switch (CurrentState)
        {
            case "Idle":
                AnimManagerDict[CurrentState].Update("Idle",GunPosition);
                break;
            case "Fire":
                AnimManagerDict[CurrentState].Update("Fire",GunPosition);
                break;
            case "Reload":
                AnimManagerDict[CurrentState].Update("Reload",GunPosition);
                break;
        }

        switch (VFXCurrentState)
        {
            case "Reload":
                VFXManagerDict[VFXCurrentState].Update("Reload",BarrelTipPosition);
                break;
        }
    }

    public override void Draw(Vector2 position, float scale)
    {
        position = GunPosition;
        base.Draw(position, scale);

        List<RogueSpecialProjectile> projectilesToRemove = new List<RogueSpecialProjectile>();

        for (int i = 0; i < _projectiles.Count; i++)
        {
            CheckProjectileDistanceTraveled(_projectiles[i], projectilesToRemove);

            if (_projectiles.Count != 0 && _projectiles[i] != null)
            {
                _projectiles[i].Draw();
            }
        }

        foreach (var projectile in projectilesToRemove)
        {
            _projectiles.Remove(projectile);
        }

        Globals.SpriteBatch.DrawString(Globals.Font,"Gun State:" + CurrentState,new Vector2(0,120),Color.White);
    }

    public void CheckProjectileDistanceTraveled(RogueSpecialProjectile projectile, List<RogueSpecialProjectile> projectilesToRemove)
    {
        projectile.DistanceTraveled = Vector2.Distance(projectile.StartVectorPoint, projectile.Position);
        if (projectile.DistanceTraveled > ProjectileDistanceTotal)
        {
            projectilesToRemove.Add(projectile);    
        } 
    }


    

    public override void Shoot(Vector2 bulletDirection)
    {
        RogueSpecialProjectile newProjectile = new RogueSpecialProjectile(BarrelTipPosition, bulletDirection, GunOrigin, 5f, _projectileFireSpeed, GunRotation, ProjectileDistanceTotal);
        newProjectile.StartVectorPoint = BarrelTipPosition;
        _projectiles.Add(newProjectile);
        AmmoCount--;
    }

    public override void Reload()
    {
        ShowReloadText = false;
        if (ReloadTimer > ReloadTime)
        {
            AmmoCount = 8;
            ReloadTimer = 0;
            Isreloading = false;
            ReloadFinished = true;
        }
        
        
    }

    private Vector2 CalculateOrigin(Vector2 direction)
    {
        // Calculate the position of the gun's origin in world space
        Vector2 originInWorld = GunPosition + Vector2.Transform(InitialGunOrigin, Matrix.CreateRotationZ(GunRotation));

        // Calculate the direction from the origin to the gun's position
        Vector2 directionFromOrigin = GunPosition - originInWorld;

        // Project the directionFromOrigin vector onto the direction vector
        Vector2 projectedDirection = Vector2.Dot(directionFromOrigin, direction) / direction.LengthSquared() * direction;

        // Calculate the new origin relative to the gun's position
        Vector2 newOrigin = InitialGunOrigin + projectedDirection;
        
        return newOrigin;
    }
}
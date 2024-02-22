using System;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;
using Animation2;
using ETG.Guns;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ETG.Enemies;

public class BulletMan : EnemyBase
{
    private Vector2 _previousDirection;
    private Texture2D _hand;
    private int _lastTextureWidth; 
    private int _lastTextureHeight;
    private float _collisionTimer, _hitAnimationTimer;
    private bool _startTimer,_removevProjectile;
    private Revolver _revolver;    
    
    private EnemyIdle _idleState;
    private EnemyRun _runState;
    private EnemyHit _hitState;
    private EnemyDeath _deathState;
    private float _gunRotation;
    private Vector2 _revolverPosition;
    private Vector2 _handPosition;
    
    private float _newTimer;
    private bool _newTimerBool;

    public BulletMan(Vector2 position)
    {
        LeapAmount = 40;
        Health = 3;
        Position = position;
        
        //Idle Animation adding
        var idleRight = Globals.Content.Load<Texture2D>("Enemy/BulletMan/Idle/Idle_Right");
        var idleLeft = Globals.Content.Load<Texture2D>("Enemy/BulletMan/Idle/Idle_Left");
        var idleBack = Globals.Content.Load<Texture2D>("Enemy/BulletMan/Idle/Idle_Back");
        
        AnimationManager idleManager = new AnimationManager();
        idleManager.AddAnimation(EnemyIdle.Idle_Right,new Animation(idleRight,0.15f,2,1));
        idleManager.AddAnimation(EnemyIdle.Idle_Left,new Animation(idleLeft,0.15f,2,1));
        idleManager.AddAnimation(EnemyIdle.Idle_Back,new Animation(idleBack,0.15f,2,1));
        AnimationManagerDict.Add("Idle", idleManager);
        CurrentState = "Idle";
        
        
        //Run Animation adding
        var runRight = Globals.Content.Load<Texture2D>("Enemy/BulletMan/Run/Run_Right");
        var runLeft = Globals.Content.Load<Texture2D>("Enemy/BulletMan/Run/Run_Left");
        var runLeftBack = Globals.Content.Load<Texture2D>("Enemy/BulletMan/Run/Run_Left_Back");
        var runRightBack = Globals.Content.Load<Texture2D>("Enemy/BulletMan/Run/Run_Right_Back");

        AnimationManager runAnimManager = new AnimationManager();
        runAnimManager.AddAnimation(EnemyRun.Run_Right,new Animation(runRight,0.15f,6,1));
        runAnimManager.AddAnimation(EnemyRun.Run_Left,new Animation(runLeft,0.15f,6,1));
        runAnimManager.AddAnimation(EnemyRun.Run_Left_Back,new Animation(runLeftBack,0.15f,6,1));
        runAnimManager.AddAnimation(EnemyRun.Run_Right_Back,new Animation(runRightBack,0.15f,6,1));
        AnimationManagerDict.Add("Run", runAnimManager);
        
        
        //Hit Animation Adding
        var hitBackLeft = Globals.Content.Load<Texture2D>("Enemy/BulletMan/Hit/Hit_Back_Left");
        var hitBackRight = Globals.Content.Load<Texture2D>("Enemy/BulletMan/Hit/Hit_Back_Right");
        var hitLeft = Globals.Content.Load<Texture2D>("Enemy/BulletMan/Hit/Hit_Left");
        var hitRight = Globals.Content.Load<Texture2D>("Enemy/BulletMan/Hit/Hit_Right");
        
        AnimationManager hitAnimManager = new AnimationManager();
        hitAnimManager.AddAnimation(EnemyHit.Hit_Back_Left,new Animation(hitBackLeft,0.15f,1,1));
        hitAnimManager.AddAnimation(EnemyHit.Hit_Back_Right,new Animation(hitBackRight,0.15f,1,1));
        hitAnimManager.AddAnimation(EnemyHit.Hit_Right,new Animation(hitRight,0.15f,1,1));
        hitAnimManager.AddAnimation(EnemyHit.Hit_Left,new Animation(hitLeft,0.15f,1,1));
        AnimationManagerDict.Add("Hit", hitAnimManager);
        
        //Death Animation Adding
        var deathBack = Globals.Content.Load<Texture2D>("Enemy/BulletMan/Death/Death_Back");
        var deathFront = Globals.Content.Load<Texture2D>("Enemy/BulletMan/Death/Death_Front");
        var deathFrontLeft = Globals.Content.Load<Texture2D>("Enemy/BulletMan/Death/Death_Front_Left");
        var deathBackLeft = Globals.Content.Load<Texture2D>("Enemy/BulletMan/Death/Death_Back_Left");
        var deathBackRight = Globals.Content.Load<Texture2D>("Enemy/BulletMan/Death/Death_Back_Right");
        var deathLeft = Globals.Content.Load<Texture2D>("Enemy/BulletMan/Death/Death_Left");
        var deathFrontRight = Globals.Content.Load<Texture2D>("Enemy/BulletMan/Death/Death_Front_Right");
        var deathRight = Globals.Content.Load<Texture2D>("Enemy/BulletMan/Death/Death_Right");
        
        AnimationManager deathAnimManager = new AnimationManager();
        deathAnimManager.AddAnimation(EnemyDeath.Death_Back,new Animation(deathBack,0.15f,4,1));
        deathAnimManager.AddAnimation(EnemyDeath.Death_Front,new Animation(deathFront,0.15f,4,1));
        deathAnimManager.AddAnimation(EnemyDeath.Death_Front_Left,new Animation(deathFrontLeft,0.15f,4,1));
        deathAnimManager.AddAnimation(EnemyDeath.Death_Back_Left,new Animation(deathBackLeft,0.15f,5,1));
        deathAnimManager.AddAnimation(EnemyDeath.Death_Back_Right,new Animation(deathBackRight,0.15f,5,1));
        deathAnimManager.AddAnimation(EnemyDeath.Death_Left,new Animation(deathLeft,0.15f,4,1));
        deathAnimManager.AddAnimation(EnemyDeath.Death_Front_Right,new Animation(deathFrontRight,0.15f,4,1));
        deathAnimManager.AddAnimation(EnemyDeath.Death_Right,new Animation(deathRight,0.15f,4,1));
        AnimationManagerDict.Add("death",deathAnimManager);

        _hand = Globals.Content.Load<Texture2D>("Enemy/BulletMan/bullet_hand_001");
        _gunRotation = 0f;
        _handPosition = Position + new Vector2(40,65);
        _revolverPosition = _handPosition;
        _revolver = new Revolver(_revolverPosition, 0f);
    }

    public override void LoadContent()
    {
        base.LoadContent();
        BoundsTexture = new Texture2D(Globals.GraphicsDevice, 1, 1);
        BoundsTexture.SetData(new[] { Color.White });
    }

    public override void Update()
    {
        if (_newTimerBool) _newTimer += Globals.TotalSeconds; 
        LeapAmount = 100;
        DistanceToHero = GetDistanceFromHero();
        Vector2 direction = new Vector2((float)Math.Cos(_revolver.GunRotation), (float)Math.Sin(_revolver.GunRotation));
        _revolver.Update(_revolver.GunPosition);
        _revolver.ShootTimer += Globals.TotalSeconds;

        //If Player is dead, stop everything
        if (StopEverything)
        {
            CanDrawGun = false;
            return;
        }
        
        //Add revolver to enemy's hand
        AddRevolver();

        //Make collision and movement
        _collisionTimer += Globals.TotalSeconds;
        _previousDirection = Position;

        if (CurrentState != "death" && DistanceToHero >=200) //&& DistanceToHero >=700
        {
            FollowPlayer(50f);
            if (_revolver.ShootTimer >= 2)
            {
                _revolver.Shoot(direction);
                _revolver.ShootTimer = 0;
                _newTimerBool = true;
            }
            
            
        }
        
        //if Not dead and Distance is 200 or less stop moving to character
        // if (CurrentState != "death" && DistanceToHero >=500 && _revolver.ShootTimer >= 2)
        // {
        //     _revolver.Shoot(direction);
        //     _revolver.ShootTimer = 0;
        // }
        
        //Get Last Drawn Texture's Width and Height 
        if (AnimationManagerDict[CurrentState].LastTexture != null && _hitAnimationTimer == 0)
        {
            _lastTextureWidth = AnimationManagerDict[CurrentState].LastTexture.Width;
            _lastTextureHeight = AnimationManagerDict[CurrentState].LastTexture.Height;
        }
        
        EnemyBounds = new Rectangle((int)Position.X, (int)Position.Y, _lastTextureWidth, _lastTextureHeight*4);

        
        for (int i = RogueSpecial._projectiles.Count - 1; i >= 0; i--)
        {
            var projectile = RogueSpecial._projectiles[i];
            if (EnemyBounds.Intersects(projectile.RogueSpecialBounds) && _collisionTimer > 0.4f)
            {
                // Remove the collided projectile
                RogueSpecial._projectiles.RemoveAt(i);

                // Process collision
                Health -= RogueSpecialProjectile.RogueSpecialDamage;
                CurrentState = "Hit";
                _collisionTimer = 0;
                _startTimer = true;

                // Break loop after processing collision
                break;
            }
        }


        //Run this if hit animation needs to be played.
        if (_startTimer) _hitAnimationTimer += Globals.TotalSeconds;
        
        //Kill Enemy if Health is 0
        if (Health <= 0)
        {
            CurrentState = "death";
        }
        // If you delete this code rectangle width will increase for no reason. Anyway I am checking here if hitAnimation is set to 0 which means it's now played hitAnimation, set back to Idle. That's all
         else if (_previousDirection == Position && _hitAnimationTimer == 0)
         {
             CurrentState = "Idle";
         }

        //Check if Hit Animation is played for 0.116 seconds if yes set to 0 which means now it's not played anymore
        else if (_hitAnimationTimer >= 0.116f)
        {
            _hitAnimationTimer = 0f; 
            _startTimer = false;
        }

        //If Position is not idle Run
        else if (_previousDirection != Position && _hitAnimationTimer == 0)
        {
            CurrentState = "Run";
        }
        
        //Set BeforeDeathLeap to make deathLeap when Death animation is playing
        DeathLeapX = (int)Position.X;
        DeathLeapY = (int)Position.Y;

        if (AnimationManagerDict["death"].IsAnimationFinished())
        {
            //make leap 
            StopEverything = true;
        }

        //Assign Enum States with their directions
        if (CurrentState != "death")
        {
            DirectionAngle = GetDirectionAngle();
            _idleState = GetIdleDirectionEnum(DirectionAngle);
            _runState = GetRunDirectionEnum(DirectionAngle);
            _hitState = GetHitDirectionEnum(DirectionAngle);
        }
        else
        {
            _deathState = GetDeathDirectionEnum(DirectionAngle);
            
        }
        
        //Play the animation finally based on current state
        switch (CurrentState)
        {
            case "Run":
                AnimationManagerDict["Run"].Update(_runState,Position);
                break;
            case "Idle":
                AnimationManagerDict["Idle"].Update(_idleState,Position);
                break;
            
            case "death":
                AnimationManagerDict["death"].Update(_deathState,Position);
                LeapTimer += Globals.TotalSeconds/10;
                DeadLeap();
                break;
        }
    }

    private void AddRevolver()
    {
        // //Add Revolver to Enemy's hand
        _revolver.GunRotation = _revolver.GetDirectionAngle();

        //Put hand to right position from center of the enemy NOTE I DON'T KNOW WHY BUT THIS IS THE RIGHT HAND LOCATION
        _handPosition = Position + new Vector2(50, 65);
        _revolverPosition = _handPosition;

        //Left Direction
        if (_revolver.Angle > 115 && _revolver.Angle < 260)
        {
            if (_revolver.CurrentState == "Fire")
            {
                _handPosition.X -= 60;
                _revolver.GunSpriteEffects = SpriteEffects.FlipVertically;
                _revolver.GunOriginX = 8;
                _revolver.GunOriginY = 10;    
            }
            else
            {
                _handPosition.X -= 60;
                _revolver.GunSpriteEffects = SpriteEffects.FlipVertically;
                _revolver.GunOriginX = 8;
                _revolver.GunOriginY = 18;
            }
            
        }
        //Right Direction
        else
        {
            if (_revolver.CurrentState == "Fire")
            {
                _revolver.GunOriginX = 3;
                _revolver.GunOriginY = 25;
                _revolver.GunSpriteEffects = SpriteEffects.None;   
            }
            else
            {
                _revolver.GunOriginX = 2;
                _revolver.GunOriginY = 15;
                _revolver.GunSpriteEffects = SpriteEffects.None;
            }
            
            
        }

        _revolver.Update(_revolverPosition);
        _revolverPosition = _handPosition;
    }

    public override void Draw()
    {
        if (CurrentState != "death") Globals.SpriteBatch.DrawString(Globals.Font,"Enemy Direction Angle: " + DirectionAngle, new Vector2(0, 135), Color.White);
        if (CurrentState != "death") Globals.SpriteBatch.DrawString(Globals.Font,"Current State: " + CurrentState, new Vector2(0, 150), Color.White);
        if (CurrentState != "death") Globals.SpriteBatch.DrawString(Globals.Font,"Current State: " + _deathState, new Vector2(0, 165), Color.White);
        if (CurrentState != "death") Globals.SpriteBatch.DrawString(Globals.Font,"DirectionAngle: " + DirectionAngle, new Vector2(0, 180), Color.White);
        if (CurrentState != "death") Globals.SpriteBatch.DrawString(Globals.Font,"DeathState: " + _deathState, new Vector2(0, 195), Color.White);
        if (CurrentState != "death") Globals.SpriteBatch.DrawString(Globals.Font,"Distance to Hero: " + DistanceToHero, new Vector2(0, 210), Color.White);
        // Globals.SpriteBatch.Draw(BoundsTexture, EnemyBounds, Color.Red);
        // Globals.SpriteBatch.Draw(BoundsTexture, RogueSpecialProjectile., Color.Red);

        if (CurrentState != "death") _revolver.Draw(_revolverPosition, 2f);
        if (CurrentState != "death") Globals.SpriteBatch.Draw(_hand,_handPosition,null,Color.White,_gunRotation,new Vector2(0,0),3f,SpriteEffects.None,0.2f);
        base.Draw();
    }

    private void FollowPlayer(float speed)
    {
        speed = 100f;
        Vector2 direction = Hero.Position - Position;
        direction.Normalize();
        Position += direction * speed * Globals.TotalSeconds;
    }

    //Make Dead Leap to Opposite Position to feel Death Impact
    private void DeadLeap()
    {
        switch (_deathState)
        {
            case EnemyDeath.Death_Right:
                Position.X = MathHelper.Lerp(DeathLeapX, DeathLeapX - LeapAmount, LeapTimer);
                break;
            case EnemyDeath.Death_Left:
                Position.X = MathHelper.Lerp(DeathLeapX, DeathLeapX + LeapAmount, LeapTimer);
                break;
            case EnemyDeath.Death_Front:
                Position.Y = MathHelper.Lerp(DeathLeapY, DeathLeapY - LeapAmount, LeapTimer);
                break;
            case EnemyDeath.Death_Back:
                Position.Y = MathHelper.Lerp(DeathLeapY, DeathLeapY + LeapAmount, LeapTimer);
                break;
            case EnemyDeath.Death_Front_Right:
                Position.X = MathHelper.Lerp(DeathLeapX, DeathLeapX - LeapAmount / (float)Math.Sqrt(2), LeapTimer);
                Position.Y = MathHelper.Lerp(DeathLeapY, DeathLeapY + LeapAmount / (float)Math.Sqrt(2), LeapTimer);
                break;
            case EnemyDeath.Death_Front_Left:
                Position.X = MathHelper.Lerp(DeathLeapX, DeathLeapX + LeapAmount / (float)Math.Sqrt(2), LeapTimer);
                Position.Y = MathHelper.Lerp(DeathLeapY, DeathLeapY + LeapAmount / (float)Math.Sqrt(2), LeapTimer);
                break;
            case EnemyDeath.Death_Back_Right:
                Position.X = MathHelper.Lerp(DeathLeapX, DeathLeapX - LeapAmount / (float)Math.Sqrt(2), LeapTimer);
                Position.Y = MathHelper.Lerp(DeathLeapY, DeathLeapY - LeapAmount / (float)Math.Sqrt(2), LeapTimer);
                break;
            case EnemyDeath.Death_Back_Left:
                Position.X = MathHelper.Lerp(DeathLeapX, DeathLeapX + LeapAmount / (float)Math.Sqrt(2), LeapTimer);
                Position.Y = MathHelper.Lerp(DeathLeapY, DeathLeapY - LeapAmount / (float)Math.Sqrt(2), LeapTimer);
                break;
        }
    }


    private EnemyIdle GetIdleDirectionEnum(float angle)
    {
        if (angle > 45 && angle < 135)
        {
            return EnemyIdle.Idle_Back;
        }
        else if (angle > 135 && angle < 275)
        {
            return EnemyIdle.Idle_Right;
        }
        else if (angle > 275 && angle < 360)
        {
            return EnemyIdle.Idle_Left;
        }
        else if (angle > 0 && angle < 45)
        {
            return EnemyIdle.Idle_Left;
        }
        else
        {
            return EnemyIdle.Idle_Right;
        }
    }

    private EnemyRun GetRunDirectionEnum(float angle)
    {
        if (angle > 30 && angle < 115)
        {
            return EnemyRun.Run_Left_Back;
        }
        else if (angle > 115 && angle < 160)
        {
            return EnemyRun.Run_Right_Back;
        }
        else if (angle > 160 && angle < 250)
        {
            return EnemyRun.Run_Right;
        }
        else if (angle > 250 && angle < 360)
        {
            return EnemyRun.Run_Left;
        }
        else
        {
            return EnemyRun.Run_Right;
        }
    }

    private EnemyHit GetHitDirectionEnum(float angle)
    {
        if (angle > 30 && angle < 115)
        {
            return EnemyHit.Hit_Back_Left;
        }
        else if (angle > 115 && angle < 160)
        {
            return EnemyHit.Hit_Back_Left;
        }
        else if (angle > 160 && angle < 250)
        {
            return EnemyHit.Hit_Right;
        }
        else if (angle > 250 && angle < 360)
        {
            return EnemyHit.Hit_Left;
        }
        else
        {
            return EnemyHit.Hit_Left;
        }
    }

    private EnemyDeath GetDeathDirectionEnum(float angle)
    {
        if (angle >= 0 && angle < 90)
        {
            return EnemyDeath.Death_Front_Left;
        }
        else if (angle >= 90 && angle < 135)
        {
            return EnemyDeath.Death_Back; ////Flipped with Death_Front
        }
        else if (angle >= 135 && angle < 180)
        {
            return EnemyDeath.Death_Front_Right;
        }
        else if (angle >= 180 && angle < 210)
        {
            return EnemyDeath.Death_Right;
        }
        else if (angle >= 210 && angle < 240)
        {
            return EnemyDeath.Death_Back_Right;
        }
        else if (angle >= 240 && angle < 275)
        {
            return EnemyDeath.Death_Front; //Flipped with Death_Back
        }
        else if (angle >= 275 && angle < 320)
        {
            return EnemyDeath.Death_Back_Left;
        }
        else if (angle >= 320 && angle < 360)
        {
            return EnemyDeath.Death_Left;
        }
        else
        {
            return EnemyDeath.Death_Left;
        }
    }
}
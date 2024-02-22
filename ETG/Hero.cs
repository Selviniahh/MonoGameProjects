using System;
using System.Collections.Generic;
using System.Diagnostics;
using Animation2;
using ETG.Guns;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ETG;

public class Hero
{
    public static Vector2 Position = new Vector2(150,150);
    private readonly float _speed = 300f;
    private readonly Dictionary<string, AnimationManager> AnimManagerDict = new Dictionary<string, AnimationManager>();
    public static string _currentState = "EnemyIdle";
    private float _timer = 0f, _dashTimer;
    private bool _isDashing, _startDashTimer, _dashOnce = true;
    private int _beforeLerpX, _beforeLerpY;
    private int _dashAmount = 250, _dashCooldown = 1;
    private  Texture2D _redPixel;

    
    private float _rotation;                                                                                
    public RogueSpecial _gun;                                                                              
                                                                                                            
    private readonly IdlEnum[] _idleDirectionEnum = new[]                                                  
    {                                                                                                       
        IdlEnum.Idle_Back_Hand_Left,                                                                       
        IdlEnum.Idle_Back_Hand_Right,                                                                      
        IdlEnum.Idle_Diagonal_Hand_Left,                                                                   
        IdlEnum.Idle_Diagonal_Hand_Right,                                                                  
        IdlEnum.Idle_Front_Hand_Left,                                                                      
        IdlEnum.Idle_Front_Hand_Right,                                                                     
        IdlEnum.Idle_Left,                                                                                 
        IdlEnum.Idle_Right_Hands_Left,                                                                     
    };

    private readonly RunEnum[] _runDirectionEnum = new[]                                                    
    {                                                                                                       
        RunEnum.Run_Back_Hand_Left,                                                                         
        RunEnum.Run_Back_Hand_Right,                                                                        
        RunEnum.Run_Diagonal_Left,                                                                          
        RunEnum.Run_Diagonal_Right,                                                                         
        RunEnum.Run_Front_Hands_Left,                                                                       
        RunEnum.Run_Front_Hands_Right,                                                                      
        RunEnum.Run_Left,                                                                                   
        RunEnum.Run_Right,                                                                                  
    };

    private readonly DashEnum[] _dashDirectionEnum = new[]                                                  
    {             
        DashEnum.Dash_Diagonal_Right,
        DashEnum.Dash_Diagonal_Left,
        DashEnum.Dash_Left,
        DashEnum.Dash_Right,
        DashEnum.Dash_Back,                                                                                 
        DashEnum.Dash_Front,
        DashEnum.DashDiagonalDownLeft,
        DashEnum.DashDiagonalDownRight,
    };


    private IdlEnum _idleEnum;
    private static DashEnum _dashEnum;
    private static RunEnum _runEnum;
    private float _angleInDegrees;


    public Hero()
    {
        //just Textures
        var runTextures = new List<Texture2D>()
        {
            Globals.Content.Load<Texture2D>("New/Run/Run_Back_Hand_Left"),         //0
            Globals.Content.Load<Texture2D>("New/Run/Run_Back_Hand_Right"),         //1
            Globals.Content.Load<Texture2D>("New/Run/Run_Diagonal_Left"),         //2
            Globals.Content.Load<Texture2D>("New/Run/Run_Diagonal_Right"),         //3
            Globals.Content.Load<Texture2D>("New/Run/Run_Front_Hands_Left"),         //5
            Globals.Content.Load<Texture2D>("New/Run/Run_Front_Hands_Right"),         //6
            Globals.Content.Load<Texture2D>("New/Run/Run_Left"),         //7
            Globals.Content.Load<Texture2D>("New/Run/Run_Right"),         //8
        };

        var idleTextures = new List<Texture2D>()
        {
            Globals.Content.Load<Texture2D>("New/Idle/Idle_Back_Hand_Left2"),         //0
            Globals.Content.Load<Texture2D>("New/Idle/Idle_Back_Hand_Right"),         //0
            Globals.Content.Load<Texture2D>("New/Idle/Idle_Diagonal_Hand_Left"),         //0
            Globals.Content.Load<Texture2D>("New/Idle/Idle_Diagonal_Hand_Right"),         //0
            Globals.Content.Load<Texture2D>("New/Idle/Idle_Front_Hand_Left2"),         //0
            Globals.Content.Load<Texture2D>("New/Idle/Idle_Front_Hand_Right"),         //0
            Globals.Content.Load<Texture2D>("New/Idle/Idle_Left"),         //0
            Globals.Content.Load<Texture2D>("New/Idle/Idle_Right"),         //0 
        };
        var dashTextures = new List<Texture2D>()
        {
            Globals.Content.Load<Texture2D>("New/Dash/Dash_Diagonal_Right"), 
            Globals.Content.Load<Texture2D>("New/Dash/Dash_Diagonal_Left"),
            Globals.Content.Load<Texture2D>("New/Dash/Dash_Left"), 
            Globals.Content.Load<Texture2D>("New/Dash/Dash_Right"), 
            Globals.Content.Load<Texture2D>("New/Dash/Dash_Back"), 
            Globals.Content.Load<Texture2D>("New/Dash/Dash_Front"),
            Globals.Content.Load<Texture2D>("New/Dash/Dash_Left"), // Reuse Dash_Left texture for DashDiagonalDownLeft
            Globals.Content.Load<Texture2D>("New/Dash/Dash_Right"), // Reuse Dash_Right texture for DashDiagonalDownRight
        };


        var animationManagerRun = new AnimationManager();
        var animationManagerIdle = new AnimationManager();
        var animationManagerDash = new AnimationManager();

        //add animation for run store animations inside AnimationManager's disctionary
        for (int i = 0; i < runTextures.Count; i++)
        {
            animationManagerRun.AddAnimation(_runDirectionEnum[i], new Animation(runTextures[i], 0.15f,6,1));
            animationManagerRun.SetOrigin(_runDirectionEnum[i],new Vector2((float)runTextures[i].Width / 6 / 2, (float)runTextures[i].Height/2));
        }
        AnimManagerDict.Add("EnemyRun",animationManagerRun);
        

        //add animation for idle GunIdleTexture there's if else block due to different frame sizes
        for (int i = 0; i < idleTextures.Count; i++)
        {
            if (i is 2 or 3 or 6 or 7)
            {
                animationManagerIdle.AddAnimation(_idleDirectionEnum[i], new Animation(idleTextures[i], 0.15f,4,1));
                animationManagerIdle.SetOrigin(_idleDirectionEnum[i],new Vector2((float)idleTextures[i].Width / 4 / 2, (float)idleTextures[i].Height/2));
            }
            else if (i is 4 or 5)
            {
                animationManagerIdle.AddAnimation(_idleDirectionEnum[i], new Animation(idleTextures[i], 0.15f,5,1));
                animationManagerIdle.SetOrigin(_idleDirectionEnum[i],new Vector2((float)idleTextures[i].Width / 5 / 2, (float)idleTextures[i].Height/2));
            }
            else
            {
                animationManagerIdle.AddAnimation(_idleDirectionEnum[i], new Animation(idleTextures[i], 0.15f,6,1));
                animationManagerIdle.SetOrigin(_idleDirectionEnum[i],new Vector2((float)idleTextures[i].Width / 6 / 2, (float)idleTextures[i].Height/2));
            }

            
        }
        AnimManagerDict.Add("EnemyIdle",animationManagerIdle);

        //add animation for all dash textures
        for (int i = 0; i < dashTextures.Count; i++)
        {
            if (i is 2 or 3 or 6 or 7)
            {
                animationManagerDash.AddAnimation(_dashDirectionEnum[i], new Animation(dashTextures[i], 0.08f, 6,1));
                animationManagerDash.SetOrigin(_dashDirectionEnum[i], new Vector2((float)dashTextures[i].Width / 6 / 2,(float)dashTextures[i].Height/2));    
            }
            else
            {
                animationManagerDash.AddAnimation(_dashDirectionEnum[i], new Animation(dashTextures[i], 0.05f, 9,1));
                animationManagerDash.SetOrigin(_dashDirectionEnum[i], new Vector2((float)dashTextures[i].Width / 9 / 2,(float)dashTextures[i].Height/2));    
            }
            
        }

        AnimManagerDict.Add("Dash", animationManagerDash);
        
        _gun = new RogueSpecial(new Vector2(250,250), 0f);
    }

    public void Update()
    {
        _gun.Update(_gun.GunPosition);
        InputManager.HeroPosition = Position;
        InputManager.HeroOrigin = AnimManagerDict[_currentState].AnimationDict[AnimManagerDict[_currentState].LastKey].Origin;

        float angle = InputManager.GetMouseAngleRelativeToHero();
        _gun.GunRotation = angle;
        _idleEnum = GetIdleDirectionEnum(angle);
        _runEnum = GetRunDirectionEnum(angle);
             

        //Make Gun adjustments based on idleEnum
        switch (_idleEnum)
        {
            case IdlEnum.Idle_Back_Hand_Right:
                _gun.GunPosition = Position + new Vector2(40,20); //25 0
                _gun.GunSpriteEffects = SpriteEffects.None;
                _gun.GunOrigin = new Vector2(1,9);
                break;
            
            case IdlEnum.Idle_Back_Hand_Left:
                _gun.GunPosition = Position + new Vector2(-40,22); //25 0
                _gun.GunSpriteEffects = SpriteEffects.FlipVertically;
                _gun.GunOrigin = new Vector2(4,-1);
                break;
            
            case IdlEnum.Idle_Diagonal_Hand_Right:
                _gun.GunPosition = Position + new Vector2(35,30); //25 0
                _gun.GunSpriteEffects = SpriteEffects.None;
                _gun.GunOrigin = new Vector2(1,9);

                break;
            case IdlEnum.Idle_Diagonal_Hand_Left:
                _gun.GunPosition = Position + new Vector2(-40,30); //25 0
                _gun.GunSpriteEffects = SpriteEffects.FlipVertically;
                _gun.GunOrigin = new Vector2(4,-1);

                break;
            case IdlEnum.Idle_Left:
                _gun.GunPosition = Position + new Vector2(-45,38); //25 0
                _gun.GunSpriteEffects = SpriteEffects.FlipVertically;
                _gun.GunOrigin = new Vector2(4,-1);
                break;
            
            case IdlEnum.Idle_Right_Hands_Left: //Right
                _gun.GunPosition = Position + new Vector2(24,25); //25 0
                _gun.GunSpriteEffects = SpriteEffects.None;
                _gun.GunOrigin = new Vector2(1,9);
                break;
            
            case IdlEnum.Idle_Front_Hand_Left: 
                _gun.GunPosition = Position + new Vector2(45,35); //25 0
                _gun.GunSpriteEffects = SpriteEffects.None;
                _gun.GunOrigin = new Vector2(1,9);
                break;
            
            case IdlEnum.Idle_Front_Hand_Right: 
                _gun.GunPosition = Position + new Vector2(-25,50); //25 0
                _gun.GunSpriteEffects = SpriteEffects.FlipVertically;
                _gun.GunOrigin = new Vector2(4,-1);
                break;
            
            default: 
                _gun.GunPosition = Position + new Vector2(25,35); //25 0
                _gun.GunSpriteEffects = SpriteEffects.None;
                _gun.GunOrigin = new Vector2(1,9);
                break;
        }
        if (_currentState == "EnemyRun")
        {
            switch (_runEnum)
            {
                case RunEnum.Run_Front_Hands_Left:
                    _gun.GunPosition = Position + new Vector2(40,25); //25 0
                    _gun.GunSpriteEffects = SpriteEffects.None;
                    _gun.GunOrigin = new Vector2(1,9);
                    break;
                case RunEnum.Run_Front_Hands_Right: //Right
                    _gun.GunPosition = Position + new Vector2(-25,35); //25 0
                    _gun.GunSpriteEffects = SpriteEffects.FlipVertically;
                    _gun.GunOrigin = new Vector2(4,-1);
                    break;
            }
        }

        //set Current State
        if (InputManager.Moving && !_isDashing)
        {
            _currentState = "EnemyRun";
            Position += Vector2.Normalize(InputManager.Direction) * _speed * Globals.TotalSeconds;
        }
        else if (!InputManager.Moving && !_isDashing)
        {
            _currentState = "EnemyIdle";
        }
        else if (_isDashing)
        {
            _currentState = "Dash";
        }
        
        //if right button is clicked dash cooldown is finished and character is moving
        if (InputManager.CurrentMouse.RightButton == ButtonState.Released && InputManager.PreviousMouse.RightButton == ButtonState.Pressed && _dashTimer == 0 && InputManager.Moving)
        {
            _beforeLerpX = (int)Position.X;
            _beforeLerpY = (int)Position.Y;
            _isDashing = true;
            _timer = 0f;
        }

        if (_isDashing)
        {
            if (_dashOnce)
            {
                _dashEnum = GetDashDirectionEnum();
                _dashOnce = false;
            }
            _timer += Globals.TotalSeconds;
            float lerpAmount = _timer * 2;
            if (lerpAmount >= 1f)
            {
                _isDashing = false;
                lerpAmount = 1f;
            }

            switch (_dashEnum)
            {
                case DashEnum.Dash_Right : //D
                    Position.X = MathHelper.Lerp(_beforeLerpX, _beforeLerpX + _dashAmount, lerpAmount);
                    break;
                case DashEnum.Dash_Front: //S
                    Position.Y = MathHelper.Lerp(_beforeLerpY, _beforeLerpY + _dashAmount, lerpAmount);
                    break;
                case DashEnum.Dash_Left: //A
                    Position.X = MathHelper.Lerp(_beforeLerpX, _beforeLerpX - _dashAmount, lerpAmount);
                    break;
                case DashEnum.Dash_Back: //W
                    Position.Y = MathHelper.Lerp(_beforeLerpY, _beforeLerpY - _dashAmount, lerpAmount);
                    break;
                case DashEnum.Dash_Diagonal_Right: //WD
                    Position.X = MathHelper.Lerp(_beforeLerpX, _beforeLerpX + (float)_dashAmount / (float)Math.Sqrt(2), lerpAmount); 
                    Position.Y = MathHelper.Lerp(_beforeLerpY, _beforeLerpY - (float)_dashAmount / (float)Math.Sqrt(2), lerpAmount);
                    break;
                case DashEnum.Dash_Diagonal_Left: //AW
                    Position.X = MathHelper.Lerp(_beforeLerpX, _beforeLerpX - (float)_dashAmount / (float)Math.Sqrt(2), lerpAmount); 
                    Position.Y = MathHelper.Lerp(_beforeLerpY, _beforeLerpY - (float)_dashAmount / (float)Math.Sqrt(2), lerpAmount);
                    break;
                case DashEnum.DashDiagonalDownLeft: //SA NO ANIMATION
                    Position.X = MathHelper.Lerp(_beforeLerpX, _beforeLerpX - (float)_dashAmount / (float)Math.Sqrt(2), lerpAmount); 
                    Position.Y = MathHelper.Lerp(_beforeLerpY, _beforeLerpY + (float)_dashAmount / (float)Math.Sqrt(2), lerpAmount);
                    break;
                case DashEnum.DashDiagonalDownRight: //SD NO ANIMATION 
                    Position.X = MathHelper.Lerp(_beforeLerpX, _beforeLerpX + (float)_dashAmount / (float)Math.Sqrt(2), lerpAmount);
                    Position.Y = MathHelper.Lerp(_beforeLerpY, _beforeLerpY + (float)_dashAmount / (float)Math.Sqrt(2), lerpAmount);
                    break;
            }
            
            _startDashTimer = true;
        }
        
        //cooldown
        if (_startDashTimer)
        {
            _dashTimer += Globals.TotalSeconds;
            if (_dashTimer > _dashCooldown)
            {
                _dashTimer = 0;
                _startDashTimer = false;
                _dashOnce = true;
            }
        }

        switch (_currentState)
        {
            case "Dash":
                AnimManagerDict[_currentState].Update(_dashEnum, Position);
                break;
            case "EnemyRun":
                AnimManagerDict[_currentState].Update(_runEnum, Position);
                break;
            case "EnemyIdle":
                AnimManagerDict[_currentState].Update(_idleEnum, Position);
                break;
        }
    }
    
    public void Draw()
    {
        if (!_isDashing) _gun.Draw(_gun.GunPosition, 4f);
        AnimManagerDict[_currentState].Draw(Position, 0.01f);
        

        Globals.SpriteBatch.DrawString(Globals.Font,"Angle: " + _angleInDegrees,new Vector2(0,90),Color.White);
        Globals.SpriteBatch.DrawString(Globals.Font,"CurrentState: " + _currentState,new Vector2(0,105),Color.White);

    }
    
    //get Angle and just return correct enum based on Angle 
    private IdlEnum GetIdleDirectionEnum(float angle)
    {
        _angleInDegrees = MathHelper.ToDegrees(angle);
        // Debug.WriteLine(_angleInDegrees);

        if (_angleInDegrees < 0) _angleInDegrees += 360;
        
        if (_angleInDegrees >= 337.5 || _angleInDegrees < 22.5)
        {
            return IdlEnum.Idle_Right_Hands_Left;
        }
        if (_angleInDegrees >= 22.5 && _angleInDegrees < 67.5)
        {
            return IdlEnum.Idle_Right_Hands_Left;
        }
        if (_angleInDegrees >= 67.5 && _angleInDegrees < 100.5)
        {
            return IdlEnum.Idle_Front_Hand_Left;
        }
        if (_angleInDegrees >= 100.5 && _angleInDegrees < 157.5)
        {
            return IdlEnum.Idle_Front_Hand_Right;
        }
        if (_angleInDegrees >= 157.5 && _angleInDegrees < 202.5)
        {
            return IdlEnum.Idle_Left;
        }
        if (_angleInDegrees >= 202.5 && _angleInDegrees < 247.5)
        {
            // Add a new enum for the diagonal left-down direction
            return IdlEnum.Idle_Diagonal_Hand_Left;
        }
        if (_angleInDegrees >= 247.6 && _angleInDegrees < 270.5)
        {
            return IdlEnum.Idle_Back_Hand_Left;
        }
        if (_angleInDegrees >= 270.5 && _angleInDegrees < 290)
        {
            return IdlEnum.Idle_Back_Hand_Right;
        }
        
        return IdlEnum.Idle_Right_Hands_Left;
    }
    private RunEnum GetRunDirectionEnum(float angle)
    {
        _angleInDegrees = MathHelper.ToDegrees(angle);
        // Debug.WriteLine(_angleInDegrees);

        if (_angleInDegrees < 0) _angleInDegrees += 360;
        
        if (_angleInDegrees >= 337.5 || _angleInDegrees < 22.5)
        {
            return RunEnum.Run_Right;
        }
        if (_angleInDegrees >= 22.5 && _angleInDegrees < 67.5)
        {
            return RunEnum.Run_Right;
        }
        if (_angleInDegrees >= 67.5 && _angleInDegrees < 100.5)
        {
            return RunEnum.Run_Front_Hands_Left;
        }
        if (_angleInDegrees >= 100.5 && _angleInDegrees < 157.5)
        {
            return RunEnum.Run_Front_Hands_Right;
        }
        if (_angleInDegrees >= 157.5 && _angleInDegrees < 202.5)
        {
            return RunEnum.Run_Left;
        }
        if (_angleInDegrees >= 202.5 && _angleInDegrees < 247.5)
        {
            // Add a new enum for the diagonal left-down direction
            return RunEnum.Run_Diagonal_Left;
        }
        if (_angleInDegrees >= 247.6 && _angleInDegrees < 270.5)
        {
            return RunEnum.Run_Back_Hand_Left;
        }
        if (_angleInDegrees >= 270.5 && _angleInDegrees < 290)
        {
            return RunEnum.Run_Back_Hand_Right;
        }
        
        return RunEnum.Run_Right;
    }

    private DashEnum GetDashDirectionEnum()
    {
        if (InputManager.CurrentKeyboard.IsKeyDown(Keys.D) && InputManager.CurrentKeyboard.IsKeyDown(Keys.W))
        {
            return DashEnum.Dash_Diagonal_Right; 
        }
        if (InputManager.CurrentKeyboard.IsKeyDown(Keys.A) && InputManager.CurrentKeyboard.IsKeyDown(Keys.W))
        {
            return DashEnum.Dash_Diagonal_Left; 
        }
        if (InputManager.CurrentKeyboard.IsKeyDown(Keys.A) && InputManager.CurrentKeyboard.IsKeyDown(Keys.S))
        {
            return DashEnum.DashDiagonalDownLeft; 
        }

        if (InputManager.CurrentKeyboard.IsKeyDown(Keys.D) && InputManager.CurrentKeyboard.IsKeyDown(Keys.S))
        {
            return DashEnum.DashDiagonalDownRight; 
        }
        if (InputManager.CurrentKeyboard.IsKeyDown(Keys.A))
        {
            return DashEnum.Dash_Left;
        }
        if (InputManager.CurrentKeyboard.IsKeyDown(Keys.D))
        {
            return DashEnum.Dash_Right;
        }
        if (InputManager.CurrentKeyboard.IsKeyDown(Keys.W))
        {
            return DashEnum.Dash_Back;
        }
        if (InputManager.CurrentKeyboard.IsKeyDown(Keys.S))
        {
            return DashEnum.Dash_Front;
        }

       
        return DashEnum.Dash_Front;
    }
}
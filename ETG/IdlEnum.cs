namespace ETG;

//Hero Enums
public enum IdlEnum
{
    Idle_Back_Hand_Left,
    Idle_Back_Hand_Right,
    Idle_Diagonal_Hand_Left,
    Idle_Diagonal_Hand_Right,
    Idle_Front_Hand_Left,
    Idle_Front_Hand_Right,
    Idle_Left,
    Idle_Right_Hands_Left,
}

public enum DashEnum
{
    Dash_Back,
    Dash_Diagonal_Left,
    Dash_Diagonal_Right,
    Dash_Front,
    Dash_Left,
    Dash_Right,
    DashDiagonalDownLeft,
    DashDiagonalDownRight,
}

public enum RunEnum
{
    Run_Back_Hand_Left,
    Run_Back_Hand_Right,
    Run_Diagonal_Left,
    Run_Diagonal_Right,
    Run_Front_Hands_Left,
    Run_Front_Hands_Right,
    Run_Left,
    Run_Right,
}

//Enemy Enums
public enum EnemyIdle
{
    Idle_Back,
    Idle_Left,
    Idle_Right,
}

public enum EnemyRun
{
    Run_Left,
    Run_Left_Back,
    Run_Right,
    Run_Right_Back,
}

public enum EnemyHit
{
    Hit_Right,
    Hit_Left,
    Hit_Back_Left,
    Hit_Back_Right,
}

public enum EnemyDeath
{
    Death_Back,
    Death_Front,
    Death_Front_Left,
    Death_Back_Left,
    Death_Back_Right,
    Death_Left,
    Death_Front_Right,
    Death_Right,
}
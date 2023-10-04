using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//using an enum to control the different game states. 
//player turn and enemy turn should alternate with one another

[System.Serializable] //can be serialized in the inspector

public class GameState
{
    public enum Game_States { START, PLAYER_TURN, ENEMY_TURN }// 0     // 1     // 2    

    public Game_States currentState;
}

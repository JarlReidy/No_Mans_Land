using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//Setting the time limit for the players turn
//This is to increase pacing and the frequency of interactions between the systems
//Will be a countdown timer that resets when it is no longer the player's turn
public class TimerScript : MonoBehaviour
{
    public Game_Manager manager;//referencing the game manager script
    public GameState gameState;
    public TextMeshProUGUI scoretext;//the text on the canvas which communicates
    //timer score to the player
    public float timer = 15.0f;//timer which the player has
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(manager.playerTurn)//if it's the player turn
        {
            timer -= Time.deltaTime;//will decrease in accordance with the
            //run-time
            
            if(timer <= 0)//if the timer reaches it's limit
            {
                //manager.playerTurn = false;//not the players turn anymore
                //manager.enemyTurn = true;//it's the enemy turn
                timer = 0;//timer has reset.
                //gameState.currentState = GameState.Game_States.ENEMY_TURN;
            }
        }

        scoretext.text = timer.ToString("f1");//setting the value of the textbox to be timer
        //ToString as timer is a float value
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//------------------------------------------------------------
//Controlling the Artillery on the tile board
//------------------------------------------------------------
public class ArtilleryScript : MonoBehaviour
{
    [Header("Artillery Objects")]

    public GameObject friendlyFire;//showing the friendly artillery firing, it will toggle from active/inactive based on the players turn
    public GameObject enemyFire;//same as above, but for the enemy artillery
    public GameState gamestate;//referencing the grid class script, to fetch the game state
    public Game_Manager grid;//the grid class for reference
    public float counter = 0;


    public AudioClip fire;//the audioclip which is used for the firing of the artillery

    public int friendlyStrike = 0;//to record the fact only one shot per turn

    // Start is called before the first frame update
    void Start()
    {
        //both GameObjects are set to false at the start of the game
        friendlyFire.SetActive(false);
        enemyFire.SetActive(false);
        InvokeRepeating("EnemyFire", 2.0f, 21.0f);//call the fire mechanic every 22 seconds
        InvokeRepeating("EnemyFireOff", 1.5f, 20.0f);
    }

    // Update is called once per frame
    void Update()
    {
        //Controlling the shooting mechanics based on the game-state
        // if the player can shoot the artillery will interact as such and fire will come from the body
        // if the player cannot shoot, the fire will be set to inactive as the artillery cannot be used
        if (grid.playerTurn)
        {
            enemyFire.SetActive(false);//enemy artillery inactive as it's player turn
            grid.enemyTurn = false;

            if (TroopScript.friendlySupplies >=50)
            {
                if(Input.GetMouseButtonDown(1))
                {
                    friendlyFire.SetActive(true);//player artillery active to make shot
                    AudioSource.PlayClipAtPoint(fire, transform.position, 1000.0f);//plays the clip at the field guns at 10 decibels
                    Invoke("Fire", 1.5f);
                }
                
            }

        }
       

    }

     private void Fire()
    {
        friendlyStrike = friendlyStrike + 1;
        friendlyFire.SetActive(false);
        grid.enemyTurn = true;

    }

    private void EnemyFire()
    {
        enemyFire.SetActive(true);
        AudioSource.PlayClipAtPoint(fire, transform.position, 1000.0f);
        grid.playerTurn = true;
    }

    private void EnemyFireOff()
    {
        enemyFire.SetActive(false);
    }
}


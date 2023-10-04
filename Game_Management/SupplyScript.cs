using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//--------------------------------------------------------------------------
// Script responsible for the supplies which will be randomly spawned across the battlefield.
// Supplies provide the opportunity to spawn new units as reinforcements both for the player and the enemy
// random distribution of the supplies ensures fairness for both sides
//--------------------------------------------------------------------------
public class SupplyScript : MonoBehaviour
{
    [Header(" Supply Qualities :")]

    public int currentX;//current tile x co-ordinate
    public int currentY;//current tile y co-ordinate
    public Vector3 pos;//position in worldspace
    public int friendlySupplies = 0;//current count for friendly supplies
    public int enemySupplies = 0;//current count for enemy supplies

   

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    //Setting the supplies spawn position on the tile
    public virtual void SetPosition(Vector3 position, bool force = false)//can be overriden in main game loop
    {
        pos = position;//position of the spawned box
        if (force)//if force is applied to the spawn box.
        {
            transform.position = pos;
        }
    } 
}

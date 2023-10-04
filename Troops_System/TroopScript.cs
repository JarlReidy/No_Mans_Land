using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//enum used to index the different units involved
public enum TroopType
{
    None = 0,
    Light_Infantry = 1,
    Heavy_Infantry = 2,
    Normal_Infantry = 3
}

public enum EnemyTroopType
{
    None = 0,
    Light_Infantry = 1,
    Heavy_Infantry = 2,
    Normal_Infantry = 3
}
//--------------------------------------------------------
//each troop type inherits from the base class
//--------------------------------------------------------
public class TroopScript : MonoBehaviour
{
    [Header("Unit Qualities :")]
    public TroopType type;//using enum for differentiating between them
    public EnemyTroopType enemytype;//using enum for differentiating between them
    public int team;//an integer which denotes the player/enemy units from one another
    public int currentX;//current tile x co-ordinate
    public int currentY;//current tile y co-ordinate
    private Vector3 targetPos;

    [Header(" AudioClips :")]
    [SerializeField] public AudioClip munitionPickUp;//sound when a unit picks up a supply box.

    //static integers so it's will not be adjusted despite multiple instances of the script allocated to multiple prefabs
    [Header(" Munitions :")]
    public static int friendlySupplies;//integer used to denote which
    public static int enemySupplies;


    private void Update()
    {
        //smooth function that interpolates between a starting (transform.position) position and a target transform.position
        //targetPos will store the relative area which the unit is commanded to move to
        //TileCenter will snap the troop movement to the center of the tile targetPos is located within
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * 2);//higher values can be used to simulate a 'skip' mechanic
        
    }

    //Setting the position of the troop units
    public virtual void SetPosition(Vector3 position, bool force = false)
    {
        targetPos = position;
        if(force)
        {
            transform.position = targetPos;
        }
    }
    //OnTriggerStay for when the units box collider intersects with the box collider of the supply box
    private void OnTriggerStay(Collider other)
    {
        if (this.gameObject.tag == "friendly")//all friendly units have the 'friendly' tag
        {
            if (other.gameObject.tag == "munitions")//if a friendly unit collides with a gameObject with the tag = 'munitions'.
            {
                friendlySupplies = friendlySupplies + 50;//adding 50 supplies each time, can be used for reinforcements or artillery strikes
                AudioSource.PlayClipAtPoint(munitionPickUp, other.gameObject.transform.position, 10f);//playing the munitions pick-up sound effect. 
                other.gameObject.SetActive(false);//setting supply box as inactive
            }
        }
        if (this.gameObject.tag == "enemy")//all enemy units have an 'enemy' tag
        {
            if (other.gameObject.tag == "munitions")//if an enmy unit collides with a gameObject with the tag = 'munitions'.
            {
                enemySupplies = enemySupplies + 50;//adding 50 enemy supplies, can be used for reinforcements, artillery strikes have no AI cost.
                AudioSource.PlayClipAtPoint(munitionPickUp, other.gameObject.transform.position, 10f);//playing the munitions pick-up sound effect.
                other.gameObject.SetActive(false);
            }
        }
    }

    //overriden in the troop classes
    //this is because the different units will have the capability to execute different moves and hence, need different iterations of this function.
    public virtual List <Vector2Int> AvailableMoves(ref TroopScript[,] tileboard, int tileX, int tileY)//passing in the board as readonly
    {
        List<Vector2Int> list = new List<Vector2Int>();//a list of vector 2 integers denoting the different tiles within the tile board
        //if the targetPos is outside of the list bounds, there will be no moves executed

        

        return list;//returning the list of available moves for each of the units
    }
}

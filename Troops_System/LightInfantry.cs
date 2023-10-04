using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//foundation class for the military units, using the override for Available Moves
//light infantry will be able to move the furthest out of all 3 infantry types
//an extension of the troopscript
public class LightInfantry : TroopScript
{
    
    
    // Update is called once per frame
    public override List<Vector2Int> AvailableMoves(ref TroopScript[,] troop, int tileCountX, int tileCountY)
    {
        List<Vector2Int> lightmoves = new List<Vector2Int>();

        int advanceDir = (team == 0) ? 1 : -1;//friendly team

        
        //One to the left
        if (troop[currentX, currentY + advanceDir] == null)//if there is nothing in the tile which the light infantry is moving into
        {
            lightmoves.Add(new Vector2Int(currentX, currentY + advanceDir)); // advance into the next tile
        }
        //One to the right
        else if(troop[currentX,currentY-advanceDir]==null)
        {
            lightmoves.Add(new Vector2Int(currentX, currentY - advanceDir));
        }
        
        //Advance One in front
        if(troop[currentX + advanceDir, currentY] == null)
        {
            lightmoves.Add(new Vector2Int(currentX + advanceDir, currentY)); // advance into the next tile
        }
        //Advance Two In Front
        if (troop[currentX + advanceDir, currentY] == null)
        {
            //Friendly Team?
            if(team==0 &&currentX == 1 && troop[currentX, currentY + (advanceDir*2)] == null)
            {
                lightmoves.Add(new Vector2Int((currentX + advanceDir)*2, currentY));
            }
            if (team == 1 && currentX == 17 && troop[currentX, currentY + (advanceDir * 2)] == null)
            {
                lightmoves.Add(new Vector2Int((currentX + advanceDir) * 2, currentY));
            }
        }

        //Debug.Log(lightmoves);
        return lightmoves;//return the listed move
        
    }
}

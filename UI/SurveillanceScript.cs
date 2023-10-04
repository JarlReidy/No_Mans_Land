using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//------------------------------------------------------------------------------------------------------------------------------
//plane object will move between a set of waypoints before being destroyed, this is to simulate real-time surveillance from WW1.
//overhead surveillance provided great intelligence used by the artillery below in order to be as potent as possible.
//------------------------------------------------------------------------------------------------------------------------------
public class SurveillanceScript : MonoBehaviour
{
    // the plane game object
    [Header("Plane Game Object :")]
    //[SerializeField] private GameObject biPlane;//the plane object
    [SerializeField] private int speed = 5;//speed of the planes translations
    [SerializeField] private int moveIndex = 0;//currently stored index of the current waypoint marker
    [SerializeField] private bool looping = true;//setting the plane to loop in its orbit
    [SerializeField] private AudioClip planeHover;//will play whenever the plane is over the map.
    [Header("Way-points :")]
    public List<GameObject> wayPoints;//the different waypoints

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Fly(); //calling the fly script
    }

    private void Fly()//plane travels along the waypoint system, calling in audio as it reaches a certain point
    {
        
        Vector3 flyingPosition = Vector3.MoveTowards(transform.position, wayPoints[moveIndex].transform.position, speed * Time.deltaTime);
        Quaternion flyingRotation = Quaternion.Euler(wayPoints[moveIndex].transform.position.x, 180, wayPoints[moveIndex].transform.position.z);
        transform.position = flyingPosition;//updating the transform based on the flight position
        transform.rotation = flyingRotation;
       // transform.LookAt(flyingRotation.x, 180, flyingRotation.z);//updating the transform based on the flight rotation
        float distance = Vector3.Distance(transform.position, wayPoints[moveIndex].transform.position);
        if (distance <= 0.05)
        {
            if (moveIndex < wayPoints.Count - 1)//if the move is valid and there are elements left in the list
            {
                moveIndex++;
                if(moveIndex == 4)//if moveIndex = 4 or 5
                {
                    AudioSource.PlayClipAtPoint(planeHover, flyingPosition, 2.5f);
                }
                
            }
            else
            {
                if(looping)
                {
                    moveIndex = 0;
                }
            }
        }
      //  Debug.Log(moveIndex);
    }
}

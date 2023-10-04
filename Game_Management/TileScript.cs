using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileScript : MonoBehaviour
{
    Game_Manager gridClass;

    [Header("Properties of the Tiles")]
    [SerializeField] private Material baseMaterial, hoverMaterial;
    [SerializeField] private MeshRenderer baseRender;
    [SerializeField] public bool isPlaced = false;

    Ray ray;
    RaycastHit strike;



    private bool isHover = false;//is currently hovering over this specific tile
    private bool shellStrike = false;//has the tile been clicked for shell striking?

    // Start is called before the first frame update

    void Start()
    {
        gridClass = GameObject.Find("Game_Manager").GetComponent<Game_Manager>();
    }

    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out strike))
        {
            if (Input.GetMouseButtonDown(0) && strike.collider.gameObject.name == gameObject.name)//has this tile been clicked?
            {
                // gridClass.Shoot(strike.collider.gameObject);
            }
        }


    }

    public void Init(bool hovering)
    {
        baseRender.material = hovering ? baseMaterial : hoverMaterial;
    }

    private void OnMouseEnter()
    {
        baseRender.material = hoverMaterial;
        isHover = true;
    }

    private void OnMouseExit()
    {
        baseRender.material = baseMaterial;
        isHover = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "friendly")
        {
            isPlaced = true;//has the unit been placed on the current tile
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class Cursor : MonoBehaviour
{
    private Player player;
    public int playerId = 0;

    Rigidbody rb;
    float moveSpeed = 1200f;
    Vector3 moveVector;
    CharacterController cc;

    GameObject currentlyHoveredItem;//the shop item the player is currently hovering over
    GameObject currentlyHeldItem;//the item the player has already paid for and can place somewhere

    bool canPlaceItem = false;// set to true when the player's cursor is over a valid location for the trap/tower to be placed

    public bool towerMode = false;//

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //cc = GetComponent<CharacterController>();
        // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
        player = ReInput.players.GetPlayer(playerId);
    }

    void Start()
    {
        Debug.Log(player + " " +  playerId);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "ShopItem")
        {
            //make this item the "currently hovered" item
            currentlyHoveredItem = other.gameObject.GetComponent<ShopItem>().item;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "ShopItem")
        {
            currentlyHoveredItem = null;
        }
    }

    void BuyItem()
    {
        //check scrap cost
        if(currentlyHeldItem)
        {
            if(currentlyHeldItem == currentlyHoveredItem)
            {
                //player is trying to buy what they already have
                //make sure they know they currently have that item already, and how to place it
                return;
            }
            //can't buy things while holding something
            //make an error noise & visual, emphasize the "sell item" button/shop area
            //or let the player trade their old item + cost difference for the new item?
            return;
        }
        if (currentlyHoveredItem)
        {
            GameObject o = Instantiate(currentlyHoveredItem, transform.position, Quaternion.identity);
            currentlyHeldItem = o;
            currentlyHeldItem.transform.parent = gameObject.transform;
        }
        else
        {
            Debug.Log("Player tried to buy something while not hovering any item at shop");
        }
    }

    void MoveCursor()
    {
        if (!towerMode)
        {
            return;
        }
        moveVector.x = player.GetAxis("MoveHorizontal");
        moveVector.y = player.GetAxis("MoveVertical");

        if (moveVector.x != 0.0f || moveVector.y != 0.0f)
        {
            rb.velocity = new Vector3(1 * player.GetAxis("MoveHorizontal") * Time.deltaTime * moveSpeed, 0, 1 * player.GetAxis("MoveVertical") * Time.deltaTime * moveSpeed);
        }
        else
        {
            rb.velocity = new Vector3(0, 0, 0);
        }
    }

    void SelectPressed()
    {
        Debug.Log("select");
        if (currentlyHoveredItem)
        {
            BuyItem();
        }
        else if (currentlyHeldItem)
        {
            //try to place item
            //check if there's anything in the way when activating collider
            currentlyHeldItem.GetComponent<BoxCollider>().enabled = true;
            currentlyHeldItem.transform.parent = null;
            currentlyHeldItem = null;
        }
    }

    void Update()
    {
        MoveCursor();
        if (player.GetButtonDown("Select"))
        {
            SelectPressed();
        }
    }
}

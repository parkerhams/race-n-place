using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class Cursor : MonoBehaviour
{
	public int playerId = 0;
	
	public bool towerMode = false;//
	
	[HideInInspector]
	public Player player;

	private GameObject center;
	private Rigidbody rb;
    
    private float moveSpeed = 1200f;
	private Vector3 moveVector;
    
    private Vector3 startPosition;

	private int scrap = 0;
    
	private int layerMask_adjustHeightLayer = 1<< 12;//bit shift index of ground layer (12), get bit mask

	private GameObject currentlyHoveredItem;//when the player hovers their cursor over a shop item; this is the actual item that the shop item has a reference to
    private GameObject currentlyHeldItem;//the item the player has already paid for and can place somewhere

	// set to true when the player's cursor is over a valid location for the trap/tower to be placed
	private bool canPlaceItem = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //cc = GetComponent<CharacterController>();
        // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
        player = ReInput.players.GetPlayer(playerId);
        startPosition = transform.position;
    }

	private void Start()
	{
		center = this.transform.Find("center").gameObject;
        Debug.Log(player + " " +  playerId + " start position is " + startPosition);
	}
	
	private void Update()
	{
		MoveCursor();
		
		if (player.GetButtonDown("Select"))
		{
			SelectPressed();
		}
		if(player.GetButtonDown("DebugSwapMode"))
		{
			GameManager.Instance.SwapMode();
		}
	}
    
	private void AdjustCursorHeight()
	{
		//raycast hit downwards, tell the cursor to change its Y height to match what the raycast just hit
		RaycastHit hit;
		//does the ray intersect with the ground layer?
		if(Physics.Raycast(this.transform.position, transform.TransformDirection(Vector3.down), 
			out hit, Mathf.Infinity, layerMask_adjustHeightLayer))
		{
			//draw a line from wherever the cursor height is that is the distance between the cursor and the height of what it just hit 
			Debug.DrawRay(this.transform.position, 
				this.transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
			
			float heightDifference =  this.transform.position.y - (.5f*hit.distance);
			
			//this.transform.position = new Vector3(this.transform.position.x, heightDifference, 
			//this.transform.position.z);
		}
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

	private void BuyItem()
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
	        currentlyHeldItem.transform.SetParent(this.gameObject.transform);
        }
        else
        {
            Debug.Log("Player tried to buy something while not hovering any item at shop");
        }
    }

	private void MoveCursor()
    {
	    if (GameManager.Instance.mode != GameManager.Mode.Place)
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
        
	    AdjustCursorHeight();
    }

	private void SelectPressed()
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
	        if(currentlyHeldItem.GetComponent<SpikeTrap>())
	        {
	        	currentlyHeldItem.GetComponent<SpikeTrap>().isPlaced = true;
	        }
            currentlyHeldItem.transform.parent = null;
	        currentlyHeldItem = null;
        }
    }

	/// <summary>
    /// When switching to or from place mode, hide or show the cursors
    /// </summary>
    public void ToggleActive(bool enable)
    {
        if(!enable)//turn off cursors and go into race mode
        {
            if(currentlyHeldItem)
            {
	            if(!currentlyHeldItem.GetComponent<Item>()) 
	            { 
	            	Debug.LogWarning("Item has no Item component!"); 
	            	return; 
	            }
                scrap += currentlyHeldItem.GetComponent<Item>().scrapCost;
            }
            
            Destroy(currentlyHeldItem);
            currentlyHeldItem = null;
            currentlyHoveredItem = null;
            transform.position = startPosition;
            gameObject.SetActive(false);
        }
        
        else//turn on cursors
        {
            gameObject.SetActive(true);
            transform.position = startPosition;
        }
    }
}

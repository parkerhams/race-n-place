using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is for the versions of traps/towers that are sitting in the shop area waiting for players to purchase them.
/// They hold a reference to the real version of their trap/tower and send that reference to cursors that hover over them.
/// </summary>
public class ShopItem : MonoBehaviour
{
	public GameObject item;
	[Tooltip("The quantity of traps up for sale when place mode starts.")]
	public int defaultQuantity = 1;
	[HideInInspector]
	public int quantity = 1;//how many more times this trap can currently be purchased
	public bool canBePurchased = true;
	Vector3 normalSize;
	
	void OnEnable()
	{
		GameManager.ModeSwapped += Restock;
	}
	void OnDisable()
	{
		GameManager.ModeSwapped -= Restock;
	}

    private void Start()
    {
        if(!item)
        {
            Debug.LogWarning("Shop item " + gameObject + " has no item assigned in inspector!");
        }
	    normalSize = transform.localScale;
    }

	int cursorsHovering = 0;//the number of player cursors currently hovering over this shop item
    
	void Restock()
	{
		if(GameManager.Instance.mode == GameManager.Mode.Place)
		{
			quantity = defaultQuantity;
			CheckQuantity();
		}
		else if(GameManager.Instance.mode == GameManager.Mode.Race)
		{
			//hide shop items during the race
			ToggleVisible(false);
		}
	}
    
	public void ChangeQuantity(int change)
	{
		quantity += change;
		CheckQuantity();
		
	}
	
    
	public void CheckQuantity()//if the shop item runs out of quantity, remove it so others can't purchase it
	{
		if(quantity <= 0)
		{
			quantity = 0;
			//hide shop item
			canBePurchased = false;
			ToggleVisible(false);
		}
		if(quantity >= 1)
		{
			//show shop item
			canBePurchased = true;
			ToggleVisible(true);
		}
	}
	
	private void ToggleVisible(bool visible)
	{
		if(visible)
		{
			transform.localScale = normalSize;
		}
		else
		{
			transform.localScale = new Vector3(0, 0, 0);
		}
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Cursor")
        {
            //highlight this shop item if not already highlighted
            //show name and cost if we hide that info until it's highlighted
            cursorsHovering++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Cursor")
        {
            cursorsHovering--;
            if(cursorsHovering <= 0)
            {
                cursorsHovering = 0;
                //unhighlight, hide name/cost
            }
        }
    }
}

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

    private void Start()
    {
        if(!item)
        {
            Debug.LogWarning("Shop item " + gameObject + " has no item assigned in inspector!");
        }
    }

    int cursorsHovering = 0;//the number of player cursors currently hovering over this shop item

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Cursor")
        {
            //highlight this shop item if not already highlighted
            //show name and cost if we hide that info until it's highlighted
            cursorsHovering++;
            Debug.Log("entered");
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

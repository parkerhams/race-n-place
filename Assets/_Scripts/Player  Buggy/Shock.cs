using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shock : MonoBehaviour
{
	//Minimum height to start exerting force
	public float raycastDistance = .5f;
	public float maxShockForce = 1f;
	public float damping = 0.9f;
	private Rigidbody buggyRigidbody;
	
	[Header("Shock Transforms")]
	//Serialized fields for each shock transform
	public Transform[] shockTransforms;
    // Start is called before the first frame update
    void Start()
    {
	    buggyRigidbody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
	void FixedUpdate()
	{
		CheckRaycastsFixed();
	}
	
	void CheckRaycastsFixed()
	{
		//Trans Rights
		foreach (Transform shockTrans in shockTransforms)
		{
			//set downward direction vector for the individual shock transform
			Vector3 down = shockTrans.TransformDirection(Vector3.down);
			//find velocity in that direction
			
			
			//debug cast ray for visual
			Debug.DrawRay(shockTrans.position, down * raycastDistance, Color.cyan);
			
			//determine damping force in accordance to velocity
			float dampForce = buggyRigidbody.velocity.y * 
			(damping * maxShockForce);
			
			//cast a ray down to see if if intersects with the ground
			//If the ray hits, add force (Will un-simplify later
			RaycastHit hit;
			if (Physics.Raycast(shockTrans.position, shockTrans.TransformDirection(Vector3.down), out hit, raycastDistance))
			{
				//proportionally adjust the shock Force
				buggyRigidbody.AddForceAtPosition(
					shockTrans.TransformDirection(Vector3.up) * 
					//multiply by set shock force
					(maxShockForce - dampForce) * 
					//proportion to how close hit is to the shock point
					//low distance, high force, high distance, low force
					(1 - (hit.distance/raycastDistance)), 
					shockTrans.transform.position);
			}
		}
		
		//TODO FOR NEXT CODING SESSION: Make the shocks Squishy
	}
}

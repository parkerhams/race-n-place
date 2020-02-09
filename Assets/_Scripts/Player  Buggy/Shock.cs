using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shock : MonoBehaviour
{
	
	public float raycastDistance = .5f;
	public float shockForce = 1f;
	private Rigidbody rigidbody;
	
	[Header("Shock Transforms")]
	//Serialized fields for each shock transform
	public Transform[] shockTransforms;
    // Start is called before the first frame update
    void Start()
    {
	    rigidbody = this.GetComponent<Rigidbody>();
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
			//cast a ray down to see if if intersects with the ground
			Vector3 down = shockTrans.TransformDirection(Vector3.down);
		
			Debug.DrawRay(shockTrans.position, down * raycastDistance, Color.cyan);
			//If the ray hits, add force (Will un-simplify later
			if (Physics.Raycast(shockTrans.position, shockTrans.TransformDirection(Vector3.down), raycastDistance))
			{
				rigidbody.AddForceAtPosition(shockTrans.TransformDirection(Vector3.up) * shockForce, shockTrans.transform.position);
			}
		}
		
		//TODO FOR NEXT CODING SESSION: Make the shocks Squishy
	}
}

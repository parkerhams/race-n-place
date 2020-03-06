using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilTrap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
	// OnTriggerEnter is called when the Collider other enters the trigger.
	protected void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			//other.gameObject.GetComponentInParent<Buggy>().inOil = true;
			Debug.Log("entered oil");
			WheelCollider[] wheels = other.gameObject.GetComponentInParent<Buggy>().wheelColliders;
			foreach(WheelCollider wheel in wheels)
			{
				WheelFrictionCurve myWfc;
				myWfc = wheel.sidewaysFriction;
				myWfc.stiffness = 5f;
				wheel.sidewaysFriction = myWfc;
				//wheel.forwardFriction.stiffness = .5f;
				//wheel.sidewaysFriction.stiffness = .1f;
			}
			
		}
	}
	
	// OnTriggerExit is called when the Collider other has stopped touching the trigger.
	protected void OnTriggerExit(Collider other)
	{
		if(other.tag == "Player")
		{
			//other.gameObject.GetComponentInParent<Buggy>().inOil = false;
			Debug.Log("exited oil");
			WheelCollider[] wheels = other.gameObject.GetComponentInParent<Buggy>().wheelColliders;
			foreach(WheelCollider wheel in wheels)
			{
				WheelFrictionCurve myWfc;
				myWfc = wheel.sidewaysFriction;
				myWfc.stiffness = 1f;
				wheel.sidewaysFriction = myWfc;
			}
		}
	}
}

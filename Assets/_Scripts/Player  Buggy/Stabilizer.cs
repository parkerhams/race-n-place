using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stabilizer : MonoBehaviour
{
	[Header("Rigidbody Center of Mass Offset")]
	//public float xOffset;
	public float yOffset;
	public float zOffset;
	
	//Buggy components
	private Rigidbody buggyRB;
	
	[Header("Front Axle Wheel Colliders")]
	public WheelCollider RFC;
	public WheelCollider LFC;
	
	[Header("Back Axle Wheel Colliders")]
	public WheelCollider RBC;
	public WheelCollider LBC;
	
	public float springForce;
    // Start is called before the first frame update
    void Start()
    {
	    buggyRB = this.GetComponent<Rigidbody>();
	    OffsetCenter();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
	void FixedUpdate()
	{
		StabilizeFrontWheels();
		StabilizeBackWheels();
	}
	
	void StabilizeFrontWheels()
	{
		WheelHit hit = new WheelHit();
		float travelL = 1.0f;
		float travelR = 1.0f;

		bool groundedL = LFC.GetGroundHit(out hit);

		if (groundedL)
		{
			travelL = (-LFC.transform.InverseTransformPoint(hit.point).y 
				- LFC.radius) / LFC.suspensionDistance;
		}

		bool groundedR = RFC.GetGroundHit(out hit);

		if (groundedR)
		{
			travelR = (-RFC.transform.InverseTransformPoint(hit.point).y 
				- RFC.radius) / RFC.suspensionDistance;
		}

		var antiRollForce = (travelL - travelR) * springForce;

		if (groundedL)
			buggyRB.AddForceAtPosition(LFC.transform.up * -antiRollForce,
			LFC.transform.position); 
		if (groundedR)
			buggyRB.AddForceAtPosition(RFC.transform.up * antiRollForce,
			RFC.transform.position); 
	}
	
	void StabilizeBackWheels()
	{
		WheelHit hit = new WheelHit();
		float travelL = 1.0f;
		float travelR = 1.0f;

		bool groundedL = LBC.GetGroundHit(out hit);

		if (groundedL)
		{
			travelL = (-LBC.transform.InverseTransformPoint(hit.point).y 
				- LBC.radius) / LBC.suspensionDistance;
		}

		bool groundedR = RBC.GetGroundHit(out hit);

		if (groundedR)
		{
			travelR = (-RBC.transform.InverseTransformPoint(hit.point).y 
				- RBC.radius) / RBC.suspensionDistance;
		}

		var antiRollForce = (travelL - travelR) * springForce;

		if (groundedL)
			buggyRB.AddForceAtPosition(LBC.transform.up * -antiRollForce,
			LBC.transform.position); 
		if (groundedR)
			buggyRB.AddForceAtPosition(RBC.transform.up * antiRollForce,
			RBC.transform.position); 
	}
	
    
	//offsets center of mass of the rigidbody
	void OffsetCenter()
	{
		buggyRB.centerOfMass = 
			new Vector3(
			buggyRB.centerOfMass.x, 
			(buggyRB.centerOfMass.y + yOffset), 
			(buggyRB.centerOfMass.z + zOffset));
	}
}

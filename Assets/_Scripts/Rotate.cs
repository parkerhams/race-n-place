using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
	
	public float rotationSpeed = 8f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
	    transform.Rotate(Vector3.up* Time.deltaTime*rotationSpeed);
    }
}

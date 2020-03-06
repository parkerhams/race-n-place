using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBillboarding : MonoBehaviour
{
	Camera m_Camera;
 
 
	void Start()
	{
		m_Camera = GameManager.Instance.camera;
	}
	//Orient the camera after all movement is completed this frame to avoid jittering
	void LateUpdate()
	{
		transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
			m_Camera.transform.rotation * Vector3.up);
	}
	
}

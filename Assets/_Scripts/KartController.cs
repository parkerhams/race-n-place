using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartController : MonoBehaviour
{
    /*

    This controller uses a sphere method - 
    The movement comes from rotating a sphere collider to get more simplified motion
    In rotating the sphere collider, we get car physics to speed up, slow down, etc.
    (We can change this too if we want!! There could be better methods)

    */

    #region PUBLIC_VARIABLES

    [SerializeField]
    public bool isDrifting;

    [Header("Driving Parameters")]
    [SerializeField]
    public float acceleration = 30f;
    [SerializeField]
    public float steerHandling = 80f;
    [SerializeField]
    public float kartGravity = 10f;
    [SerializeField]
    public LayerMask layerMask;

    #endregion


    #region PRIVATE_VARIABLES    

    [SerializeField]
    private Transform kartModel;

    [SerializeField]
    private Rigidbody sphereRigidbody;

    [Header("Drifting")]
    private float speed, currentSpeed;
    private float rotation, currentRotation;
    private int driftDirection;
    private float driftPower;
    private int driftMode = 0;

    #endregion

    private void Start()
    {

    } 

    private void Update()
    {

    }

    private void FixedUpdate()
    {

    }

    private void Steer()
    {

    }

    private void Drift()
    {

    }

    private void Boost()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + transform.up, transform.position - (transform.up * 2));
    }
}

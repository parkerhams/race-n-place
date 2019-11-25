using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Rewired;

public class KartController : MonoBehaviour
{
    /*

    This controller uses a sphere method - 
    The movement comes from rotating a sphere collider to get more simplified motion
    In rotating the sphere collider, we get car physics to speed up, slow down, etc.
    (We can change this too if we want!! There could be better methods)

    */

    #region PUBLIC_DRIVING_VARIABLES

    [SerializeField]
    public bool isDrifting;

    [Header("Driving Parameters")]
    [SerializeField]
    public float acceleration = 30f;
    [SerializeField]
    public float steerHandling = 80f; //how much/little can the player control their steering (I believe kart racers call this handling)
    [SerializeField]
    public float kartGravity = 10f;
    [SerializeField]
    public LayerMask layerMask;

    //Rewired player input ID
    public int playerId = 1;
    //Rewired player 
    public Player player;

    #endregion


    #region PRIVATE_HANDLING_VARIABLES    

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

    private void Awake()
    {
        player = ReInput.players.GetPlayer(playerId);
    } 

    private void Start()
    {

    }

    private void Update()
    {
        //Follow the sphere collider that is rolling
        transform.position = sphereRigidbody.transform.position - new Vector3(0, 0.4f, 0);

        //Accelerate
        if (Input.GetButton("Fire1"))
            speed = acceleration;

        //Steer a direction by an amount based on the input axis
        if (Input.GetAxis("Horizontal") != 0)
        {
            int dir = Input.GetAxis("Horizontal") > 0 ? 1 : -1;
            float amount = Mathf.Abs((Input.GetAxis("Horizontal")));
            Steer(dir, amount);
        }

    }

    private void FixedUpdate()
    {

    }

    //using the steerHandling variable in the private region, tell the kart which direction to rotate in by a factor of handling
    private void Steer(int steerDirection, float handling)
    {
        rotation = (steerHandling * steerDirection) * handling;
    }

    private void Drift()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + transform.up, transform.position - (transform.up * 2));
    }
}

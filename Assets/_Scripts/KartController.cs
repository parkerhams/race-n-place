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

    [Header("Driving Parameters")]
    [SerializeField]
    public float acceleration = 30f;

    [SerializeField]
    public float steerHandling = 80f; //how much/little can the player control their steering (I believe kart racers call this handling)

    [SerializeField]
    public float kartGravity = 10f;

    [SerializeField]
    public LayerMask layerMask;

    [SerializeField]
    public bool isDrifting;

    //Rewired player input ID
    public int playerId = 1;
    //Rewired player 
    public Player player;
    

    [SerializeField]
    private Transform kartModel, kartNormal;

    [SerializeField]
    private Rigidbody sphereRigidbody;

    [Header("Drifting")]
    private float speed, currentSpeed;
    private float rotation, currentRotation;
    private int driftDirection;
    private float driftPower;
    private int driftMode = 0;

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
        {
            speed = acceleration;
            Debug.Log("should accelerate, speed = " + speed);
        }

        //Steer a direction by a float amount based on the input axis
        if(Input.GetAxis("Horizontal") != 0)//
        {
            int dir = Input.GetAxis("Horizontal") > 0 ? 1 : -1;
            float amount = Mathf.Abs((Input.GetAxis("Horizontal")));
            Steer(dir, amount);
        }
    }

    private void FixedUpdate()
    {
        //forward acceleration
        if (!isDrifting)
            sphereRigidbody.AddForce(-kartModel.transform.right * currentSpeed, ForceMode.Acceleration);
        else
            sphereRigidbody.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);

        //Gravity
        sphereRigidbody.AddForce(Vector3.down * kartGravity, ForceMode.Acceleration);

        //Steering
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + rotation, 0), Time.deltaTime * 5f);

        RaycastHit hitOn;
        RaycastHit hitNear;

        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitOn, 1.1f, layerMask);
        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitNear, 2.0f, layerMask);

        //Normal Rotation
        kartNormal.up = Vector3.Lerp(kartNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        kartNormal.Rotate(0, transform.eulerAngles.y, 0);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerControls : MonoBehaviour
{
    private Player player;
    public int playerId = 1;

    Rigidbody rb;

    public float maxSpeed = 5f;
    Vector3 speed;
    float steer;
    float acceleration;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
        player = ReInput.players.GetPlayer(playerId);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void GetInput()//all input is fetched in Update so that no inputs are lost
    {
        steer = player.GetAxis("Steer");
        acceleration = player.GetAxis("Accelerate");
    }

    void ProcessInput()//runs in Update, for actions that don't need to use physics
    {

    }

    void ProcessFixedInput()//runs in FixedUpdate, for actions that use physics
    {
        if (Mathf.Abs(steer) > .1f)//quick and dirty deadzone, although I think deadzones can be handled in Rewired
        {
            Steer();
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        ProcessInput();
    }
    private void FixedUpdate()
    {
        ProcessFixedInput();
    }

    void Steer()
    {
        //rotate vehicle left or right based on steer value
    }
    void Accelerate()
    {
        //gas gas gas
    }
}

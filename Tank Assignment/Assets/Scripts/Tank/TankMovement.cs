using System;
using System.Collections.Specialized;
using System.Linq;
using System.Media;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

public class TankMovement : MonoBehaviour
{
    public int              m_PlayerNumber      = 1;                                                // Number id attached to the tank instance with this TankMovement component.
    public float            m_Speed             = 12f;                                              // Speed at which the tank will move.
    public float            m_TurnSpeed         = 180f;                                             // Speed at which the tank will rotate on it's y axis.
    public AudioSource      m_MovementAudio;                                                        // Ref. to the AudioSource component of the tank instance with this TankMovement component.
    public AudioClip        m_EngineIdling;                                                         // Ref. to the AudioClip object that will store the audio clip for when the tank is idle.
    public AudioClip        m_EngineDriving;                                                        // Ref. to the AudioClip object that will hold the audio clip for when the tank is driving.
    public float            m_PitchRange        = 0.2f;                                             // Defines the maximum variation in pitch from the original pitch. Randomized.

    public Transform        m_target_transform;                                                     // Ref. to the transform of the tank instance set as the target of the one with this comp.
    public string           m_AI_behaviour;                                                         // Will define the AI behaviour of the tank instance with this comp. "Wander" or "Patrol".
    public Transform[]      waypoints;                                                              // Array of transforms that will be used as waypoints while patrolling.

    private string          m_MovementAxisName;                                                     // String with which get the correct Movement input for the tank instance with this comp.
    private string          m_TurnAxisName;                                                         // String with which get the correct Turn input for the tank instance with this component.
    private Rigidbody       m_Rigidbody;                                                            // Reference to the rigid body of the tank instance with this TankMovement component.
    private float           m_MovementInputValue;                                                   // Movement Input value for the tank instance with this TankMovement component.
    private float           m_TurnInputValue;                                                       // Turn Input value for the tank instance with this tank movement component.
    private float           m_OriginalPitch;                                                        // Base pitch value for the Idling and Driving audio clips.

    private string          m_toggle_AI_name = "Toggle_AI";                                         // Name for the input used to toggle the AI on and off.
    private bool            m_managed_by_AI = true;                                                 // Will keep track of whether or not the tank instance with this comp. is managed by the AI.
    private int             m_destination;                                                          // Index of a waypoint in the waypoints array. Sets where the agent will traverse next.
    private bool            m_can_patrol = true;                                                    // If there are no waypoints, then the tank instance with this component cannot patrol.
    private NavMeshAgent    m_agent;                                                                // NavMeshAgent that will be used if the tank instance with this comp. is a patroller.


    delegate void AIMovement();
    AIMovement AIMovementDelegate;
    
    
    private void Awake()
    {
        m_Rigidbody     = GetComponent<Rigidbody>();
        m_agent         = GetComponent<NavMeshAgent>();
    }


    private void OnEnable ()
    {
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue    = 0.0f;
        m_TurnInputValue        = 0.0f;

        m_destination = 0;
    }


    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        if (m_AI_behaviour == "Wander")
        {
            WanderSetUp();
        }
        else if (m_AI_behaviour == "Patrol")
        {
            PatrolSetUp();
        }
        else
        {
            string log = "Could not ascertain the AI Behaviour for Tank " + m_PlayerNumber;
            Debug.LogError(log);
        }

        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        m_OriginalPitch = m_MovementAudio.pitch;
    }


    private void Update()
    {
        m_MovementInputValue    = Input.GetAxis(m_MovementAxisName);                                // Will store the player's input value and make sure the audio for the engine is playing.
        m_TurnInputValue        = Input.GetAxis(m_TurnAxisName);                                    //
                                                                                                   
        if (Input.GetButtonDown(m_toggle_AI_name))
        {
            m_managed_by_AI = !m_managed_by_AI;

            string log = m_managed_by_AI == true ? "AI was toggled on" : "AI was toggled off";
            Debug.Log(log);
        }

        EngineAudio();                                                                              // --------------------------------------------------------------------------------------
    }

    private void FixedUpdate()
    {
        // Move and turn the tank.
        if (m_managed_by_AI)
        {
            m_agent.isStopped = false;
            AIMovementDelegate();
        }
        else
        {
            m_agent.isStopped = true;

            Move();
            Turn();
        }
    }

    private void EngineAudio()
    {
        if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)           // Plays an audio based on whether or not the tank is moving and which is the current audio.
        {
            if (m_MovementAudio.clip == m_EngineDriving)
            {
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = UnityEngine.Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        else
        {
            if (m_MovementAudio.clip == m_EngineIdling)
            {
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = UnityEngine.Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
    }

    private void Move()
    {
        // Adjust the position of the tank based on the player's input.
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
        
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }

    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
       float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

       Quaternion turnRotation = Quaternion.Euler(0.0f, turn, 0.0f);

       m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }

    private void WanderSetUp()
    {
        AIMovementDelegate = Wander;

        m_agent.autoBraking = false;

        string log = "Tank " + m_PlayerNumber + " is a Wanderer";
        Debug.Log(log);
    }

    private void PatrolSetUp()
    {
        //m_agent.autoBraking = false;
        //SetNextWaypoint();

        AIMovementDelegate = Patrol;

        string log = "Tank " + m_PlayerNumber + " is a Patroller";
        Debug.Log(log);
    }

    private void Wander()
    {
        //Vector3 forward_to_target   = m_target_transform.position - transform.position;
        float distance_to_target    = Vector3.Distance(transform.position, m_target_transform.position);

        if (!m_agent.pathPending && m_agent.remainingDistance < 0.5f)
        {
            float radius        = 3.0f;
            float offset        = 3.0f;
            float variability   = 5.0f;

            Vector3 local_target = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), 0, UnityEngine.Random.Range(-1.0f, 1.0f));

            local_target.Normalize();
            local_target *= radius /*+ variability*/;

            local_target += new Vector3(0.0f, 0.0f, offset);

            Vector3 world_target = m_Rigidbody.transform.TransformPoint(local_target);
            world_target.y = 0.0f;

            if (distance_to_target < 40.0f)                                                                                         // Constraint so it does not wander too far.
            {
                m_agent.destination = world_target;
            }
            else
            {
                m_agent.destination = m_target_transform.position;
            }
        }
        
        //Move();
        //Turn();

        //Debug.Log("Wandering");
    }

    private void Patrol()
    {
        if (m_can_patrol)
        {
            if (!m_agent.pathPending && m_agent.remainingDistance < 0.5f)
            {
                SetNextWaypoint();

                string log = "Waypoint " + (m_destination - 1) + " reached! Next waypoint: " + m_destination;
                Debug.Log(log);
            }
        }
    }

    private void SetNextWaypoint()
    {
        if (waypoints.Length == 0)
        {
            m_can_patrol = false;
            
            string log = "Tank " + m_PlayerNumber + " cannot patrol: No patrol waypoints were found!";
            Debug.LogWarning(log);

            return;
        }

        m_agent.destination = waypoints[m_destination].position;                                              // Will set the destination to the waypoint with the "destination" index.

        m_destination = (m_destination + 1) % waypoints.Length;                                                 // Sets destination with the index of the next waypoint. Cycles back to origin.
    }
}
using System;
using System.Collections.Specialized;
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
    public string           m_AI_behaviour;                                                         // Will define the AI behaviour of the tank instance with this comp. "Wander" or "Patroll".

    private string          m_MovementAxisName;                                                     // String with which get the correct Movement input for the tank instance with this comp.
    private string          m_TurnAxisName;                                                         // String with which get the correct Turn input for the tank instance with this component.
    private Rigidbody       m_Rigidbody;                                                            // Reference to the rigid body of the tank instance with this TankMovement component.
    private float           m_MovementInputValue;                                                   // Movement Input value for the tank instance with this TankMovement component.
    private float           m_TurnInputValue;                                                       // Turn Input value for the tank instance with this tank movement component.
    private float           m_OriginalPitch;                                                        // Base pitch value for the Idling and Driving audio clips.

    private bool            m_managed_by_AI = true;                                                 // Will keep track of whether or not the tank instance with this comp. is managed by the AI.
    private NavMeshAgent    agent;                                                                  // NavMeshAgent that will be used if the tank instance with this comp. is a patroller.


    delegate void AI_Move();
    AI_Move AI_Movement;
    
    
    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

        //agent.path = new NavMeshPath();
    }


    private void OnEnable ()
    {
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue    = 0.0f;
        m_TurnInputValue        = 0.0f;
    }


    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        if (m_AI_behaviour == "Wander")
        {
            AI_Movement = Wander;

            string log = "Tank " + m_PlayerNumber + " is a Wanderer";
            Debug.Log(log);
        }
        else if (m_AI_behaviour == "Patroll")
        {
            AI_Movement = Patroll;

            string log = "Tank " + m_PlayerNumber + " is a Patroller";
            Debug.Log(log);
        }
        else
        {
            string log = "Could not ascertain the AI Behaviour for Tank " + m_PlayerNumber;

            Debug.LogError(log);
        }

        m_OriginalPitch = m_MovementAudio.pitch;
    }


    private void Update()
    {
        m_MovementInputValue    = Input.GetAxis(m_MovementAxisName);                                // Will store the player's input value and make sure the audio for the engine is playing.
        m_TurnInputValue        = Input.GetAxis(m_TurnAxisName);                                    //
                                                                                                    //
        EngineAudio();                                                                              // --------------------------------------------------------------------------------------
    }

    private void FixedUpdate()
    {
        // Move and turn the tank.
        Move();
        Turn();

        if (m_managed_by_AI)
        {
            AI_Movement();
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
        // Wander or Patrolling

        
        // Adjust the position of the tank based on the player's input.
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
        
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }

    private void Wander()
    {
        
    }

    private void Patroll()
    {
        
    }

    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
       float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

       Quaternion turnRotation = Quaternion.Euler(0.0f, turn, 0.0f);

       m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }
}
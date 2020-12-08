using System;
using UnityEngine;

[Serializable]
public class TankManager
{
    public Color                            m_PlayerColor;                                  // RGB Colour with which the tank instance's mesh will be painted with.
    public Transform                        m_SpawnPoint;                                   // Transform that will be used as the position where the tank instance will spawn.
    [HideInInspector] public int            m_PlayerNumber;                                 // Number id attached to the tank instance. Will be used to manage user inputs.
    [HideInInspector] public string         m_ColoredPlayerText;                            // String that will appear on screen displaying the player num in its player color.
    [HideInInspector] public GameObject     m_Instance;                                     // GameObject that represents the instanced tank.
    [HideInInspector] public int            m_Wins;                                         // Will keep track of the amount of rounds won by the tank instance.

    public string                           m_AI_behaviour;                                 // Will def. the AI behaviour of the tank instance. Could be "Wander" or "Patrol".
    [HideInInspector] public Transform      m_root_waypoint;
    [HideInInspector] public Transform[]    m_patrol_waypoints;                             // Array that will store the waypoints that will be used by a patrolling tank.

    private TankMovement                    m_Movement;                                     // Reference to the TankMovement component of the tank instance. See TankMovement.cs.
    private TankShooting                    m_Shooting;                                     // Reference to the TankShooting component of the tank instance. See TankShooting.cs.
    private GameObject                      m_CanvasGameObject;                             // GameObject related to the UI elements attached to the tank instance.

    [HideInInspector] public GameObject     m_target;                                       // Reference to the tank instance that will be set to be the target of this one.
    [HideInInspector] public bool           m_managed_by_AI;                                // Will keep track of whether or not the tank instance is being controlled by the AI.

    private BehaviorExecutor                m_behaviour_executor;

    public void Setup()
    {
        m_Instance.name = m_AI_behaviour;
        
        //m_Movement                          = m_Instance.GetComponent<TankMovement>();
        //m_Shooting                          = m_Instance.GetComponent<TankShooting>();
        m_CanvasGameObject                  = m_Instance.GetComponentInChildren<Canvas>().gameObject;

        m_ColoredPlayerText                 = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";
        MeshRenderer[] renderers            = m_Instance.GetComponentsInChildren<MeshRenderer>();
        Transform[] transforms              = m_Instance.GetComponentsInChildren<Transform>();

        for (int i = 0; i < renderers.Length; ++i)
        {
            renderers[i].material.color = m_PlayerColor;
        }

        //m_Movement.m_PlayerNumber           = m_PlayerNumber;
        //m_Movement.m_target_transform       = m_target.transform;
        //m_Movement.m_AI_behaviour           = m_AI_behaviour;
        //m_Movement.waypoints                = m_patrol_waypoints;

        //m_Shooting.m_PlayerNumber           = m_PlayerNumber;
        //m_Shooting.m_target_transform       = m_target.transform;
        //m_Shooting.m_turret_renderer        = renderers[3];


        m_behaviour_executor = m_Instance.GetComponent<BehaviorExecutor>();

        Debug.Log(m_behaviour_executor.name);

        m_behaviour_executor.blackboard.SetBehaviorParam("target", m_target);
        m_behaviour_executor.blackboard.SetBehaviorParam("ai_behaviour", m_AI_behaviour);
        m_behaviour_executor.blackboard.SetBehaviorParam("root_waypoint", m_root_waypoint);
        m_behaviour_executor.blackboard.SetBehaviorParam("fire_transform", transforms[16]);
        m_behaviour_executor.blackboard.SetBehaviorParam("turret", renderers[3]);
        m_behaviour_executor.blackboard.SetBehaviorParam("cooldown", 0.0f);
        m_behaviour_executor.blackboard.SetBehaviorParam("max_cooldown", 3.0f);
        m_behaviour_executor.blackboard.SetBehaviorParam("fired", true);
        m_behaviour_executor.blackboard.SetBehaviorParam("ammo", 3);
        m_behaviour_executor.blackboard.SetBehaviorParam("base_waypoint", m_SpawnPoint);
    }

    public void DisableControl()
    {
        //m_Movement.enabled = false;
        //m_Shooting.enabled = false;

        m_behaviour_executor.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }


    public void EnableControl()
    {
        //m_Movement.enabled = true;
        //m_Shooting.enabled = true;

        m_behaviour_executor.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }


    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}

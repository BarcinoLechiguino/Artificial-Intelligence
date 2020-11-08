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

    public string                           m_AI_behaviour;                                 // Will def. the AI behaviour of the tank instance. Could be "Wander" or "Patroll".

    private TankMovement                    m_Movement;                                     // Reference to the TankMovement component of the tank instance. See TankMovement.cs.
    private TankShooting                    m_Shooting;                                     // Reference to the TankShooting component of the tank instance. See TankShooting.cs.
    private GameObject                      m_CanvasGameObject;                             // GameObject related to the UI elements attached to the tank instance.

    [HideInInspector] public GameObject     m_target;                                       // Reference to the tank instance that will be set to be the target of this one.
    [HideInInspector] public bool           m_managed_by_AI;                                // Will keep track of whether or not the tank instance is being controlled by the AI.

    public void Setup()
    {   
        m_Movement                          = m_Instance.GetComponent<TankMovement>();
        m_Shooting                          = m_Instance.GetComponent<TankShooting>();
        m_CanvasGameObject                  = m_Instance.GetComponentInChildren<Canvas>().gameObject;

        m_ColoredPlayerText                 = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";
        MeshRenderer[] renderers            = m_Instance.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = m_PlayerColor;
        }

        m_Movement.m_PlayerNumber           = m_PlayerNumber;
        m_Movement.m_target_transform       = m_target.transform;
        m_Movement.m_AI_behaviour           = m_AI_behaviour;

        m_Shooting.m_PlayerNumber           = m_PlayerNumber;
        m_Shooting.m_target_transform       = m_target.transform;
        m_Shooting.m_turret_renderer        = renderers[3];
    }

    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }


    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

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

using System;
using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int          m_PlayerNumber;       

    public Rigidbody    m_Shell;            
    public Transform    m_FireTransform;    
    public AudioSource  m_ShootingAudio;   
    public AudioClip    m_FireClip;

    public Transform    m_own_transform;
    public Transform    m_target_transform;
    public float        m_launch_force;
    public float        m_min_pitch_angle;
    public float        m_max_pitch_angle;
    public float        m_shot_cooldown;
    
    private string      m_FireButton;
    private bool        m_Fired;
    private float       m_current_cooldown          = 0.0f;
    private bool        m_managed_by_AI             = false;
    private bool        m_suitable_angle            = false;
    private float       m_shot_angle                = 45.0f;
    private float       m_max_shot_reach            = 0.0f;
    
    [HideInInspector] public MeshRenderer m_turret_renderer;
    private void OnEnable()
    {
        Vector3 velocity = m_launch_force * m_FireTransform.forward;

        // As per R = v^2 * sin2(angle) / g
        m_max_shot_reach = ((m_launch_force * m_launch_force) * Mathf.Sin(2.0f * 45.0f)) / Physics.gravity.y;                              // Max reach in a parabolic shot happens at 45º.

        print(m_max_shot_reach);
    }


    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber;
    }
    

    private void Update()
    {
        LookAtEnemyTank();                                                                                                                  // Must be first to update the shot transform corr.

        FindSuitableAngle();

        if (m_Fired)
        {
            m_current_cooldown += Time.deltaTime;

            if (m_current_cooldown >= m_shot_cooldown)
            {
                m_Fired = false;
                m_current_cooldown = 0.0f;
            }
        }

        if (m_managed_by_AI)
        {
            if (m_suitable_angle)                                                                                                           // Condition: Find a suitable angle.
            {

            }
        }
        else
        {
            if (Input.GetButtonUp(m_FireButton) && !m_Fired)
            {
                Fire();
            }
        }
    }

    private void Fire()                                                                                                                     // Instantiate and launch the shell.
    {
        m_Fired                     = true;
        
        //m_FireTransform.rotation    = Quaternion.Euler(0.0f, m_min_pitch_angle, 0.0f);

        Rigidbody shellInstance     = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
        shellInstance.velocity      = m_launch_force * m_FireTransform.forward;

        m_ShootingAudio.clip        = m_FireClip;
        m_ShootingAudio.Play();
    }

    private void LookAtEnemyTank()
    {
        //m_turret_renderer.transform.rotation = m_target_transform;                                                                        // Use target_transform to create the LookAt().

        m_min_pitch_angle += 1.0f; 

        m_turret_renderer.transform.rotation = Quaternion.Euler(0.0f, m_min_pitch_angle, 0.0f);
    }

    private void FindSuitableAngle()
    {
        if (DistanceToTarget() < m_max_shot_reach)
        {

        }
    }

    private float DistanceToTarget()
    {
        Vector3 own_pos = m_own_transform.position;
        Vector3 target_pos = m_target_transform.position;

        Vector3 dist_vec = target_pos - own_pos;

        float distance = Mathf.Sqrt(dist_vec.x * 2 + dist_vec.y * 2 + dist_vec.z * 2);

        return distance;
    }
}
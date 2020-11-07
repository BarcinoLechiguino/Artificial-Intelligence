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

    public Transform    m_target_transform;
    public float        m_launch_force              = 25.0f;
    public float        m_min_pitch_angle;
    public float        m_max_pitch_angle;
    public float        m_shot_cooldown;
    
    private string      m_fire_button;
    private bool        m_fired;
    private float       m_current_cooldown          = 0.0f;
    private bool        m_managed_by_AI             = true;
    private bool        m_suitable_angle            = false;
    private float       m_shot_angle                = 45.0f;
    private float       m_max_shot_reach            = 0.0f;
    
    [HideInInspector] public MeshRenderer m_turret_renderer;
    private void OnEnable()
    {
        // Assigning the turret_renderer's transform as the parent of the fire transform.
        m_FireTransform.parent = m_turret_renderer.transform;

        m_shot_cooldown = GetNewCooldown();

        m_max_shot_reach = GetMaxReach();
    }


    private void Start()
    {
        m_fire_button = "Fire" + m_PlayerNumber;
    }
    

    private void Update()
    {
        LookAtEnemyTank();                                                                                                                  // Must be first to update the shot transform corr.

        FindSuitableAngle();

        if (m_fired)
        {
            m_current_cooldown += Time.deltaTime;

            if (m_current_cooldown >= m_shot_cooldown)
            {
                m_fired = false;
                m_current_cooldown = 0.0f;

                m_shot_cooldown = GetNewCooldown();
            }
        }

        if (m_managed_by_AI)
        {
            if (m_suitable_angle && !m_fired)                                                                                           // Condition: Find a suitable angle.
            {
                AI_Fire();
            }
        }
        else
        {
            if (Input.GetButtonUp(m_fire_button) && !m_fired)
            {
                Fire();
            }
        }
    }

    private void Fire()                                                                                                                     // Instantiate and launch the shell.
    {
        m_fired                     = true;
        
        //m_FireTransform.rotation    = Quaternion.Euler(0.0f, m_min_pitch_angle, 0.0f);

        Rigidbody shellInstance     = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
        shellInstance.velocity      = m_launch_force * m_FireTransform.forward;

        //shellInstance.transform.forward = shellInstance.velocity;

        m_ShootingAudio.clip        = m_FireClip;
        m_ShootingAudio.Play();
    }

    private void AI_Fire()
    {
        m_fired = true;

        //m_FireTransform.rotation    = Quaternion.Euler(0.0f, m_min_pitch_angle, 0.0f);

        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        Vector3 fire_transform = m_FireTransform.forward;

        shellInstance.velocity = m_launch_force * m_FireTransform.forward;

        //shellInstance.transform.forward = shellInstance.velocity;

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();
    }

    private void LookAtEnemyTank()
    {
        Vector3 new_forward =  m_target_transform.position - transform.position;                                                // Getting the vector that points from origin to target.

        m_turret_renderer.transform.forward = new_forward;                                                                      // Setting the the turret_transform with the new forward vector.
        m_FireTransform.forward = m_turret_renderer.transform.forward;                                                          // Also applying the new forward vector to the fire_transform.
        
        //m_FireTransform.rotation = Quaternion.Euler(-135.0f, 0.0f, 0.0f);
    }

    private void FindSuitableAngle()
    {
        float distance = Vector3.Distance(transform.position, m_target_transform.position);

        if (distance < m_max_shot_reach)
        {
            print(distance);

            //Fire();

            m_suitable_angle = true;
        }
    }

    private float GetMaxReach()
    {
        Vector3 velocity = m_launch_force * m_FireTransform.forward;

        // As per R = v^2 * sin2(angle) / g
        float max_reach = ((m_launch_force * m_launch_force) * Mathf.Sin(2.0f * 45.0f)) / Physics.gravity.y;                        // Max reach in a parabolic shot happens at 45º.

        if (max_reach < 0)                                                                                                          // In case distance is negative.  
        {
            max_reach = -max_reach;
        }

        print(max_reach);

        return max_reach;
    }

    private float GetNewCooldown()
    {
        float new_cooldown = 2.0f + UnityEngine.Random.Range(0.0f, 0.5f);                                                           // It's randomized 

        return new_cooldown;
    }
}
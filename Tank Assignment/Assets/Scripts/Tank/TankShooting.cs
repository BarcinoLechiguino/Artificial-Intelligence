using System;
using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int          m_PlayerNumber              = 1;                                                        // Number id attached to the tank instance with this TankShooting component.
    public Rigidbody    m_Shell;                                                                                // Ref. to the r.b. that will be used as the blueprint for all shell instances.
    public Transform    m_FireTransform;                                                                        // Will define the direction and angle at which a shell will be shot at.
    public AudioSource  m_ShootingAudio;                                                                        // Ref. to the AudioSource component of the tank instance with this component.
    public AudioClip    m_FireClip;                                                                             // Ref. to the AudioClip object that will store the audio clip for shooting.

    public Transform    m_target_transform;                                                                     // Ref. to the transform of the target of the tank instance with this component.
    public float        m_launch_speed              = 15.0f;                                                    // Speed at which a shell will be shot from the tank instance with this comp.
    public float        m_min_pitch_angle           = 0.0f;                                                     // Minimum pitch angle at which m_FireTransform can be at and shoot a shell.
    public float        m_max_pitch_angle           = 45.0f;                                                    // Maximum pitch angle at which m_FireTransform can be at and shoot a shell.
    public float        m_min_shot_cooldown         = 2.0f;                                                     // Minimum amount of cooldown that there can be between two shots.
    public float        m_cooldown_offset           = 1.0f;                                                     // Defines the maximum variation in shot cooldown from the minimum. Randomized.
    
    private string      m_fire_button;                                                                          // Str. with which get the cor. Fire Input for the tank instance with this comp.
    private bool        m_fired;                                                                                // Will keep track of whether or not a shell has been shot. Cooldown.
    private float       m_current_cooldown          = 0.0f;                                                     // Will store the amount of elapsed time since a shell was fired. Cooldown.
    private bool        m_managed_by_AI             = true;                                                     // Will keep track of whether or not the shooting will be controlled by the AI.
    private bool        m_found_suitable_angle      = false;                                                    // Will keep track of whether or not an a at which shoot the tgt.has been found.
    private float       m_shot_angle                = 0.0f;                                                     // Angle at which the shell will be fired at to hit the target. AI control.
    private float       m_max_shot_reach            = 0.0f;                                                     // Maximum distance at which the shell can be fired at.
    private float       m_shot_cooldown             = 0.0f;                                                     // Amount of cooldown that will be applied after a shell is fired.
    
    [HideInInspector] public MeshRenderer m_turret_renderer;
    
    private void OnEnable()
    {
        m_FireTransform.parent = m_turret_renderer.transform;                                                   // Setting the turret renderer's transform as the parent of the fire transform.

        m_shot_cooldown = GetNewCooldown();                                                                     // Calculating the shot cooldown to wait for after firing the first shot.

        m_max_shot_reach = GetMaxReach();                                                                       // Calculating the maximum reach that a given shot will have.
    }


    private void Start()
    {
        m_fire_button = "Fire" + m_PlayerNumber;
    }
    

    private void Update()
    {
        LookAtTargetTank();                                                                                     // Must be first to update the shot transform corr.

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
            if (!m_fired)
            {
                FindSuitableAngle();                                                                            // Only calculate the angle if tank can shoot.

                if (m_found_suitable_angle)                                                                     // Condition: Find a suitable angle.
                {
                    AI_Fire();
                }
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

    private float GetMaxReach()
    {
        // As per R = v^2 * sin(2a) / g
        float v = m_launch_speed;                                                                               // Projectile's speed.
        float a = 45.0f;                                                                                        // Max reach in a parabolic shot happens at 45º.
        float g = Physics.gravity.y;                                                                            // Gravity constant for the current environment.

        float max_reach = ((v * v) * Mathf.Sin(2 * a)) / g;

        max_reach = Mathf.Abs(max_reach);                                                                       // Gets the absolute value in case max_reach is negative.

        Debug.Log(max_reach);

        return max_reach;
    }

    private float GetNewCooldown()
    {
        float new_cooldown = m_min_shot_cooldown + UnityEngine.Random.Range(0.0f, m_cooldown_offset);           // It's randomized 

        return new_cooldown;
    }

    private void LookAtTargetTank()
    {
        Vector3 new_forward = m_target_transform.position - transform.position;                                 // Getting the vector that points from origin to target.

        m_turret_renderer.transform.forward = new_forward;                                                      // Setting the the turret_transform with the new forward vector.
        m_FireTransform.forward = m_turret_renderer.transform.forward;                                          // Also applying the new forward vector to the fire_transform.
    }

    private void FindSuitableAngle()
    {
        float distance_to_target = Vector3.Distance(m_FireTransform.position, m_target_transform.position);

        if (distance_to_target < m_max_shot_reach)                                                              // Checks that the target is within reach.
        {
            // tan(a) = (v^2 +- sqrt(v^4 - g(gx^2 + 2yv^2))) / gx;
            float v = m_launch_speed;                                                                           // Projectile's Speed
            float g = Physics.gravity.y;                                                                        // Gravity on the y axis.
            float x = distance_to_target;                                                                       // Distance to the target from the fire transform.
            float y = m_target_transform.position.y;                                                            // Target's position in the y axis.

            if (y < 0.0f)                                                                                       // In case the target's y position is negative.
            {
                y = 0.0f;
            }

            float v2 = v * v;                                                                                   // Separating the operations for readability's sake.
            float v4 = v * v * v * v;                                                                           //
            float x2 = x * x;                                                                                   // -------------------------------------------------

            float tan = (v2 - Mathf.Sqrt(v4 - g * (g * x2 + 2 * y * v2))) / (g * x);                            // Gets the tangent for the "-" version of the equation.
            float rad_angle = Mathf.Atan(tan);                                                                  // Angle in radiants.

            m_shot_angle = GetValidShotAngle(rad_angle);                                                        // GetValidShotAngle returns the correct angle in degrees. Returns 0 on ERROR.

            if (m_shot_angle > 0.0f)
            {
                m_found_suitable_angle = true;
            }
        }
        else
        {
            Debug.Log(distance_to_target);
            Debug.Log(m_max_shot_reach);

            Debug.LogWarning("[WARNING] Unable to shoot: The target is too far!");
        }
    }

    private float GetValidShotAngle(double angle)
    {
        float ret = 0.0f;

        float pitch = Math.Abs((float)angle * Mathf.Rad2Deg);

        if (PitchAngleIsValid(pitch))
        {
            ret = pitch;
        }
        else
        {
            Debug.LogError("[ERROR] Pitch angle was not valid: It was too big or too small!");
        }

        return ret;
    }

    private bool PitchAngleIsValid(float angle)
    {
        return (angle > m_min_pitch_angle && angle < m_max_pitch_angle);                                        // The angle (pitch angle) will be valid if it's within the specified bounds.
        //return (angle > 0.0f && angle < 45.0f);                                                               // Reach only increases/decreases in the range from 0º to 45º and viceversa.
    }

    private void AI_Fire()
    {
        m_found_suitable_angle = false;
        m_fired = true;

        m_FireTransform.Rotate(-m_shot_angle, 0.0f, 0.0f);

        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
        shellInstance.velocity = m_launch_speed * m_FireTransform.forward;

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();
    }

    private void Fire()                                                                                                                     // Instantiate and launch the shell.
    {
        m_fired                     = true;

        Rigidbody shellInstance     = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
        shellInstance.velocity      = m_launch_speed * m_FireTransform.forward;

        m_ShootingAudio.clip        = m_FireClip;
        m_ShootingAudio.Play();
    }
}
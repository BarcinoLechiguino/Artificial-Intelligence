using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;
using System;

namespace BBUnity.Actions
{
    [Action("OwnBricks/Actions/ShootAtTarget")]
    [Help("Moves the game object to a given position by using a NavMeshAgent")]
    public class ShootAtTarget : GOAction
    {
        ///<value>Input target position where the game object will be moved Parameter.</value>
        [InParam("target")]
        [Help("Target to which shot")]
        public GameObject target;

        [InParam("fire_transform")]
        public Transform fire_transform;

        [InParam("shell")]
        public Rigidbody shell;

        [InParam("shot_audio")]
        public AudioSource shot_audio;

        [InParam("shot_clip")]
        public AudioClip shot_clip;

        [InParam("ammo")]
        public int ammo;

        [OutParam("ammo_out")]
        public int ammo_out;

        [InParam("fired")]
        public bool fired;

        [OutParam("fired_out")]
        public bool fired_out;

        private float launch_speed  = 45.0f;
        private float max_reach     = 0;
        private float shot_angle    = 0.0f;
        private float min_pitch     = 0.0f;
        private float max_pitch     = 45.0f;

        public override void OnStart()
        {
            ammo = ammo_out;
            fired = fired_out;

            if (fired || ammo == 0)
            {
                return;
            }

            shot_angle = FindSuitableAngle();

            if (shot_angle != 0.0f)
            {
                AI_Fire();
            }
        }

        public override TaskStatus OnUpdate()
        {   
            return TaskStatus.COMPLETED;
        }

        public float FindSuitableAngle()
        {
            float distance_to_target = Vector3.Distance(fire_transform.position, target.transform.position);

            max_reach = GetMaxReach();

            if (distance_to_target < max_reach)                                                                 // Checks that the target is within reach.
            {
                // tan(a) = (v^2 +- sqrt(v^4 - g(gx^2 + 2yv^2))) / gx;
                float v = launch_speed;                                                                         // Projectile's Speed
                float g = Physics.gravity.y;                                                                    // Gravity on the y axis.
                float x = distance_to_target;                                                                   // Distance to the target from the fire transform.
                float y = target.transform.position.y;                                                          // Target's position in the y axis.

                if (y < 0.0f)                                                                                   // In case the target's y position is negative.
                {
                    y = 0.0f;
                }

                float v2 = v * v;                                                                               // Separating the operations for readability's sake.
                float v4 = v * v * v * v;                                                                       //
                float x2 = x * x;                                                                               // -------------------------------------------------

                float tan = (v2 - Mathf.Sqrt(v4 - g * (g * x2 + 2 * y * v2))) / (g * x);                        // Gets the tangent for the "-" version of the equation.
                float rad_angle = Mathf.Atan(tan);                                                              // Angle in radiants.

                shot_angle = GetValidShotAngle(rad_angle);                                                    // GetValidShotAngle returns the correct angle in degrees. Returns 0 on ERROR.

                return shot_angle;
            }

            return 0.0f;
        }

        public float GetMaxReach()
        {
            // As per R = v^2 * sin(2a) / g
            float v = 15.0f;                                                                                        // Projectile's speed.
            float a = 45.0f;                                                                                        // Max reach in a parabolic shot happens at 45º.
            float g = Physics.gravity.y;                                                                            // Gravity constant for the current environment.

            float max_reach = ((v * v) * Mathf.Sin(2 * a)) / g;

            max_reach = Mathf.Abs(max_reach);                                                                       // Gets the absolute value in case max_reach is negative.

            return max_reach;
        }

        public float GetValidShotAngle(float rad_angle)
        {
            float ret = 0.0f;

            float pitch = Mathf.Abs((float)rad_angle * Mathf.Rad2Deg);

            if (pitch > min_pitch && pitch < max_pitch)
            {
                ret = pitch;
            }
            else
            {
                Debug.LogError("[ERROR] Pitch angle was not valid: It was too big or too small!");
            }

            return ret;
        }

       public void AI_Fire()
        {
            fired_out = true;
            --ammo_out;

            fire_transform.Rotate(-shot_angle, 0.0f, 0.0f);

            Rigidbody shell_instance = UnityEngine.Object.Instantiate(shell, fire_transform.position, fire_transform.rotation) as Rigidbody;
            shell_instance.velocity = launch_speed * fire_transform.forward;

            shot_audio.clip = shot_clip;
            shot_audio.Play();
        }
    }
}

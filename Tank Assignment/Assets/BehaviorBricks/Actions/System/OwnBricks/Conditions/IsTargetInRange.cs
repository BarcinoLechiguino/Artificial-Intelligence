using Pada1.BBCore;
using UnityEngine;

namespace BBUnity.Conditions
{
    [Condition("OwnBricks/Conditions/IsTargetInRange")]
    [Help("Checks whether a target is close depending on a given distance")]
    public class IsTargetInRange : GOCondition
    {
        [InParam("target")]
        [Help("Target to check the distance to.")]
        public GameObject target;

        private float max_reach = 0.0f;

        public override bool Check()
        {   
            max_reach                   = GetMaxReach();
            float distance_to_target    = Vector3.Distance(gameObject.transform.position, target.transform.position);

            if (distance_to_target < max_reach)
            {
                Debug.Log("TARGET IS WITHIN REACH");
            }

            return (distance_to_target < max_reach);
        }

        public float GetMaxReach()
        {
            // As per R = v^2 * sin(2a) / g
            float v = 17.5f;                                                                                        // Projectile's speed.
            float a = 45.0f;                                                                                        // Max reach in a parabolic shot happens at 45º.
            float g = Physics.gravity.y;                                                                            // Gravity constant for the current environment.

            float max_reach = ((v * v) * Mathf.Sin(2 * a)) / g;

            max_reach = Mathf.Abs(max_reach);                                                                       // Gets the absolute value in case max_reach is negative.

            return max_reach;
        }
    }
}
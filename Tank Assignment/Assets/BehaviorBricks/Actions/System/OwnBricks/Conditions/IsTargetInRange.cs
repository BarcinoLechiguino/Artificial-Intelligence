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

        [InParam("fired")]
        public bool fired;

        [OutParam("max_reach_out")]
        [Help("Maximum distance at which the tank can shoot at.")]
        public float max_reach_out;

        public override bool Check()
        {
            if (fired)
            {
                return false;
            }
            
            max_reach_out               = GetMaxReach();
            float distance_to_target    = Vector3.Distance(gameObject.transform.position, target.transform.position);

            return (distance_to_target < max_reach_out);
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
    }
}
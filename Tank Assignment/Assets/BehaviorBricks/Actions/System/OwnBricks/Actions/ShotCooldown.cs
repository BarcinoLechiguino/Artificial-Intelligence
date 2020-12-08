using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;

namespace BBUnity.Actions
{
    [Action("OwnBricks/Actions/ShotCooldown")]
    [Help("Moves the game object to a given position by using a NavMeshAgent")]
    public class ShotCooldown : GOAction
    {
        [InParam("fired")]
        public bool fired;

        [OutParam("fired_out")]
        public bool fired_out;
        
        [InParam("cooldown")]
        [Help("Current amount of cooldown that has been accumulated.")]
        public float cooldown;

        [InParam("max_cooldown")]
        public float max_cooldown;

        [OutParam("cooldown_out")]
        public float cooldown_out;

        [OutParam("max_cooldown_out")]
        public float max_cooldown_out;

        public override void OnStart()
        {
            fired_out = fired;

            if (!fired_out)
            {
                return;
            }

            max_cooldown_out    = max_cooldown;
            cooldown_out        = cooldown + Time.deltaTime;

            if (max_cooldown_out < 2.0f)
            {
                max_cooldown_out = 2.0f + UnityEngine.Random.Range(0.0f, 1.5f);
                Debug.Log("RECALC MAX COOLDOWN " + max_cooldown_out);
            }

            // Debug.Log("Current Cooldown: " + cooldown_out);
            // Debug.Log("Current Max Cooldown: " + max_cooldown_out);
        }

        public override TaskStatus OnUpdate()
        {
            if (!fired_out)
            {
                //Debug.Log("NOT IN COOLDOWN");
                return TaskStatus.FAILED;
            }

            if (cooldown_out >= max_cooldown_out)
            {
                fired_out = false;

                cooldown_out        = 0.0f;
                max_cooldown_out    = 2.0f + UnityEngine.Random.Range(0.0f, 1.0f);
                
                Debug.Log("RESETTING COOLDOWNS");

                return TaskStatus.FAILED;
            }

            return TaskStatus.COMPLETED;
        }
    }
}
using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;

namespace BBUnity.Actions
{
    [Action("OwnBricks/Actions/ShotCooldown")]
    [Help("Moves the game object to a given position by using a NavMeshAgent")]
    public class ShotCooldown : GOAction
    {
        ///<value>Input target position where the game object will be moved Parameter.</value>
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
            cooldown            = cooldown_out;
            max_cooldown        = max_cooldown_out;

            cooldown_out        += Time.deltaTime;
        }

        public override TaskStatus OnUpdate()
        {
            if (cooldown_out >= max_cooldown)
            {
                //cooldown_out        = 0.0f;
                max_cooldown_out    = 2.5f + UnityEngine.Random.Range(0.0f, 1.0f);

                Debug.Log(cooldown_out);

                return TaskStatus.FAILED;
            }

            return TaskStatus.COMPLETED;
        }
    }
}

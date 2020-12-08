using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;

namespace BBUnity.Actions
{
    [Action("OwnBricks/Actions/Wander")]
    [Help("Moves the game object to a given position by using a NavMeshAgent")]
    public class Wander : GOAction
    {
        ///<value>Input target position where the game object will be moved Parameter.</value>
        [InParam("target")]
        [Help("Target Tank to which attack")]
        public GameObject target;

        [InParam("ai_behaviour")]
        public string ai_behaviour;


        private UnityEngine.AI.NavMeshAgent agent;
        private float radius = 3.0f;
        private float offset = 3.0f;

        public override void OnStart()
        {
            if (ai_behaviour == "Patroller")
            {
                return;
            }

            agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();

            //Vector3 forward_to_target   = m_target_transform.position - transform.position;
            float distance_to_target = Vector3.Distance(gameObject.transform.position, target.transform.position);

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                Vector3 local_target = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), 0, UnityEngine.Random.Range(-1.0f, 1.0f));

                local_target.Normalize();
                local_target *= radius;

                local_target += new Vector3(0.0f, 0.0f, offset);

                Vector3 world_target = gameObject.transform.TransformPoint(local_target);
                world_target.y = 0.0f;

                if (distance_to_target < 40.0f)                                                                                         // Constraint so it does not wander too far.
                {
                    agent.destination = world_target;
                }
                else
                {
                   agent.destination = target.transform.position;
                }
            }
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.COMPLETED;
        }
    }
}
using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;

namespace BBUnity.Actions
{
    [Action("OwnBricks/Actions/ReturnBaseToReload")]
    [Help("Moves the game object to a given position by using a NavMeshAgent")]
    public class ReturnBaseToReload : GOAction
    {
        ///<value>Input target position where the game object will be moved Parameter.</value>
        [InParam("base_waypoint")]
        public Transform base_waypoint;

        [InParam("ai_behaviour")]
        public string ai_behaviour;

        [InParam("destination")]
        public int destination;

        [OutParam("destination_out")]
        public int destination_out;

        [InParam("ammo")]
        public int ammo;

        [OutParam("ammo_out")]
        public int ammo_out;

        private UnityEngine.AI.NavMeshAgent agent;

        public override void OnStart()
        {
            destination_out = destination;
            ammo_out = ammo;

            if (ammo_out != 0)
            {
                Debug.Log("Tank " + gameObject.name + "Still has ammo! Ammo: " + ammo_out);
                return;
            }

            agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();

            agent.SetDestination(base_waypoint.position);                                                // Will set the destination to the waypoint with the "destination" index.

            agent.isStopped = false;
        }

        public override TaskStatus OnUpdate()
        {
            if (ammo_out != 0)
            {
                return TaskStatus.COMPLETED;
            }
            
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                ammo_out = 3;
                destination_out = 0;
                return TaskStatus.COMPLETED;
            }
            else if (agent.destination != base_waypoint.transform.position)
            {
                agent.SetDestination(base_waypoint.position);
                
            }
            
            return TaskStatus.RUNNING;
        }
    }
}

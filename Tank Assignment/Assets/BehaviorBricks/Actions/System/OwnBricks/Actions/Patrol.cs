using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;

namespace BBUnity.Actions
{
    [Action("OwnBricks/Actions/Patrol")]
    [Help("Moves the game object to a given position by using a NavMeshAgent")]
    public class Patrol : GOAction
    {
        ///<value>Input target position where the game object will be moved Parameter.</value>
        [InParam("target")]
        public GameObject target;
        
        [InParam("ai_behaviour")]
        public string ai_behaviour;

        [InParam("root_waypoint")]
        public Transform root_waypoint;

        [InParam("destination")]
        public int destination;

        [OutParam("destination_out")]
        public int destination_out;

        private UnityEngine.AI.NavMeshAgent agent;
        private Transform[] waypoints;

        public override void OnStart()
        {
            if (ai_behaviour == "Wanderer")
            {
                return;
            }

            destination_out = destination;

            agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();

            GetWaypoints();

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                SetNextWaypoint();

                string log = "Waypoint " + (destination_out - 1) + " reached! Next waypoint: " + destination_out;
                Debug.Log(log);
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (ai_behaviour == "Wanderer")
            {
                return TaskStatus.FAILED;
            }
            else
            {
                return TaskStatus.COMPLETED;
            }
        }

        public void SetNextWaypoint()
        {
            if (waypoints.Length == 0)
            {
                string log = "Patroller Tank cannot patrol: No patrol waypoints were found!";
                Debug.LogWarning(log);

                return;
            }

            agent.destination = waypoints[destination_out].position;                                                // Will set the destination to the waypoint with the "destination" index.

            destination_out = (destination_out + 1) % root_waypoint.childCount;                                         // Sets destination with the index of the next waypoint. Cycles back to origin.

            //Debug.Log("Destionation " + destination_out);
        }

        public void GetWaypoints()
        {
            int num_waypoints = root_waypoint.childCount;                                                                                   // Getting the total amount of childs in the root.

            waypoints = new Transform[num_waypoints];                                                                                       // Allocating the required memory.

            for (int i = 0; i < num_waypoints; ++i)
            {
                waypoints[i] = root_waypoint.GetChild(i);                                                                                   // Getting the child i inside the root transform.
            }
        }
    }
}

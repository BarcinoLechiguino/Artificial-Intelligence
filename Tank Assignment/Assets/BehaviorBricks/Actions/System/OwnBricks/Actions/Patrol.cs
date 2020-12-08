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
        [Help("Target Tank at which shot")]
        public GameObject target;

        [InParam("ai_behaviour")]
        public string ai_behaviour;

        [InParam("waypoints")]
        public Transform[] waypoints;

        [InParam("root_waypoint")]
        public Transform root_waypoint;

        private UnityEngine.AI.NavMeshAgent agent;
        private bool can_patrol = true;
        private int destination = 0;

        public override void OnStart()
        {
            if (ai_behaviour == "Wanderer")
            {
                return;
            }

            agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();

            GetWaypoints();

            if (can_patrol)
            {
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    SetNextWaypoint();

                    string log = "Waypoint " + (destination - 1) + " reached! Next waypoint: " + destination;
                    Debug.Log(log);
                }
            }
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.COMPLETED;
        }
        public void SetNextWaypoint()
        {
            if (waypoints.Length == 0)
            {
                can_patrol = false;

                string log = "Patroller Tank cannot patrol: No patrol waypoints were found!";
                Debug.LogWarning(log);

                return;
            }

            agent.destination = waypoints[destination].position;                                                // Will set the destination to the waypoint with the "destination" index.

            destination = (destination + 1) % waypoints.Length;                                                 // Sets destination with the index of the next waypoint. Cycles back to origin.
        }

        public void GetWaypoints()
        {
            int num_waypoints = root_waypoint.childCount;                                                                                   // Getting the total amount of childs in the root.

            if (waypoints.Length != num_waypoints)
            {
                waypoints = new Transform[num_waypoints];                                                                                       // Allocating the required memory.

                for (int i = 0; i < num_waypoints; ++i)
                {
                    waypoints[i] = root_waypoint.GetChild(i);                                                                                   // Getting the child i inside the root transform.
                }
            }
        }
    }
}

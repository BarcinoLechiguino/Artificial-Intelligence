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
        [Help("Target position where the game object will be moved")]
        public Vector3 target;

        [OutParam("target_out")]
        public Vector3 target_out;


        public override void OnStart()
        {

        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.COMPLETED;
        }
    }
}

/*private UnityEngine.AI.NavMeshAgent navAgent;
if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)*/

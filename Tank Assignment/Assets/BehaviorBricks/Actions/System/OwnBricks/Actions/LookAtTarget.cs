using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;

namespace BBUnity.Actions
{
    [Action("OwnBricks/Actions/LookAtTarget")]
    [Help("Moves the game object to a given position by using a NavMeshAgent")]
    public class LookAtTarget : GOAction
    {
        [InParam("target")]
        [Help("Target Tank at which look.")]
        public GameObject target;

        [InParam("fire_transform")]
        public Transform fire_transform;

        [InParam("turret")]
        public MeshRenderer turret;

        public override void OnStart()
        {
            //Vector3 new_forward = target.transform.position - gameObject.transform.position;          // Getting the vector that points from origin to target.
            Vector3 new_forward = target.transform.position - fire_transform.position;                  // Getting the vector that points from origin to target.

            turret.transform.forward = new_forward;                                                     // Setting the the turret_transform with the new forward vector.
            fire_transform.forward = turret.transform.forward;                                          // Also applying the new forward vector to the fire_transform.
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.COMPLETED;
        }
    }
}

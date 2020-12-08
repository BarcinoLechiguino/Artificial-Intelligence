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
            turret.transform.LookAt(target.transform);
            fire_transform.forward = turret.transform.forward;                                          // Also applying the new forward vector to the fire_transform.
        }

        public override TaskStatus OnUpdate()
        {


            return TaskStatus.COMPLETED;
        }
    }
}

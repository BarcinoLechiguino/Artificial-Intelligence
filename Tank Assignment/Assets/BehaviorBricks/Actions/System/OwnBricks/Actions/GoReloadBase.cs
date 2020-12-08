using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;

namespace BBUnity.Actions
{
    [Action("OwnBricks/Actions/GoReloadBase")]
    [Help("Paths to the tank's base to reload")]
    public class GoReloadBase : GOAction
    {
        ///<value>Input target position where the game object will be moved Parameter.</value>
        [InParam("base_waypoint")]
        public Transform base_waypoint;

        public override void OnStart()
        {

        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.COMPLETED;
        }
    }
}

using Pada1.BBCore;
using UnityEngine;

namespace BBUnity.Conditions
{
    [Condition("OwnBricks/Conditions/IsOutOfAmmo")]
    [Help("Checks whether the tank has any ammo left")]
    public class IsOutOfAmmo : GOCondition
    {
        [InParam("ammo")]
        [Help("Current amount of ammo")]
        public int ammo;

        public override bool Check()
        {   
            if (ammo == 0)
            {
                Debug.Log(gameObject.name + " is out of ammo!");
            }

            return (ammo == 0);
        }
    }
}
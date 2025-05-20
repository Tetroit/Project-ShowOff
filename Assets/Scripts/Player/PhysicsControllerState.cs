using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

namespace amogus
{
    [CreateAssetMenu(fileName = "PhysicsControllerState", menuName = "Player/PhysicsControllerState", order = 1)]
    public class PhysicsControllerState : ScriptableObject
    {
        public string controllerName;
        public float movementSpeed;
        public float speedLimit;
        public float airSpeed;
        public float criticalAngle;
        public float jumpHeight;
        public float acceleration;
        public float height;


        public PhysicsControllerState(
            string name = "lobotomy",
            float movementSpeed = 5f,
            float speedLimit = 10f,
            float airSpeed = 1f,
            float criticalAngle = 30f,
            float jumpHeight = 5f,
            float acceleration = 1f,
            float height = 2f
        )
        {
            this.controllerName = name;
            this.movementSpeed = movementSpeed;
            this.speedLimit = speedLimit;
            this.airSpeed = airSpeed;
            this.criticalAngle = criticalAngle;
            this.jumpHeight = jumpHeight;
            this.acceleration = acceleration;
            this.height = height;
        }
        public static PhysicsControllerState Lerp(PhysicsControllerState s1, PhysicsControllerState s2, float fac)
        {
            return new PhysicsControllerState(
            $"lerp between {s1.controllerName} and {s2.controllerName}",
                Mathf.Lerp(s1.movementSpeed, s2.movementSpeed, fac),
                Mathf.Lerp(s1.speedLimit, s2.speedLimit, fac),
                Mathf.Lerp(s1.airSpeed, s2.airSpeed, fac),
                Mathf.Lerp(s1.criticalAngle, s2.criticalAngle, fac),
                Mathf.Lerp(s1.jumpHeight, s2.jumpHeight, fac),
                Mathf.Lerp(s1.acceleration, s2.acceleration, fac),
                Mathf.Lerp(s1.height, s2.height, fac)
                );
        }

        public override string ToString()
        {
            return controllerName;
        }
    }
}

using amogus;
using UnityEngine;

namespace amogus
{
    public class Ladder : MonoBehaviour
    {
        public enum EndType
        {
            Start = 0,
            End = 1
        }
        [SerializeField] Vector3 start;
        [SerializeField] Vector3 end;
        /// <summary>
        /// wheere the player exists the ladder on the start side
        /// </summary>
        [SerializeField] Vector3 entryPointStart;

        /// <summary>
        /// wheere the player exists the ladder on the end side
        /// </summary>
        [SerializeField] Vector3 entryPointEnd;

        [field: SerializeField] public Quaternion facing { get; private set; }
        [field: SerializeField] public Quaternion startFacing { get; private set; }
        [field: SerializeField] public Quaternion endFacing { get; private set; }

        public Vector3 startGS => start + transform.position;
        public Vector3 endGS => end + transform.position;
        public Vector3 entryPointStartGS => entryPointStart + transform.position;
        public Vector3 entryPointEndGS => entryPointEnd + transform.position;

        [field: SerializeField] public LadderSwitch startLadderSwitch { get; private set; }
        [field: SerializeField] public LadderSwitch endLadderSwitch { get; private set; }
        public Vector3 GetEntryPoint(EndType end)
        {
            return end == EndType.Start ? entryPointStartGS : entryPointEndGS;
        }
        public Quaternion GetEntryRotation(EndType end)
        {
            return end == EndType.Start ? startFacing : endFacing;
        }
        public Vector3 GetEndPos(EndType end)
        {
            return end == EndType.Start ? startGS : endGS;
        }

        public Vector3 GetDir()
        {
            return (endGS - startGS).normalized;
        }
        [ExecuteInEditMode]
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(startGS, 0.2f);
            Gizmos.matrix = Matrix4x4.Translate(transform.position);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(start, end);

            Gizmos.DrawWireSphere(entryPointStart, 0.3f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(entryPointEnd, 0.3f);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(Vector3.zero, facing * Vector3.forward);
            Gizmos.DrawLine(entryPointStart, entryPointStart + startFacing * Vector3.forward);
            Gizmos.DrawLine(entryPointEnd, entryPointEnd + endFacing * Vector3.forward);
        }

        /// <summary>
        /// returns a radius-vector from the pointGS to the closest point on the ladder
        /// </summary>
        /// <param name="pointGS"></param>
        /// <returns></returns>
        public Vector3 GetDisplacement(Vector3 pointGS)
        {
            //duplicate for faster computation
            Vector3 closestPoint = GetClosestPoint(pointGS);
            return pointGS - closestPoint;
        }
        /// <summary>
        /// returns the closest point on the ladder to the given point
        /// </summary>
        /// <param name="pointGS"></param>
        /// <returns></returns>
        public Vector3 GetClosestPoint(Vector3 pointGS)
        {
            float fac = GetHeight(pointGS);
            Vector3 a = endGS - startGS;
            return startGS + fac * a;
        }
        public float GetHeight(Vector3 pointGS)
        {
            Vector3 a = endGS - startGS;

            if (a.magnitude < 0.001f)
                return 0;

            Vector3 d0 = pointGS - startGS;
            float fac = Vector3.Dot(a, d0) / Vector3.Dot(a, a);
            return Mathf.Clamp01(fac);
        }
    }
}


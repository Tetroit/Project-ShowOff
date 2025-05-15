using UnityEngine;

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

    public Vector3 startGS => start + transform.position; 
    public Vector3 endGS => end + transform.position;
    public Vector3 entryPointStartGS => entryPointStart + transform.position;
    public Vector3 entryPointEndGS => entryPointEnd + transform.position;

    public Vector3 GetEntryPoint(EndType end)
    {
        return end == EndType.Start ? entryPointStartGS : entryPointEndGS;
    }
    public Vector3 GetEndPos(EndType end)
    {
        return end == EndType.Start ? startGS : endGS;
    }

    [ExecuteInEditMode]
    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = Matrix4x4.Translate(transform.position);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(start, end);

        Gizmos.DrawWireSphere(entryPointStart, 0.3f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(entryPointEnd, 0.3f);
    }

    /// <summary>
    /// returns a radius-vector from the pointGS to the closest point on the ladder
    /// </summary>
    /// <param name="pointGS"></param>
    /// <returns></returns>
    public Vector3 GetDisplacement (Vector3 pointGS)
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
    public Vector3 GetClosestPoint (Vector3 pointGS)
    {
        //duplicate for faster computation
        Vector3 startGS = this.startGS;
        Vector3 endGS = this.endGS;
        Vector3 a = endGS - startGS;

        if (a.magnitude < 0.001f)
            return startGS;

        Vector3 d0 = pointGS - startGS;
        float fac = Vector3.Dot(a, d0) / Vector3.Dot(a, a);
        fac = Mathf.Clamp01(fac);
        return startGS + fac * a;
    }
}

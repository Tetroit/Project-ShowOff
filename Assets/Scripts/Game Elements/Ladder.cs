using UnityEngine;

public class Ladder : MonoBehaviour
{
    Vector3 start;
    Vector3 end;

    public Vector3 startGS => start + transform.position; 
    public Vector3 endGS => end + transform.position;


}

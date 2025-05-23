using UnityEngine;
using System.Collections;

public class CutsceneTrigger : MonoBehaviour
{
    [Header("Cutscene Object")]
    public GameObject objectToSpawn;
    public Transform spawnPoint;
    public Transform targetPoint;
    public float moveSpeed = 5f;
    [Header("cameraOffset")]
    public float cameraOffsetX;
    public float cameraOffsetY;
    public float cameraOffsetZ;

    public string pathParentName = "MillerPath";

    [Header("Cutscene Camera")]
    public Camera cutsceneCamera;

    private bool cutscenePlaying = false;
    private GameObject spawnedObject;

    void OnTriggerEnter(Collider other)
    {
        if (!cutscenePlaying && other.CompareTag("Player"))
        {
            cutscenePlaying = true;
            StartCoroutine(PlayCutscene(other.gameObject));
        }
    }

    IEnumerator PlayCutscene(GameObject player)
    {
        spawnedObject = Instantiate(objectToSpawn, spawnPoint.position, Quaternion.identity);

        cutsceneCamera.enabled = true;

        Vector3 offset = new Vector3(cameraOffsetX, cameraOffsetY, cameraOffsetZ);

        Vector3 worldOffset = player.transform.TransformDirection(offset);

        cutsceneCamera.transform.position = player.transform.position + worldOffset;

        cutsceneCamera.transform.LookAt(spawnedObject.transform);

        yield return new WaitForSeconds(0.5f);

        while (Vector3.Distance(spawnedObject.transform.position, targetPoint.position) > 0.1f)
        {   
            spawnedObject.transform.position = Vector3.MoveTowards(
                spawnedObject.transform.position,
                targetPoint.position,
                moveSpeed * Time.deltaTime
            );

            Quaternion targetRotation = Quaternion.LookRotation(
                spawnedObject.transform.position - cutsceneCamera.transform.position
            );
            cutsceneCamera.transform.rotation = Quaternion.Slerp(
                cutsceneCamera.transform.rotation,
                targetRotation,
                Time.deltaTime * 3f
            );

            yield return null;
        }

        yield return new WaitForSeconds(1f);

        cutsceneCamera.enabled = false;
        Destroy(spawnedObject); 
    }
}
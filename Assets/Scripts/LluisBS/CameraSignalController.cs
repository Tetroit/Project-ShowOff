using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class CameraRigController : MonoBehaviour
{
    [SerializeField] private Transform rigTransform;
    [SerializeField] private Transform millerTr;

    [SerializeField] private Transform cameraTransform;

    [Header("Animation Settings")]
    [SerializeField] private float turnDuration = 0.5f;
    [SerializeField] private float xAngle = 10f;

    public UnityEvent OnTurnComlpete;

    private bool isTurning = false;

    public void TurnCamera180()
    {
        if (isTurning || rigTransform == null || cameraTransform == null) return;


        Vector3 direction = millerTr.position - rigTransform.position;
        direction.y = 0; 

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            Vector3 targetEuler = targetRotation.eulerAngles;
            isTurning = true;
            cameraTransform.eulerAngles = new Vector3(xAngle, 0, 0);
            rigTransform.DORotate(targetEuler, turnDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    OnTurnComlpete?.Invoke();
                    isTurning = false;
                });
            var camRot = cameraTransform.eulerAngles;
             //cameraTransform.DORotate(new Vector3(xAngle, camRot.y, camRot.z), turnDuration);
        }
        else
        {
            cameraTransform.eulerAngles = new Vector3(xAngle, 0, 0);
            OnTurnComlpete?.Invoke();
            isTurning = false;

        }

    }
}
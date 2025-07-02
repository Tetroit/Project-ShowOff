using amogus;
using UnityEngine;
[RequireComponent (typeof(FreePlayerController))]
public class MovementChanger : MonoBehaviour
{
    [SerializeField] PhysicsControllerState _walk;
    [SerializeField] PhysicsControllerState _crouch;
    [SerializeField] PhysicsControllerState _sprint;

    [Header("Camera Shake")]
    [SerializeField]CameraWalkingShake _shakeController;
    [SerializeField] CameraShakeState _walkShake;
    [SerializeField] CameraShakeState _crouchShake;
    [SerializeField] CameraShakeState _sprintShake;

    FreePlayerController _controller;

    private void OnEnable()
    {
        _controller = GetComponent<FreePlayerController>();
        _controller.SetNewStates(_walk, _crouch, _sprint);
        _shakeController.ChangeShakeStates(_walkShake, _crouchShake, _sprintShake);
    }
}

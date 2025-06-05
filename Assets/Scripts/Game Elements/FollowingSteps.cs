using System.Collections;
using UnityEngine;

namespace amogus
{
    public class FollowingSteps : MonoBehaviour
    {
        [SerializeField] float _speed;
        [SerializeField] Curve curve;
        [SerializeField] AnimationCurve _timeCurve01;
        [SerializeField] PlayAudioOnDistanceTraveled _footSteps;
        [SerializeField] AnimationCurve _footStepDistanceCHanger;
        [Header("Camera Shake")]
        [SerializeField] CameraWalkingShake _cameraShake;
        [SerializeField] float _time;
        [SerializeField] float _scale;
        [SerializeField] float _frequency;

        Coroutine movement;
        public void StartFollowing()
        {
            movement = StartCoroutine(nameof(Step));
            _footSteps.OnStep.AddListener(OnStepSound);
        }
        public void EndFollowing()
        {
            if (movement != null) StopCoroutine(movement);
            _footSteps.OnStep.RemoveListener(OnStepSound);

        }

        IEnumerator Step()
        {
            float dist = curve.Getlength();
            float time01 = 0;
            while (time01 < 1)
            {
                time01 += _speed / dist * Time.deltaTime;
                transform.position = curve.GetPositionFromDistanceGS(_timeCurve01.Evaluate(time01) * dist);
                _footSteps.DistanceUntilStep = _footStepDistanceCHanger.Evaluate(time01);
                yield return new WaitForEndOfFrame();
            }
            yield break;
        }

        void OnStepSound()
        {
            _cameraShake.Shake(_time, _scale, _frequency);
        }

        public void Start()
        {
            transform.position = curve.startGS;
        }
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class HandController : MonoBehaviour
{
    [SerializeField] TimelineAsset _grabAnimator;
    [SerializeField] TimelineAsset _lighterAnimator;
    [SerializeField] PlayableDirector _playableDirector;
    bool _canGrab = true;
    public void DisableGrab()
    {
        _canGrab = false;
    }

    [ContextMenu("Grab")]
    public void PlayGrab()
    {
        if (!_canGrab) return;

        _playableDirector.time = 1;
        _playableDirector.Evaluate();
        _playableDirector.playableAsset = _grabAnimator;
        _playableDirector.Play();

        //disable obj after
        //StartCoroutine(DisableAfter());
    }

    [ContextMenu("Lighter")]
    public void PlayLighter()
    {
        //_playableDirector.time = 1;
        //_playableDirector.Evaluate();

        _playableDirector.playableAsset = _lighterAnimator;
        _playableDirector.Play();
    }

    IEnumerator DisableAfter(GameObject obj, float t)
    {
        yield return new WaitForSeconds(t);
        obj.SetActive(false);
    }
}

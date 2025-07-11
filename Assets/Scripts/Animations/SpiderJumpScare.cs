using System.Collections;
using UnityEngine;

public class SpiderJumpScare : MonoBehaviour
{
    [SerializeField] SpiderController _spider;
    [SerializeField] Note _note;
    [SerializeField] float _waitBeforePlaying = .5f;
    bool _animationPlayed;

    void OnEnable()
    {
        _note.OnRotate.AddListener(OnNoteRotate);
        _note.OnDismiss.AddListener(OnDismiss);
        _spider.transform.parent.gameObject.SetActive(false);
        
    }

    void OnDisable()
    {
        CancelAnimation();

    }

    void CancelAnimation()
    {
        if (_note == null || _spider == null) return;
        _note.OnRotate.RemoveListener(OnNoteRotate);
        _note.OnDismiss.RemoveListener(OnDismiss);
        _spider.OnFinishedWalking -= OnSpiderFinishedWalking;

        _spider.transform.parent.gameObject.SetActive(false);
    }

    void OnNoteRotate()
    {
        _note.OnRotate.RemoveListener(OnNoteRotate);

        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        _animationPlayed = true;
        _spider.transform.parent.gameObject.SetActive(true);
        yield return new WaitForSeconds(_waitBeforePlaying);
        _spider.StartMove();
        _spider.OnFinishedWalking += OnSpiderFinishedWalking;
    }

    void OnSpiderFinishedWalking()
    {

        CancelAnimation();
    }

    void OnDismiss()
    {
        if (!_animationPlayed) return;

        CancelAnimation();
    }
}

using System.Collections;
using UnityEngine;

public class SpiderJumpScare : MonoBehaviour
{
    [SerializeField] SpiderController _spider;
    [SerializeField] Note _note;
    [SerializeField] float _waitBeforePlaying = .5f;
    void OnEnable()
    {
        _note.OnRotate.AddListener(OnNoteRotate);
        _note.OnDismiss.AddListener(CancelAnimation);
        _spider.transform.parent.gameObject.SetActive(false);
        
    }

    void OnDisable()
    {
        CancelAnimation();

    }

    void CancelAnimation()
    {
        _note.OnRotate.RemoveListener(OnNoteRotate);
        _note.OnDismiss.RemoveListener(CancelAnimation);
        _spider.OnFinishedWalking += OnSpiderFinishedWalking;

        _spider.transform.parent.gameObject.SetActive(false);
    }

    void OnNoteRotate()
    {
        _note.OnRotate.RemoveListener(OnNoteRotate);

        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        _spider.transform.parent.gameObject.SetActive(true);
        yield return new WaitForSeconds(_waitBeforePlaying);
        _spider.StartMove();
        _spider.OnFinishedWalking += OnSpiderFinishedWalking;
    }

    void OnSpiderFinishedWalking()
    {
        CancelAnimation();
    }
}

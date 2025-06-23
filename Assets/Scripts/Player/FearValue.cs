using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class FearValue : MonoBehaviour
{
    GameObject[] enemies;
    [SerializeField] Volume fearPostProcessing;

    [SerializeField] float closeCapDist = 2f;
    [SerializeField] float farCapDist = 30f;
    [SerializeField] float closeCapValue = 1f;
    [SerializeField] float farCapValue = 0f;

    float fearValue;
    private void OnEnable()
    {
        SmoothlyEnable(1f);
    }

    public void SmoothlyDisable(float duration)
    {
        if (fearPostProcessing != null)
        {
            var tween = DOTween.To(() => fade, x => fade = x, 0, duration);
            tween.onComplete += () => { this.enabled = false; };
        }
        else
        {
            this.enabled = false;
        }
    }


    float fade = 0;
    public void SmoothlyEnable(float duration)
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        fade = 0;
        var tween = DOTween.To(() => fade, x => fade = x, 1, duration);

    }
    private void Update()
    {
        EvaluateFearLevel();
        SetFearValue();
    }

    void EvaluateFearLevel()
    {
        float minDist = float.MaxValue;
        foreach (GameObject enemy in enemies)
        {
            Transform tr = enemy.transform;
            float currentDist = Vector3.Distance(tr.position, transform.position);
            if (currentDist < minDist)
            {
                minDist = currentDist;
            }
        }
        fearValue = Mathf.Lerp(closeCapValue, farCapValue, Mathf.InverseLerp(closeCapDist, farCapDist, minDist));
    }

    void SetFearValue()
    {
        if (fearPostProcessing == null) return;
        
        fearPostProcessing.weight = fearValue * fade;
    }
}

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class FadeToggle : MonoBehaviour
{
    public float fadeTime = 0.12f;
    [SerializeField] bool blockRaycastsWhenVisible;

    CanvasGroup cg;
    Coroutine co;

    void Awake()
    {
        EnsureCanvasGroup();
        if (cg)
        {
            cg.alpha = 0;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
    }

    void EnsureCanvasGroup()
    {
        if (cg == null) cg = GetComponent<CanvasGroup>();
    }

    public void SetVisible(bool on)
    {
        EnsureCanvasGroup();
        if (!gameObject.activeSelf && on)
            gameObject.SetActive(true);             // ensure object stays alive for fade

        if (co != null) StopCoroutine(co);
        co = StartCoroutine(CoFade(on));
    }

    IEnumerator CoFade(bool on)
    {
        EnsureCanvasGroup();
        if (cg == null)
        {
            gameObject.SetActive(on);
            yield break;
        }

        float t = 0f, dur = Mathf.Max(0.0001f, fadeTime);
        float a0 = cg.alpha, a1 = on ? 1f : 0f;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float s = Mathf.SmoothStep(0,1,t/dur);
            cg.alpha = Mathf.LerpUnclamped(a0, a1, s);
            yield return null;
        }
        cg.alpha = a1;
        cg.interactable = false;
        cg.blocksRaycasts = on && blockRaycastsWhenVisible;
    }
}

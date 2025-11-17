using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public enum ToastType { Info, Success, Warning, Error }

public class ToastMessageUI : MonoBehaviour
{
    [Header("Wiring")]
    public CanvasGroup cg;
    public RectTransform rt;
    public Image background;
    public TextMeshProUGUI title;
    public TextMeshProUGUI body;

    [Header("Style")]
    public Color infoColor    = new Color(0.20f,0.55f,1f,1f);
    public Color successColor = new Color(0.25f,0.8f,0.45f,1f);
    public Color warningColor = new Color(1f,0.7f,0.2f,1f);
    public Color errorColor   = new Color(1f,0.3f,0.3f,1f);
    public Color bgBase       = new Color(0.10f,0.10f,0.12f,0.90f);

    [Header("Behavior")]
    public float showTime = 2.2f;
    public float fadeTime = 0.16f;
    public float slidePx  = 22f;
    public bool  pauseOnHover = true;

    Coroutine lifeCo;
    bool hovered;

    void Reset()
    {
        cg = GetComponent<CanvasGroup>();
        rt = GetComponent<RectTransform>();
    }

    public void Setup(ToastType type, string titleText, string bodyText = null)
    {
        if (!cg) cg = gameObject.AddComponent<CanvasGroup>();
        if (!rt) rt = GetComponent<RectTransform>();

        if (title) title.text = titleText ?? "";
        if (body)
        {
            body.text = string.IsNullOrEmpty(bodyText) ? "" : bodyText;
            body.gameObject.SetActive(!string.IsNullOrEmpty(bodyText));
        }

        var tint = type switch
        {
            ToastType.Success => successColor,
            ToastType.Warning => warningColor,
            ToastType.Error   => errorColor,
            _                 => infoColor
        };
        if (background) background.color = new Color(
            Mathf.Lerp(bgBase.r, tint.r, 0.25f),
            Mathf.Lerp(bgBase.g, tint.g, 0.25f),
            Mathf.Lerp(bgBase.b, tint.b, 0.25f),
            bgBase.a
        );

        // start hidden
        cg.alpha = 0f;
        if (rt) rt.anchoredPosition += new Vector2(0, -slidePx);
    }

    public void Play()
    {
        if (lifeCo != null) StopCoroutine(lifeCo);
        lifeCo = StartCoroutine(CoLife());
    }

    IEnumerator CoLife()
    {
        // fade/slide in
        yield return CoFade(0f, 1f, +slidePx);

        float t = 0f;
        while (t < showTime)
        {
            if (!(pauseOnHover && hovered)) t += Time.unscaledDeltaTime;
            yield return null;
        }

        // fade/slide out
        yield return CoFade(1f, 0f, -slidePx);

        Destroy(gameObject);
    }

    IEnumerator CoFade(float a0, float a1, float slideDelta)
    {
        float t = 0f, dur = Mathf.Max(0.0001f, fadeTime);
        var basePos = rt ? rt.anchoredPosition : Vector2.zero;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float s = Mathf.SmoothStep(0,1,t/dur);
            if (cg) cg.alpha = Mathf.Lerp(a0, a1, s);
            if (rt) rt.anchoredPosition = basePos + new Vector2(0, slideDelta * (1f - s));
            yield return null;
        }
        if (cg) cg.alpha = a1;
        if (rt) rt.anchoredPosition = basePos;
    }

    // Hover-Pause
    public void OnPointerEnter() { hovered = true; }
    public void OnPointerExit()  { hovered = false; }
}

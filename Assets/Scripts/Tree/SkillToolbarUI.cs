using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SkillToolbarUI : MonoBehaviour
{
    [Header("Wiring")]
    public TreePanelController panel; // irgendein Controller aus dem aktiven Tab
    public TreeState state;

    [Header("UI")]
    public TextMeshProUGUI pointsLabel;
    public TextMeshProUGUI queuedLabel;
    public Button confirmButton;
    public Button clearButton;
    public Button respecButton;

    RectTransform pointsRect;
    Vector3 pointsBaseScale = Vector3.one;
    Vector2 pointsBasePos;
    Coroutine pointsAnim;
    TreePanelController subscribedPanel;

    void OnEnable()
    {
        if (!state && panel) state = panel.state;
        if (!panel) panel = GetComponentInParent<TreePanelController>();
        CachePointsLabel();
        Hook(true);
        Refresh();
    }

    void OnDisable()
    {
        Hook(false);
        if (pointsAnim != null && pointsRect != null)
        {
            StopCoroutine(pointsAnim);
            pointsAnim = null;
            pointsRect.localScale = pointsBaseScale;
            pointsRect.anchoredPosition = pointsBasePos;
        }
    }

    void Hook(bool on)
    {
        var targetPanel = ResolvePanel();
        if (targetPanel != subscribedPanel)
        {
            if (subscribedPanel) subscribedPanel.OnInsufficientPoints -= PulsePointsLabel;
            subscribedPanel = targetPanel;
        }
        if (subscribedPanel)
        {
            if (on) subscribedPanel.OnInsufficientPoints += PulsePointsLabel;
            else    subscribedPanel.OnInsufficientPoints -= PulsePointsLabel;
        }

        if (!state) return;
        if (on)
        {
            state.OnChanged += Refresh;
            state.OnQueueChanged += Refresh;
        }
        else
        {
            state.OnChanged -= Refresh;
            state.OnQueueChanged -= Refresh;
        }

        if (confirmButton)
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(Confirm);
        }
        if (clearButton)
        {
            clearButton.onClick.RemoveAllListeners();
            clearButton.onClick.AddListener(Clear);
        }
        if (respecButton)
        {
            respecButton.onClick.RemoveAllListeners();
            respecButton.onClick.AddListener(Respec);
        }
    }

    void CachePointsLabel()
    {
        if (!pointsLabel) return;
        pointsRect = pointsLabel.rectTransform;
        pointsBaseScale = pointsRect.localScale;
        pointsBasePos = pointsRect.anchoredPosition;
    }

    TreePanelController ResolvePanel()
    {
        if (panel) return panel;
        panel = GetComponentInParent<TreePanelController>();
        return panel;
    }

    void Refresh()
    {
        if (!state) return;

        int QueuedCostLookup(string id) => panel ? panel.CostOf(id) : 0;

        int queuedCost = state.GetQueuedTotalCost(QueuedCostLookup);
        int available  = state.GetAvailablePoints(QueuedCostLookup);

        if (pointsLabel) pointsLabel.text = $"Points: {available}";
        if (queuedLabel) queuedLabel.text = queuedCost > 0 ? $"Queued: -{queuedCost}" : "Queued: 0";

        if (confirmButton) confirmButton.interactable = queuedCost > 0 && available + queuedCost >= queuedCost;
        if (clearButton)   clearButton.interactable   = state.Queued.Count > 0;
        if (respecButton)  respecButton.interactable  = state.Unlocked.Count > 0 || state.Queued.Count > 0;
    }

    void Confirm()
    {
        if (!state) return;
        int queuedCost = state.GetQueuedTotalCost(id => panel ? panel.CostOf(id) : 0);
        state.ApplyQueue(id => panel ? panel.CostOf(id) : 0);
        panel?.RefreshAll();
        var bj = confirmButton ? confirmButton.GetComponent<ButtonJuice>() : null;
        if (bj) bj.PulseSuccess();
        if (queuedCost > 0)
            ToastSystem.Success("Skills angewendet", $"-{queuedCost} Punkte");
    }

    void Clear()
    {
        if (!state) return;
        state.ClearQueue();
        panel?.RefreshAll();
    }

    void Respec()
    {
        if (!state) return;
        state.Respec(id => panel ? panel.CostOf(id) : 0);
        panel?.RefreshAll();
    }

    void PulsePointsLabel()
    {
        if (!isActiveAndEnabled || !pointsRect) return;
        if (pointsAnim != null) return; // ignore rapid re-triggers until finished
        pointsAnim = StartCoroutine(CoPulsePoints());
    }

    IEnumerator CoPulsePoints()
    {
        float duration = 0.45f;
        float maxScale = 1.25f;
        float wiggles = 4f;
        float t = 0f;

        Vector3 startScale = pointsRect.localScale;
        Vector2 startPos = pointsRect.anchoredPosition;
        var rect = pointsRect.rect;
        var pivot = pointsRect.pivot;
        Vector2 size = new Vector2(rect.width * startScale.x, rect.height * startScale.y);

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float n = Mathf.Clamp01(t / duration);
            float scale = Mathf.Lerp(maxScale, 1f, n);
            float wobble = Mathf.Sin(n * Mathf.PI * wiggles) * 0.05f;
            float totalScale = Mathf.Lerp(1f, maxScale, EaseOutBack(1f - n)) + wobble;
            pointsRect.localScale = startScale * totalScale;

            float expansion = totalScale - 1f;
            Vector2 centerOffset = new Vector2((pivot.x - 0.5f) * size.x * expansion,
                                               (pivot.y - 0.5f) * size.y * expansion);
            Vector2 wiggleOffset = Vector2.zero;
            pointsRect.anchoredPosition = startPos + centerOffset + wiggleOffset;
            yield return null;
        }

        pointsRect.localScale = startScale;
        pointsRect.anchoredPosition = startPos;
        pointsBaseScale = startScale;
        pointsBasePos = startPos;
        pointsAnim = null;
    }

    static float EaseOutBack(float x)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
    }
}

using UnityEngine;
using TMPro;
using System.Linq;

public class SkillEmptyState : MonoBehaviour
{
    public TreeState state;
    public GameObject emptyPanel;
    public TextMeshProUGUI label;
    [SerializeField, TextArea]
    private string emptyMessage = "Earn skill points by progressing.\nSelect a node to queue when you have points.";

    // Optional: auch anzeigen, wenn zwar Punkte = 0, aber noch nichts unlocked ist
    public bool showWhenNoPoints = true;
    public bool hideIfAnyUnlocked = true;

    void OnEnable()
    {
        if (state)
        {
            state.OnChanged += Refresh;
            state.OnQueueChanged += Refresh;
        }
        Refresh();
    }
    void OnDisable()
    {
        if (state)
        {
            state.OnChanged -= Refresh;
            state.OnQueueChanged -= Refresh;
        }
    }

    void Refresh()
    {
        bool anyUnlocked = state != null && state.Unlocked.Count > 0;
        bool hasPointsOrQueued = state != null && (state.points > 0 || state.Queued.Count > 0);

        bool show = false;
        if (showWhenNoPoints && !hasPointsOrQueued) show = true;
        if (hideIfAnyUnlocked && anyUnlocked) show = false;

        if (label) label.text = emptyMessage;
        if (emptyPanel)
        {
            var fade = emptyPanel.GetComponent<FadeToggle>();
            if (fade) fade.SetVisible(show);
            else emptyPanel.SetActive(show);
        }
    }
}

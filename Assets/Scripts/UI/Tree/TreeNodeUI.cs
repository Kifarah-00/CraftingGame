using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TreeNodeUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Refs")]
    public Image icon;
    public Image frame;
    public Button button;
    public TextMeshProUGUI label;
    public TextMeshProUGUI costText;

    [HideInInspector] public SkillDefinition def;
    [HideInInspector] public TreePanelController controller;

    public void Bind(SkillDefinition d, TreePanelController c)
    {
        def = d; controller = c;
        if (icon) icon.sprite = d.icon;
        if (label) label.text = d.displayName;
        if (costText) costText.text = d.cost > 0 ? d.cost.ToString() : "";
        if (button)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }
    }

    void OnClick()
    {
        if (controller) controller.ToggleQueue(def);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!controller || controller.state == null || def == null) return;
        var st = controller.state;
        bool unlocked = st.IsUnlocked(def.id);
        bool queued = st.IsQueued(def.id);
        if (unlocked || queued) return;

        bool prereqsMet = controller.ArePrereqsMet(def, considerQueued:true);
        if (!prereqsMet) return;

        int available = st.GetAvailablePoints(controller.CostOf);
        if (available < def.cost) controller.NotifyInsufficientPoints();
    }
}

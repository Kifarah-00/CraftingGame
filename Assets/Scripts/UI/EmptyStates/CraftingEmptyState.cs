using UnityEngine;
using TMPro;

public class CraftingEmptyState : MonoBehaviour
{
    public CraftingGrid grid;          // data/UI wrapper for 4x4 grid
    public GameObject emptyPanel;
    public TextMeshProUGUI label;
    [SerializeField, TextArea]
    private string emptyMessage = "Drag items here to craft";

    void OnEnable()
    {
        if (grid) grid.OnChanged += Refresh;  // falls du kein Event hast, ruf Refresh() nach jedem Slot-Update manuell
        Refresh();
    }
    void OnDisable()
    {
        if (grid) grid.OnChanged -= Refresh;
    }

    void Refresh()
    {
        bool any = false;
        if (grid != null)
        {
            for (int i = 0; i < grid.SlotCount; i++)
                if (!grid.GetSlot(i).IsEmpty) { any = true; break; }
        }
        if (label) label.text = emptyMessage;
        if (emptyPanel)
        {
            var fade = emptyPanel.GetComponent<FadeToggle>();
            if (fade) fade.SetVisible(!any);
            else emptyPanel.SetActive(!any);
        }
    }
}

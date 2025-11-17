using UnityEngine;
using TMPro;

public class InventoryEmptyState : MonoBehaviour
{
    public Inventory inventory;
    public GameObject emptyPanel;          // Empty_Inventory
    public TextMeshProUGUI label;          // optional
    [SerializeField, TextArea]
    private string emptyMessage = "No items yet.\nFind or craft materials.";

    void OnEnable()
    {
        if (inventory) inventory.OnChanged += Refresh;
        Refresh();
    }
    void OnDisable()
    {
        if (inventory) inventory.OnChanged -= Refresh;
    }

    void Refresh()
    {
        bool any = false;
        if (inventory != null)
        {
            var slots = inventory.Slots;
            for (int i = 0; i < slots.Count; i++)
                if (!slots[i].IsEmpty) { any = true; break; }
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

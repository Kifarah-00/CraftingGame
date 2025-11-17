using UnityEngine;

public class CraftingGrid : MonoBehaviour
{
    public const int Width = 4;
    public const int Height = 4;

    [SerializeField] private ItemStack[] cells = new ItemStack[Width * Height];

    public int SlotCount => cells.Length;
    public ItemStack GetSlot(int index) => cells[index];
    public ItemStack GetSlot(int x, int y) => cells[y * Width + x];
    public ItemStack GetCell(int index) => GetSlot(index);
    public ItemStack GetCell(int x, int y) => GetSlot(x, y);
    public event System.Action OnChanged;
public void RaiseChanged() => OnChanged?.Invoke();

    private void Awake()
    {
        for (int i = 0; i < cells.Length; i++)
            if (cells[i] == null) cells[i] = new ItemStack();
    }

    public void Clear()
    {
        foreach (var c in cells) c.Clear();
    }
}

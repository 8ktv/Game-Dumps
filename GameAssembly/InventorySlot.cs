public struct InventorySlot
{
	public ItemType itemType;

	public int remainingUses;

	public static InventorySlot Empty => default(InventorySlot);

	public InventorySlot(ItemType itemType, int remainingUses)
	{
		this.itemType = itemType;
		this.remainingUses = remainingUses;
	}
}

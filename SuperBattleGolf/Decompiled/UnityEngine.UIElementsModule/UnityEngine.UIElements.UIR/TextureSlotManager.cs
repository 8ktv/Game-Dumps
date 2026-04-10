using System.Runtime.CompilerServices;

namespace UnityEngine.UIElements.UIR;

internal class TextureSlotManager
{
	internal static readonly int k_MaxSlotCount;

	internal static readonly int k_SlotSize;

	internal static int[] slotIds;

	internal static readonly int textureTableId;

	private TextureId[] m_Textures;

	private int[] m_LastUseTime;

	private int m_CurrentTime;

	private int m_BatchTime;

	private Vector4[] m_GpuTextures;

	private int m_SlotCount;

	internal TextureRegistry textureRegistry = TextureRegistry.instance;

	public int FreeSlots { get; private set; }

	static TextureSlotManager()
	{
		k_SlotSize = 2;
		textureTableId = Shader.PropertyToID("_TextureInfo");
		k_MaxSlotCount = 8;
		slotIds = new int[k_MaxSlotCount];
		for (int i = 0; i < k_MaxSlotCount; i++)
		{
			slotIds[i] = Shader.PropertyToID($"_Texture{i}");
		}
	}

	public TextureSlotManager()
	{
		m_Textures = new TextureId[k_MaxSlotCount];
		m_LastUseTime = new int[k_MaxSlotCount];
		m_GpuTextures = new Vector4[k_MaxSlotCount * k_SlotSize];
		m_SlotCount = k_MaxSlotCount;
		FreeSlots = k_MaxSlotCount;
		Reset();
	}

	public void Reset()
	{
		m_CurrentTime = 0;
		m_BatchTime = 0;
		Unbind(0, k_MaxSlotCount);
	}

	private void Unbind(int first, int count = 1)
	{
		for (int i = first; i < first + count; i++)
		{
			m_Textures[i] = TextureId.invalid;
			m_LastUseTime[i] = -1;
			SetGpuData(i, TextureId.invalid, 1, 1, 0f, 0f, isPremultiplied: false);
		}
	}

	public void StartNewBatch(int slotCount)
	{
		if (slotCount < m_SlotCount)
		{
			Unbind(slotCount, m_SlotCount - slotCount);
		}
		m_BatchTime = ++m_CurrentTime;
		m_SlotCount = slotCount;
		FreeSlots = slotCount;
	}

	public int IndexOf(TextureId id)
	{
		for (int i = 0; i < m_SlotCount; i++)
		{
			if (m_Textures[i].index == id.index)
			{
				return i;
			}
		}
		return -1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void MarkUsed(int slotIndex)
	{
		int num = m_LastUseTime[slotIndex];
		if (num < m_BatchTime)
		{
			int freeSlots = FreeSlots - 1;
			FreeSlots = freeSlots;
		}
		m_LastUseTime[slotIndex] = ++m_CurrentTime;
	}

	public int FindOldestSlot()
	{
		int num = m_LastUseTime[0];
		int result = 0;
		for (int i = 1; i < m_SlotCount; i++)
		{
			if (m_LastUseTime[i] < num)
			{
				num = m_LastUseTime[i];
				result = i;
			}
		}
		return result;
	}

	public void Bind(TextureId id, float sdfScale, float sharpness, bool isPremultiplied, int slot, MaterialPropertyBlock mat, CommandList commandList = null)
	{
		Texture texture = textureRegistry.GetTexture(id);
		if (texture == null)
		{
			texture = Texture2D.whiteTexture;
		}
		m_Textures[slot] = id;
		MarkUsed(slot);
		SetGpuData(slot, id, texture.width, texture.height, sdfScale, sharpness, isPremultiplied);
		if (commandList == null)
		{
			mat.SetTexture(slotIds[slot], texture);
			mat.SetVectorArray(textureTableId, m_GpuTextures);
		}
		else
		{
			int num = slot * k_SlotSize;
			commandList.SetTexture(slotIds[slot], texture, num, m_GpuTextures[num], m_GpuTextures[num + 1]);
		}
	}

	public void SetGpuData(int slotIndex, TextureId id, int textureWidth, int textureHeight, float sdfScale, float sharpness, bool isPremultiplied)
	{
		int num = slotIndex * k_SlotSize;
		float y = 1f / (float)textureWidth;
		float z = 1f / (float)textureHeight;
		m_GpuTextures[num] = new Vector4(id.ConvertToGpu(), y, z, sdfScale);
		m_GpuTextures[num + 1] = new Vector4(textureWidth, textureHeight, sharpness, isPremultiplied ? 1f : 0f);
	}
}

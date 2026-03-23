using System;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Picnic;

internal sealed class Tree
{
	private static int MAX_SEED_SIZE_BYTES = 32;

	private uint MAX_AUX_BYTES;

	private uint depth;

	internal byte[][] nodes;

	private int dataSize;

	private bool[] haveNode;

	private byte[] exists;

	private uint numNodes;

	private uint numLeaves;

	private PicnicEngine engine;

	internal byte[][] GetLeaves()
	{
		return nodes;
	}

	internal uint GetLeavesOffset()
	{
		return numNodes - numLeaves;
	}

	internal Tree(PicnicEngine engine, uint numLeaves, int dataSize)
	{
		this.engine = engine;
		MAX_AUX_BYTES = (PicnicEngine.LOWMC_MAX_AND_GATES + PicnicEngine.LOWMC_MAX_KEY_BITS) / 8 + 1;
		depth = PicnicUtilities.ceil_log2(numLeaves) + 1;
		numNodes = (uint)((1 << (int)depth) - 1 - ((1 << (int)(depth - 1)) - numLeaves));
		this.numLeaves = numLeaves;
		this.dataSize = dataSize;
		nodes = new byte[numNodes][];
		for (int i = 0; i < numNodes; i++)
		{
			nodes[i] = new byte[dataSize];
		}
		haveNode = new bool[numNodes];
		exists = new byte[numNodes];
		Arrays.Fill(exists, (int)(numNodes - this.numLeaves), (int)numNodes, 1);
		for (uint num = numNodes - this.numLeaves; num != 0; num--)
		{
			if (Exists(2 * num + 1) || Exists(2 * num + 2))
			{
				exists[num] = 1;
			}
		}
		exists[0] = 1;
	}

	internal void BuildMerkleTree(byte[][] leafData, byte[] salt)
	{
		uint num = numNodes - numLeaves;
		for (int i = 0; i < numLeaves; i++)
		{
			if (leafData[i] != null)
			{
				Array.Copy(leafData[i], 0, nodes[num + i], 0, dataSize);
				haveNode[num + i] = true;
			}
		}
		for (uint num2 = numNodes; num2 != 0; num2--)
		{
			ComputeParentHash(num2, salt);
		}
	}

	internal int VerifyMerkleTree(byte[][] leafData, byte[] salt)
	{
		uint num = numNodes - numLeaves;
		for (int i = 0; i < numLeaves; i++)
		{
			if (leafData[i] != null)
			{
				if (haveNode[num + i])
				{
					return -1;
				}
				if (leafData[i] != null)
				{
					Array.Copy(leafData[i], 0, nodes[num + i], 0, dataSize);
					haveNode[num + i] = true;
				}
			}
		}
		for (uint num2 = numNodes; num2 != 0; num2--)
		{
			ComputeParentHash(num2, salt);
		}
		if (!haveNode[0])
		{
			return -1;
		}
		return 0;
	}

	internal int ReconstructSeeds(uint[] hideList, uint hideListSize, byte[] input, uint inputLen, byte[] salt, uint repIndex)
	{
		int result = 0;
		uint num = inputLen;
		uint[] array = new uint[1] { 0u };
		uint[] revealedNodes = GetRevealedNodes(hideList, hideListSize, array);
		for (int i = 0; i < array[0]; i++)
		{
			num -= (uint)engine.seedSizeBytes;
			if (num < 0)
			{
				return -1;
			}
			Array.Copy(input, i * engine.seedSizeBytes, nodes[revealedNodes[i]], 0, engine.seedSizeBytes);
			haveNode[revealedNodes[i]] = true;
		}
		ExpandSeeds(salt, repIndex);
		return result;
	}

	internal byte[] OpenMerkleTree(uint[] missingLeaves, uint missingLeavesSize, int[] outputSizeBytes)
	{
		uint[] array = new uint[1];
		uint[] revealedMerkleNodes = GetRevealedMerkleNodes(missingLeaves, missingLeavesSize, array);
		outputSizeBytes[0] = (int)array[0] * dataSize;
		byte[] array2 = new byte[outputSizeBytes[0]];
		byte[] result = array2;
		for (int i = 0; i < array[0]; i++)
		{
			Array.Copy(nodes[revealedMerkleNodes[i]], 0, array2, i * dataSize, dataSize);
		}
		return result;
	}

	private uint[] GetRevealedNodes(uint[] hideList, uint hideListSize, uint[] outputSize)
	{
		uint num = depth - 1;
		uint[][] array = new uint[num][];
		for (int i = 0; i < num; i++)
		{
			array[i] = new uint[hideListSize];
		}
		for (int j = 0; j < hideListSize; j++)
		{
			uint num2 = 0u;
			uint num3 = hideList[j] + (numNodes - numLeaves);
			array[num2][j] = num3;
			num2++;
			while ((num3 = GetParent(num3)) != 0)
			{
				array[num2][j] = num3;
				num2++;
			}
		}
		uint[] array2 = new uint[numLeaves];
		uint num4 = 0u;
		for (int k = 0; k < num; k++)
		{
			for (int l = 0; l < hideListSize; l++)
			{
				if (!HasSibling(array[k][l]))
				{
					continue;
				}
				uint num5 = GetSibling(array[k][l]);
				if (!Contains(array[k], hideListSize, num5))
				{
					while (!HasRightChild(num5) && !IsLeafNode(num5))
					{
						num5 = 2 * num5 + 1;
					}
					if (!Contains(array2, num4, num5))
					{
						array2[num4] = num5;
						num4++;
					}
				}
			}
		}
		outputSize[0] = num4;
		return array2;
	}

	private uint GetSibling(uint node)
	{
		if (IsLeftChild(node))
		{
			if (node + 1 < numNodes)
			{
				return node + 1;
			}
			Console.Error.Write("getSibling: request for node with not sibling");
			return 0u;
		}
		return node - 1;
	}

	private bool IsLeafNode(uint node)
	{
		return 2 * node + 1 >= numNodes;
	}

	private bool HasSibling(uint node)
	{
		if (!Exists(node))
		{
			return false;
		}
		if (IsLeftChild(node) && !Exists(node + 1))
		{
			return false;
		}
		return true;
	}

	internal uint RevealSeedsSize(uint[] hideList, uint hideListSize)
	{
		uint[] array = new uint[1] { 0u };
		GetRevealedNodes(hideList, hideListSize, array);
		return array[0] * (uint)engine.seedSizeBytes;
	}

	internal int RevealSeeds(uint[] hideList, uint hideListSize, byte[] output, int outputSize)
	{
		uint[] array = new uint[1] { 0u };
		int num = outputSize;
		uint[] revealedNodes = GetRevealedNodes(hideList, hideListSize, array);
		for (int i = 0; i < array[0]; i++)
		{
			num -= engine.seedSizeBytes;
			if (num < 0)
			{
				Console.Error.Write("Insufficient sized buffer provided to revealSeeds");
				return 0;
			}
			Array.Copy(nodes[revealedNodes[i]], 0, output, i * engine.seedSizeBytes, engine.seedSizeBytes);
		}
		return output.Length - num;
	}

	internal uint OpenMerkleTreeSize(uint[] missingLeaves, uint missingLeavesSize)
	{
		uint[] array = new uint[1];
		GetRevealedMerkleNodes(missingLeaves, missingLeavesSize, array);
		return array[0] * (uint)engine.digestSizeBytes;
	}

	private uint[] GetRevealedMerkleNodes(uint[] missingLeaves, uint missingLeavesSize, uint[] outputSize)
	{
		uint num = numNodes - numLeaves;
		bool[] array = new bool[numNodes];
		for (int i = 0; i < missingLeavesSize; i++)
		{
			array[num + missingLeaves[i]] = true;
		}
		for (uint num2 = GetParent(numNodes - 1); num2 != 0; num2--)
		{
			if (Exists(num2))
			{
				if (Exists(2 * num2 + 2))
				{
					if (array[2 * num2 + 1] && array[2 * num2 + 2])
					{
						array[num2] = true;
					}
				}
				else if (array[2 * num2 + 1])
				{
					array[num2] = true;
				}
			}
		}
		uint[] array2 = new uint[numLeaves];
		uint num3 = 0u;
		for (int j = 0; j < missingLeavesSize; j++)
		{
			uint num4 = missingLeaves[j] + num;
			do
			{
				if (!array[GetParent(num4)])
				{
					if (!Contains(array2, num3, num4))
					{
						array2[num3] = num4;
						num3++;
					}
					break;
				}
			}
			while ((num4 = GetParent(num4)) != 0);
		}
		outputSize[0] = num3;
		return array2;
	}

	private bool Contains(uint[] list, uint len, uint value)
	{
		for (int i = 0; i < len; i++)
		{
			if (list[i] == value)
			{
				return true;
			}
		}
		return false;
	}

	private void ComputeParentHash(uint child, byte[] salt)
	{
		if (!Exists(child))
		{
			return;
		}
		uint parent = GetParent(child);
		if (!haveNode[parent] && haveNode[2 * parent + 1] && (!Exists(2 * parent + 2) || haveNode[2 * parent + 2]))
		{
			engine.digest.Update(3);
			engine.digest.BlockUpdate(nodes[2 * parent + 1], 0, engine.digestSizeBytes);
			if (HasRightChild(parent))
			{
				engine.digest.BlockUpdate(nodes[2 * parent + 2], 0, engine.digestSizeBytes);
			}
			engine.digest.BlockUpdate(salt, 0, PicnicEngine.saltSizeBytes);
			engine.digest.BlockUpdate(Pack.UInt32_To_LE(parent), 0, 2);
			engine.digest.OutputFinal(nodes[parent], 0, engine.digestSizeBytes);
			haveNode[parent] = true;
		}
	}

	internal byte[] GetLeaf(uint leafIndex)
	{
		uint num = numNodes - numLeaves;
		return nodes[num + leafIndex];
	}

	internal int AddMerkleNodes(uint[] missingLeaves, uint missingLeavesSize, byte[] input, uint inputSize)
	{
		int num = (int)inputSize;
		uint[] array = new uint[1] { 0u };
		uint[] revealedMerkleNodes = GetRevealedMerkleNodes(missingLeaves, missingLeavesSize, array);
		for (int i = 0; i < array[0]; i++)
		{
			num -= dataSize;
			if (num < 0)
			{
				return -1;
			}
			Array.Copy(input, i * dataSize, nodes[revealedMerkleNodes[i]], 0, dataSize);
			haveNode[revealedMerkleNodes[i]] = true;
		}
		if (num != 0)
		{
			return -1;
		}
		return 0;
	}

	internal void GenerateSeeds(byte[] rootSeed, byte[] salt, uint repIndex)
	{
		nodes[0] = rootSeed;
		haveNode[0] = true;
		ExpandSeeds(salt, repIndex);
	}

	private void ExpandSeeds(byte[] salt, uint repIndex)
	{
		byte[] array = new byte[2 * MAX_SEED_SIZE_BYTES];
		uint parent = GetParent(numNodes - 1);
		for (uint num = 0u; num <= parent; num++)
		{
			if (haveNode[num])
			{
				HashSeed(array, nodes[num], salt, 1, repIndex, num);
				if (!haveNode[2 * num + 1])
				{
					Array.Copy(array, 0, nodes[2 * num + 1], 0, engine.seedSizeBytes);
					haveNode[2 * num + 1] = true;
				}
				if (Exists(2 * num + 2) && !haveNode[2 * num + 2])
				{
					Array.Copy(array, engine.seedSizeBytes, nodes[2 * num + 2], 0, engine.seedSizeBytes);
					haveNode[2 * num + 2] = true;
				}
			}
		}
	}

	private void HashSeed(byte[] digest_arr, byte[] inputSeed, byte[] salt, byte hashPrefix, uint repIndex, uint nodeIndex)
	{
		engine.digest.Update(hashPrefix);
		engine.digest.BlockUpdate(inputSeed, 0, engine.seedSizeBytes);
		engine.digest.BlockUpdate(salt, 0, PicnicEngine.saltSizeBytes);
		engine.digest.BlockUpdate(Pack.UInt16_To_LE((ushort)(repIndex & 0xFFFF)), 0, 2);
		engine.digest.BlockUpdate(Pack.UInt16_To_LE((ushort)(nodeIndex & 0xFFFF)), 0, 2);
		engine.digest.OutputFinal(digest_arr, 0, 2 * engine.seedSizeBytes);
	}

	private bool IsLeftChild(uint node)
	{
		return node % 2 == 1;
	}

	private bool HasRightChild(uint node)
	{
		if (2 * node + 2 < numNodes)
		{
			return Exists(node);
		}
		return false;
	}

	private uint GetParent(uint node)
	{
		if (IsLeftChild(node))
		{
			return (node - 1) / 2;
		}
		return (node - 2) / 2;
	}

	private bool Exists(uint i)
	{
		if (i >= numNodes)
		{
			return false;
		}
		return exists[i] == 1;
	}
}

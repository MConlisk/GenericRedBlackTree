using DataStructures.Nodes;

using System;

namespace DataStructures.Balancers.Helpers;

internal static class RedBlackFamilyHandling<TKey, TValue> where TKey : IComparable<TKey>
{
	internal static RedBlackNode<TKey, TValue> GetGrandparent(RedBlackNode<TKey, TValue> node)
	{
		return GetParent(GetParent(node));
	}

	internal static RedBlackNode<TKey, TValue> GetGreatGrandparent(RedBlackNode<TKey, TValue> node)
	{
		return GetParent(GetGrandparent(node));
	}

	internal static RedBlackNode<TKey, TValue> GetLeftChild(RedBlackNode<TKey, TValue> node)
	{
		return node?.Nodes[1] as RedBlackNode<TKey, TValue>; // Nodes[1] = Left Node
	}

	internal static RedBlackNode<TKey, TValue> GetParent(RedBlackNode<TKey, TValue> node)
	{
		return node?.Nodes[0] as RedBlackNode<TKey, TValue>; // Nodes[0] = Parent Node
	}

	internal static RedBlackNode<TKey, TValue> GetRightChild(RedBlackNode<TKey, TValue> node)
	{
		return node?.Nodes[2] as RedBlackNode<TKey, TValue>; // Nodes[2] = Right Node
	}

	internal static RedBlackNode<TKey, TValue> GetSibling(RedBlackNode<TKey, TValue> parent, bool isLeftChild)
	{
		if (isLeftChild)
		{
			return parent.Nodes[2] as RedBlackNode<TKey, TValue>; // Nodes[2] = Right Node
		}
		else
		{
			return parent.Nodes[1] as RedBlackNode<TKey, TValue>; // Nodes[1] = Left Node
		}
	}

	internal static RedBlackNode<TKey, TValue> GetUncle(RedBlackNode<TKey, TValue> parent, RedBlackNode<TKey, TValue> grandparent)
	{
		return IsLeftChild(parent, grandparent) ? grandparent.Nodes[2] as RedBlackNode<TKey, TValue> : grandparent.Nodes[1] as RedBlackNode<TKey, TValue>; // Nodes[1] = Left Node, Nodes[2] = Right Node
	}

	internal static bool HandleBlackSiblingLeftNephewRedCase(ref RedBlackNode<TKey, TValue> parent, bool isLeftChild)
	{
		RedBlackNode<TKey, TValue> sibling = GetSibling(GetGrandparent(parent), isLeftChild);
		RedBlackNode<TKey, TValue> leftNephew = (RedBlackNode<TKey, TValue>)sibling?.Nodes[1]; // Nodes[1] = Left Node

		if (sibling != null && !sibling.IsRed && leftNephew != null && leftNephew.IsRed)
		{
			sibling.IsRed = parent.IsRed;
			parent.IsRed = false;
			leftNephew.IsRed = false;
			RedBlackRotations<TKey, TValue>.PerformRotation(leftNephew, sibling, parent);

			if (isLeftChild)
			{
				parent.Nodes[2] = sibling; // Nodes[2] = Right Node
			}
			else
			{
				parent.Nodes[1] = sibling; // Nodes[1] = Left Node
			}

			return true;
		}

		return false;
	}

	internal static bool HandleBlackSiblingRightNephewRedCase(RedBlackNode<TKey, TValue> sibling, RedBlackNode<TKey, TValue> rightNephew, RedBlackNode<TKey, TValue> parent, bool isLeftChild)
	{
		if (sibling != null && !sibling.IsRed && rightNephew != null && rightNephew.IsRed)
		{
			sibling.IsRed = parent.IsRed;
			parent.IsRed = false;
			rightNephew.IsRed = false;

			if (isLeftChild)
			{
				RedBlackRotations<TKey, TValue>.RotateRight(rightNephew, parent);
			}
			else
			{
				RedBlackRotations<TKey, TValue>.RotateLeft(rightNephew, GetGrandparent(parent));
			}

			return true;
		}

		return false;
	}

	internal static bool IsLeftChild(RedBlackNode<TKey, TValue> node, RedBlackNode<TKey, TValue> parent)
	{
		return parent != null && parent.Nodes[1] == node; // Nodes[1] = Left Node
	}
}
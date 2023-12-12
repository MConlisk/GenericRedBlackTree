using DataStructures.Interfaces;
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
		return node?.Nodes["Left"] as RedBlackNode<TKey, TValue>;
	}

	internal static RedBlackNode<TKey, TValue> GetParent(RedBlackNode<TKey, TValue> node)
	{
		return node?.Nodes["Parent"] as RedBlackNode<TKey, TValue>;
	}

	internal static RedBlackNode<TKey, TValue> GetRightChild(RedBlackNode<TKey, TValue> node)
	{
		return node?.Nodes["Right"] as RedBlackNode<TKey, TValue>;
	}

	internal static RedBlackNode<TKey, TValue> GetSibling(RedBlackNode<TKey, TValue> parentNode, bool isLeftChild)
	{
		if (isLeftChild)
		{
			return parentNode.Nodes["Right"] as RedBlackNode<TKey, TValue>;
		}
		else
		{
			return parentNode.Nodes["Left"] as RedBlackNode<TKey, TValue>;
		}
	}

	internal static RedBlackNode<TKey, TValue> GetUncle(RedBlackNode<TKey, TValue> parentNode, RedBlackNode<TKey, TValue> grandparent)
	{
		return IsLeftChild(parentNode, grandparent) ? grandparent.Nodes["Right"] as RedBlackNode<TKey, TValue> : grandparent.Nodes["Left"] as RedBlackNode<TKey, TValue>; // Nodes[1] = Left Nodes, Nodes[2] = Right Nodes
	}

	internal static bool HandleBlackSiblingLeftNephewRedCase(ref RedBlackNode<TKey, TValue> parentNode, bool isLeftChild)
	{
		RedBlackNode<TKey, TValue> sibling = GetSibling(GetGrandparent(parentNode), isLeftChild);
		RedBlackNode<TKey, TValue> leftNephew = (RedBlackNode<TKey, TValue>)sibling?.Nodes["Left"];

		if (sibling is not null && !sibling.IsRed && leftNephew is not null && leftNephew.IsRed)
		{
			sibling.IsRed = parentNode.IsRed;
			parentNode.IsRed = false;
			leftNephew.IsRed = false;
			RedBlackRotations<TKey, TValue>.PerformRotation(leftNephew, sibling, parentNode);

			if (isLeftChild)
			{
				parentNode.Nodes["Right"] = sibling;
			}
			else
			{
				parentNode.Nodes["Left"] = sibling;
			}

			return true;
		}

		return false;
	}

	internal static bool HandleBlackSiblingRightNephewRedCase(RedBlackNode<TKey, TValue> sibling, RedBlackNode<TKey, TValue> rightNephew, RedBlackNode<TKey, TValue> parentNode, bool isLeftChild)
	{
		if (sibling is not null && !sibling.IsRed && rightNephew is not null && rightNephew.IsRed)
		{
			sibling.IsRed = parentNode.IsRed;
			parentNode.IsRed = false;
			rightNephew.IsRed = false;

			if (isLeftChild)
			{
				RedBlackRotations<TKey, TValue>.RotateRight(rightNephew, parentNode);
			}
			else
			{
				RedBlackRotations<TKey, TValue>.RotateLeft(rightNephew, GetGrandparent(parentNode));
			}

			return true;
		}

		return false;
	}

	internal static bool IsLeftChild(INode<TKey, TValue> node, RedBlackNode<TKey, TValue> parentNode)
	{
		return parentNode is not null && parentNode.Nodes["Left"] == node;
	}
}

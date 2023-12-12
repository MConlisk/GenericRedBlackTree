using DataStructures.Nodes;

using System;

namespace DataStructures.Balancers.Helpers;

internal static class RedBlackRotations<TKey, TValue> where TKey : IComparable<TKey>
{

	internal static void PerformRotation(RedBlackNode<TKey, TValue> pivot, RedBlackNode<TKey, TValue> parentNode, RedBlackNode<TKey, TValue> grandparent)
	{
		bool isLeftChild = RedBlackFamilyHandling<TKey, TValue>.IsLeftChild(pivot, parentNode);

		// Right Rotation
		if (isLeftChild)
		{
			RotateRight(pivot, grandparent);
		}
		// Left Rotation
		else
		{
			RotateLeft(pivot, grandparent);
		}

		pivot.IsRed = false;
		grandparent.IsRed = true;
	}

	internal static void RotateLeft(RedBlackNode<TKey, TValue> pivot, RedBlackNode<TKey, TValue> grandparent)
	{
		grandparent.Nodes["Right"] = pivot.Nodes["Left"]; // Nodes[1] = Left Nodes, Nodes[2] = Right Nodes
		pivot.Nodes["Left"] = grandparent; // Nodes[1] = Left Nodes

		pivot.IsRed = false;
		grandparent.IsRed = true;
	}

	internal static void RotateRight(RedBlackNode<TKey, TValue> pivot, RedBlackNode<TKey, TValue> grandparent)
	{
		grandparent.Nodes["Left"] = pivot.Nodes["Parent"]; // Nodes[1] = Left Nodes, Nodes[0] = Parent Nodes
		pivot.Nodes["Parent"] = grandparent; // Nodes[0] = Parent Nodes

		pivot.IsRed = false;
		grandparent.IsRed = true;
	}
}
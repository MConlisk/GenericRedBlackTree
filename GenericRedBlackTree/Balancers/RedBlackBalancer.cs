using DataStructures.Balancers.Helpers;
using DataStructures.Interfaces;
using DataStructures.Nodes;

using System;

namespace DataStructures.Balancers;

public class RedBlackBalancer<TKey, TValue> : IBalancer<TKey, TValue, RedBlackNode<TKey, TValue>> where TKey : IComparable<TKey>
{
	public bool AfterInsert(ref RedBlackNode<TKey, TValue> currentNode, RedBlackNode<TKey, TValue> nodeToInsert)
	{

		// Assuming nodeToInsert is always marked as red after insertion
		if (currentNode is null || !currentNode.IsRed)
		{
			// No need to rebalance if the current node is black or null
			return false;
		}

		RedBlackNode<TKey, TValue> parentNode = RedBlackFamilyHandling<TKey, TValue>.GetParent(currentNode);
		RedBlackNode<TKey, TValue> grandparent = RedBlackFamilyHandling<TKey, TValue>.GetParent(parentNode);

		if (parentNode is null)
		{
			// Parent is null, meaning currentNode is the root
			currentNode.IsRed = false; // Ensure the root is black
		}
		else if (parentNode.IsRed)
		{
			// Parent is red, indicating a violation
			RedBlackNode<TKey, TValue> uncle = RedBlackFamilyHandling<TKey, TValue>.GetUncle(parentNode, grandparent);

			if (uncle != null && uncle.IsRed)
			{
				// Case 1: Parent and Uncle are red
				parentNode.IsRed = false;
				uncle.IsRed = false;
				grandparent.IsRed = true;
				AfterInsert(ref grandparent, nodeToInsert);

				return true;
			}
			else
			{
				// Cases 2 and 3: Parent is red, but Uncle is black or null
				if (RedBlackFamilyHandling<TKey, TValue>.IsLeftChild(parentNode, grandparent) != RedBlackFamilyHandling<TKey, TValue>.IsLeftChild(currentNode, parentNode))
				{
					// Case 2: Nodes and parentNode are not on the same side
					RedBlackRotations<TKey, TValue>.PerformRotation(currentNode, parentNode, grandparent);
					parentNode = (RedBlackNode<TKey, TValue>)currentNode.Nodes["Parent"]; // Update parentNode after rotation, Nodes[0] = Parent Nodes
				}

				// Case 3: Nodes and parentNode are on the same side
				grandparent.IsRed = true;
				parentNode.IsRed = false;
				RedBlackRotations<TKey, TValue>.PerformRotation(parentNode, grandparent, RedBlackFamilyHandling<TKey, TValue>.GetGreatGrandparent(grandparent));

				return true;
			}
		}

		// Ensure the root is black
		if (RedBlackFamilyHandling<TKey, TValue>.GetGreatGrandparent(grandparent) == null)
		{
			currentNode.IsRed = false;
		}

		return false;
	}

	public bool AfterRemoval(ref RedBlackNode<TKey, TValue> currentNode, TKey removedKey)
	{
		if (currentNode is null)
		{
			return false;
		}

		var replacementNode = DetermineReplacementNode(FindNodeToRemove(currentNode, removedKey));

		if (replacementNode is not null && replacementNode.IsRed)
		{
			replacementNode.IsRed = false;
			return true;
		}

		RedBlackNode<TKey, TValue> parentNode = RedBlackFamilyHandling<TKey, TValue>.GetParent(currentNode);
		bool isLeftChild = RedBlackFamilyHandling<TKey, TValue>.IsLeftChild(currentNode, parentNode);

		if (parentNode is null)
		{
			return false;
		}

		RedBlackNode<TKey, TValue> sibling = RedBlackFamilyHandling<TKey, TValue>.GetSibling(parentNode, isLeftChild);
		RedBlackNode<TKey, TValue> leftNephew = RedBlackFamilyHandling<TKey, TValue>.GetLeftChild(sibling);
		RedBlackNode<TKey, TValue> rightNephew = RedBlackFamilyHandling<TKey, TValue>.GetRightChild(sibling);

		if (HandleRedSiblingCase(parentNode, sibling))
		{
			return true;
		}

		if (HandleBlackSiblingCase(parentNode, sibling, leftNephew, rightNephew))
		{
			return true;
		}

		RedBlackFamilyHandling<TKey, TValue>.HandleBlackSiblingLeftNephewRedCase(ref parentNode, isLeftChild);
		RedBlackFamilyHandling<TKey, TValue>.HandleBlackSiblingRightNephewRedCase(sibling, rightNephew, parentNode, isLeftChild);

		return false;
	}

	private bool HandleBlackSiblingCase(RedBlackNode<TKey, TValue> parentNode, RedBlackNode<TKey, TValue> sibling, RedBlackNode<TKey, TValue> leftNephew, RedBlackNode<TKey, TValue> rightNephew)
	{
		if (sibling is not null && !sibling.IsRed && (leftNephew is null || !leftNephew.IsRed) && (rightNephew is null || !rightNephew.IsRed))
		{
			sibling.IsRed = true;
			return AfterRemoval(ref parentNode, default);
		}

		if (parentNode.IsRed && (sibling is null || !sibling.IsRed) && (leftNephew is null || !leftNephew.IsRed) && (rightNephew is null || !rightNephew.IsRed))
		{
			parentNode.IsRed = false;
			if (sibling is not null) sibling.IsRed = true;
			return true;
		}

		return false;
	}
	private static RedBlackNode<TKey, TValue> FindNodeToRemove(RedBlackNode<TKey, TValue> currentNode, TKey removedKey)
	{
		// Perform a search operation to find the node with the specified key
		while (currentNode is not null)
		{
			int comparisonResult = removedKey.CompareTo(currentNode.Key);

			if (comparisonResult is 0)
			{
				// Nodes with the specified key found
				return currentNode;
			}
			else if (comparisonResult < 0)
			{
				currentNode = (RedBlackNode<TKey, TValue>)currentNode.Nodes["Left"]; // Left child
			}
			else
			{
				currentNode = (RedBlackNode<TKey, TValue>)currentNode.Nodes["Right"]; // Right child
			}
		}

		// Nodes with the specified key not found
		return null;
	}

	private static RedBlackNode<TKey, TValue> DetermineReplacementNode(RedBlackNode<TKey, TValue> nodeToRemove)
	{
		if (nodeToRemove.Nodes["Left"] is not null && nodeToRemove.Nodes["Right"] != null)
		{
			// Nodes has two children, find the in-order successor
			return FindSuccessor((RedBlackNode<TKey, TValue>)nodeToRemove.Nodes["Right"]);
		}
		else
		{
			// Nodes has at most one child, return the non-null child
			return (RedBlackNode<TKey, TValue>)(nodeToRemove.Nodes["Left"] ?? nodeToRemove.Nodes["Right"]);
		}
	}

	private static RedBlackNode<TKey, TValue> FindSuccessor(RedBlackNode<TKey, TValue> node)
	{
		// Find the leftmost node in the right subtree
		while (node.Nodes["Left"] is not null)
		{
			node = (RedBlackNode<TKey, TValue>)node.Nodes["Left"];
		}
		return node;
	}

	private static bool HandleRedSiblingCase(RedBlackNode<TKey, TValue> parentNode, RedBlackNode<TKey, TValue> sibling)
	{
		if (sibling is not null && sibling.IsRed)
		{
			parentNode.IsRed = true;
			sibling.IsRed = false;

			if (RedBlackFamilyHandling<TKey, TValue>.IsLeftChild(sibling, parentNode))
			{
				RedBlackRotations<TKey, TValue>.RotateRight(sibling, RedBlackFamilyHandling<TKey, TValue>.GetGrandparent(parentNode));
			}
			else
			{
				RedBlackRotations<TKey, TValue>.RotateLeft(sibling, RedBlackFamilyHandling<TKey, TValue>.GetGrandparent(parentNode));
			}

			return true;
		}

		return false;
	}
}

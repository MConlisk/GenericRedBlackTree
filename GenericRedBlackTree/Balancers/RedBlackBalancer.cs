using DataStructures.Balancers.Helpers;
using DataStructures.Interfaces;
using DataStructures.Nodes;

using System;

namespace DataStructures.Balancers;

public class RedBlackBalancer<TKey, TValue> : IBalancer<TKey, TValue, RedBlackNode<TKey, TValue>> where TKey : IComparable<TKey>
{
	public bool AfterInsert(ref RedBlackNode<TKey, TValue> startNode, RedBlackNode<TKey, TValue> newNode)
	{

		// Assuming newNode is always marked as red after insertion
		if (startNode == null || !startNode.IsRed)
		{
			// No need to rebalance if the current node is black or null
			return false;
		}

		RedBlackNode<TKey, TValue> parent = RedBlackFamilyHandling<TKey, TValue>.GetParent(startNode);
		RedBlackNode<TKey, TValue> grandparent = RedBlackFamilyHandling<TKey, TValue>.GetParent(parent);

		if (parent == null)
		{
			// Parent is null, meaning startNode is the root
			startNode.IsRed = false; // Ensure the root is black
		}
		else if (parent.IsRed)
		{
			// Parent is red, indicating a violation
			RedBlackNode<TKey, TValue> uncle = RedBlackFamilyHandling<TKey, TValue>.GetUncle(parent, grandparent);

			if (uncle != null && uncle.IsRed)
			{
				// Case 1: Parent and Uncle are red
				parent.IsRed = false;
				uncle.IsRed = false;
				grandparent.IsRed = true;
				AfterInsert(ref grandparent, newNode);

				return true;
			}
			else
			{
				// Cases 2 and 3: Parent is red, but Uncle is black or null
				if (RedBlackFamilyHandling<TKey, TValue>.IsLeftChild(parent, grandparent) != RedBlackFamilyHandling<TKey, TValue>.IsLeftChild(startNode, parent))
				{
					RedBlackRotations<TKey, TValue>.
										// Case 2: Node and parent are not on the same side
										PerformRotation(startNode, parent, grandparent);
					parent = (RedBlackNode<TKey, TValue>)startNode.Nodes[0]; // Update parent after rotation, Nodes[0] = Parent Node
				}

				// Case 3: Node and parent are on the same side
				grandparent.IsRed = true;
				parent.IsRed = false;
				RedBlackRotations<TKey, TValue>.PerformRotation(parent, grandparent, RedBlackFamilyHandling<TKey, TValue>.GetGreatGrandparent(grandparent));

				return true;
			}
		}

		// Ensure the root is black
		if (RedBlackFamilyHandling<TKey, TValue>.GetGreatGrandparent(grandparent) == null)
		{
			startNode.IsRed = false;
		}

		return false;
	}

	public bool AfterRemoval(ref RedBlackNode<TKey, TValue> startNode, TKey removedKey)
	{
		if (startNode == null)
		{
			return false;
		}

		var replacementNode = DetermineReplacementNode(FindNodeToRemove(startNode, removedKey));

		if (replacementNode != null && replacementNode.IsRed)
		{
			replacementNode.IsRed = false;
			return true;
		}

		RedBlackNode<TKey, TValue> parent = RedBlackFamilyHandling<TKey, TValue>.GetParent(startNode);
		bool isLeftChild = RedBlackFamilyHandling<TKey, TValue>.IsLeftChild(startNode, parent);

		if (parent == null)
		{
			return false;
		}

		RedBlackNode<TKey, TValue> sibling = RedBlackFamilyHandling<TKey, TValue>.GetSibling(parent, isLeftChild);
		RedBlackNode<TKey, TValue> leftNephew = RedBlackFamilyHandling<TKey, TValue>.GetLeftChild(sibling);
		RedBlackNode<TKey, TValue> rightNephew = RedBlackFamilyHandling<TKey, TValue>.GetRightChild(sibling);

		if (HandleRedSiblingCase(parent, sibling))
		{
			return true;
		}

		if (HandleBlackSiblingCase(parent, sibling, leftNephew, rightNephew))
		{
			return true;
		}

		RedBlackFamilyHandling<TKey, TValue>.
				HandleBlackSiblingLeftNephewRedCase(ref parent, isLeftChild);

		RedBlackFamilyHandling<TKey, TValue>.
				HandleBlackSiblingRightNephewRedCase(sibling, rightNephew, parent, isLeftChild);

		return false;
	}

	private static RedBlackNode<TKey, TValue> FindNodeToRemove(RedBlackNode<TKey, TValue> startNode, TKey removedKey)
	{
		// Perform a search operation to find the node with the specified key
		while (startNode != null)
		{
			int comparisonResult = removedKey.CompareTo(startNode.Key);

			if (comparisonResult == 0)
			{
				// Node with the specified key found
				return startNode;
			}
			else if (comparisonResult < 0)
			{
				startNode = (RedBlackNode<TKey, TValue>)startNode.Nodes[1]; // Left child
			}
			else
			{
				startNode = (RedBlackNode<TKey, TValue>)startNode.Nodes[2]; // Right child
			}
		}

		// Node with the specified key not found
		return null;
	}

	private static RedBlackNode<TKey, TValue> DetermineReplacementNode(RedBlackNode<TKey, TValue> nodeToRemove)
	{
		if (nodeToRemove.Nodes[1] != null && nodeToRemove.Nodes[2] != null)
		{
			// Node has two children, find the in-order successor
			return FindSuccessor((RedBlackNode<TKey, TValue>)nodeToRemove.Nodes[2]);
		}
		else
		{
			// Node has at most one child, return the non-null child
			return (RedBlackNode<TKey, TValue>)(nodeToRemove.Nodes[1] ?? nodeToRemove.Nodes[2]);
		}
	}

	private static RedBlackNode<TKey, TValue> FindSuccessor(RedBlackNode<TKey, TValue> node)
	{
		// Find the leftmost node in the right subtree
		while (node.Nodes[1] != null)
		{
			node = (RedBlackNode<TKey, TValue>)node.Nodes[1];
		}
		return node;
	}

	private static bool HandleRedSiblingCase(RedBlackNode<TKey, TValue> parent, RedBlackNode<TKey, TValue> sibling)
	{
		if (sibling != null && sibling.IsRed)
		{
			parent.IsRed = true;
			sibling.IsRed = false;

			if (RedBlackFamilyHandling<TKey, TValue>.IsLeftChild(sibling, parent))
			{
				RedBlackRotations<TKey, TValue>.RotateRight(sibling, RedBlackFamilyHandling<TKey, TValue>.GetGrandparent(parent));
			}
			else
			{
				RedBlackRotations<TKey, TValue>.RotateLeft(sibling, RedBlackFamilyHandling<TKey, TValue>.GetGrandparent(parent));
			}

			return true;
		}

		return false;
	}

	private bool HandleBlackSiblingCase(RedBlackNode<TKey, TValue> parent, RedBlackNode<TKey, TValue> sibling, RedBlackNode<TKey, TValue> leftNephew, RedBlackNode<TKey, TValue> rightNephew)
	{
		if (sibling != null && !sibling.IsRed && (leftNephew == null || !leftNephew.IsRed) && (rightNephew == null || !rightNephew.IsRed))
		{
			sibling.IsRed = true;
			 return AfterRemoval(ref parent, default);
		}

		if (parent.IsRed && (sibling == null || !sibling.IsRed) && (leftNephew == null || !leftNephew.IsRed) && (rightNephew == null || !rightNephew.IsRed))
		{
			parent.IsRed = false;
			if (sibling != null) sibling.IsRed = true;
			return true;
		}

		return false;
	}
}
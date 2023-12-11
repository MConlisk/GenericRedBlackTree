using DataStructures.Interfaces;
using DataStructures.Nodes;

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace DataStructures.Traversers;

// Example Traversal Component implementation
public class InOrderRedBlackTraverser<TKey, TValue> : ITraverser<TKey, TValue, RedBlackNode<TKey, TValue>> where TKey : IComparable<TKey>
{
	public RedBlackNode<TKey, TValue> KeyTraversal(RedBlackNode<TKey, TValue> startNode, TKey key)
	{
		while (startNode != null)
		{
			KeyTraversal((RedBlackNode<TKey, TValue>)startNode.Nodes[1], key);
			if (startNode.Key.Equals(key))
			{
				return startNode;
			}
			KeyTraversal((RedBlackNode<TKey, TValue>)startNode.Nodes[2], key);
		}
		return default;
	}

	public RedBlackNode<TKey, TValue> ValueTraversal(RedBlackNode<TKey, TValue> startNode, TValue value)
	{
		while (startNode != null)
		{
			ValueTraversal((RedBlackNode<TKey, TValue>)startNode.Nodes[1], value);
			if (startNode.Value.Equals(value))
			{
				return startNode;
			}
			ValueTraversal((RedBlackNode<TKey, TValue>)startNode.Nodes[2], value);
		}
		return default;
	}

	public bool InsertNew(RedBlackNode<TKey, TValue> startNode, RedBlackNode<TKey, TValue> newNode)
	{
		RedBlackNode<TKey, TValue> parent = null;

		while (startNode != null)
		{
			parent = startNode;

			if (newNode.Key.CompareTo(startNode.Key) < 0)
			{
				startNode = (RedBlackNode<TKey, TValue>)startNode.Nodes[1]; // Left left
			}
			else
			{
				startNode = (RedBlackNode<TKey, TValue>)startNode.Nodes[2]; // Right child
			}
		}

		newNode.Nodes[0] = parent; // Nodes[0] is the parent node

		if (parent == null)
		{
			parent.Nodes[0] = newNode; // set startNode to newNode
			return true;
		}

		parent.Nodes ??= new RedBlackNode<TKey, TValue>[3]; // initialize Children

		if (newNode.Key.CompareTo(parent.Key) < 0)
		{
			parent.Nodes[1] = newNode; // Set Left child to newNode
			return true;
		}
		else
		{
			parent.Nodes[2] = newNode; // set right child to newNode
			return true;
		}

	}

	public IEnumerable<KeyValuePair<TKey, TValue>> GetAll(RedBlackNode<TKey, TValue> startNode)
	{
		while (startNode != null)
		{
			if (startNode.Nodes[2] != null) GetAll((RedBlackNode<TKey, TValue>)startNode.Nodes[2]);
			yield return new(startNode.Key, startNode.Value);
			if (startNode.Nodes[3] != null) GetAll((RedBlackNode<TKey, TValue>)startNode.Nodes[3]);
		}
	}

}

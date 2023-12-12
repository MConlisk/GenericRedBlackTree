using DataStructures.Interfaces;
using DataStructures.Nodes;

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace DataStructures.Traversers;

// Example Traversal Component implementation
public class RedBlackInOrderTraverser<TKey, TValue> : ITraverser<TKey, TValue, RedBlackNode<TKey, TValue>> where TKey : IComparable<TKey>
{
	public bool Insert(RedBlackNode<TKey, TValue> startNode, RedBlackNode<TKey, TValue> newNode)
	{
		RedBlackNode<TKey, TValue> parent = null;

		while (startNode != null)
		{
			parent = startNode;

			if (newNode.Key.CompareTo(startNode.Key) < 0)
			{
				startNode = (RedBlackNode<TKey, TValue>)startNode.Nodes[1]; // Nodes[1] Left child
			}
			else
			{
				startNode = (RedBlackNode<TKey, TValue>)startNode.Nodes[2];  // Nodes[2] Right child
			}
		}

		newNode.Nodes[0] = parent; // Nodes[0] parent node

		if (parent == null)
		{
			parent.Nodes[0] = newNode; // Nodes[0] parent node
			return true;
		}

		parent.Nodes ??= new RedBlackNode<TKey, TValue>[parent.MaxSubNodes]; // initialize Nodes

		if (newNode.Key.CompareTo(parent.Key) < 0)
		{
			parent.Nodes[1] = newNode; // Nodes[1] Left child
			return true;
		}
		else
		{
			parent.Nodes[2] = newNode;  // Nodes[2] Right child
			return true;
		}

	}

	public IEnumerable<KeyValuePair<TKey, TValue>> GetAll(RedBlackNode<TKey, TValue> startNode)
	{
		while (startNode != null)
		{
			if (startNode.Nodes[1] != null) GetAll((RedBlackNode<TKey, TValue>)startNode.Nodes[1]); // Nodes[1] Left child
			yield return new(startNode.Key, startNode.Value);
			if (startNode.Nodes[2] != null) GetAll((RedBlackNode<TKey, TValue>)startNode.Nodes[2]); // Nodes[2] Right child
		}
	}

	public bool Remove(RedBlackNode<TKey, TValue> startNode, TKey key)
	{
		throw new NotImplementedException();
	}

	public bool Update(RedBlackNode<TKey, TValue> startNode, TKey key, TValue value)
	{
		throw new NotImplementedException();
	}

	public TValue GetValue(RedBlackNode<TKey, TValue> startNode, TKey key)
	{
		throw new NotImplementedException();
	}

	public IEnumerable<KeyValuePair<TKey, TValue>> Search(RedBlackNode<TKey, TValue> startNode, Func<TKey, bool> condition)
	{
		while (startNode != null)
		{
			if (condition(startNode.Key))
			{
				yield return new KeyValuePair<TKey, TValue>(startNode.Key, startNode.Value);
			}
			else
			{
				foreach (var item in Search((RedBlackNode<TKey, TValue>)startNode.Nodes[1], condition)) // Nodes[1] Left child
				{
					yield return item;
				}
				foreach (var item in Search((RedBlackNode<TKey, TValue>)startNode.Nodes[2], condition)) // Nodes[2] Right child
				{
					yield return item;
				}
			}
		}
	}
}

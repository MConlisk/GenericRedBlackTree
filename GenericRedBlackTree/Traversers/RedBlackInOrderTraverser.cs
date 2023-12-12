using DataStructures.Interfaces;
using DataStructures.Nodes;

using Factories;

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace DataStructures.Traversers;

public class RedBlackInOrderTraverser<TKey, TValue> : ITraverser<TKey, TValue, RedBlackNode<TKey, TValue>> where TKey : IComparable<TKey>
{
	public bool Insert(ref RedBlackNode<TKey, TValue> currentNode, RedBlackNode<TKey, TValue> nodeToInsert)
	{
		if (currentNode is null)
		{
			return false;
		}

		RedBlackNode<TKey, TValue> parentNode = null;

		while (currentNode != null)
		{
			parentNode = currentNode;
			if (nodeToInsert.Key.CompareTo(currentNode.Key) < 0)
			{
				currentNode = (RedBlackNode<TKey, TValue>)currentNode.Nodes["Left"];
			}
			else
			{
				currentNode = (RedBlackNode<TKey, TValue>)currentNode.Nodes["Right"];
			}
		}

		nodeToInsert.Nodes["Parent"] = parentNode;

		if (parentNode is null)
		{
			currentNode = nodeToInsert;
			return true;
		}

		if (nodeToInsert.Key.CompareTo(parentNode.Key) < 0)
		{
			parentNode.Nodes["Left"] = nodeToInsert;
			return true;
		}
		else
		{
			parentNode.Nodes["Right"] = nodeToInsert;
			return true;
		}
	}

	public IEnumerable<KeyValuePair<TKey, TValue>> GetAll(RedBlackNode<TKey, TValue> currentNode)
	{
		if (currentNode is not null)
		{
			if (currentNode.Nodes["Left"] is not null)
			{
				foreach (var item in GetAll((RedBlackNode<TKey, TValue>)currentNode.Nodes["Left"]))
				{
					yield return item;
				}
			}

			yield return new KeyValuePair<TKey, TValue>(currentNode.Key, currentNode.Value);

			if (currentNode.Nodes["Right"] is not null)
			{
				foreach (var item in GetAll((RedBlackNode<TKey, TValue>)currentNode.Nodes["Right"]))
				{
					yield return item;
				}
			}
		}
	}

	public bool Remove(ref RedBlackNode<TKey, TValue> currentNode, TKey key)
	{
		if (currentNode is null)
		{
			return false;
		}

		if (key.CompareTo(currentNode.Key) < 0)
		{
			var node = (RedBlackNode<TKey, TValue>)currentNode.Nodes["Left"]; 
			var result = Remove(ref node, key);
			currentNode.Nodes["Left"] = node; // Assign the updated left child to the parent node
			return result;
		}
		else if (key.CompareTo(currentNode.Key) > 0)
		{
			var node = (RedBlackNode<TKey, TValue>)currentNode.Nodes["Right"]; 
			var result = Remove(ref node, key);
			currentNode.Nodes["Right"] = node; // Assign the updated right child to the parent node
			return result;
		}
		else
		{
			PoolFactory.Recycle(currentNode);
			currentNode = null;
			return true;
		}
	}

	public bool Update(ref RedBlackNode<TKey, TValue> currentNode, TKey key, TValue value)
	{
		if (currentNode is null)
		{
			return false;
		}
		return true;
	}

	public TValue GetValue(RedBlackNode<TKey, TValue> currentNode, TKey key)
	{
		if (currentNode is null)
		{
			return default;
		}
		return default;
	}

	public IEnumerable<KeyValuePair<TKey, TValue>> Search(RedBlackNode<TKey, TValue> currentNode, Func<TKey, bool> condition)
	{
		if (currentNode is not null)
		{
			if (condition(currentNode.Key))
			{
				yield return new KeyValuePair<TKey, TValue>(currentNode.Key, currentNode.Value);
			}
			else
			{
				foreach (var item in Search((RedBlackNode<TKey, TValue>)currentNode.Nodes["Left"], condition)) 
				{
					yield return item;
				}
				foreach (var item in Search((RedBlackNode<TKey, TValue>)currentNode.Nodes["Right"], condition)) 
				{
					yield return item;
				}
			}
		}
	}
}

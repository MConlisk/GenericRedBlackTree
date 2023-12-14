using DataStructures.Balancers;
using DataStructures.Exceptions;
using DataStructures.Interfaces;
using DataStructures.Nodes;

using Factories;
using Factories.Interfaces;

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace DataStructures.Traversers;

public class RedBlackInOrderTraverser<TKey, TValue> : ITraverser<TKey, TValue, RedBlackNode<TKey, TValue>> where TKey : IComparable<TKey>
{
	private RedBlackBalancer<TKey, TValue> _balancer;
	private Func<TKey, TValue, RedBlackNode<TKey, TValue>> _nodeFactory;

	public IBalancer<TKey, TValue, RedBlackNode<TKey, TValue>> Balancer 
	{
		get => _balancer; 
		set => _balancer = value as RedBlackBalancer<TKey, TValue>; 
	}
	
	public RedBlackInOrderTraverser(RedBlackBalancer<TKey, TValue> balancer)
	{
		_nodeFactory = (key, value) => PoolFactory.Create(() => new RedBlackNode<TKey, TValue>(key, value));
		_balancer = balancer;
	}

	public void SetFactory(Func<TKey, TValue, RedBlackNode<TKey, TValue>> factory)
	{
		ArgumentNullException.ThrowIfNull(factory);

		_nodeFactory = factory;
	}

	public bool Insert(ref RedBlackNode<TKey, TValue> currentNode, RedBlackNode<TKey, TValue> nodeToInsert)
	{
		if (currentNode.IsNil)
		{
			currentNode = nodeToInsert;
			return true;
		}

		RedBlackNode<TKey, TValue> parentNode = _nodeFactory(default, default);

		while (!currentNode.IsNil)
		{
			parentNode = currentNode;

			if (nodeToInsert.Key.CompareTo(currentNode.Key) > 0)
			{
				if (currentNode.Nodes["Left"].IsEmpty)
				{
					currentNode.Nodes["Left"] = nodeToInsert;
					currentNode.Nodes["Left"].Nodes["Parent"] = currentNode;
					return true;
				}
				currentNode = (RedBlackNode<TKey, TValue>)currentNode.Nodes["Left"];
			}
			else
			{
				if (currentNode.Nodes["Right"].IsEmpty)
				{
					currentNode.Nodes["Right"] = nodeToInsert;
					currentNode.Nodes["Right"].Nodes["Parent"] = currentNode;
					return true;
				}
				currentNode = (RedBlackNode<TKey, TValue>)currentNode.Nodes["Right"];
			}
		}

		nodeToInsert.Nodes["Parent"] = parentNode;

		if (nodeToInsert.Key.CompareTo(parentNode.Key) < 0)
		{
			parentNode.Nodes["Left"] = nodeToInsert;
		}
		else
		{
			parentNode.Nodes["Right"] = nodeToInsert;
		}
		currentNode = parentNode;
		return true;
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

	public bool Remove(RedBlackNode<TKey, TValue> currentNode, TKey key)
	{
		if (currentNode is null)
		{
			return false;
		}

		if (key.CompareTo(currentNode.Key) < 0)
		{
			var node = (RedBlackNode<TKey, TValue>)currentNode.Nodes["Left"]; 
			var result = Remove(node, key);
			currentNode.Nodes["Left"] = node; // Assign the updated left child to the parent node
			return result;
		}
		else if (key.CompareTo(currentNode.Key) > 0)
		{
			var node = (RedBlackNode<TKey, TValue>)currentNode.Nodes["Right"]; 
			var result = Remove(node, key);
			currentNode.Nodes["Right"] = node; // Assign the updated right child to the parent node
			return result;
		}
		else
		{
			PoolFactory.Recycle(currentNode);
			return true;
		}
	}

	public bool Update(RedBlackNode<TKey, TValue> currentNode, TKey key, TValue value)
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

		if (key.CompareTo(currentNode.Key) < 0)
		{
			GetValue((RedBlackNode<TKey, TValue>)currentNode.Nodes["Left"], key);
			
		}
		else if (key.CompareTo(currentNode.Key) > 0)
		{
			GetValue((RedBlackNode<TKey, TValue>)currentNode.Nodes["Right"], key);
		}
		else
		{
			return currentNode.Value;
		}
		
		throw new KeyNotFoundException($"An attempt was made to return the Value of Key: {key} that failed to locate the Node holding the key.");
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

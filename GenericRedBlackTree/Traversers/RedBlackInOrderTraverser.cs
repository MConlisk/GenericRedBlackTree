using DataStructures.Balancers;
using DataStructures.Exceptions;
using DataStructures.Interfaces;
using DataStructures.Nodes;

using Factories;
using Factories.Interfaces;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DataStructures.Traversers
{
	public class RedBlackInOrderTraverser<TKey, TValue> : ITraverser<TKey, TValue, RedBlackNode<TKey, TValue>> where TKey : IComparable<TKey>
	{
		private RedBlackBalancer<TKey, TValue> _balancer;
		private Func<TKey, TValue, RedBlackNode<TKey, TValue>> _nodeFactory;

		/// <summary>
		/// Gets or sets the Red-Black balancer.
		/// </summary>
		public IBalancer<TKey, TValue, RedBlackNode<TKey, TValue>> Balancer
		{
			get => _balancer;
			set => _balancer = value as RedBlackBalancer<TKey, TValue>;
		}

		/// <summary>
		/// Initializes a new instance of the RedBlackInOrderTraverser class.
		/// </summary>
		/// <param name="balancer">The Red-Black balancer.</param>
		public RedBlackInOrderTraverser(RedBlackBalancer<TKey, TValue> balancer)
		{
			_nodeFactory = (key, value) => PoolFactory.Create(() => new RedBlackNode<TKey, TValue>(key, value));
			_balancer = balancer;
		}

		/// <summary>
		/// Sets the node factory for creating Red-Black nodes.
		/// </summary>
		/// <param name="factory">The node factory function.</param>
		public void SetFactory(Func<TKey, TValue, RedBlackNode<TKey, TValue>> factory)
		{
			ArgumentNullException.ThrowIfNull(factory);
			_nodeFactory = factory;
		}

		/// <summary>
		/// Inserts a Red-Black node into the tree.
		/// </summary>
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
					if (currentNode.Nodes["Left"] is null)
					{
						currentNode.Nodes["Left"] = nodeToInsert;
						nodeToInsert.Nodes["Parent"] = currentNode;
						return true;
					}
					currentNode = currentNode.Nodes["Left"];
				}
				else
				{
					if (currentNode.Nodes["Right"] is null)
					{
						currentNode.Nodes["Right"] = nodeToInsert;
						nodeToInsert.Nodes["Parent"] = currentNode;
						return true;
					}
					currentNode = currentNode.Nodes["Right"];
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
			return true;
		}

		/// <summary>
		/// Retrieves all key-value pairs in in-order traversal.
		/// </summary>
		public IEnumerable<KeyValuePair<TKey, TValue>> GetAll(RedBlackNode<TKey, TValue> rootNode)
		{
			ArgumentNullException.ThrowIfNull(rootNode);

			if (rootNode is not null)
			{
				if (rootNode.Nodes["Left"] is not null)
				{
					foreach (var item in GetAll(rootNode.Nodes["Left"]))
					{
						yield return item;
					}
				}

				yield return new KeyValuePair<TKey, TValue>(rootNode.Key, rootNode.Value);

				if (rootNode.Nodes["Right"] is not null)
				{
					foreach (var item in GetAll(rootNode.Nodes["Right"]))
					{
						yield return item;
					}
				}
			}
		}

		/// <summary>
		/// Removes a Red-Black node from the tree.
		/// </summary>
		public bool Remove(RedBlackNode<TKey, TValue> currentNode, TKey key)
		{
			if (currentNode is null)
			{
				return false;
			}

			if (key.CompareTo(currentNode.Key) < 0)
			{
				var node = currentNode.Nodes["Left"];
				var result = Remove(node, key);
				currentNode.Nodes["Left"] = node?.Nodes["Left"]; // Assign the updated left child to the parent node
				return result;
			}
			else if (key.CompareTo(currentNode.Key) > 0)
			{
				var node = currentNode.Nodes["Right"];
				var result = Remove(node, key);
				currentNode.Nodes["Right"] = node?.Nodes["Right"]; // Assign the updated right child to the parent node
				return result;
			}
			else
			{
				PoolFactory.Recycle(currentNode);
				return true;
			}
		}

		/// <summary>
		/// Updates the value of a Red-Black node.
		/// </summary>
		public bool Update(RedBlackNode<TKey, TValue> currentNode, TKey key, TValue value)
		{
			// Implementation based on your specific requirements.
			// Add your logic here if needed.
			if (currentNode is null)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Gets the value associated with a key in the Red-Black tree.
		/// </summary>
		public TValue GetValue(RedBlackNode<TKey, TValue> rootNode, TKey key)
		{
			ArgumentNullException.ThrowIfNull(rootNode);
			ConcurrentStack<RedBlackNode<TKey, TValue>> stack = new();

			stack.Push(rootNode);

			while (!stack.IsEmpty)
			{
				stack.TryPop(out RedBlackNode<TKey, TValue> currentNode);

				if (currentNode.Key.Equals(key)) return currentNode.Value;

				if (currentNode.Nodes != null)
				{
					if (currentNode.Nodes["Left"] != null) stack.Push(currentNode.Nodes["Left"]);

					if (currentNode.Nodes["Right"] != null) stack.Push(currentNode.Nodes["Right"]);
				}
			}
			throw new InvalidOperationException($"An attempt was made to return the Value of Key: {key} that failed to locate the Node holding the key.");
		}

		/// <summary>
		/// Searches for key-value pairs in the Red-Black tree based on a condition.
		/// </summary>
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
					foreach (var item in Search(currentNode.Nodes["Left"], condition))
					{
						yield return item;
					}
					foreach (var item in Search(currentNode.Nodes["Right"], condition))
					{
						yield return item;
					}
				}
			}
		}
	}
}

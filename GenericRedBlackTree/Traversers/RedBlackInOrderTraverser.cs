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
	public class RedBlackInOrderTraverser<TKey, TValue> : ITraverser<TKey, TValue, RedBlackNode<TKey, TValue>> 
	where TKey : IComparable<TKey>
	where TValue : IComparable<TValue>
	{
		private RedBlackBalancer<TKey, TValue> _balancer;

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
			_balancer = balancer;
		}

		/// <summary>
		/// Inserts a Red-Black node into the tree.
		/// </summary>
		public bool Insert(ref RBBaseNode<TKey, TValue> rootNode, RedBlackNode<TKey, TValue> nodeToInsert)
		{
			if (rootNode.IsNil)
			{
				rootNode = nodeToInsert;
				return true;
			}

			RBBaseNode<TKey, TValue> parentNode;

			Stack<RBBaseNode<TKey, TValue>> stack = new();
			stack.Push(rootNode);

			while (stack.TryPop(out RBBaseNode<TKey, TValue> currentNode))
			{
				parentNode = currentNode;

				if (currentNode.Left == default || currentNode.Right == default)
				{
					if (currentNode.Left == default)
					{
						currentNode.Left = nodeToInsert;
						nodeToInsert.Parent = parentNode;
						if (CheckBalanceIsNeeded(currentNode, parentNode))
						{
							_balancer.FixInsertion(ref rootNode, nodeToInsert);
						}
						return true;
					}

					if (currentNode.Right == default)
					{
						currentNode.Right = nodeToInsert;
						nodeToInsert.Parent = parentNode;
						if (CheckBalanceIsNeeded(currentNode, parentNode))
						{
							_balancer.FixInsertion(ref rootNode, nodeToInsert);
						}
						return true;
					}
				}
				else
				{
					currentNode.IsRed = false;
					stack.Push(currentNode.Left);
					stack.Push(currentNode.Right);
				}
			}

			return false;
		}

		private static bool CheckBalanceIsNeeded(RBBaseNode<TKey, TValue> currentNode, RBBaseNode<TKey, TValue> parentNode)
		{
			if (parentNode.IsRed && currentNode.IsRed) return true;
			return false;
		}

		/// <summary>
		/// Retrieves all key-value pairs in in-order traversal.
		/// </summary>
		public IEnumerable<KeyValuePair<TKey, TValue>> GetAll(RBBaseNode<TKey, TValue> rootNode)
		{
			ArgumentNullException.ThrowIfNull(rootNode);

			Stack<RBBaseNode<TKey, TValue>> stack = new();
			stack.Push(rootNode);

			while (stack.TryPop(out RBBaseNode<TKey, TValue> currentNode))
			{
				if (currentNode.Left != default)
				{
					stack.Push(currentNode.Left);
				}
				yield return new(currentNode.Key, currentNode.Value);
				if (currentNode.Right != default)
				{
					stack.Push(currentNode.Right);
				}
			}

		}

		public void MapTree(RBBaseNode<TKey, TValue> rootNode)
		{
			ArgumentNullException.ThrowIfNull(rootNode);

			Queue<RBBaseNode<TKey, TValue>> queue = new();
			queue.Enqueue(rootNode);

			while (queue.TryDequeue(out RBBaseNode<TKey, TValue> currentNode))
			{
				Console.WriteLine($"-------------------------");
				if (!currentNode.Key.Equals(default)) Console.WriteLine($"Node:{currentNode.Key}");

				if (currentNode.Value.Equals(default)) Console.WriteLine($"Has No Value!");
				if (!currentNode.Value.Equals(default)) Console.WriteLine($"Value:{currentNode.Value}");

				if (currentNode.IsRed) Console.WriteLine($"Is Red");
				if (!currentNode.IsRed) Console.WriteLine($"Is Black");

				if (currentNode.Parent == default) Console.WriteLine(value: $"Is the Root Node");
				if (currentNode.Parent != default) Console.WriteLine(value: $"Parent Node:{currentNode.Parent.Key}");


				if (currentNode.Left != default || currentNode.Right != default)
				{
					if (currentNode.Left != default)
					{
						if (!currentNode.Left.Key.Equals(default)) Console.WriteLine($"Left Child:{currentNode.Left.Key}");
						queue.Enqueue(currentNode.Left);
					}

					if (currentNode.Right != default)
					{
						if (!currentNode.Right.Key.Equals(default)) Console.WriteLine($"Right Child:{currentNode.Right.Key}");
						queue.Enqueue(currentNode.Right);
					}
				}
				else
				{
					Console.WriteLine(value: $"Is a leaf node");
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
				var node = currentNode.Left;
				var result = Remove(node, key);
				currentNode.Left = node?.Left; // Assign the updated left child to the parent node
				return result;
			}
			else if (key.CompareTo(currentNode.Key) > 0)
			{
				var node = currentNode.Right;
				var result = Remove(node, key);
				currentNode.Right = node?.Right; // Assign the updated right child to the parent node
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
			Stack<RedBlackNode<TKey, TValue>> stack = new();

			stack.Push(rootNode);

			while (stack.TryPop(out RedBlackNode<TKey, TValue> currentNode))
			{
				if (currentNode.Key.Equals(key)) return currentNode.Value;

				if (currentNode.Nodes != null)
				{
					if (currentNode.Left != null) stack.Push(currentNode.Left);

					if (currentNode.Right != null) stack.Push(currentNode.Right);
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
					foreach (var item in Search(currentNode.Left, condition))
					{
						yield return item;
					}
					foreach (var item in Search(currentNode.Right, condition))
					{
						yield return item;
					}
				}
			}
		}
	}
}

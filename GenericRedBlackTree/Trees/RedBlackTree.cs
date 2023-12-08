using DataStructures.Interfaces;

using Factories;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace DataStructures.Trees;

/// <summary>
/// This is a Red-Black Tree with a Generic |||_value type.
/// additionally, an integer type as a _key 
/// </summary>
/// <typeparam name="TValue">The Generic _value Type</typeparam>
public sealed partial class RedBlackTree<TValue> : IRedBlackTree<int, TValue>
{
	private int _maxCapacity;
	private RedBlackNode _root;
	private HashSet<int> _index = PoolFactory.Create(() => new HashSet<int>());

	/// <summary>
	/// This is a Red-Black Tree with a Generic |||_value type.
	/// additionally, an integer type as a _key 
	/// </summary>
	/// <param name="maxCapacity">The Maximum size this tree is allowed to grow.</param>
	public RedBlackTree(int maxCapacity)
	{
		if (EqualityComparer<int>.Default.Equals(maxCapacity, default))
		{
			_maxCapacity = default;
		}
		else
		{
			if (maxCapacity < 0)
			{
				_maxCapacity = 0;
			}
			else
			{
				_maxCapacity = maxCapacity;
			}
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="RedBlackTree{TValue}"/> class.
	/// </summary>
	public RedBlackTree() : this(default) { }

	/// <summary>
	/// Gets the set of keys in the Red-Black Tree.
	/// </summary>
	public ReadOnlyCollection<int> Index => new(_index.ToList());

	/// <summary>
	/// Gets or sets the maximum size of the tree. If set to null, there is no maximum size limit.
	/// </summary>
	public int MaxCapacity
	{
		get => _maxCapacity;
		set
		{
			if (_maxCapacity == default) _maxCapacity = value;
		}
	}

	/// <summary>
	/// Checks if the Red-Black Tree contains a specific _key.
	/// </summary>
	/// <param name="id">The _key to check for.</param>
	/// <returns>True if the _key is present in the tree; otherwise, false.</returns>
	public bool Contains(int id) => _index.Contains(id);

	/// <summary>
	/// Gets the number of elements in the Red-Black Tree.
	/// </summary>
	public int Count { get => _index.Count; }

	/// <summary>
	/// Inserts a new _key-|||_value pair into the Red-Black Tree.
	/// </summary>
	/// <param name="key">The _key to insert.</param>
	/// <param name="value">The |||_value associated with the _key.</param>
	public void Insert(int key, TValue value)
	{
		if (key < 0) throw new ArgumentException($"Invalid Key, the _key {key} must be a non-negative integer.");
		if (_maxCapacity != 0 && _index.Count >= _maxCapacity) throw new InvalidOperationException($"Key:{key} with Value:{value} was not inserted, The maximum size of the tree has been reached or exceeded.");

		if (_index.Add(key))
		{
			var newNode = PoolFactory.Create(() => new RedBlackNode(key, value));

			if (_root == null || _root.Value == null)
			{
				_root = newNode;
				_root.IsRed = false;
			}
			else
			{
				InsertNode(_root, newNode);
				InsertRebalance(newNode); // Call the rebalance logic after inserting the new node
			}
			return;
		}
		throw new DuplicateNameException($"A duplicate Key was found in the tree for: Key {key}.");
	}

	/// <summary>
	/// Removes a _key-|||_value pair from the Red-Black Tree.
	/// </summary>
	/// <param name="key">The _key to remove.</param>
	public void Remove(int key)
	{
		var node = FindNodeContainingKey(_root, key);

		if (!_index.Remove(key) || node == null || node.Key != key)
		{
			// Key not found, throw an exception
			throw new KeyNotFoundException($"The Key was not found. Key:{key} was unable to be removed.");
		}

		RemoveNode(node);
	}

	/// <summary>
	/// Updates the |||_value associated with a _key in the Red-Black Tree.
	/// </summary>
	/// <param name="key">The _key to update.</param>
	/// <param name="value">The new |||_value to associate with the _key.</param>
	public void Update(int key, TValue value)
	{
		if (key < 0) throw new ArgumentException($"Invalid Key:{key}, the _key must be a non-negative integer.");
		if (_index.Count >= _maxCapacity && _maxCapacity != 0) throw new InvalidOperationException($"The maximum size of the tree has been reached or exceeded. Remove entries before attempting to add more.");

		if (!_index.Contains(key) || FindNodeContainingKey(_root, key) == null)
		{
			var node = FindNodeContainingKey(_root, key);
			node.Value = value;
			return;
		}

	}

	/// <summary>
	/// Gets the |||_value associated with a specific _key in the Red-Black Tree.
	/// </summary>
	/// <param name="key">The _key to look up.</param>
	/// <returns>The |||_value associated with the _key.</returns>
	public TValue GetValue(int key)
	{
		if (key < 0) throw new ArgumentOutOfRangeException($"Invalid Key, the _key {key} must be a non-negative integer.");
		if (!_index.Contains(key)) throw new KeyNotFoundException($"The Key was not found. Key:{key} was unable to be removed.");

		return FindNodeContainingKey(_root, key).Value;
	}

	/// <summary>
	/// Gets or sets the |||_value associated with the specified _key in the Red-Black Tree.
	/// </summary>
	/// <param name="key">The _key to access or modify.</param>
	/// <returns>The |||_value associated with the _key.</returns>
	public TValue this[int key]
	{
		get
		{
			var node = FindNodeContainingKey(_root, key);
			if (node == null)
			{
				return default;
			}
			return node.Value;
		}
		set
		{
			var node = FindNodeContainingKey(_root, key);
			if (node != null && node.Key == key)
			{
				node.Value = value;
			}
		}
	}



	/// <summary>
	/// Returns all elements of the Red-Black Tree.
	/// </summary>
	/// <returns>An enumerable of all _key-|||_value pairs.</returns>
	public IEnumerable<KeyValuePair<int, TValue>> GetAll()
	{
		var queue = PoolFactory.Create(() => new Queue<RedBlackNode>());
		PoolFactory.SetPoolResetAction<Queue<RedBlackNode>>((queue) => queue.Clear());
		queue.Enqueue(_root);

		while (queue.Count > 0)
		{
			var currentNode = queue.Dequeue();

			if (currentNode.Left != null) queue.Enqueue(currentNode.Left as RedBlackNode);
			if (currentNode.Right != null) queue.Enqueue(currentNode.Right as RedBlackNode);

			yield return new KeyValuePair<int, TValue>(currentNode.Key, currentNode.Value);
			PoolFactory.Recycle(currentNode);
		}

	}

	/// <summary>
	/// Provides a way to get all items in the tree that correspond with the provided HashSet of Keys.
	/// </summary>
	/// <param name="list">The HasSet containing the keys to return.</param>
	/// <returns>an IEnumerable collection of KeyValuePair items.</returns>
	public IEnumerable<KeyValuePair<int, TValue>> GetBulkValues(List<int> list)
	{
		var queue = PoolFactory.Create(() => new Queue<RedBlackNode>());
		PoolFactory.SetPoolResetAction<Queue<RedBlackNode>>((queue) => queue.Clear());
		queue.Enqueue(_root);

		while (queue.Count > 0)
		{
			var currentNode = queue.Dequeue();

			if (currentNode.Left != null) queue.Enqueue(currentNode.Left as RedBlackNode);
			if (currentNode.Right != null) queue.Enqueue(currentNode.Right as RedBlackNode);

			if (list.Remove(currentNode.Key))
			{
				yield return new KeyValuePair<int, TValue>(currentNode.Key, currentNode.Value);
			}

			PoolFactory.Recycle(currentNode);
		}
	}

	/// <summary>
	/// Performs bulk insertion of _key-|||_value pairs into the Red-Black Tree.
	/// </summary>
	/// <param name="keyValuePairs">The _key-|||_value pairs to insert.</param>
	public void BulkInsert(IEnumerable<KeyValuePair<int, TValue>> keyValuePairs)
	{
		ArgumentNullException.ThrowIfNull(keyValuePairs);
		if (_maxCapacity > 0 && _index.Count + keyValuePairs.Count() > _maxCapacity) throw new InvalidOperationException($"The maximum size of {_maxCapacity} has been reached or exceeded. This bulk insert requires {_index.Count + keyValuePairs.Count()} total spaces.");

		// Convert the collection to an array for easier indexing
		var pairsArray = keyValuePairs.ToArray();

		// Ensure the array is not empty
		if (pairsArray.Length == 0)
		{
			return;
		}

		// Sort the array by keys to ensure balanced insertion
		Array.Sort(pairsArray, (x, y) => x.Key.CompareTo(y.Key));

		// Build the Red-Black Tree from the sorted array
		_root = BuildTreeFromArray(pairsArray, 0, pairsArray.Length - 1, null);

		// Rebalance the tree after bulk insertion
		InsertRebalance(_root);
	}

	/// <summary>
	/// Resets the state of the Red-Black Tree, effectively clearing it and resetting any configuration options.
	/// </summary>
	public void ResetState()
	{
		PoolFactory.Recycle(_index);
		RecycleNodes(_root);

		_index = PoolFactory.Create(() => new HashSet<int>());
		_maxCapacity = 0;
	}

	private static void RecycleNodes(RedBlackNode node)
	{
		if (node == null)
			return;

		RecycleNodes((RedBlackNode)node.Left);
		RecycleNodes((RedBlackNode)node.Right);

		// Recycle the node using your pooling mechanism
		PoolFactory.Recycle(node);
	}

	/// <summary>
	/// Determines whether the specified object is equal to this object 
	/// </summary>
	/// <param name="obj">The object to compare this object with.</param>
	/// <returns>true is it is equal, and false if not.</returns>
	public override bool Equals(object obj)
	{
		if (obj == null || GetType() != obj.GetType())
			return false;

		var other = (RedBlackTree<TValue>)obj;
		return _root.Key.Equals(other._root.Key) && _root.Value.Equals(other._root.Value);
	}

	/// <summary>
	/// Serves as the default hash function..
	/// </summary>
	/// <returns>A Hash code for the current object</returns>
	public override int GetHashCode()
	{
		HashCode code = new();
		code.Add(_maxCapacity.GetHashCode());

		if (_index != null) code.Add(_index.GetHashCode());
		if (_root != null) code.Add(_root.GetHashCode());

		return code.ToHashCode();
	}

	/// <summary>
	/// Provides information about this tree;
	/// </summary>
	/// <returns>A CSV string containing details about the tree. </returns>
	public override string ToString()
	{
		return $"HashCode:{GetHashCode()}, Count:{Count}, Max Capacity:{MaxCapacity} ";
	}

	/// <summary>
	/// Provides a way to get information about a specified node.
	/// </summary>
	/// <param name="key">The Key for which node to return information from. </param>
	/// <returns>Information from the node specified by its Key. </returns>
	public string ToString(int key)
	{
		return FindNodeContainingKey(_root, key).ToString();
	}

	private void InsertRebalance(RedBlackNode node)
	{
		ArgumentNullException.ThrowIfNull(node);
		if (node.Parent == null) throw new InvalidOperationException($"The provided node, Details:{node}, Cannot be Rebalanced with no Parent node.");


		while (node.Parent != null && node.Parent.IsRed)
		{
			if (node.Parent == node.Parent.Parent?.Left)
			{
				var uncle = node.Parent.Parent.Right;

				if (uncle != null && uncle.IsRed)
				{
					// Case 1: Recolor the parent, uncle, and grandparent
					node.Parent.IsRed = false;
					uncle.IsRed = false;
					node.Parent.Parent.IsRed = true;
					node = (RedBlackNode)node.Parent.Parent; // Move up the tree for further checking
				}
				else
				{
					if (node == node.Parent.Right)
					{
						// Case 2: Left rotate to make it Case 3
						node = (RedBlackNode)node.Parent;
						LeftRotate(node);
					}

					// Case 3: Recolor and right rotate the grandparent
					node.Parent.IsRed = false;
					node.Parent.Parent.IsRed = true;
					RightRotate((RedBlackNode)node.Parent.Parent);
				}
			}
			else
			{
				var uncle = node.Parent.Parent.Left;

				if (uncle != null && uncle.IsRed)
				{
					// Case 1: Recolor the parent, uncle, and grandparent
					node.Parent.IsRed = false;
					uncle.IsRed = false;
					node.Parent.Parent.IsRed = true;
					node = (RedBlackNode)node.Parent.Parent; // Move up the tree for further checking
				}
				else
				{
					if (node == node.Parent.Left)
					{
						// Case 2: Right rotate to make it Case 3
						node = (RedBlackNode)node.Parent;
						RightRotate(node);
					}

					// Case 3: Recolor and left rotate the grandparent
					node.Parent.IsRed = false;
					node.Parent.Parent.IsRed = true;
					LeftRotate((RedBlackNode)node.Parent.Parent);
				}
			}
		}

		// Ensure the _root is black
		_root.IsRed = false;
	}

	private void LeftRotate(RedBlackNode pivot)
	{
		if (pivot == null || pivot.Right == null)
		{
			// Cannot perform left rotation
			return;
		}

		var newRoot = pivot.Right;
		pivot.Right = newRoot.Left;

		if (pivot.Right != null)
		{
			pivot.Right.Parent = pivot;
		}

		newRoot.Parent = pivot.Parent;

		if (pivot.Parent == null)
		{
			_root = newRoot as RedBlackNode;
		}
		else if (pivot == pivot.Parent.Left)
		{
			pivot.Parent.Left = newRoot;
		}
		else
		{
			pivot.Parent.Right = newRoot;
		}

		newRoot.Left = pivot;
		pivot.Parent = newRoot;
	}

	private void RightRotate(RedBlackNode pivot)
	{
		if (pivot == null || pivot.Left == null)
		{
			// Cannot perform right rotation
			return;
		}

		var newRoot = pivot.Left;
		pivot.Left = newRoot.Right;

		if (pivot.Left != null)
		{
			pivot.Left.Parent = pivot;
		}

		newRoot.Parent = pivot.Parent;

		if (pivot.Parent == null)
		{
			_root = newRoot as RedBlackNode;
		}
		else if (pivot == pivot.Parent.Left)
		{
			pivot.Parent.Left = newRoot;
		}
		else
		{
			pivot.Parent.Right = newRoot;
		}

		newRoot.Right = pivot;
		pivot.Parent = newRoot;
	}

	private static void InsertNode(RedBlackNode parentNode, RedBlackNode newNode)
	{
		ArgumentNullException.ThrowIfNull(parentNode);
		ArgumentNullException.ThrowIfNull(newNode);


		if (newNode.Key < parentNode.Key)
		{
			if (parentNode.Left == null)
			{
				parentNode.Left = newNode;
				newNode.Parent = parentNode;
			}
			else
			{
				InsertNode(parentNode.Left as RedBlackNode, newNode);
			}
		}
		else if (newNode.Key > parentNode.Key)
		{
			if (parentNode.Right == null)
			{
				parentNode.Right = newNode;
				newNode.Parent = parentNode;
			}
			else
			{
				InsertNode(parentNode.Right as RedBlackNode, newNode);
			}
		}
		else if (newNode.Key == parentNode.Key)
		{
			throw new DuplicateNameException($"{newNode.Key}");
		}

	}

	private void RemoveNode(RedBlackNode node)
	{
		var replacementNode = node.Left == null || node.Right == null ? node : Successor(ref node);
		var child = replacementNode.Left ?? replacementNode.Right as RedBlackNode;

		ReplaceNode(replacementNode, child as RedBlackNode);

		if (replacementNode != node)
		{
			node.Value = replacementNode.Value;
		}

		// Recycle the node after all operations are complete
		PoolFactory.Recycle(node);

		if (!replacementNode.IsRed)
		{
			RedBlackNode childNode = child as RedBlackNode;
			FixRemove(ref childNode, replacementNode.Parent as RedBlackNode);
		}
	}

	/// <summary>
	/// Replaces a node in the Red-Black Tree with a new node, updating parent and child relationships.
	/// </summary>
	/// <param name="replacementNode">The node to be replaced.</param>
	/// <param name="child">The replacement node.</param>
	private void ReplaceNode(RedBlackNode replacementNode, RedBlackNode child)
	{
		if (child != null)
		{
			child.Parent = replacementNode.Parent;
		}

		if (replacementNode.Parent == null)
		{
			_root = child;
		}
		else if (replacementNode == replacementNode.Parent.Left)
		{
			replacementNode.Parent.Left = child;
		}
		else
		{
			replacementNode.Parent.Right = child;
		}
	}

	/// <summary>
	/// Finds a node in the Red-Black Tree that contains a specific _key.
	/// </summary>
	/// <param name="currentNode">The current node to start the search from.</param>
	/// <param name="key">The _key to search for.</param>
	/// <returns>The node containing the _key, or null if not found.</returns>
	private static RedBlackNode FindNodeContainingKey(RedBlackNode currentNode, int key)
	{
		ConcurrentQueue<RedBlackNode> Queue = new();
		Queue.Enqueue(currentNode);

		while (Queue.TryDequeue(out RedBlackNode node))
		{
			if (node.Key == key) return node;

			if (node.Left != null) Queue.Enqueue(node.Left as RedBlackNode);
			if (node.Right != null) Queue.Enqueue(node.Right as RedBlackNode);
		}

		throw new KeyNotFoundException($"Key:{key} was not found in the Tree.");
	}

	/// <summary>
	/// Finds the minimum node in a given Red-Black Tree.
	/// </summary>
	/// <param name="currentNode">The _root node of the tree.</param>
	/// <returns>The minimum node in the tree.</returns>
	private static RedBlackNode Minimum(RedBlackNode currentNode)
	{
		while (currentNode.Left != null)
			currentNode = (RedBlackNode)currentNode.Left;
		return currentNode;
	}

	/// <summary>
	/// Finds the successor node in a given Red-Black Tree.
	/// </summary>
	/// <param name="currentNode">The node to find the successor for.</param>
	/// <returns>The successor node of the given node.</returns>
	private static RedBlackNode Successor(ref RedBlackNode currentNode)
	{
		if (currentNode.Right != null)
			return Minimum((RedBlackNode)currentNode.Right);

		var parent = currentNode.Parent;
		while (parent != null && currentNode == parent.Right)
		{
			currentNode = (RedBlackNode)parent;
			parent = parent.Parent;
		}
		return (RedBlackNode)parent;
	}

	/// <summary>
	/// Checks if a given node is red.
	/// </summary>
	/// <param name="currentNode">The node to check.</param>
	/// <returns>True if the node is red; otherwise, false.</returns>
	private static bool IsRed(RedBlackNode currentNode) => currentNode != null && currentNode.IsRed;

	/// <summary>
	/// Checks if a given node is the left child of its parent.
	/// </summary>
	/// <param name="currentNode">The node to check.</param>
	/// <returns>True if the node is the left child; otherwise, false.</returns>
	private static bool IsLeftChild(RedBlackNode currentNode) => currentNode == currentNode.Parent.Left;

	/// <summary>
	/// Gets the left sibling of a given node.
	/// </summary>
	/// <param name="currentNode">The node to find the left sibling for.</param>
	/// <returns>The left sibling of the node.</returns>
	private static RedBlackNode GetLeftSibling(RedBlackNode currentNode) =>
		(RedBlackNode)currentNode.Parent.Left;

	/// <summary>
	/// Gets the right sibling of a given node.
	/// </summary>
	/// <param name="currentNode">The node to find the right sibling for.</param>
	/// <returns>The right sibling of the node.</returns>
	private static RedBlackNode GetRightSibling(RedBlackNode currentNode) =>
		(RedBlackNode)currentNode.Parent.Right;

	/// <summary>
	/// Recolors the given nodes in the Red-Black Tree, toggling their color between red and black.
	/// </summary>
	/// <param name="nodes">The nodes to recolor.</param>
	private static void RecolorNodes(params RedBlackNode[] nodes)
	{
		foreach (var n in nodes)
		{
			n.IsRed = !n.IsRed;
		}
	}

	/// <summary>
	/// Fixes the Red-Black Tree after a remove operation to maintain the properties of the tree.
	/// </summary>
	/// <param name="currentNode">The node to fix.</param>
	/// <param name="parent">The parent of the node.</param>
	private void FixRemove(ref RedBlackNode currentNode, RedBlackNode parent)
	{
		while (IsBlack(currentNode) && currentNode != _root)
		{
			if (IsLeftChild(currentNode))
			{
				FixRemoveForChild(ref currentNode, parent, GetRightSibling);
			}
			else
			{
				FixRemoveForChild(ref currentNode, parent, GetLeftSibling);
			}
		}

		if (currentNode != null)
			currentNode.IsRed = false;
	}

	private void FixRemoveForChild(ref RedBlackNode currentNode, RedBlackNode parent, Func<RedBlackNode, RedBlackNode> getSiblingChild)
	{
		while (IsBlack(currentNode) && currentNode != _root)
		{
			var sibling = getSiblingChild(currentNode.Parent as RedBlackNode);

			if (IsRed(sibling))
			{
				// Case 1: Recoloring
				RecolorNodes(sibling, parent);
				if (getSiblingChild == GetLeftSibling)
					RotateRight(parent);
				else
					RotateLeft(parent);

				sibling = getSiblingChild(parent);
			}

			if (IsBlack(getSiblingChild(sibling)) && IsBlack(getSiblingChild(sibling)))
			{
				// Case 2: Recoloring
				sibling.IsRed = true;
				currentNode = parent;
			}
			else
			{
				var siblingChild = getSiblingChild(sibling);

				if (IsBlack(siblingChild))
				{
					// Case 3: Rotation
					RecolorNodes(sibling, siblingChild);
					if (getSiblingChild == GetLeftSibling)
						RotateLeft(sibling);
					else
						RotateRight(sibling);

					sibling = getSiblingChild(parent);
				}

				// Case 4: Recoloring
				RecolorNodes(parent, sibling, getSiblingChild(sibling));
				if (getSiblingChild == GetLeftSibling)
					RotateRight(parent);
				else
					RotateLeft(parent);

				currentNode = _root;
			}
		}

		if (currentNode != null)
			currentNode.IsRed = false;
	}

	/// <summary>
	/// Checks if a given node is black.
	/// </summary>
	/// <param name="currentNode">The node to check.</param>
	/// <returns>True if the node is black; otherwise, false.</returns>
	private static bool IsBlack(RedBlackNode currentNode) => currentNode == null || !currentNode.IsRed;

	/// <summary>
	/// Rotates the tree to the left, preserving the Red-Black Tree properties.
	/// </summary>
	/// <param name="leftNode">The node to rotate.</param>
	private void RotateLeft(RedBlackNode leftNode)
	{
		RedBlackNode rightNode = (RedBlackNode)leftNode.Right;
		leftNode.Right = rightNode.Left;

		if (rightNode.Left != null)
			rightNode.Left.Parent = leftNode;

		rightNode.Parent = leftNode.Parent;

		if (leftNode.Parent == null)
			_root = rightNode;
		else if (leftNode == leftNode.Parent.Left)
			leftNode.Parent.Left = rightNode;
		else
			leftNode.Parent.Right = rightNode;

		rightNode.Left = leftNode;
		leftNode.Parent = rightNode;
	}

	/// <summary>
	/// Rotates the tree to the right, preserving the Red-Black Tree properties.
	/// </summary>
	/// <param name="rightNode">The node to rotate.</param>
	private void RotateRight(RedBlackNode rightNode)
	{
		RedBlackNode leftNode = (RedBlackNode)rightNode.Left;
		rightNode.Left = leftNode.Right;

		if (leftNode.Right != null)
			leftNode.Right.Parent = rightNode;

		leftNode.Parent = rightNode.Parent;

		if (rightNode.Parent == null)
			_root = leftNode;
		else if (rightNode == rightNode.Parent.Left)
			rightNode.Parent.Left = leftNode;
		else
			rightNode.Parent.Right = leftNode;

		leftNode.Right = rightNode;
		rightNode.Parent = leftNode;
	}

	/// <summary>
	/// Builds a Red-Black Tree from an array of _key-|||_value pairs.
	/// </summary>
	/// <param name="pairsArray">The array of KeyValuePairs to build the tree with.</param>
	/// <param name="start">The position to start adding items from..</param>
	/// <param name="end">The position to stop adding items at.</param>
	/// <param name="parent">The node where the tree is merged.</param>
	/// <returns>the newly formed tree at a node to add into a Tree.</returns>
	private static RedBlackNode BuildTreeFromArray(KeyValuePair<int, TValue>[] pairsArray, int start, int end, RedBlackNode parent)
	{
		if (start > end)
		{
			return null;
		}

		int mid = (start + end) / 2;

		var newNode = PoolFactory.Create(() => new RedBlackNode(pairsArray[mid].Key, pairsArray[mid].Value));
		newNode.Parent = parent;

		// Recursively build the left and right subtrees
		newNode.Left = BuildTreeFromArray(pairsArray, start, mid - 1, newNode);
		newNode.Right = BuildTreeFromArray(pairsArray, mid + 1, end, newNode);

		return newNode;
	}

	
}
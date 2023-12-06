using Factories;

using Interfaces;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace Trees;


/// <summary>
/// This is a Red-Black Tree with a Generic value type.
/// additionally, an integer type as a key 
/// </summary>
/// <typeparam name="TValue">The Generic _value Type</typeparam>
public sealed partial class GenericRedBlackTree<TValue> : IGenericRedBlackTree<TValue>
{
	private int _maxSize;
	private GenericRedBlackTreeNode _root;

	private HashSet<int> _index = PoolFactory.Create(() => new HashSet<int>());

	/// <summary>
	/// This is a Red-Black Tree with a Generic value type.
	/// additionally, an integer type as a key 
	/// </summary>
	/// <param name="maxSize">The Maximum size this tree is allowed to grow.</param>
	public GenericRedBlackTree(int maxSize)
	{
		if (EqualityComparer<int>.Default.Equals(maxSize, default))
		{
			_maxSize = default;
		}
		else
		{
			if (maxSize < 0)
			{
				_maxSize = 0;
			}
			else
			{
				_maxSize = maxSize;
			}
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="GenericRedBlackTree{TValue}"/> class.
	/// </summary>
	public GenericRedBlackTree() : this(default) { }

	/// <summary>
	/// Gets the set of keys in the Red-Black Tree.
	/// </summary>
	public ReadOnlyCollection<int> Index => new(_index.ToList());

	/// <summary>
	/// Gets or sets the maximum size of the tree. If set to null, there is no maximum size limit.
	/// </summary>
	public int MaxSize
	{
		get => _maxSize;
		set
		{
			if (_maxSize == default) _maxSize = value;
		}
	}

	/// <summary>
	/// Checks if the Red-Black Tree contains a specific key.
	/// </summary>
	/// <param name="id">The key to check for.</param>
	/// <returns>True if the key is present in the tree; otherwise, false.</returns>
	public bool Contains(int id) => _index.Contains(id);

	/// <summary>
	/// Gets the number of elements in the Red-Black Tree.
	/// </summary>
	public int Count { get => _index.Count; }

	/// <summary>
	/// Inserts a new key-value pair into the Red-Black Tree.
	/// </summary>
	/// <param name="key">The key to insert.</param>
	/// <param name="value">The value associated with the key.</param>
	public void Insert(int key, TValue value)
	{
		ArgumentNullException.ThrowIfNull(value);
		if (key < 0) throw new ArgumentException($"Invalid Key:{key}, the key must be a positive integer.");
		if (_index.Count >= _maxSize && _maxSize != 0) throw new InvalidOperationException($"The maximum size of the tree has been reached, Remove entries before attempting to add more.");

		if (_index.Add(key))
		{
			var newNode = PoolFactory.Create(() => new GenericRedBlackTreeNode(key, value));

			if (_root == null)
			{
				_root = newNode;
				_root.IsRed = false;
			}
			else
			{
				InsertNode(_root, newNode);
				InsertRebalance(newNode); // Call the rebalance logic after inserting the new node
			}
			PoolFactory.Recycle(newNode);
			return;
		}
		// Optionally handle updating the value associated with an existing key
		throw new DuplicateNameException($"{key}");
	}

	private void InsertRebalance(GenericRedBlackTreeNode node)
	{
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
					node = (GenericRedBlackTreeNode)node.Parent.Parent; // Move up the tree for further checking
				}
				else
				{
					if (node == node.Parent.Right)
					{
						// Case 2: Left rotate to make it Case 3
						node = (GenericRedBlackTreeNode)node.Parent;
						LeftRotate(node);
					}

					// Case 3: Recolor and right rotate the grandparent
					node.Parent.IsRed = false;
					node.Parent.Parent.IsRed = true;
					RightRotate((GenericRedBlackTreeNode)node.Parent.Parent);
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
					node = (GenericRedBlackTreeNode)node.Parent.Parent; // Move up the tree for further checking
				}
				else
				{
					if (node == node.Parent.Left)
					{
						// Case 2: Right rotate to make it Case 3
						node = (GenericRedBlackTreeNode)node.Parent;
						RightRotate(node);
					}

					// Case 3: Recolor and left rotate the grandparent
					node.Parent.IsRed = false;
					node.Parent.Parent.IsRed = true;
					LeftRotate((GenericRedBlackTreeNode)node.Parent.Parent);
				}
			}
		}

		// Ensure the root is black
		_root.IsRed = false;
	}

	private void LeftRotate(GenericRedBlackTreeNode pivot)
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
			_root = newRoot as GenericRedBlackTreeNode;
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

	private void RightRotate(GenericRedBlackTreeNode pivot)
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
			_root = newRoot as GenericRedBlackTreeNode;
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

	private static void InsertNode(GenericRedBlackTreeNode parentNode, GenericRedBlackTreeNode newNode)
	{
		if (newNode.Key < parentNode.Key)
		{
			if (parentNode.Left == null)
			{
				parentNode.Left = newNode;
			}
			else
			{
				InsertNode((GenericRedBlackTreeNode)parentNode.Left, newNode);
			}
		}
		else if (newNode.Key > parentNode.Key)
		{
			if (parentNode.Right == null)
			{
				parentNode.Right = newNode;
			}
			else
			{
				InsertNode((GenericRedBlackTreeNode)parentNode.Right, newNode);
			}
		}
		// Optionally handle updating the value associated with an existing key
		else
		{
			throw new DuplicateNameException($"{newNode.Key}");
		}
	}

	/// <summary>
	/// Removes a key-value pair from the Red-Black Tree.
	/// </summary>
	/// <param name="key">The key to remove.</param>
	public void Remove(int key)
	{
		var node = FindNodeContainingKey(_root, key);

		if (!_index.Remove(key) || node == null)
		{
			// Key not found, throw an exception
			throw new KeyNotFoundException($"Unable to find Key:{key} in the tree.");
		}

		var replacementNode = node.Left == null || node.Right == null ? node : Successor(ref node);
		var child = replacementNode.Left ?? replacementNode.Right as GenericRedBlackTreeNode;

		ReplaceNode(replacementNode, child as GenericRedBlackTreeNode);

		if (replacementNode != node)
		{
			node.Value = replacementNode.Value;
		}

		if (!replacementNode.IsRed)
		{
			GenericRedBlackTreeNode childNode = child as GenericRedBlackTreeNode;
			FixRemove(ref childNode, replacementNode.Parent as GenericRedBlackTreeNode);
		}

		// Recycle of the node after all operations are complete
		PoolFactory.Recycle(node);
	}

	/// <summary>
	/// Replaces a node in the Red-Black Tree with a new node, updating parent and child relationships.
	/// </summary>
	/// <param name="replacementNode">The node to be replaced.</param>
	/// <param name="child">The replacement node.</param>
	private void ReplaceNode(GenericRedBlackTreeNode replacementNode, GenericRedBlackTreeNode child)
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
	/// Updates the value associated with a key in the Red-Black Tree.
	/// </summary>
	/// <param name="key">The key to update.</param>
	/// <param name="value">The new value to associate with the key.</param>
	public void Update(int key, TValue value)
	{
		Remove(key);
		Insert(key, value);
	}

	/// <summary>
	/// Gets the value associated with a specific key in the Red-Black Tree.
	/// </summary>
	/// <param name="key">The key to look up.</param>
	/// <returns>The value associated with the key.</returns>
	public TValue GetValue(int key)
	{
		if (!_index.Contains(key)) throw new KeyNotFoundException($"Key:{key} was not found in the index.");

		return FindNodeContainingKey(_root, key).Value;
	}

	/// <summary>
	/// Finds a node in the Red-Black Tree that contains a specific key.
	/// </summary>
	/// <param name="currentNode">The current node to start the search from.</param>
	/// <param name="key">The key to search for.</param>
	/// <returns>The node containing the key, or null if not found.</returns>
	private static GenericRedBlackTreeNode FindNodeContainingKey(GenericRedBlackTreeNode currentNode, int key)
	{
		ConcurrentQueue<GenericRedBlackTreeNode> Queue = new();
		Queue.Enqueue(currentNode);

		while (Queue.TryDequeue(out GenericRedBlackTreeNode node))
		{
			if (node.Key == key) return node;

			if (node.Left != null) Queue.Enqueue(node.Left as GenericRedBlackTreeNode);
			if (node.Right != null) Queue.Enqueue(node.Right as GenericRedBlackTreeNode);
		}

		throw new KeyNotFoundException($"Key:{key} was not found in the Tree.");
	}

	/// <summary>
	/// Finds the minimum node in a given Red-Black Tree.
	/// </summary>
	/// <param name="currentNode">The root node of the tree.</param>
	/// <returns>The minimum node in the tree.</returns>
	private static GenericRedBlackTreeNode Minimum(GenericRedBlackTreeNode currentNode)
	{
		while (currentNode.Left != null)
			currentNode = (GenericRedBlackTreeNode)currentNode.Left;
		return currentNode;
	}

	/// <summary>
	/// Finds the successor node in a given Red-Black Tree.
	/// </summary>
	/// <param name="currentNode">The node to find the successor for.</param>
	/// <returns>The successor node of the given node.</returns>
	private static GenericRedBlackTreeNode Successor(ref GenericRedBlackTreeNode currentNode)
	{
		if (currentNode.Right != null)
			return Minimum((GenericRedBlackTreeNode)currentNode.Right);

		var parent = currentNode.Parent;
		while (parent != null && currentNode == parent.Right)
		{
			currentNode = (GenericRedBlackTreeNode)parent;
			parent = parent.Parent;
		}
		return (GenericRedBlackTreeNode)parent;
	}

	/// <summary>
	/// Checks if a given node is red.
	/// </summary>
	/// <param name="currentNode">The node to check.</param>
	/// <returns>True if the node is red; otherwise, false.</returns>
	private static bool IsRed(GenericRedBlackTreeNode currentNode) => currentNode != null && currentNode.IsRed;

	/// <summary>
	/// Checks if a given node is the left child of its parent.
	/// </summary>
	/// <param name="currentNode">The node to check.</param>
	/// <returns>True if the node is the left child; otherwise, false.</returns>
	private static bool IsLeftChild(GenericRedBlackTreeNode currentNode) => currentNode == currentNode.Parent.Left;

	/// <summary>
	/// Gets the left sibling of a given node.
	/// </summary>
	/// <param name="currentNode">The node to find the left sibling for.</param>
	/// <returns>The left sibling of the node.</returns>
	private static GenericRedBlackTreeNode GetLeftSibling(GenericRedBlackTreeNode currentNode) =>
		(GenericRedBlackTreeNode)currentNode.Parent.Left;

	/// <summary>
	/// Gets the right sibling of a given node.
	/// </summary>
	/// <param name="currentNode">The node to find the right sibling for.</param>
	/// <returns>The right sibling of the node.</returns>
	private static GenericRedBlackTreeNode GetRightSibling(GenericRedBlackTreeNode currentNode) =>
		(GenericRedBlackTreeNode)currentNode.Parent.Right;

	/// <summary>
	/// Recolors the given nodes in the Red-Black Tree, toggling their color between red and black.
	/// </summary>
	/// <param name="nodes">The nodes to recolor.</param>
	private static void RecolorNodes(params GenericRedBlackTreeNode[] nodes)
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
	private void FixRemove(ref GenericRedBlackTreeNode currentNode, GenericRedBlackTreeNode parent)
	{
		while (IsBlack(currentNode) && currentNode != _root)
		{
			if (IsLeftChild(currentNode))
			{
				FixRemoveForLeftChild(ref currentNode, parent);
			}
			else
			{
				FixRemoveForRightChild(ref currentNode, parent);
			}
		}

		if (currentNode != null)
			currentNode.IsRed = false;
	}

	/// <summary>
	/// Fixes the Red-Black Tree for a left child during a remove operation.
	/// </summary>
	/// <param name="currentNode">The node to fix.</param>
	/// <param name="parent">The parent of the node.</param>
	private void FixRemoveForLeftChild(ref GenericRedBlackTreeNode currentNode, GenericRedBlackTreeNode parent)
	{
		var sibling = GetRightSibling(currentNode);

		if (IsRed(sibling))
		{
			// Case 1: Recoloring
			RecolorNodes(sibling, parent);
			RotateLeft(parent);
			sibling = GetRightSibling(currentNode.Parent as GenericRedBlackTreeNode);
		}

		FixRemoveCasesForLeftChild(ref currentNode, parent, sibling);
	}

	/// <summary>
	/// Handles various cases for a left child during a remove operation.
	/// </summary>
	/// <param name="currentNode">The node to fix.</param>
	/// <param name="parent">The parent of the node.</param>
	/// <param name="sibling">The sibling of the node.</param>
	private void FixRemoveCasesForLeftChild(ref GenericRedBlackTreeNode currentNode, GenericRedBlackTreeNode parent, GenericRedBlackTreeNode sibling)
	{
		if (IsBlack(sibling.Left as GenericRedBlackTreeNode) && IsBlack(sibling.Right as GenericRedBlackTreeNode))
		{
			// Case 2: Recoloring
			sibling.IsRed = true;
			currentNode = parent;
		}
		else
		{
			if (IsBlack(sibling.Right as GenericRedBlackTreeNode))
			{
				// Case 3: Right rotation
				RecolorNodes(sibling, sibling.Left as GenericRedBlackTreeNode);
				RotateRight(sibling);
				sibling = GetRightSibling(parent);
			}

			// Case 4: Recoloring
			RecolorNodes(parent, sibling, sibling.Right as GenericRedBlackTreeNode);
			RotateLeft(parent);
			currentNode = _root;
		}
	}

	/// <summary>
	/// Fixes the Red-Black Tree for a right child during a remove operation.
	/// </summary>
	/// <param name="currentNode">The node to fix.</param>
	/// <param name="parent">The parent of the node.</param>
	private void FixRemoveForRightChild(ref GenericRedBlackTreeNode currentNode, GenericRedBlackTreeNode parent)
	{
		var sibling = GetLeftSibling(currentNode);

		if (IsRed(sibling))
		{
			// Case 1: Recoloring
			RecolorNodes(sibling, parent);
			RotateRight(parent);
			sibling = GetLeftSibling(currentNode.Parent as GenericRedBlackTreeNode);
		}

		FixRemoveCasesForRightChild(ref currentNode, parent, sibling);
	}

	/// <summary>
	/// Handles various cases for a right child during a remove operation.
	/// </summary>
	/// <param name="currentNode">The node to fix.</param>
	/// <param name="parent">The parent of the node.</param>
	/// <param name="sibling">The sibling of the node.</param>
	private void FixRemoveCasesForRightChild(ref GenericRedBlackTreeNode currentNode, GenericRedBlackTreeNode parent, GenericRedBlackTreeNode sibling)
	{
		if (IsBlack(sibling.Left as GenericRedBlackTreeNode) && IsBlack(sibling.Right as GenericRedBlackTreeNode))
		{
			// Case 2: Recoloring
			sibling.IsRed = true;
			currentNode = parent;
		}
		else
		{
			if (IsBlack(sibling.Left as GenericRedBlackTreeNode))
			{
				// Case 3: Left rotation
				RecolorNodes(sibling, sibling.Right as GenericRedBlackTreeNode);
				RotateLeft(sibling);
				sibling = GetLeftSibling(parent);
			}

			// Case 4: Recoloring
			RecolorNodes(parent, sibling, sibling.Left as GenericRedBlackTreeNode);
			RotateRight(parent);
			currentNode = _root;
		}
	}

	/// <summary>
	/// Checks if a given node is black.
	/// </summary>
	/// <param name="currentNode">The node to check.</param>
	/// <returns>True if the node is black; otherwise, false.</returns>
	private static bool IsBlack(GenericRedBlackTreeNode currentNode) => currentNode == null || !currentNode.IsRed;

	/// <summary>
	/// Rotates the tree to the left, preserving the Red-Black Tree properties.
	/// </summary>
	/// <param name="leftNode">The node to rotate.</param>
	private void RotateLeft(GenericRedBlackTreeNode leftNode)
	{
		GenericRedBlackTreeNode rightNode = (GenericRedBlackTreeNode)leftNode.Right;
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
	private void RotateRight(GenericRedBlackTreeNode rightNode)
	{
		GenericRedBlackTreeNode leftNode = (GenericRedBlackTreeNode)rightNode.Left;
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
	/// Gets or sets the value associated with the specified key in the Red-Black Tree.
	/// </summary>
	/// <param name="key">The key to access or modify.</param>
	/// <returns>The value associated with the key.</returns>
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
			if (node.Key == key)
			{
				node.Value = value;
			}
		}
	}

	/// <summary>
	/// Returns an enumerator that iterates through the elements of the Red-Black Tree in an in-order traversal.
	/// </summary>
	/// <returns>An enumerator that can be used to iterate through the elements of the Red-Black Tree in an in-order traversal.</returns>
	public IEnumerator<KeyValuePair<int, TValue>> GetEnumerator()
	{
		var queue = PoolFactory.Create(() => new Queue<GenericRedBlackTreeNode>());
		PoolFactory.SetPoolResetAction<Queue<GenericRedBlackTreeNode>>((queue) => queue.Clear());
		queue.Enqueue(_root);

		while (queue.Count > 0)
		{
			var currentNode = queue.Dequeue();

			if (currentNode.Left != null) queue.Enqueue(currentNode.Left as GenericRedBlackTreeNode);
			if (currentNode.Right != null) queue.Enqueue(currentNode.Right as GenericRedBlackTreeNode);

			yield return new KeyValuePair<int, TValue>(currentNode.Key, currentNode.Value);
		}

		PoolFactory.Recycle(queue);
	}

	/// <summary>
	/// Returns all elements of the Red-Black Tree.
	/// </summary>
	/// <returns>An enumerable of all key-value pairs.</returns>
	public IEnumerable<KeyValuePair<int, TValue>> GetAll()
	{
		var queue = PoolFactory.Create(() => new Queue<GenericRedBlackTreeNode>());
		PoolFactory.SetPoolResetAction<Queue<GenericRedBlackTreeNode>>((queue) => queue.Clear());
		queue.Enqueue(_root);

		while (queue.Count > 0)
		{
			var currentNode = queue.Dequeue();

			if (currentNode.Left != null) queue.Enqueue(currentNode.Left as GenericRedBlackTreeNode);
			if (currentNode.Right != null) queue.Enqueue(currentNode.Right as GenericRedBlackTreeNode);

			yield return new KeyValuePair<int, TValue>(currentNode.Key, currentNode.Value);

			PoolFactory.Recycle(currentNode);
		}

		PoolFactory.Recycle(queue);
	}

	/// <summary>
	/// Provides a way to get all items in the tree that correspond with the provided HashSet of Keys.
	/// </summary>
	/// <param name="list">The HasSet containing the keys to return.</param>
	/// <returns>an IEnumerable collection of KeyValuePair items.</returns>
	public IEnumerable<KeyValuePair<int, TValue>> GetList(List<int> list)
	{
		var queue = PoolFactory.Create(() => new Queue<GenericRedBlackTreeNode>());
		PoolFactory.SetPoolResetAction<Queue<GenericRedBlackTreeNode>>((queue) => queue.Clear());
		queue.Enqueue(_root);

		while (queue.Count > 0)
		{
			var currentNode = queue.Dequeue();

			if (currentNode.Left != null) queue.Enqueue(currentNode.Left as GenericRedBlackTreeNode);
			if (currentNode.Right != null) queue.Enqueue(currentNode.Right as GenericRedBlackTreeNode);

			if (list.Remove(currentNode.Key))
			{
				yield return new KeyValuePair<int, TValue>(currentNode.Key, currentNode.Value);
			}
			PoolFactory.Recycle(currentNode);

		}
		PoolFactory.Recycle(queue);
	}

	/// <summary>
	/// Performs bulk insertion of key-value pairs into the Red-Black Tree.
	/// </summary>
	/// <param name="keyValuePairs">The key-value pairs to insert.</param>
	public void BulkInsert(IEnumerable<KeyValuePair<int, TValue>> keyValuePairs)
	{
		ArgumentNullException.ThrowIfNull(keyValuePairs);
		if (_index.Count + keyValuePairs.Count() > _maxSize && _maxSize > 0) throw new InvalidOperationException($"The maximum size of {_maxSize} has been reached or exceeded. This bulk insert requires {_index.Count + keyValuePairs.Count()} total spaces.");

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
	/// Builds a Red-Black Tree from an array of key-value pairs.
	/// </summary>
	/// <param name="pairsArray">The array of KeyValuePairs to build the tree with.</param>
	/// <param name="start">The position to start adding items from..</param>
	/// <param name="end">The position to stop adding items at.</param>
	/// <param name="parent">The node where the tree is merged.</param>
	/// <returns>the newly formed tree at a node to add into a Tree.</returns>
	private static GenericRedBlackTreeNode BuildTreeFromArray(KeyValuePair<int, TValue>[] pairsArray, int start, int end, GenericRedBlackTreeNode parent)
	{
		if (start > end)
		{
			return null;
		}

		int mid = (start + end) / 2;

		var newNode = PoolFactory.Create(() => new GenericRedBlackTreeNode(pairsArray[mid].Key, pairsArray[mid].Value));
		newNode.Parent = parent;

		// Recursively build the left and right subtrees
		newNode.Left = BuildTreeFromArray(pairsArray, start, mid - 1, newNode);
		newNode.Right = BuildTreeFromArray(pairsArray, mid + 1, end, newNode);

		return newNode;
	}

	/// <summary>
	/// Resets the state of the Red-Black Tree, effectively clearing it and resetting any configuration options.
	/// </summary>
	public void ResetState()
	{
		_root = null;
		_index = null;

		PoolFactory.Recycle(_index);
		PoolFactory.Recycle(_root);

		_index = PoolFactory.Create(() => new HashSet<int>());
		_maxSize = default;
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

		var other = (GenericRedBlackTree<TValue>)obj;
		return _root.Key.Equals(other._root.Key) && _root.Value.Equals(other._root.Value);
	}

	/// <summary>
	/// Serves as the default hash function..
	/// </summary>
	/// <returns>A Hash code for the current object</returns>
	public override int GetHashCode()
	{
		return HashCode.Combine(_root.Key, _root.Value);
	}

	/// <summary>
	/// Provides information about this tree;
	/// </summary>
	/// <returns>A CSV string containing details about the tree. </returns>
	public override string ToString() 
	{
		return $"HashCode:{GetHashCode}, Count:{Count}, Max Capacity:{MaxSize} ";
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

}
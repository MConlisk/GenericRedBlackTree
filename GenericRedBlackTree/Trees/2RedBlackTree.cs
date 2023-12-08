using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

using Factories;

namespace DataStructures.Trees;

public class TreeEventArgs<TKey, TValue> : EventArgs where TKey : IComparable<TKey>
{
	public TKey Key { get; }
	public TValue Value { get; }

	public ITreeNode<TKey, TValue> StartingNode { get; }

	public TreeEventArgs(TKey key, TValue value, ITreeNode<TKey, TValue> startingNode)
	{
		Key = key;
		Value = value;
		StartingNode = startingNode;
	}
}

public class InsertTreeEventArgs<TKey, TValue> : TreeEventArgs<TKey, TValue> where TKey : IComparable<TKey>
{
	public InsertTreeEventArgs(TKey key, TValue value, ITreeNode<TKey, TValue> startingNode)
		: base(key, value, startingNode)
	{
	}
}

public class RemoveTreeEventArgs<TKey, TValue> : TreeEventArgs<TKey, TValue> where TKey : IComparable<TKey>
{
	public RemoveTreeEventArgs(TKey key, TValue value, ITreeNode<TKey, TValue> startingNode)
		: base(key, value, startingNode)
	{
	}
}

public class UpdateTreeEventArgs<TKey, TValue> : TreeEventArgs<TKey, TValue> where TKey : IComparable<TKey>
{
	public UpdateTreeEventArgs(TKey key, TValue value, ITreeNode<TKey, TValue> startingNode)
		: base(key, value, startingNode)
	{
	}
}

public class EnumerateTreeEventArgs<TKey, TValue> : TreeEventArgs<TKey, TValue> where TKey : IComparable<TKey>
{
	public EnumerateTreeEventArgs(TKey key, TValue value, ITreeNode<TKey, TValue> startingNode)
		: base(default, default, startingNode)
	{
	}
}

public class BalanceTreeEventArgs<TKey, TValue> : TreeEventArgs<TKey, TValue> where TKey : IComparable<TKey>
{
	public BalanceTreeEventArgs(TKey key, TValue value, ITreeNode<TKey, TValue> startingNode)
		: base(default, default, startingNode)
	{
	}
}

public interface ITreeNode<TKey, TValue> where TKey : IComparable<TKey>
{
	TKey Key { get; set; }
	TValue Value { get; set; }
	Dictionary<string, ITreeNode<TKey, TValue>> SubNodes { get; set; }
	int SubNodesCapacity { get; }
	Func<ITreeNode<TKey, TValue>> NodeFactory { get; }
	void AddSubNode(ITreeNode<TKey, TValue> subNode);
}

public class RedBlackTreeNode<TKey, TValue> : ITreeNode<TKey, TValue> where TKey : IComparable<TKey>
{

	private TKey _key;
	private TValue _value;
	private bool _isRed;
	private Dictionary<string, ITreeNode<TKey, TValue>> _subNodes;

	private readonly Func<ITreeNode<TKey, TValue>> _nodeFactory = () => PoolFactory.Create<RedBlackTreeNode<TKey, TValue>>(() => new());

	public int SubNodesCapacity => 2;

	public TKey Key { get => _key; set => _key = value; }
	public TValue Value { get => _value; set => _value = value; }
	public bool IsRed { get => _isRed; set => _isRed = value; }
	public Dictionary<string, ITreeNode<TKey, TValue>> SubNodes { get => _subNodes; set => _subNodes = value; }
	public Func<ITreeNode<TKey, TValue>> NodeFactory { get; }
	
	public void AddSubNode(ITreeNode<TKey, TValue> subNode)
	{
		
	}
}

public interface ITreeObserver<TKey, TValue> where TKey : IComparable<TKey>
{
	void OnInsert(object sender, InsertTreeEventArgs<TKey, TValue> e);
	void OnRemove(object sender, RemoveTreeEventArgs<TKey, TValue> e);
	void OnUpdate(object sender, UpdateTreeEventArgs<TKey, TValue> e);
	void OnEnumerate(object sender, EnumerateTreeEventArgs<TKey, TValue> e);
	void OnBalance(object sender, BalanceTreeEventArgs<TKey, TValue> e);
}

// Base Tree class
public class Tree<TKey, TValue> where TKey : IComparable<TKey>
{
	// Root node of the tree
	private ITreeNode<TKey, TValue> _root;
	protected ITreeNode<TKey, TValue> Root { get => _root; set => _root = value; }


	private Func<ITreeNode<TKey, TValue>> _nodeFactory { get; set; }
	private ITreeNode<TKey, TValue> _nodeType { get; set; }

	public Tree(ITreeNode<TKey, TValue> nodeType)
	{
		_nodeType = nodeType;
		_nodeFactory = nodeType.NodeFactory;
	}

	// Custom events for different tree events
	public event EventHandler<InsertTreeEventArgs<TKey, TValue>> InsertEvent;
	public event EventHandler<RemoveTreeEventArgs<TKey, TValue>> RemoveEvent;
	public event EventHandler<UpdateTreeEventArgs<TKey, TValue>> UpdateEvent;
	public event EventHandler<EnumerateTreeEventArgs<TKey, TValue>> EnumerateEvent;
	public event EventHandler<BalanceTreeEventArgs<TKey, TValue>> BalanceEvent;

	// List of _observers interested in changes to the tree
	private List<ITreeObserver<TKey, TValue>> _observers = new List<ITreeObserver<TKey, TValue>>();

	// Public method to insert a key-value pair into the tree
	public virtual void Insert(KeyValuePair<TKey, TValue> keyValuePair)
	{
		// Check if the _root node is null
		if (_root == null)
		{
			// If the _root is null, create a new _root node
			_root = _nodeFactory();

			_root.Key = keyValuePair.Key;
			_root.Value = keyValuePair.Value;
			_root.SubNodes = new Dictionary<string, ITreeNode<TKey, TValue>>(_root.SubNodesCapacity);
			


			// Notify _observers that an item has been added
			OnInsert(new InsertTreeEventArgs<TKey, TValue>(keyValuePair.Key, keyValuePair.Value, _root));
		}
		else
		{
			// Handle insertion into an existing tree
			// This part will be specific to the rules of your tree structure
			// You might want to implement the Red-Black Tree insertion logic here

			// For now, let's simply create a new node and add it to the _root's SubNodes
			var newNode = _nodeFactory();
			newNode.Key = keyValuePair.Key;
			newNode.Value = keyValuePair.Value;
			newNode.SubNodes = new Dictionary<string, ITreeNode<TKey, TValue>>(_nodeType.SubNodesCapacity);

			_root.AddSubNode(newNode);

			// Notify _observers that an item has been added
			OnInsert(new InsertTreeEventArgs<TKey, TValue>(keyValuePair.Key, keyValuePair.Value, _root));
		}
	}

	// Public method to remove a _key-|||_value pair from the tree
	public virtual void Remove(TKey key)
	{
		// Implementation to be added

		// Notify _observers that an item has been removed
		OnRemove(new RemoveTreeEventArgs<TKey, TValue>(key, default(TValue), _root));
	}

	// Public method to update a _key-|||_value pair in the tree
	public virtual void Update(TKey key, TValue value)
	{
		// Implementation to be added

		// Notify _observers that an item has been updated
		OnUpdate(new UpdateTreeEventArgs<TKey, TValue>(key, value, _root));
	}

	// Public method to get the |||_value associated with a _key
	public virtual TValue GetValue(TKey key)
	{
		// Implementation to be added
		return default(TValue); // Placeholder, replace with actual implementation
	}

	// Public method to check if a _key exists in the tree
	public virtual bool ContainsKey(TKey key)
	{
		// Implementation to be added
		return false;
	}

	// Public method to check if the tree is empty
	public bool IsEmpty()
	{
		return _root == null;
	}

	// Method to add an observer to the tree
	public void AddObserver(ITreeObserver<TKey, TValue> observer)
	{
		_observers.Add(observer);
	}

	// Method to remove an observer from the tree
	public void RemoveObserver(ITreeObserver<TKey, TValue> observer)
	{
		_observers.Remove(observer);
	}

	// Method to trigger the InsertEvent and notify _observers
	protected virtual void OnInsert(InsertTreeEventArgs<TKey, TValue> e)
	{
		InsertEvent?.Invoke(this, e);
	}

	// Method to trigger the RemoveEvent and notify _observers
	protected virtual void OnRemove(RemoveTreeEventArgs<TKey, TValue> e)
	{
		RemoveEvent?.Invoke(this, e);
	}

	// Method to trigger the UpdateEvent and notify _observers
	protected virtual void OnUpdate(UpdateTreeEventArgs<TKey, TValue> e)
	{
		UpdateEvent?.Invoke(this, e);
	}

	// Method to trigger the EnumerateEvent and notify _observers
	protected virtual void OnEnumerate(EnumerateTreeEventArgs<TKey, TValue> e)
	{
		EnumerateEvent?.Invoke(this, e);
	}

	// Method to trigger the BalanceEvent and notify _observers
	protected virtual void OnBalance(BalanceTreeEventArgs<TKey, TValue> e)
	{
		BalanceEvent?.Invoke(this, e);
	}
}

// Example Traversal Component implementation
public class InOrderTraversalComponent<TKey, TValue> : ITreeObserver<TKey, TValue> where TKey : IComparable<TKey>
{
	// Specific In-Order Traversal logic can be implemented here
	public void OnInsert(object sender, InsertTreeEventArgs<TKey, TValue> e)
	{
		// Implement logic to perform In-Order Traversal after an item is inserted
		Console.WriteLine($"Performing In-Order Traversal after adding item: Key={e.Key}, Value={e.Value}");
		InOrderTraversal(e.StartingNode);
	}

	public void OnRemove(object sender, RemoveTreeEventArgs<TKey, TValue> e)
	{
		// Implement logic to handle In-Order Traversal after an item is removed
		Console.WriteLine($"Performing In-Order Traversal after removing item: Key={e.Key}, Value={e.Value}");
		InOrderTraversal(e.StartingNode);
	}

	public void OnUpdate(object sender, UpdateTreeEventArgs<TKey, TValue> e)
	{
		// Implement logic to handle In-Order Traversal after an item is updated
		Console.WriteLine($"Performing In-Order Traversal after updating item: Key={e.Key}, Value={e.Value}");
		InOrderTraversal(e.StartingNode);
	}

	public void OnEnumerate(object sender, EnumerateTreeEventArgs<TKey, TValue> e)
	{
		// Implement logic to handle In-Order Traversal during enumeration
		Console.WriteLine($"Performing In-Order Traversal during enumeration");
		InOrderTraversal(e.StartingNode);
	}

	public void OnBalance(object sender, BalanceTreeEventArgs<TKey, TValue> e)
	{
		// Implement logic to handle In-Order Traversal after balancing
		Console.WriteLine($"Performing In-Order Traversal after balancing");
		InOrderTraversal(e.StartingNode);
	}

	// Recursive method for In-Order Traversal
	private void InOrderTraversal(ITreeNode<TKey, TValue> node)
	{
		if (node != null)
		{
			InOrderTraversal(node.SubNodes["Left"]); // Left subtree
			Console.WriteLine($"Node: Key={node.Key}, Value={node.Value}");
			InOrderTraversal(node.SubNodes["Right"]); // Right subtree
		}
	}
}

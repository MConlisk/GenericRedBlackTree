using DataStructures.Interfaces;

using System;

namespace DataStructures.Nodes;

public interface INode<TKey, TValue> where TKey : IComparable<TKey>
{
	int MaxSubNodes { get; }
	TKey Key { get; set; }
	TValue Value { get; set; }
	INode<TKey, TValue>[] Nodes { get; set; }

}



// Strategy interface for search algorithms
public interface ITraversalStrategy<TKey, TValue> where TKey : IComparable<TKey>
{
	bool Traverse(ITreeModel<TKey, TValue> node, TKey key, out ITreeModel<TKey, TValue> output);
}

// Concrete strategy for the default search algorithm
public class RedBlackDefaultTraversal<TKey, TValue> : ITraversalStrategy<TKey, TValue>
	where TKey : IComparable<TKey>
{
	public bool Traverse(ITreeModel<TKey, TValue> node, TKey key, out ITreeModel<TKey, TValue> output)
	{
		// Default search logic
		// ...
		output = null;
		return false;
	}
}

// Context class that uses the search strategy
public class TraversalContext<TKey, TValue> where TKey : IComparable<TKey>
{
	private ITraversalStrategy<TKey, TValue> _traversalStrategy;

	public void SetTraversalStrategy(ITraversalStrategy<TKey, TValue> searchStrategy)
	{
		_traversalStrategy = searchStrategy;
	}

	public bool ExecuteStrategy(ITreeModel<TKey, TValue> node, TKey key, out ITreeModel<TKey, TValue> output)
	{
		if (_traversalStrategy != null)
		{
			return _traversalStrategy.Traverse(node, key, out output);
		}

		// Default behavior
		// ...
		output = null;
		return false;
	}
}
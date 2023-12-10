using DataStructures.Interfaces;
using DataStructures.Nodes;

using Factories;

using System;
using System.Collections.Generic;

namespace DataStructures.Components;

public class RedBlackTreeModel<TKey, TValue> : ITreeModel<TKey, TValue> where TKey : IComparable<TKey>
{
	private INode<TKey, TValue> _rootNode;
	private readonly Func<INode<TKey, TValue>> _nodeFactory;
	private readonly ITraversalComponent<TKey, TValue> _traversalComponent;

	public RedBlackTreeModel(ITraversalComponent<TKey, TValue> traversalComponent) 
	{
		_traversalComponent = traversalComponent;
		_nodeFactory = () => PoolFactory.Create(() => new RedBlackNode<TKey, TValue>());
	}

	public bool IsEmpty => _rootNode == null || _rootNode.Value.Equals(default);
	public INode<TKey, TValue> RootNode => _rootNode;
	public Func<INode<TKey, TValue>> NodeFactory => _nodeFactory;
	public ITraversalComponent<TKey, TValue> TraversalComponent => _traversalComponent;

	public void Insert(TKey key, TValue value)
	{
		var newNode = _nodeFactory() as RedBlackNode<TKey, TValue>;

		newNode.Key = key;
		newNode.Value = value;
		newNode.IsRed = true;
		newNode.Nodes = new INode<TKey, TValue>[4];

		// Assuming _rootNode is initialized appropriately
		TraversalComponent.InsertNew(ref _rootNode, (INode<TKey, TKey>)newNode);
	}


	public void Remove(TKey key)
	{
		
	}

	public void Update(TKey key, TValue value)
	{
		
	}

	public TValue GetValue(TKey key)
	{
		return default;
	}

	public IEnumerable<KeyValuePair<TKey, TValue>> GetAll()
	{
		yield return default;
	}

	public void ResetState()
	{ }

}

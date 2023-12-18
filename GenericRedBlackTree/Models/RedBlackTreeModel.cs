using DataStructures.Balancers;
using DataStructures.Exceptions;
using DataStructures.Interfaces;
using DataStructures.Nodes;

using Factories;

using System;
using System.Collections.Generic;

namespace DataStructures.Models;

public class RedBlackTreeModel<TKey, TValue> : ITreeModel<TKey, TValue, RedBlackNode<TKey, TValue>> where TKey : IComparable<TKey>
{
    private RedBlackNode<TKey, TValue> _rootNode;
    private readonly Func<TKey, TValue, RedBlackNode<TKey, TValue>> _nodeFactory;
    private readonly ITraverser<TKey, TValue, RedBlackNode<TKey, TValue>> _traverser;

    public RedBlackTreeModel(ITraverser<TKey, TValue, RedBlackNode<TKey, TValue>> traverser)
    {
        _traverser = traverser;
        _nodeFactory = (key, value) => PoolFactory.Create(() => new RedBlackNode<TKey, TValue>(key, value));
        _rootNode = _nodeFactory(default, default);
    }

    public int Depth { get; private set; }
    public int Width { get; private set; }

    public bool IsEmpty => _rootNode is null || _rootNode.Value.Equals(default);
    public RedBlackNode<TKey, TValue> RootNode => _rootNode;
    public Func<TKey, TValue, RedBlackNode<TKey, TValue>> NodeFactory => _nodeFactory;
    public ITraverser<TKey, TValue, RedBlackNode<TKey, TValue>> Traverser => _traverser;

	public void Insert(TKey key, TValue value)
    {
		var nodeToInsert = _nodeFactory(key, value);

		if (_rootNode.IsNil)
        {
            _rootNode = nodeToInsert;
            _rootNode.IsRed = false;
            return;
        }
        
		if (!_traverser.Insert(ref _rootNode, nodeToInsert))
            throw new InsertTraversalException(new KeyValuePair<object, object>(key, value));
    }

	public void Remove(TKey key) => _traverser.Remove(_rootNode, key);

	public void Update(TKey key, TValue value)
    {

    }

	public TValue GetValue(TKey key) => _traverser.GetValue(_rootNode, key);

	public IEnumerable<KeyValuePair<TKey, TValue>> GetAll()
    {
        foreach (var item in Traverser.GetAll(_rootNode))
        {
            yield return item;
        }
    }

    public IEnumerable<KeyValuePair<TKey, TValue>> Search(Func<TKey, bool> condition)
    {
        foreach (var item in Traverser.Search(_rootNode, condition))
        {
            yield return item;
        }
    }

    public void MapTree()
    {
        _traverser.MapTree(_rootNode);
    }

    public void ResetState()
    {

    }

}

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
    private readonly Func<RedBlackNode<TKey, TValue>> _nodeFactory;
    private readonly ITraverser<TKey, TValue, RedBlackNode<TKey, TValue>> _traverser;
    private readonly IBalancer<TKey, TValue, RedBlackNode<TKey, TValue>> _balancer;

    public RedBlackTreeModel(ITraverser<TKey, TValue, RedBlackNode<TKey, TValue>> traversalComponent)
    {
        _traverser = traversalComponent;
        _nodeFactory = () => PoolFactory.Create(() => new RedBlackNode<TKey, TValue>());
    }

    public bool IsEmpty => _rootNode == null || _rootNode.Value.Equals(default);
    public INode<TKey, TValue> RootNode => _rootNode;
    public Func<INode<TKey, TValue>> NodeFactory => _nodeFactory;
    public ITraverser<TKey, TValue, RedBlackNode<TKey, TValue>> Traverser => _traverser;
    public IBalancer<TKey, TValue, RedBlackNode<TKey, TValue>> Balancer => _balancer;

	public void Insert(TKey key, TValue value)
    {
        var newNode = _nodeFactory();

        newNode.Key = key;
        newNode.Value = value;
        newNode.IsRed = true;
        newNode.Nodes = new RedBlackNode<TKey, TValue>[_rootNode.MaxSubNodes];

        // Assuming _rootNode is initialized appropriately
        if (Traverser.InsertNew(_rootNode, newNode))
        {
            if (!Balancer.AfterInsert(ref _rootNode, newNode))
            {
                throw new TreeBalanceException();
            }
			return;
		}
        throw new InsertTraversalException();
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
        foreach (var item in Traverser.GetAll((RedBlackNode<TKey, TValue>)_rootNode))
        {
            yield return item;
        }
    }

    public void ResetState()
    {

    }

}

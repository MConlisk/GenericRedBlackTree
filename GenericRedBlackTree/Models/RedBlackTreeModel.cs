using DataStructures.Balancers;
using DataStructures.Exceptions;
using DataStructures.Interfaces;
using DataStructures.Nodes;

using Factories;

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

namespace DataStructures.Models;

public class RedBlackTreeModel<TKey, TValue> : ITreeModel<TKey, TValue, RedBlackNode<TKey, TValue>> where TKey : IComparable<TKey>
{
    private RedBlackNode<TKey, TValue> _rootNode;
    private readonly Func<RedBlackNode<TKey, TValue>> _nodeFactory;
    private readonly ITraverser<TKey, TValue, RedBlackNode<TKey, TValue>> _traverser;
    private readonly IBalancer<TKey, TValue, RedBlackNode<TKey, TValue>> _balancer;

    public RedBlackTreeModel(ITraverser<TKey, TValue, RedBlackNode<TKey, TValue>> traverser, RedBlackBalancer<TKey, TValue> balancer)
    {
        _traverser = traverser;
        _balancer = balancer;
        _nodeFactory = () => PoolFactory.Create(() => new RedBlackNode<TKey, TValue>());
    }

    public bool IsEmpty => _rootNode == null || _rootNode.Value.Equals(default);
    public RedBlackNode<TKey, TValue> RootNode => _rootNode;
    public Func<RedBlackNode<TKey, TValue>> NodeFactory => _nodeFactory;
    public ITraverser<TKey, TValue, RedBlackNode<TKey, TValue>> Traverser => _traverser;
    public IBalancer<TKey, TValue, RedBlackNode<TKey, TValue>> Balancer => _balancer;

	public void Insert(TKey key, TValue value)
    {
        var nodeToInsert = _nodeFactory();

        nodeToInsert.Key = key;
        nodeToInsert.Value = value;
        nodeToInsert.IsRed = true;

		// Assuming _rootNode is initialized appropriately
		if (_traverser.Insert(ref _rootNode, nodeToInsert))
        {
            if (!_balancer.AfterInsert(ref _rootNode, nodeToInsert))
            {
                throw new TreeBalanceException();
            }
			return;
		}
        throw new InsertTraversalException(new KeyValuePair<object, object>(key, value));
    }


    public void Remove(TKey key)
    {
        if(!_traverser.Remove(ref _rootNode, key))
        {
            if (!_balancer.AfterRemoval(ref _rootNode, key))
            {
                throw new TreeBalanceException();
            }
            return;
        }
        throw new RemoveTraversalException(new KeyValuePair<object, object>(key, default));
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

    public void ResetState()
    {

    }

}

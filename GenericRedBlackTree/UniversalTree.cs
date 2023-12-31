﻿using DataStructures.Exceptions;
using DataStructures.Interfaces;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography;

namespace DataStructures;

/// <summary>
/// The Universal Tree is a Tree Structure that can take on the shape of any tree you need it to.
/// By providing a Tree Model you essentially made the tree into that type of Data Structure.
/// This Tree Model takes a Traversal Component, a Nodes type and a Balancing Component.
/// How the model interacts with the data all depends on the different pieces you have decided to add.
/// Additionally, this Tree Structure being as versatile, The KeyValuePair this stores uses generic TKey, and TValue types.
/// And to top it off, The creation and destruction of nodes in this structure is handled by my PoolFactory to further it's performance.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class UniversalTree<TKey, TValue, TNode> 
	where TNode : ITreeNode
	where TKey : IComparable<TKey>
	where TValue : IComparable<TValue>
{
    private readonly bool _acceptsDuplicateKeys;
    private readonly int _maxCapacity;

    private readonly ITreeModel<TKey, TValue, TNode> _treeModel;
    private readonly List<TKey> _index;

    public UniversalTree(ITreeModel<TKey, TValue, TNode> treeModel) : this(treeModel, false, 0) { }
    public UniversalTree(ITreeModel<TKey, TValue, TNode> treeModel, bool acceptDuplicateKeys) : this(treeModel, acceptDuplicateKeys, 0) { }
    public UniversalTree(ITreeModel<TKey, TValue, TNode> treeModel, bool acceptDuplicateKeys, int maxCapacity)
	{
        _maxCapacity = maxCapacity;
        _acceptsDuplicateKeys = acceptDuplicateKeys;
		_treeModel = treeModel;
		_index = new();
	}

    public bool AcceptsDuplicateKeys => _acceptsDuplicateKeys;
    public int MaxCapacity => _maxCapacity;
    public int Count => _index.Count;
    public bool Contains(TKey key) => _index.Contains(key);
    public bool IsEmpty => _treeModel.IsEmpty;

	/// <summary>
	/// The Entrance point to Insert a new KeyValuePair object into the tree. 
	/// </summary>
	/// <param name="key"></param>
	/// <param name="value"></param>
	/// <exception cref="DuplicateKeyException">This Exception is called whe the key provided has already been added. </exception>
	public virtual void Insert(TKey key, TValue value)
	{
		ArgumentNullException.ThrowIfNull(key);
		ArgumentNullException.ThrowIfNull(value);

		if (_index.Contains(key) && !_acceptsDuplicateKeys)
		{
			throw new DuplicateKeyException($"An attempt to insert the Key:{new KeyValuePair<object, object>(key, default)} into the Tree failed because the tree already contains this Key.");
		}
		else
		{
			_index.Add(key);
			_treeModel.Insert(key, value);
		}
	}

	// Public method to remove a _key-, _value pair from the tree
	public virtual void Remove(TKey key)
    {
		ArgumentNullException.ThrowIfNull(key);

		if (!_index.Contains(key))
		{
			throw new KeyNotFoundException($"An Attempt to Remove the Key:{key} failed because the key didn't exist in the tree.");
		}
		else
		{
			_index.Remove(key);
			_treeModel.Remove(key);
		}

	}

    // Public method to update a _key, _value pair in the tree
    public virtual void Update(TKey key, TValue value)
    {
		ArgumentNullException.ThrowIfNull(key);
		ArgumentNullException.ThrowIfNull(value);

		if (!_index.Contains(key))
		{
			throw new KeyNotFoundException($"An Attempt to Update the Key:{key}failed because the key didn't exist in the tree.");
		}
		else
		{
			_treeModel.Update(key, value);
		}
	}

    // Public method to get the _value associated with a _key
    public virtual TValue GetValue(TKey key)
    {
		ArgumentNullException.ThrowIfNull(key);

		if (!_index.Contains(key))
		{
			throw new KeyNotFoundException($"An Attempt to Update the Key:{key}failed because the key didn't exist in the tree.");
		}
		else
		{
			return _treeModel.GetValue(key);
		}
    }

	public virtual void MapTree()
	{
		_treeModel.MapTree();
	}

	public virtual IEnumerable<KeyValuePair<TKey, TValue>> GetAll()
	{
		if (_index.Count <= 0)
		{
			throw new KeyNotFoundException($"GetAll KeyValuePairs in the Tree Failed, The index shows that there are no items in the tree.");
		}
		else
		{
			foreach(var item in  _treeModel.GetAll())
			{
				yield return item;
			}
		}
	}

	public virtual IEnumerable<KeyValuePair<TKey, TValue>> Search(Func<TKey, bool> condition)
	{
		if (_index.Count <= 0)
		{
			throw new KeyNotFoundException($"Unable to Search the Tree, The index shows that there are no items in the tree.");
		}
		else
		{
			foreach (var item in _treeModel.Search(condition))
			{
				yield return item;
			}
		}
	}
}

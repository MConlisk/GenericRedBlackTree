using System;
using System.Collections.Generic;

using DataStructures.Interfaces;

namespace DataStructures.Nodes;

public class RedBlackNode<TKey, TValue> : IRedBlackNode<TKey, TValue> where TKey : IComparable<TKey>
{
	private TKey _key;
	private TValue _value;
	private bool _isRed;
	private Dictionary<string, INode<TKey, TValue>> _nodes;

	public int MaxSubNodes => 4;
	public TKey Key { get => _key; set => _key = value; }
	public TValue Value { get => _value; set => _value = value; }
	public bool IsRed { get => _isRed; set => _isRed = value; }
	public Dictionary<string, INode<TKey, TValue>> Nodes { get => _nodes; set => _nodes = value; }

	public RedBlackNode() : this(default, default) { }
	public RedBlackNode(TKey key, TValue value) 
	{
		ResetState();
		_key = key;
		_value = value;
	}

	public void ResetState()
	{
		Key = default;
		Value = default;

		foreach(var index in _nodes.Keys)
		{
			_nodes[index].ResetState();
		}

		_nodes = new Dictionary<string, INode<TKey, TValue>>
		{
			{ "Parent", default },
			{ "Left", default },
			{ "Right", default }
		};
		IsRed = true;
	}

}

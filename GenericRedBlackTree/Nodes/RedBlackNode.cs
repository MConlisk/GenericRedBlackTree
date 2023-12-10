using System;

namespace DataStructures.Nodes;

public class RedBlackNode<TKey, TValue> : INode<TKey, TValue> where TKey : IComparable<TKey>
{
	private TKey _key;
	private TValue _value;
	private bool _isRed;
	private INode<TKey, TValue>[] _nodes;

	public int MaxSubNodes => 4; 
	public TKey Key { get => _key; set => _key = value; }
	public TValue Value { get => _value; set => _value = value; }
	public bool IsRed { get => _isRed; set => _isRed = value; }
	public INode<TKey, TValue>[] Nodes { get => _nodes; set => _nodes = value; }

	public void ResetState()
	{
		Key = default;
		Value = default;

		foreach(var node in _nodes as RedBlackNode<TKey, TValue>[])
		{
			node.ResetState();
		}

		Nodes = default;
	}

}

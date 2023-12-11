using System;

namespace DataStructures.Interfaces;

public interface INode<TKey, TValue> where TKey : IComparable<TKey>
{
	int MaxSubNodes { get; }
	TKey Key { get; set; }
	TValue Value { get; set; }
	INode<TKey, TValue>[] Nodes { get; set; }

}
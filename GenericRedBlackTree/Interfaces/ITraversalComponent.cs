using DataStructures.Nodes;

using System;
using System.Collections.Generic;

namespace DataStructures.Interfaces;

public interface ITraversalComponent<TKey, TValue> where TKey : IComparable<TKey>
{
    INode<TKey, TValue> KeyTraversal(ref INode<TKey, TValue> startNode, TKey key);
	INode<TKey, TValue> ValueTraversal(ref INode<TKey, TValue> startNode, TValue value);
    bool InsertNew(ref INode<TKey, TValue> startNode, INode<TKey, TKey> newNode);
    IEnumerable<KeyValuePair<TKey, TValue>> IterateThrough(INode<TKey, TValue> startNode); 
}

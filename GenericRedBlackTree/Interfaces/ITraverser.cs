using DataStructures.Nodes;

using System;
using System.Collections.Generic;

namespace DataStructures.Interfaces;

public interface ITraverser<TKey, TValue, TNode> where TKey : IComparable<TKey> where TNode : INode<TKey, TValue>
{
    TNode KeyTraversal(TNode startNode, TKey key);
	TNode ValueTraversal(TNode startNode, TValue value);
    bool InsertNew(TNode startNode, TNode newNode);
    IEnumerable<KeyValuePair<TKey, TValue>> GetAll(TNode startNode); 
}

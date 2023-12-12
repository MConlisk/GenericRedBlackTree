using System;
using System.Collections.Generic;

namespace DataStructures.Interfaces;

public interface ITraverser<TKey, TValue, TNode> where TKey : IComparable<TKey> where TNode : INode<TKey, TValue>
{
    bool Insert(TNode startNode, TNode newNode);
    bool Remove(TNode startNode, TKey key);
    bool Update(TNode startNode, TKey key, TValue value);
    TValue GetValue(TNode startNode, TKey key);
    IEnumerable<KeyValuePair<TKey, TValue>> GetAll(TNode startNode); 
    IEnumerable<KeyValuePair<TKey, TValue>> Search(TNode startNode, Func<TKey, bool> condition);
}

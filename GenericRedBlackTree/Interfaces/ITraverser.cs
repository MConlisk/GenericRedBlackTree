using System;
using System.Collections.Generic;

namespace DataStructures.Interfaces;

public interface ITraverser<TKey, TValue, TNode> where TKey : IComparable<TKey> where TNode : INode<TKey, TValue>
{
    bool Insert(ref TNode currentNode, TNode nodeToInsert);
    bool Remove(ref TNode currentNode, TKey key);
    bool Update(ref TNode currentNode, TKey key, TValue value);
    TValue GetValue(TNode currentNode, TKey key);
    IEnumerable<KeyValuePair<TKey, TValue>> GetAll(TNode currentNode); 
    IEnumerable<KeyValuePair<TKey, TValue>> Search(TNode currentNode, Func<TKey, bool> condition);
}

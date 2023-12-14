using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace DataStructures.Interfaces;

public interface ITraverser<TKey, TValue, TNode> where TKey : IComparable<TKey> where TNode : ITreeNode<TKey, TValue>
{
	IBalancer<TKey, TValue, TNode> Balancer { get; }

    bool Insert(ref TNode currentNode, TNode nodeToInsert);
    bool Remove(TNode currentNode, TKey key);
    bool Update(TNode currentNode, TKey key, TValue value);
    TValue GetValue(TNode currentNode, TKey key);
    IEnumerable<KeyValuePair<TKey, TValue>> GetAll(TNode currentNode); 
    IEnumerable<KeyValuePair<TKey, TValue>> Search(TNode currentNode, Func<TKey, bool> condition);
    
}

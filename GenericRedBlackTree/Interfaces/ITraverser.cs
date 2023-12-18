using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace DataStructures.Interfaces;

public interface ITraverser<TKey, TValue, TNode> 
where TKey : IComparable<TKey>
where TValue : IComparable<TValue>
where TNode : ITreeNode
{
	IBalancer<TKey, TValue, TNode> Balancer { get; }

    bool Insert(ref TNode rootNode, TNode nodeToInsert);
    bool Remove(TNode currentNode, TKey key);
    bool Update(TNode currentNode, TKey key, TValue value);
    TValue GetValue(TNode rootNode, TKey key);
    IEnumerable<KeyValuePair<TKey, TValue>> GetAll(TNode rootNode); 
    IEnumerable<KeyValuePair<TKey, TValue>> Search(TNode currentNode, Func<TKey, bool> condition);
    void MapTree(TNode rootNode);
}

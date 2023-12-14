using System;

namespace DataStructures.Interfaces;

public interface IBalancer<TKey, TValue, TNode> where TKey : IComparable<TKey> where TNode : ITreeNode<TKey, TValue>
{
	bool AfterInsert(ref TNode currentNode, TNode nodeToInsert);
	bool AfterRemoval(ref TNode currentNode, TKey removedKey);
}

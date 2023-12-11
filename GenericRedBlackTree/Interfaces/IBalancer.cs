using System;

namespace DataStructures.Interfaces;

public interface IBalancer<TKey, TValue, TNode> where TKey : IComparable<TKey> where TNode : INode<TKey, TValue>
{
	bool AfterInsert(ref TNode startNode, TNode newNode);
	bool AfterRemoval(ref TNode startNode, TNode replacementNode);
}

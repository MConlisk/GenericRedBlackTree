using System;
using System.Collections.Generic;

namespace DataStructures.Interfaces;


/// <summary>
/// A Tree Component will be responsible for how the tree interacts with the nodes.
/// The Tree will directly interact with this component for all transactions.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public interface ITreeModel<TKey, TValue, TNode> where TKey : IComparable<TKey> where TNode : ITreeNode<TKey, TValue>
{
	bool IsEmpty { get; }
	ITraverser<TKey, TValue, TNode> Traverser { get; }
	TNode RootNode { get; }
	Func<TKey, TValue, TNode> NodeFactory { get; }

    void Insert(TKey key, TValue value);
	void Update(TKey key, TValue value);
	void Remove(TKey key);
	TValue GetValue(TKey key);
	IEnumerable<KeyValuePair<TKey, TValue>> GetAll();
	IEnumerable<KeyValuePair<TKey, TValue>> Search(Func<TKey, bool> condition);
}

using DataStructures.Nodes;

using System;
using System.Collections.Generic;

namespace DataStructures.Interfaces;


/// <summary>
/// A Tree Component will be responsible for how the tree interacts with the nodes.
/// The Tree will directly interact with this component for all transactions.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public interface ITreeModel<TKey, TValue> where TKey : IComparable<TKey>
{
	bool IsEmpty { get; }
	ITraversalComponent<TKey, TValue> TraversalComponent { get; }
	INode<TKey, TValue> RootNode { get; }
	Func<INode<TKey, TValue>> NodeFactory { get; }

    void Insert(TKey key, TValue value);
	void Update(TKey key, TValue value);
	void Remove(TKey key);
	TValue GetValue(TKey key);
	IEnumerable<KeyValuePair<TKey, TValue>> GetAll();
}

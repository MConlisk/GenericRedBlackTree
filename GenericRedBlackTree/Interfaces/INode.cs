using Factories.Interfaces;

using System;
using System.Collections.Generic;

namespace DataStructures.Interfaces;

public interface INode<TKey, TValue> : IRecyclable where TKey : IComparable<TKey>
{
	int MaxSubNodes { get; }
	TKey Key { get; set; }
	TValue Value { get; set; }

	Dictionary<string, INode<TKey, TValue>> Nodes { get; set; }

}
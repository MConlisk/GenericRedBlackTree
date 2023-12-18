using DataStructures.Balancers;
using DataStructures.Events;
using DataStructures.Nodes;

using Factories.Interfaces;

using Microsoft.Diagnostics.Tracing;

using System;
using System.Runtime.Serialization;

namespace DataStructures.Interfaces;


public interface IRedBlackNode<TKey, TNode> : ITreeNode
	where TKey : IComparable<TKey>
	where TNode : ITreeNode
{
	TKey Key { get; set; }
	TNode Left { get; set; }
	TNode Right { get; set; }
	TNode Parent { get; set; }
	bool IsRed { get; set; }

	event EventHandler<NodeChangedEventArgs<ITreeNode>> NodeChanged;
}

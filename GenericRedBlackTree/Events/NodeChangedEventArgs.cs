using DataStructures.Interfaces;

using System;

namespace DataStructures.Events
{
		/// <summary>
		/// Event arguments for the NodeChanged event.
		/// </summary>
		public class NodeChangedEventArgs<TNode> : EventArgs 
		where TNode : ITreeNode
	{
			/// <summary>
			/// Gets the node associated with the event.
			/// </summary>
			public TNode Node { get; }

			/// <summary>
			/// Initializes a new instance of the <see cref="NodeChangedEventArgs{T}"/> class.
			/// </summary>
			/// <param name="node">The node associated with the event.</param>
			public NodeChangedEventArgs(TNode node)
			{
				Node = node;
			}
		}
}

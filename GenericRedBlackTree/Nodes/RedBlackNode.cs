using DataStructures.Interfaces;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataStructures.Nodes
{
    /// <summary>
    /// Represents a node in a Red-Black Tree.
    /// </summary>
    [Serializable]
	public class RedBlackNode<TKey, TValue> : ISerializable, IEquatable<RedBlackNode<TKey, TValue>>, IRedBlackNode, ITreeNode<TKey, TValue> where TKey : IComparable<TKey>
	{
		/// <summary>
		/// Event triggered when the node is changed.
		/// </summary>
		public event EventHandler<NodeChangedEventArgs<TKey, TValue>> NodeChanged;

		/// <summary>
		/// Gets or sets the key of the node.
		/// </summary>
		public TKey Key { get; set; }

		/// <summary>
		/// Gets or sets the value of the node.
		/// </summary>
		public TValue Value { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the node is red.
		/// </summary>
		public bool IsRed { get; set; }

		/// <summary>
		/// Gets a value indicating whether the node is nil.
		/// </summary>
		public bool IsNil => Value is null;

		/// <summary>
		/// Gets or sets the child nodes.
		/// </summary>
		public Dictionary<string, RedBlackNode<TKey, TValue>> Nodes { get; set; } = new Dictionary<string, RedBlackNode<TKey, TValue>>
		{
			["Parent"] = null,
			["Left"] = null,
			["Right"] = null
		};

		/// <summary>
		/// Gets a value indicating whether the node is empty.
		/// </summary>
		public bool IsEmpty => IsNil;

		/// <summary>
		/// Gets or sets the size of the node.
		/// </summary>
		public int Size { get; set; }

		/// <summary>
		/// Gets or sets the height of the node.
		/// </summary>
		public int Height { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="RedBlackNode{TKey, TValue}"/> class.
		/// </summary>
		/// <param name="key">The key of the node.</param>
		/// <param name="value">The value of the node.</param>
		/// <param name="keyComparer">The key comparer.</param>
		public RedBlackNode(TKey key, TValue value)
		{
			ArgumentNullException.ThrowIfNull(nameof(key));

			Key = key;
			Value = value;
			IsRed = true;

			Size = 1;
			Height = 1;

			OnNodeChanged();
		}

		/// <summary>
		/// Invokes the NodeChanged event.
		/// </summary>
		private void OnNodeChanged()
		{
			NodeChanged?.Invoke(this, new NodeChangedEventArgs<TKey, TValue>(this));
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current node.
		/// </summary>
		/// <param name="obj">The object to compare with the current node.</param>
		/// <returns>true if the specified object is equal to the current node; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			return Equals(obj as RedBlackNode<TKey, TValue>);
		}

		/// <summary>
		/// Determines whether the specified node is equal to the current node.
		/// </summary>
		/// <param name="other">The node to compare with the current node.</param>
		/// <returns>true if the specified node is equal to the current node; otherwise, false.</returns>
		public bool Equals(RedBlackNode<TKey, TValue> other)
		{
			return other != null &&
				   Key.Equals(other.Key) &&
				   Value.Equals(other.Value);
		}

		/// <summary>
		/// Serves as the default hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return HashCode.Combine(Key, Value);
		}

		/// <summary>
		/// Returns a string representation of the node.
		/// </summary>
		/// <returns>A string representation of the node.</returns>
		public override string ToString()
		{
			return $"Node: Key={Key}, Value={Value}, IsRed={IsRed}, Size={Size}, Height={Height}";
		}

		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> with the data needed to serialize the node.
		/// </summary>
		/// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
		/// <param name="context">The destination for this serialization.</param>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(nameof(Key), Key);
			info.AddValue(nameof(Value), Value);
			info.AddValue(nameof(IsRed), IsRed);
			info.AddValue(nameof(Nodes), Nodes);
			info.AddValue(nameof(Size), Size);
			info.AddValue(nameof(Height), Height);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RedBlackNode{TKey, TValue}"/> class during deserialization.
		/// </summary>
		/// <param name="info">The <see cref="SerializationInfo"/> to read data from.</param>
		/// <param name="context">The source for this deserialization.</param>
		protected RedBlackNode(SerializationInfo info, StreamingContext context)
		{
			Key = (TKey)info.GetValue(nameof(Key), typeof(TKey));
			Value = (TValue)info.GetValue(nameof(Value), typeof(TValue));
			IsRed = info.GetBoolean(nameof(IsRed));
			Nodes = (Dictionary<string, RedBlackNode<TKey, TValue>>)info.GetValue(nameof(Nodes), typeof(Dictionary<string, RedBlackNode<TKey, TValue>>));
			Size = info.GetInt32(nameof(Size));
			Height = info.GetInt32(nameof(Height));

			OnNodeChanged();
		}

		/// <summary>
		/// Event arguments for the NodeChanged event.
		/// </summary>
		public class NodeChangedEventArgs<TK, TV> : EventArgs
			where TK : IComparable<TK>
		{
			/// <summary>
			/// Gets the node associated with the event.
			/// </summary>
			public RedBlackNode<TK, TV> Node { get; }

			/// <summary>
			/// Initializes a new instance of the <see cref="NodeChangedEventArgs{TKey, TValue}"/> class.
			/// </summary>
			/// <param name="node">The node associated with the event.</param>
			public NodeChangedEventArgs(RedBlackNode<TK, TV> node)
			{
				Node = node;
			}
		}

		/// <summary>
		/// Resets the state of the node.
		/// </summary>
		public void ResetState()
		{
			Key = default;
			Value = default;
			Nodes = new Dictionary<string, RedBlackNode<TKey, TValue>>
			{
				["Parent"] = null,
				["Left"] = null,
				["Right"] = null
			};
			IsRed = true;
			Size = 1;
			Height = 1;

			OnNodeChanged();
		}
	}
}

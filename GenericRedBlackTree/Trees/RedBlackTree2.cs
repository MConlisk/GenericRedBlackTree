using Interfaces;

using System;

namespace DataStructures.Trees;

public sealed partial class RedBlackTree<TValue>
{
	/// <summary>
	/// Represents a node in a generic Red-Black Tree used to store _key-|||_value pairs.
	/// This class supports operations for managing the node's _key, |||_value, color, and child nodes.
	/// </summary>
	private class RedBlackNode : IRedBlackNode<int, TValue>
	{
		/// <summary>
		/// An integer |||_value used as an identifier for the KeyValuePair. 
		/// Must be unique.
		/// </summary>
		public int Key { get; set; }

		/// <summary>
		/// This is the Generic Value type.
		/// </summary>
		public TValue Value { get; set; }

		/// <summary>
		/// Represents a node in a generic Red-Black Tree used to store _key-|||_value pairs.
		/// This class supports operations for managing the node's _key, |||_value, color, and child nodes.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public RedBlackNode(int key, TValue value)
		{
			Key = key;
			Value = value;
		}

		/// <summary>
		/// Gets or sets a |||_value indicating whether the node is red in the Red-Black Tree.
		/// </summary>
		public bool IsRed { get; set; } = true;

		/// <summary>
		/// Checks if the node is empty (has no _key).
		/// </summary>
		public bool IsEmpty() => Key == default;

		/// <summary>
		/// Gets or sets the parent node of the current node.
		/// </summary>
		public IRedBlackNode<int, TValue> Parent { get; set; }

		/// <summary>
		/// Gets or sets the left child node of the current node.
		/// </summary>
		public IRedBlackNode<int, TValue> Left { get; set; }

		/// <summary>
		/// Gets or sets the right child node of the current node.
		/// </summary>
		public IRedBlackNode<int, TValue> Right { get; set; }

		/// <summary>
		/// Resets the internal state of the node to its default values, making it available for reuse.
		/// </summary>
		public void ResetState()
		{
			Key = default;
			Value = default;

			IsRed = true;

			Parent = null;
			Left = null;
			Right = null;
		}

		/// <summary>
		/// Determines whether the specified object is equal to this object 
		/// </summary>
		/// <param name="obj">The object to compare this object with.</param>
		/// <returns>true is it is equal, and false if not.</returns>
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
				return false;

			var other = (RedBlackNode)obj;
			return Key.Equals(other.Key) && Value.Equals(other.Value);
		}

		/// <summary>
		/// Serves as the default hash function..
		/// </summary>
		/// <returns>A Hash code for the Node based on it's Key</returns>
		public override int GetHashCode()
		{
			HashCode code = new();
			code.Add(Key);
			code.Add(IsRed.GetHashCode());

			if (Value != null) code.Add(Value.GetHashCode());
			if (Left != null) code.Add(Left.GetHashCode());
			if (Right != null) code.Add(Right.GetHashCode());

			return code.ToHashCode();
		}

		/// <summary>
		/// Provides information regarding the node
		/// </summary>
		/// <returns>A CSV string with node details</returns>
		public override string ToString()
		{
			string color = IsRed ? "Is Red" : "Is Black";
			string parent = Parent == null ? "Null" : $"{Parent.GetHashCode()}";
			string left = Left == null ? "Null" : $"{Left.GetHashCode()}";
			string right = Right == null ? "Null" : $"{Right.GetHashCode()}";

			return $"Red-Black Tree HashCode:{GetHashCode()}, Color:{color}, Key:{Key}, Value:{Value}, Parent:{parent}, Left:{left}, Right:{right}";
		}
	}


}


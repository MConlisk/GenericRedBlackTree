
using RedBlackTree.Interfaces;
using RedBlackTree.Nodes;

using System;

namespace RedBlackTree.Trees;


public sealed partial class GenericRedBlackTree<TValue>
{
	/// <summary>
	/// Represents a node in a generic Red-Black Tree used to store key-value pairs.
	/// This class supports operations for managing the node's key, value, color, and child nodes.
	/// </summary>
	private class GenericRedBlackTreeNode : IGenericRedBlackNode<int, TValue>
	{
		/// <summary>
		/// An integer value used as an identifier for the KeyValuePair. 
		/// Must be unique.
		/// </summary>
		public int Key { get; set; }

		/// <summary>
		/// This is the Generic Value type.
		/// </summary>
		public TValue Value { get; set; }

		/// <summary>
		/// Represents a node in a generic Red-Black Tree used to store key-value pairs.
		/// This class supports operations for managing the node's key, value, color, and child nodes.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public GenericRedBlackTreeNode(int key, TValue value)
		{
			Key = key;
			Value = value;
		}

		/// <summary>
		/// Gets or sets a value indicating whether the node is red in the Red-Black Tree.
		/// </summary>
		public bool IsRed { get; set; } = true;

		/// <summary>
		/// Checks if the node is empty (has no key).
		/// </summary>
		public bool IsEmpty() => Key == default;

		/// <summary>
		/// Gets or sets the parent node of the current node.
		/// </summary>
		public IGenericRedBlackNode<int, TValue> Parent { get; set; }

		/// <summary>
		/// Gets or sets the left child node of the current node.
		/// </summary>
		public IGenericRedBlackNode<int, TValue> Left { get; set; }

		/// <summary>
		/// Gets or sets the right child node of the current node.
		/// </summary>
		public IGenericRedBlackNode<int, TValue> Right { get; set; }

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

			var other = (GenericRedBlackTreeNode<TValue>)obj;
			return Key.Equals(other.Key) && Value.Equals(other.Value);
		}

		/// <summary>
		/// Serves as the default hash function..
		/// </summary>
		/// <returns>A Hash code for the Node based on it's Key</returns>
		public override int GetHashCode()
		{
			return HashCode.Combine(Key);
		}

		/// <summary>
		/// Provides information regarding the node
		/// </summary>
		/// <returns>A CSV of the nodes details</returns>
		public override string ToString()
		{
			string color = IsRed ? "Is Red" : "Is Black";
			string parent = Parent == null ? "Null" : $"{Parent.GetHashCode()}";
			string left = Left == null ? "Null" : $"{Left.GetHashCode()}";
			string right = Right == null ? "Null" : $"{Right.GetHashCode()}";

			return $"HashCode:{GetHashCode()}, Color:{color}, Key:{Key}, Value:{Value}, Parent:{parent}, Left:{left}, Right:{right}";
		}
	}


}


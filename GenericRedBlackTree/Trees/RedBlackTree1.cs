using System;
using System.Collections;
using System.Collections.Generic;

namespace DataStructures.Trees;

public sealed partial class RedBlackTree<TValue> 
{
	private class RedBlackTEnumerator :  VIEnumerable<TKey>, IEnumerator<TValue>
	{
		private Stack<RedBlackNode> stack;
		object IEnumerator.Current => Current;

		public RedBlackTEnumerator(RedBlackNode root)
		{
			stack = new Stack<RedBlackNode>();
			TraverseLeft(root);
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through the elements of the Red-Black Tree in an in-order traversal.
		/// </summary>
		/// <returns>An enumerator that can be used to iterate through the elements of the Red-Black Tree in an in-order traversal.</returns>
		public IEnumerator<KeyValuePair<int, TValue>> GetEnumerator()
		{
			return new RedBlackTEnumerator(_root);
		}

		private void TraverseLeft(RedBlackNode node)
		{
			while (node != null)
			{
				stack.Push(node);
				node = (RedBlackNode)node.Left;
			}
		}

		public KeyValuePair<int, TValue> Current
		{
			get
			{
				if (stack.Count == 0)
					throw new InvalidOperationException("Enumeration has not started or is already finished.");

				var current = stack.Peek();
				return new KeyValuePair<int, TValue>(current.Key, current.Value);
			}
		}



		public bool MoveNext()
		{
			if (stack.Count == 0)
				return false;

			var current = stack.Pop();

			// Traverse the right subtree
			TraverseLeft((RedBlackNode)current.Right);

			return true;
		}

		public void Reset()
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			stack.Clear();
			stack = null;
		}
	}
}
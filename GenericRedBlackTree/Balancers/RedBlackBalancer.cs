using DataStructures.Interfaces;
using DataStructures.Nodes;

using System;

namespace DataStructures.Balancers
{
	public partial class RedBlackBalancer<TKey, TValue> : IBalancer<TKey, TValue, ITreeNode>
	where TKey : IComparable<TKey>
	where TValue : IComparable<TValue>
	{
		public bool AfterInsert(ref RBBaseNode<TKey, TValue> currentNode, RBBaseNode<TKey, TValue> nodeToInsert)
		{
			if (currentNode is null || !currentNode.IsRed)
			{
				return false;
			}

			RBBaseNode<TKey, TValue> parentNode = currentNode.Parent;
			RBBaseNode<TKey, TValue> grandparent = parentNode?.Parent;

			if (parentNode is null)
			{
				currentNode.IsRed = false;
			}
			else if (parentNode.IsRed)
			{
				RBBaseNode<TKey, TValue> uncle = FamilyHandling.GetUncle(parentNode, grandparent);

				if (uncle != null && uncle.IsRed)
				{
					parentNode.IsRed = false;
					uncle.IsRed = false;
					grandparent.IsRed = true;
					AfterInsert(ref grandparent, nodeToInsert);
					return true;
				}
				else
				{
					if (FamilyHandling.IsLeftChild(parentNode, grandparent) != FamilyHandling.IsLeftChild(currentNode, parentNode))
					{
						Rotations.PerformRotation(currentNode, parentNode, grandparent);
						parentNode = currentNode.Parent;
					}

					grandparent.IsRed = true;
					parentNode.IsRed = false;
					Rotations.PerformRotation(parentNode, grandparent, FamilyHandling.GetGreatGrandparent(grandparent));

					return true;
				}
			}

			if (grandparent?.Parent is null)
			{
				currentNode.IsRed = false;
			}

			return false;
		}

		public void FixInsertion(ref RBBaseNode<TKey, TValue> rootNode, RBBaseNode<TKey, TValue> currentNode)
		{
			while (currentNode.Parent != null && currentNode.Parent.IsRed)
			{
				if (currentNode.Parent == currentNode.Parent.Parent.Left)
				{
					var uncle = currentNode.Parent.Parent.Right;

					if (uncle != null && uncle.IsRed)
					{
						currentNode.Parent.IsRed = false;
						uncle.IsRed = false;
						currentNode.Parent.Parent.IsRed = true;
						currentNode = currentNode.Parent.Parent;
					}
					else
					{
						if (currentNode == currentNode.Parent.Right)
						{
							currentNode = currentNode.Parent;
							RotateLeft(ref rootNode, currentNode);
						}

						currentNode.Parent.IsRed = false;
						currentNode.Parent.Parent.IsRed = true;
						RotateRight(ref rootNode, currentNode.Parent.Parent);
					}
				}
				else
				{
					var uncle = currentNode.Parent.Parent.Left;

					if (uncle != null && uncle.IsRed)
					{
						currentNode.Parent.IsRed = false;
						uncle.IsRed = false;
						currentNode.Parent.Parent.IsRed = true;
						currentNode = currentNode.Parent.Parent;
					}
					else
					{
						if (currentNode == currentNode.Parent.Left)
						{
							currentNode = currentNode.Parent;
							RotateRight(ref rootNode, currentNode);
						}

						currentNode.Parent.IsRed = false;
						currentNode.Parent.Parent.IsRed = true;
						RotateLeft(ref rootNode, currentNode.Parent.Parent);
					}
				}
			}

			rootNode.IsRed = false;
		}

		private void RotateLeft(ref RBBaseNode<TKey, TValue> rootNode, RBBaseNode<TKey, TValue> currentNode)
		{
			var pivot = currentNode.Right;
			currentNode.Right = pivot.Left;

			if (pivot.Left != null)
			{
				pivot.Left.Parent = currentNode;
			}

			pivot.Parent = currentNode.Parent;

			if (currentNode.Parent == null)
			{
				rootNode = pivot;
			}
			else if (currentNode == currentNode.Parent.Left)
			{
				currentNode.Parent.Left = pivot;
			}
			else
			{
				currentNode.Parent.Right = pivot;
			}

			pivot.Left = currentNode;
			currentNode.Parent = pivot;
		}

		private void RotateRight(ref RBBaseNode<TKey, TValue> rootNode, RBBaseNode<TKey, TValue> currentNode)
		{
			var pivot = currentNode.Left;
			currentNode.Left = pivot.Right;

			if (pivot.Right != null)
			{
				pivot.Right.Parent = currentNode;
			}

			pivot.Parent = currentNode.Parent;

			if (currentNode.Parent == null)
			{
				rootNode = pivot;
			}
			else if (currentNode == currentNode.Parent.Right)
			{
				currentNode.Parent.Right = pivot;
			}
			else
			{
				currentNode.Parent.Left = pivot;
			}

			pivot.Right = currentNode;
			currentNode.Parent = pivot;
		}

		public bool AfterRemoval(ref RBBaseNode<TKey, TValue> currentNode, TKey removedKey)
		{
			if (currentNode is null)
			{
				return false;
			}

			var replacementNode = DetermineReplacementNode(FindNodeToRemove(currentNode, removedKey));

			if (replacementNode is not null && replacementNode.IsRed)
			{
				replacementNode.IsRed = false;
				return true;
			}

			RBBaseNode<TKey, TValue> parentNode = currentNode.Parent;
			bool isLeftChild = FamilyHandling.IsLeftChild(currentNode, parentNode);

			if (parentNode is null)
			{
				Console.WriteLine($"AfterRemoval found the FamilyHandling<TKey, TValue>.GetParent(current) currentNode is Null, currentNode: Key={currentNode.Key}, Value={currentNode.Value}, IsRed={currentNode.IsRed}");
				return false;
			}

			RBBaseNode<TKey, TValue> sibling = FamilyHandling.GetSibling(parentNode, isLeftChild);
			RBBaseNode<TKey, TValue> leftNephew = sibling?.Left;
			RBBaseNode<TKey, TValue> rightNephew = sibling?.Right;

			if (HandleBlackSiblingCase(parentNode, sibling, leftNephew, rightNephew))
			{
				return true;
			}

			if (HandleRedSiblingCase(parentNode, sibling))
			{
				return true;
			}

			FamilyHandling.HandleBlackSiblingLeftNephewRedCase(ref parentNode, isLeftChild);
			FamilyHandling.HandleBlackSiblingRightNephewRedCase(sibling, rightNephew, parentNode, isLeftChild);

			return true;
		}

		private bool HandleBlackSiblingCase(RBBaseNode<TKey, TValue> parentNode, RBBaseNode<TKey, TValue> sibling, RBBaseNode<TKey, TValue> leftNephew, RBBaseNode<TKey, TValue> rightNephew)
		{
			if (sibling is not null && !sibling.IsRed && (leftNephew is null || !leftNephew.IsRed) && (rightNephew is null || !rightNephew.IsRed))
			{
				sibling.IsRed = true;
				return AfterRemoval(ref parentNode, default);
			}

			if (parentNode.IsRed && (sibling is null || !sibling.IsRed) && (leftNephew is null || !leftNephew.IsRed) && (rightNephew is null || !rightNephew.IsRed))
			{
				parentNode.IsRed = false;
				sibling.IsRed = true;
				return true;
			}

			return false;
		}

		private static RBBaseNode<TKey, TValue> FindNodeToRemove(RBBaseNode<TKey, TValue> currentNode, TKey removedKey)
		{
			while (currentNode is not null)
			{
				int comparisonResult = removedKey.CompareTo(currentNode.Key);

				if (comparisonResult is 0)
				{
					return currentNode;
				}
				else if (comparisonResult < 0)
				{
					currentNode = currentNode.Left;
				}
				else
				{
					currentNode = currentNode.Right;
				}
			}

			return null;
		}

		private static RBBaseNode<TKey, TValue> DetermineReplacementNode(RBBaseNode<TKey, TValue> nodeToRemove)
		{
			if (nodeToRemove.Left is not null && nodeToRemove.Parent != null)
			{
				return FindSuccessor(nodeToRemove.Right);
			}
			else
			{
				return nodeToRemove.Left ?? nodeToRemove.Right;
			}
		}

		private static RBBaseNode<TKey, TValue> FindSuccessor(RBBaseNode<TKey, TValue> node)
		{
			while (node.Left is not null)
			{
				node = node.Left;
			}
			return node;
		}

		private static bool HandleRedSiblingCase(RBBaseNode<TKey, TValue> parentNode, RBBaseNode<TKey, TValue> sibling)
		{
			if (sibling is not null && sibling.IsRed)
			{
				parentNode.IsRed = true;
				sibling.IsRed = false;

				if (FamilyHandling.IsLeftChild(sibling, parentNode))
				{
					Rotations.RotateRight(sibling, parentNode.Parent);
				}
				else
				{
					Rotations.RotateLeft(sibling, parentNode.Parent);
				}

				return true;
			}

			return false;
		}

		public bool AfterInsert(ref ITreeNode currentNode, ITreeNode nodeToInsert)
		{
			throw new NotImplementedException();
		}

		public bool AfterRemoval(ref ITreeNode currentNode, TKey removedKey)
		{
			throw new NotImplementedException();
		}
	}

	public partial class RedBlackBalancer<TKey, TValue> 
	where TKey : IComparable<TKey>
	where TValue : IComparable<TValue>
	{
		internal static class Rotations
		{
			internal static void PerformRotation(RBBaseNode<TKey, TValue> pivot, RBBaseNode<TKey, TValue> parentNode, RBBaseNode<TKey, TValue> grandparent)
			{
				bool isLeftChild = FamilyHandling.IsLeftChild(pivot, parentNode);

				if (isLeftChild)
				{
					RotateRight(pivot, grandparent);
				}
				else
				{
					RotateLeft(pivot, grandparent);
				}

				pivot.IsRed = false;
				grandparent.IsRed = true;
			}

			internal static void RotateLeft(RBBaseNode<TKey, TValue> pivot, RBBaseNode<TKey, TValue> grandparent)
			{
				grandparent.Right = pivot.Left;
				pivot.Left = grandparent;

				pivot.IsRed = false;
				grandparent.IsRed = true;
			}

			internal static void RotateRight(RBBaseNode<TKey, TValue> pivot, RBBaseNode<TKey, TValue> grandparent)
			{
				grandparent.Left = pivot.Parent;
				pivot.Parent = grandparent;

				pivot.IsRed = false;
				grandparent.IsRed = true;
			}
		}
	}

	public partial class RedBlackBalancer<TKey, TValue> 
		where TKey : IComparable<TKey>
		where TValue : IComparable<TValue>
	{
		internal static class FamilyHandling
		{
			internal static RBBaseNode<TKey, TValue> GetGrandparent(RBBaseNode<TKey, TValue> node)
			{
				return GetParent(GetParent(node));
			}

			internal static RBBaseNode<TKey, TValue> GetGreatGrandparent(RBBaseNode<TKey, TValue> node)
			{
				return GetParent(GetGrandparent(node));
			}

			internal static RBBaseNode<TKey, TValue> GetLeftChild(RBBaseNode<TKey, TValue> node)
			{
				return node?.Left;
			}

			internal static RBBaseNode<TKey, TValue> GetParent(RBBaseNode<TKey, TValue> node)
			{
				return node?.Parent;
			}

			internal static RBBaseNode<TKey, TValue> GetRightChild(RBBaseNode<TKey, TValue> node)
			{
				return node?.Right;
			}

			internal static RBBaseNode<TKey, TValue> GetSibling(RBBaseNode<TKey, TValue> parentNode, bool isLeftChild)
			{
				if (isLeftChild)
				{
					return parentNode.Right;
				}
				else
				{
					return parentNode.Left;
				}
			}

			internal static RBBaseNode<TKey, TValue> GetUncle(RBBaseNode<TKey, TValue> parentNode, RBBaseNode<TKey, TValue> grandparent)
			{
				return IsLeftChild(parentNode, grandparent) ? grandparent.Right : grandparent.Left;
			}

			internal static bool HandleBlackSiblingLeftNephewRedCase(ref RBBaseNode<TKey, TValue> parentNode, bool isLeftChild)
			{
				RBBaseNode<TKey, TValue> sibling = GetSibling(GetGrandparent(parentNode), isLeftChild);
				RBBaseNode<TKey, TValue> leftNephew = sibling?.Left;

				if (sibling is not null && !sibling.IsRed && leftNephew is not null && leftNephew.IsRed)
				{
					sibling.IsRed = parentNode.IsRed;
					parentNode.IsRed = false;
					leftNephew.IsRed = false;
					Rotations.PerformRotation(leftNephew, sibling, parentNode);

					if (isLeftChild)
					{
						parentNode.Right = sibling;
					}
					else
					{
						parentNode.Left = sibling;
					}

					return true;
				}

				return false;
			}

			internal static bool HandleBlackSiblingRightNephewRedCase(RBBaseNode<TKey, TValue> sibling, RBBaseNode<TKey, TValue> rightNephew, RBBaseNode<TKey, TValue> parentNode, bool isLeftChild)
			{
				if (sibling is not null && !sibling.IsRed && rightNephew is not null && rightNephew.IsRed)
				{
					sibling.IsRed = parentNode.IsRed;
					parentNode.IsRed = false;
					rightNephew.IsRed = false;

					if (isLeftChild)
					{
						Rotations.RotateRight(rightNephew, parentNode);
					}
					else
					{
						Rotations.RotateLeft(rightNephew, GetGrandparent(parentNode));
					}

					return true;
				}

				return false;
			}

			internal static bool IsLeftChild(RBBaseNode<TKey, TValue> node, RBBaseNode<TKey, TValue> parentNode)
			{
				return parentNode is not null && parentNode.Left == node;
			}
		}
	}
}

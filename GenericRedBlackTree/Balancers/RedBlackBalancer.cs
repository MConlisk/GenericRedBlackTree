using DataStructures.Interfaces;
using DataStructures.Nodes;

using System;

namespace DataStructures.Balancers
{
	public partial class RedBlackBalancer<TKey, TValue> : IBalancer<TKey, TValue, RedBlackNode<TKey, TValue>> where TKey : IComparable<TKey>
	{
		public bool AfterInsert(ref RedBlackNode<TKey, TValue> currentNode, RedBlackNode<TKey, TValue> nodeToInsert)
		{
			if (currentNode is null || !currentNode.IsRed)
			{
				return false;
			}

			RedBlackNode<TKey, TValue> parentNode = currentNode.Nodes["Parent"];
			RedBlackNode<TKey, TValue> grandparent = parentNode?.Nodes["Parent"];

			if (parentNode is null)
			{
				currentNode.IsRed = false;
			}
			else if (parentNode.IsRed)
			{
				RedBlackNode<TKey, TValue> uncle = FamilyHandling.GetUncle(parentNode, grandparent);

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
						parentNode = currentNode.Nodes["Parent"];
					}

					grandparent.IsRed = true;
					parentNode.IsRed = false;
					Rotations.PerformRotation(parentNode, grandparent, FamilyHandling.GetGreatGrandparent(grandparent));

					return true;
				}
			}

			if (grandparent?.Nodes["Parent"] is null)
			{
				currentNode.IsRed = false;
			}

			return false;
		}

		public bool AfterRemoval(ref RedBlackNode<TKey, TValue> currentNode, TKey removedKey)
		{
			if (currentNode is null)
			{
				Console.WriteLine("AfterRemoval found the current node is Null");
				return false;
			}

			var replacementNode = DetermineReplacementNode(FindNodeToRemove(currentNode, removedKey));

			if (replacementNode is not null && replacementNode.IsRed)
			{
				replacementNode.IsRed = false;
				return true;
			}

			RedBlackNode<TKey, TValue> parentNode = currentNode.Nodes["Parent"];
			bool isLeftChild = FamilyHandling.IsLeftChild(currentNode, parentNode);

			if (parentNode is null)
			{
				Console.WriteLine($"AfterRemoval found the FamilyHandling<TKey, TValue>.GetParent(current) node is Null, currentNode: Key={currentNode.Key}, Value={currentNode.Value}, IsRed={currentNode.IsRed}");
				return false;
			}

			RedBlackNode<TKey, TValue> sibling = FamilyHandling.GetSibling(parentNode, isLeftChild);
			RedBlackNode<TKey, TValue> leftNephew = sibling?.Nodes["Left"];
			RedBlackNode<TKey, TValue> rightNephew = sibling?.Nodes["Right"];

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

		private bool HandleBlackSiblingCase(RedBlackNode<TKey, TValue> parentNode, RedBlackNode<TKey, TValue> sibling, RedBlackNode<TKey, TValue> leftNephew, RedBlackNode<TKey, TValue> rightNephew)
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

		private static RedBlackNode<TKey, TValue> FindNodeToRemove(RedBlackNode<TKey, TValue> currentNode, TKey removedKey)
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
					currentNode = currentNode.Nodes["Left"];
				}
				else
				{
					currentNode = currentNode.Nodes["Right"];
				}
			}

			return null;
		}

		private static RedBlackNode<TKey, TValue> DetermineReplacementNode(RedBlackNode<TKey, TValue> nodeToRemove)
		{
			if (nodeToRemove.Nodes["Left"] is not null && nodeToRemove.Nodes["Parent"] != null)
			{
				return FindSuccessor(nodeToRemove.Nodes["Right"]);
			}
			else
			{
				return nodeToRemove.Nodes["Left"] ?? nodeToRemove.Nodes["Right"];
			}
		}

		private static RedBlackNode<TKey, TValue> FindSuccessor(RedBlackNode<TKey, TValue> node)
		{
			while (node.Nodes["Left"] is not null)
			{
				node = node.Nodes["Left"];
			}
			return node;
		}

		private static bool HandleRedSiblingCase(RedBlackNode<TKey, TValue> parentNode, RedBlackNode<TKey, TValue> sibling)
		{
			if (sibling is not null && sibling.IsRed)
			{
				parentNode.IsRed = true;
				sibling.IsRed = false;

				if (FamilyHandling.IsLeftChild(sibling, parentNode))
				{
					Rotations.RotateRight(sibling, parentNode.Nodes["Parent"]);
				}
				else
				{
					Rotations.RotateLeft(sibling, parentNode.Nodes["Parent"]);
				}

				return true;
			}

			return false;
		}
	}

	public partial class RedBlackBalancer<TKey, TValue> where TKey : IComparable<TKey>
	{
		internal static class Rotations
		{
			internal static void PerformRotation(RedBlackNode<TKey, TValue> pivot, RedBlackNode<TKey, TValue> parentNode, RedBlackNode<TKey, TValue> grandparent)
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

			internal static void RotateLeft(RedBlackNode<TKey, TValue> pivot, RedBlackNode<TKey, TValue> grandparent)
			{
				grandparent.Nodes["Right"] = pivot.Nodes["Left"];
				pivot.Nodes["Left"] = grandparent;

				pivot.IsRed = false;
				grandparent.IsRed = true;
			}

			internal static void RotateRight(RedBlackNode<TKey, TValue> pivot, RedBlackNode<TKey, TValue> grandparent)
			{
				grandparent.Nodes["Left"] = pivot.Nodes["Parent"];
				pivot.Nodes["Parent"] = grandparent;

				pivot.IsRed = false;
				grandparent.IsRed = true;
			}
		}
	}

	public partial class RedBlackBalancer<TKey, TValue> where TKey : IComparable<TKey>
	{
		internal static class FamilyHandling
		{
			internal static RedBlackNode<TKey, TValue> GetGrandparent(RedBlackNode<TKey, TValue> node)
			{
				return GetParent(GetParent(node));
			}

			internal static RedBlackNode<TKey, TValue> GetGreatGrandparent(RedBlackNode<TKey, TValue> node)
			{
				return GetParent(GetGrandparent(node));
			}

			internal static RedBlackNode<TKey, TValue> GetLeftChild(RedBlackNode<TKey, TValue> node)
			{
				return node?.Nodes["Left"];
			}

			internal static RedBlackNode<TKey, TValue> GetParent(RedBlackNode<TKey, TValue> node)
			{
				return node?.Nodes["Parent"];
			}

			internal static RedBlackNode<TKey, TValue> GetRightChild(RedBlackNode<TKey, TValue> node)
			{
				return node?.Nodes["Right"];
			}

			internal static RedBlackNode<TKey, TValue> GetSibling(RedBlackNode<TKey, TValue> parentNode, bool isLeftChild)
			{
				if (isLeftChild)
				{
					return parentNode.Nodes["Right"];
				}
				else
				{
					return parentNode.Nodes["Left"];
				}
			}

			internal static RedBlackNode<TKey, TValue> GetUncle(RedBlackNode<TKey, TValue> parentNode, RedBlackNode<TKey, TValue> grandparent)
			{
				return IsLeftChild(parentNode, grandparent) ? grandparent.Nodes["Right"] : grandparent.Nodes["Left"];
			}

			internal static bool HandleBlackSiblingLeftNephewRedCase(ref RedBlackNode<TKey, TValue> parentNode, bool isLeftChild)
			{
				RedBlackNode<TKey, TValue> sibling = GetSibling(GetGrandparent(parentNode), isLeftChild);
				RedBlackNode<TKey, TValue> leftNephew = sibling?.Nodes["Left"];

				if (sibling is not null && !sibling.IsRed && leftNephew is not null && leftNephew.IsRed)
				{
					sibling.IsRed = parentNode.IsRed;
					parentNode.IsRed = false;
					leftNephew.IsRed = false;
					Rotations.PerformRotation(leftNephew, sibling, parentNode);

					if (isLeftChild)
					{
						parentNode.Nodes["Right"] = sibling;
					}
					else
					{
						parentNode.Nodes["Left"] = sibling;
					}

					return true;
				}

				return false;
			}

			internal static bool HandleBlackSiblingRightNephewRedCase(RedBlackNode<TKey, TValue> sibling, RedBlackNode<TKey, TValue> rightNephew, RedBlackNode<TKey, TValue> parentNode, bool isLeftChild)
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

			internal static bool IsLeftChild(RedBlackNode<TKey, TValue> node, RedBlackNode<TKey, TValue> parentNode)
			{
				return parentNode is not null && parentNode.Nodes["Left"] == node;
			}
		}
	}
}

using DataStructures.Nodes;

using System;

namespace DataStructures.Balancers.Helpers;

    internal static class RedBlackRotations<TKey, TValue> where TKey : IComparable<TKey>
    {

        internal static void PerformRotation(RedBlackNode<TKey, TValue> pivot, RedBlackNode<TKey, TValue> parent, RedBlackNode<TKey, TValue> grandparent)
        {
            bool isLeftChild = RedBlackFamilyHandling<TKey, TValue>.IsLeftChild(pivot, parent);

            // Right Rotation
            if (isLeftChild)
            {
                RotateRight(pivot, grandparent);
            }
            // Left Rotation
            else
            {
                RotateLeft(pivot, grandparent);
            }

            pivot.IsRed = false;
            grandparent.IsRed = true;
        }

        internal static void RotateLeft(RedBlackNode<TKey, TValue> pivot, RedBlackNode<TKey, TValue> grandparent)
        {
            grandparent.Nodes[2] = pivot.Nodes[1]; // Nodes[1] = Left Node, Nodes[2] = Right Node
		pivot.Nodes[1] = grandparent; // Nodes[1] = Left Node

		pivot.IsRed = false;
            grandparent.IsRed = true;
        }

        internal static void RotateRight(RedBlackNode<TKey, TValue> pivot, RedBlackNode<TKey, TValue> grandparent)
        {
            grandparent.Nodes[1] = pivot.Nodes[0]; // Nodes[1] = Left Node, Nodes[0] = Parent Node
		pivot.Nodes[0] = grandparent; // Nodes[0] = Parent Node

		pivot.IsRed = false;
            grandparent.IsRed = true;
        }
    }
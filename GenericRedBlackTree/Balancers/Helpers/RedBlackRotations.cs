﻿using DataStructures.Nodes;

using System;

namespace DataStructures.Balancers.Helpers
{
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
            grandparent.Nodes[2] = pivot.Nodes[1];
            pivot.Nodes[1] = grandparent;

            pivot.IsRed = false;
            grandparent.IsRed = true;
        }

        internal static void RotateRight(RedBlackNode<TKey, TValue> pivot, RedBlackNode<TKey, TValue> grandparent)
        {
            grandparent.Nodes[1] = pivot.Nodes[0];
            pivot.Nodes[0] = grandparent;

            pivot.IsRed = false;
            grandparent.IsRed = true;
        }
    }
}
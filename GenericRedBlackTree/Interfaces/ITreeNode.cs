using System;
using System.Collections.Generic;
using DataStructures.Nodes;

namespace DataStructures.Interfaces
{
    public interface ITreeNode<TKey, TValue> where TKey : IComparable<TKey>
    {
        int Height { get; set; }
        bool IsEmpty { get; }
        bool IsNil { get; }
        TKey Key { get; set; }
        Dictionary<string, RedBlackNode<TKey, TValue>> Nodes { get; set; }
        int Size { get; set; }
        TValue Value { get; set; }

        event EventHandler<RedBlackNode<TKey, TValue>.NodeChangedEventArgs<TKey, TValue>> NodeChanged;
    }
}
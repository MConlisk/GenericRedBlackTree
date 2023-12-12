using System;

namespace DataStructures.Interfaces;

public interface IRedBlackNode<TKey,TValue> : INode<TKey, TValue> where TKey : IComparable<TKey>
{
    bool IsRed { get; set; }
}

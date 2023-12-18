using DataStructures.Events;
using DataStructures.Interfaces;
using System;
using System.Collections.Generic;

namespace DataStructures.Nodes;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public abstract class RBBaseNode<TKey, TValue> : IRedBlackNode<TKey, TValue>
	where TKey : IComparable<TKey>
	where TValue : IComparable<TValue>
{
    private TKey _key;
    private TValue _value;
    private char[] _address;

    private Dictionary<string, RBBaseNode<TKey, TValue>> _nodes = new();

    public abstract event EventHandler<NodeChangedEventArgs<RBBaseNode<TKey, TValue>>> NodeChanged;

    public TKey Key
    {
        get => _key;
        set
        {
            _key = value;
        }
    }
    public TValue Value
    {
        get => _value;
        set
        {
            _value = value;
        }
    }
    public bool IsRed { get; set; }

    public char[] Address
    {
        get
        {
            if (_address == null || _address == default)
            {
                _address = new char[100];
                UpdateAddress();
            }
            return _address;
        }
    }
    public bool IsNil { get; }
    public int Level { get; set; }
    public RBBaseNode<TKey, TValue> Parent { get; set; }
    public RBBaseNode<TKey, TValue> Left { get; set; }
    public RBBaseNode<TKey, TValue> Right { get; set; }
    public RBBaseNode<TKey, TValue>[] Children { get; set; }
	TValue IRedBlackNode<TKey, TValue>.Left { get; set; }
	TValue IRedBlackNode<TKey, TValue>.Right { get; set; }
	TValue IRedBlackNode<TKey, TValue>.Parent { get; set; }

	public RBBaseNode(TKey key, TValue value)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);

        Key = key;
        Value = value;
        IsRed = true;
        _nodes = new()
        {
            ["Parent"] = null,
            ["Left"] = null,
            ["Right"] = null
        };
    }

	protected RBBaseNode()
	{
	}

	private bool UpdateAddress()
    {
        if (Parent == null)
        {
            _address = "Root".ToCharArray();
        }

        if (Parent.Left != null && Parent.Left == this)
        {
            _address[Parent.Address.Length + 1] = (char)48;
            return true;
        }
        else if (Parent.Right != null && Parent.Right == this)
        {
            _address[Parent.Address.Length + 1] = (char)49;
            return true;
        }
        Console.WriteLine($"Unable to calculate the new address for node with Key: {_key}");
        return false;
    }

    public abstract void OnNodeChanged();

    public override bool Equals(object obj)
    {
        return base.Equals(obj as RBBaseNode<TKey, TValue>);
    }

    public override int GetHashCode()
    {
        HashCode hashCode = new();

        hashCode.Add(Key);
        hashCode.Add(IsRed);

        return hashCode.ToHashCode(); ;
    }

    public override string ToString()
    {
        string message = $"Red-Black Node at Address:{Address} {(IsRed ? "is Red" : "is Black")} Contains Key:{Key} and Value:{Value}";
        return base.ToString();
    }

    public void ResetState()
    {
        IsRed = true;
        _key = default;
        _value = default;
        _address = default;

    }
}
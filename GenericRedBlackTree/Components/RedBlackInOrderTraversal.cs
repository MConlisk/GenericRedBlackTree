using DataStructures.Interfaces;
using DataStructures.Nodes;

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace DataStructures.Components;

// Example Traversal Component implementation
public class RedBlackInOrderTraversal<TKey, TValue> : ITraversalComponent<TKey, TValue> where TKey : IComparable<TKey>
{
    public INode<TKey, TValue> KeyTraversal(ref INode<TKey, TValue> startNode, TKey key)
    {
        while (startNode != null)
        {
            KeyTraversal(ref startNode.Nodes[2], key);
            if (startNode.Key.Equals(key))
            {
                return startNode;
            }
            KeyTraversal(ref startNode.Nodes[3], key);
        }
        return default;
    }

    public INode<TKey, TValue> ValueTraversal(ref INode<TKey, TValue> startNode, TValue value)
    {
        while (startNode != null)
        {
            ValueTraversal(ref startNode.Nodes[2], value);
            if (startNode.Value.Equals(value))
            {
                return startNode;
            }
            ValueTraversal(ref startNode.Nodes[3], value);
        }
        return default;
    }

    public bool InsertNew(ref INode<TKey, TValue> startNode, INode<TKey, TKey> newNode)
	{

		InsertNew(ref startNode.Nodes[2], newNode);
        if (startNode.Nodes == null)
        {
            startNode.Nodes = new INode<TKey, TValue>[startNode.MaxSubNodes];
            startNode.Nodes[1] = startNode;
            startNode.Nodes[0] = (INode<TKey, TValue>)newNode;
            return true;
        }
		InsertNew(ref startNode.Nodes[3], newNode);

		return false;
	}

    public IEnumerable<KeyValuePair<TKey, TValue>> IterateThrough(INode<TKey, TValue> startNode)
    {
		while (startNode != null)
		{
			IterateThrough(startNode.Nodes[2]);
            yield return new (startNode.Key, startNode.Value);
			IterateThrough(startNode.Nodes[3]);
		}
	}
}

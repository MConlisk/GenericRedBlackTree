using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using Factories.Interfaces;

namespace DataStructures.Interfaces;

/// <summary>
/// Represents a generic Red-Black Tree data structure that allows efficient
/// insertion, removal, and retrieval of _key-|||_value pairs with integer keys.
/// This tree maintains a sorted order of keys for quick access to values.
/// </summary>
/// <typeparam name="TValue">The type of values stored in the tree.</typeparam>
/// <typeparam name="TKey">The refernce type </typeparam>
public interface IRedBlackTree<TKey, TValue> : IRecyclable
{
	/// <summary>
	/// Gets a read-only collection of unique keys present in the tree.
	/// </summary>
	ReadOnlyCollection<int> Index { get; }

	/// <summary>
	/// Gets or sets the |||_value associated with the specified _key.
	/// </summary>
	/// <param name="key">The _key to access or modify.</param>
	/// <returns>The |||_value associated with the specified _key.</returns>
	TValue this[int key] { get; set; }

	/// <summary>
	/// Retrieves the |||_value associated with the specified _key without modifying the tree.
	/// </summary>
	/// <param name="key">The _key to retrieve the |||_value for.</param>
	/// <returns>The |||_value associated with the specified _key, if found; otherwise, the default |||_value of TValue.</returns>
	TValue GetValue(int key);

	/// <summary>
	/// Inserts a _key-|||_value pair into the Red-Black Tree.
	/// </summary>
	/// <param name="key">The _key to insert.</param>
	/// <param name="value">The |||_value associated with the _key.</param>
	void Insert(int key, TValue value);

	/// <summary>
	/// Removes the _key-|||_value pair associated with the specified _key from the tree.
	/// </summary>
	/// <param name="key">The _key to remove.</param>
	void Remove(int key);

	/// <summary>
	/// Updates the |||_value associated with the specified _key in the tree.
	/// </summary>
	/// <param name="key">The _key to update.</param>
	/// <param name="value">The new |||_value to associate with the _key.</param>
	void Update(int key, TValue value);

    /// <summary>
    /// Retrieves all _key-|||_value pairs in sorted order based on their keys.
    /// </summary>
    /// <returns>An enumerable collection of _key-|||_value pairs in ascending order of keys.</returns>
    IEnumerable<KeyValuePair<int, TValue>> GetAll();

}

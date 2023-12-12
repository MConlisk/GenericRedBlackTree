using System;
using System.Collections.Generic;

namespace DataStructures.Exceptions;

public class RemoveTraversalException : ApplicationException
{
	private readonly object _key;
	private readonly object _value;
	private static readonly string _message = "An attempt to remove a KeyValuePair from the tree was unsuccessful.";
	
	public KeyValuePair<object, object> KeyValuePair { get => new(_key, _value); }

	/// <summary>
	/// An attempt to remove a KeyValuePair from the tree was unsuccessful.
	/// </summary>
	public RemoveTraversalException() : this(_message, new InvalidOperationException(), default) { }

	/// <param name="keyValuePair"></param>
	public RemoveTraversalException(KeyValuePair<object, object> keyValuePair) : this(_message, new InvalidOperationException(), keyValuePair) { }

	/// <param name="message"></param>
	public RemoveTraversalException(string message) : this(message, new InvalidOperationException(), default) { }

	/// <param name="message"></param>
	/// <param name="keyValuePair"></param>
	public RemoveTraversalException(string message, KeyValuePair<object, object> keyValuePair) : this(message, new InvalidOperationException(), keyValuePair) { }

	/// <param name="message"></param>
	/// <param name="innerException"></param>
	public RemoveTraversalException(string message, Exception innerException) : this(message, innerException, default) { }

	/// <param name="message"></param>
	/// <param name="innerException"></param>
	/// <param name="keyValuePair"></param>
	public RemoveTraversalException(string message, Exception innerException, KeyValuePair<object, object> keyValuePair) : base(message, innerException) { _key = keyValuePair.Key; _value = keyValuePair.Value; }
}
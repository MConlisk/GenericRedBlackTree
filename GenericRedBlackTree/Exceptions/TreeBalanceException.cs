﻿using System;
using System.Collections.Generic;

namespace DataStructures.Exceptions;

public class TreeBalanceException : ApplicationException
{
	private readonly object _key;
	private readonly object _value;
	private static readonly string _message = "An attempt was made to Balance the tree, but failed to an unknown error.";

	public KeyValuePair<object, object> KeyValuePair { get => new(_key, _value); }

	/// <summary>
	/// An attempt to remove a KeyValuePair from the tree was unsuccessful.
	/// </summary>
	public TreeBalanceException() : this(_message, new InvalidOperationException(), default) { }

	/// <param name="keyValuePair"></param>
	public TreeBalanceException(KeyValuePair<object, object> keyValuePair) : this(_message, new InvalidOperationException(), keyValuePair) { }

	/// <param name="message"></param>
	public TreeBalanceException(string message) : this(message, new InvalidOperationException(), default) { }

	/// <param name="message"></param>
	/// <param name="keyValuePair"></param>
	public TreeBalanceException(string message, KeyValuePair<object, object> keyValuePair) : this(message, new InvalidOperationException(), keyValuePair) { }

	/// <param name="message"></param>
	/// <param name="innerException"></param>
	public TreeBalanceException(string message, Exception innerException) : this(message, innerException, default) { }

	/// <param name="message"></param>
	/// <param name="innerException"></param>
	/// <param name="keyValuePair"></param>
	public TreeBalanceException(string message, Exception innerException, KeyValuePair<object, object> keyValuePair)
	{
		_key = keyValuePair.Key;
		_value = keyValuePair.Value;

		Console.WriteLine($"The Cause of the TreeBalanceException is: Key={_key}, Value={_value}");
	}

}

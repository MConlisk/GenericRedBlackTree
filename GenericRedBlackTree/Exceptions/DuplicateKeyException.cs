using System;
using System.Collections.Generic;

namespace DataStructures.Exceptions;

/// <summary>
/// An attempt was made to add a Key to the index when that Key is already indexed.
/// </summary>
public class DuplicateKeyException : ApplicationException
{
    private readonly KeyValuePair<object, object> _keyPair;
    private static readonly string _message = "An attempt was made to add a Key to the index when that Key is already indexed.";
    /// <summary>
    /// If provided, this is the Key that caused the Exception.
    /// </summary>
    public KeyValuePair<object, object> KeyPair { get => _keyPair; }

    /// <summary>
    /// An attempt was made to add a Key to the index when that Key is already indexed.
    /// </summary>
    public DuplicateKeyException() : base(_message) { }

	/// <summary>
	///  An attempt was made to add a Key to the index when that Key is already indexed.
	/// </summary>
	/// <param name="keyPair"></param>
	public DuplicateKeyException(KeyValuePair<object, object> keyPair) : base(_message + $"Key:{keyPair.Key}, Value:{(keyPair.Value ?? "null")}")
    {
        _keyPair = keyPair;
    }

    /// <param name="message"></param>
    public DuplicateKeyException(string message) : base(message) { }

	/// <param name="message"></param>
	/// <param name="keyPair"></param>
	public DuplicateKeyException(string message, KeyValuePair<object, object> keyPair) : base(message)
    {
        _keyPair = keyPair;
    }

    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public DuplicateKeyException(string message, Exception innerException) : base(message, innerException) { }

	/// <param name="message"></param>
	/// <param name="innerException"></param>
	/// <param name="keyPair"></param>
	public DuplicateKeyException(string message, Exception innerException, KeyValuePair<object, object> keyPair) : base(message, innerException)
    {
        _keyPair = keyPair;
    }
}

using System;

namespace DataStructures.Exceptions;

/// <summary>
/// An attempt was made to add a Key to the index when that Key is already indexed.
/// </summary>
public class DuplicateKeyException : ApplicationException
{
    private readonly object _duplicateKey;

    /// <summary>
    /// If provided, this is the Key that caused the Exception.
    /// </summary>
    public object DuplicateKey { get => _duplicateKey; }

    /// <summary>
    /// An attempt was made to add a Key to the index when that Key is already indexed.
    /// </summary>
    public DuplicateKeyException() : base("An attempt was made to add a Key to the index when that Key is already indexed.") { }

    /// <summary>
    ///  An attempt was made to add a Key to the index when that Key is already indexed.
    /// </summary>
    /// <param name="key"></param>
    public DuplicateKeyException(object key) : base($"An attempt was made to add this Key: {key} to the index when that Key is already indexed.")
    {
        _duplicateKey = key;
    }

    /// <param name="message"></param>
    public DuplicateKeyException(string message) : base(message) { }

    /// <param name="message"></param>
    /// <param name="key"></param>
    public DuplicateKeyException(string message, object key) : base(message)
    {
        _duplicateKey = key;
    }

    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public DuplicateKeyException(string message, Exception innerException) : base(message, innerException) { }

    /// <param name="message"></param>
    /// <param name="innerException"></param>
    /// <param name="key"></param>
    public DuplicateKeyException(string message, Exception innerException, object key) : base(message, innerException)
    {
        _duplicateKey = key;
    }
}
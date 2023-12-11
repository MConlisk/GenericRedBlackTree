using System;

namespace DataStructures.Exceptions;

public class TreeBalanceException : ApplicationException
{
    private static readonly string _message = "An attempt was made to Balance the tree, but failed to an unknown error.";

	/// <summary>
	/// An attempt was made to Balance the tree, but failed to an unknown error.
	/// </summary>
	public TreeBalanceException() : base(_message) { }

	/// <param name="message"></param>
	public TreeBalanceException(string message) : base(message) { }

	/// <param name="message"></param>
	/// <param name="innerException"></param>
	public TreeBalanceException(string message, Exception innerException) : base(message, innerException) { }

}

using System;

namespace DataStructures.Exceptions;

public class InsertTraversalException : ApplicationException
{
	private static readonly string _message = "An attempt was made to insert a KeyValuePair into the tree, but failed to an unknown reason.";

	/// <summary>
	/// An attempt was made to Balance the tree, but failed to an unknown error.
	/// </summary>
	public InsertTraversalException() : base(_message) { }

	/// <param name="message"></param>
	public InsertTraversalException(string message) : base(message) { }

	/// <param name="message"></param>
	/// <param name="innerException"></param>
	public InsertTraversalException(string message, Exception innerException) : base(message, innerException) { }

}
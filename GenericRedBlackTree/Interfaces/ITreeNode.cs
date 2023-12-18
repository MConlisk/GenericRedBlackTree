using System;
using System.Collections.Generic;

using DataStructures.Events;

using Factories.Interfaces;

namespace DataStructures.Interfaces;

/// <summary>
/// 
/// </summary>
public interface ITreeNode : IRecyclable
{
	Char[] Address { get; }
	bool IsNil { get; }
	int Level { get; set; }

}
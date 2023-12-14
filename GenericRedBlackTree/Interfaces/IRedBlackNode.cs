using DataStructures.Balancers;

using Factories.Interfaces;

using System;

namespace DataStructures.Interfaces;

public interface IRedBlackNode : IRecyclable
{
    bool IsRed { get; set; }
    bool IsNil { get; }
}

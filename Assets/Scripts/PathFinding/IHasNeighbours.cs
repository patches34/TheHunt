using System.Collections.Generic;

public interface IHasNeighbors<N>
{
    IEnumerable<N> ReachableNeighbors { get; }
}
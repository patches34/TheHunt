using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A class that handles storing the steps on a path.
/// </summary>
/// <typeparam name="Node">A generic object that represents a step on the path</typeparam>
[Serializable]
public class Path<Node>: IEnumerable<Node>
{
    /// <summary>
    /// The final step of this path
    /// </summary>
    public Node LastStep { get; private set; }

    /// <summary>
    /// A list of all the steps of this path
    /// </summary>
    Path<Node> PreviousSteps { get; set; }

    /// <summary>
    /// The total cost to take this path
    /// </summary>
    public int TotalCost { get; private set; }

    /// <summary>
    /// Initializes a new collection of Path.
    /// </summary>
    /// <param name="lastStep">The last step of this path</param>
    /// <param name="previousSteps">A collection of steps that lead to the last step</param>
    /// <param name="totalCost">The cost of taking this path</param>
    private Path(Node lastStep, Path<Node> previousSteps, int totalCost)
    {
        LastStep = lastStep;
        PreviousSteps = previousSteps;
        TotalCost = totalCost;
    }

    /// <summary>
    /// Initialies a new collection of Path.
    /// </summary>
    /// <param name="start">The last step of the path</param>
    public Path(Node start) : this(start, null, 0) { }

    /// <summary>
    /// Adds a new step to the end of the current path
    /// </summary>
    /// <param name="step">The new last step to add to the path</param>
    /// <param name="stepCost">The cost to move to this new step</param>
    /// <returns>Returns a new collection that has the new step added to the end of it</returns>
    public Path<Node> AddStep(Node step, int stepCost)
    {
        return new Path<Node>(step, this, TotalCost + stepCost);
    }

    /// <summary>
    /// This function allows Path to be used with a foreach loop
    /// </summary>
    /// <returns>Returns an enumerator collection of Path</returns>
    public IEnumerator<Node> GetEnumerator()
    {
        for (var p = this; p != null; p = p.PreviousSteps)
            yield return p.LastStep;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

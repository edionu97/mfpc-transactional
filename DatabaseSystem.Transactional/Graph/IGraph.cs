using System.Collections.Generic;
using DatabaseSystem.Transactional.Graph.Element;

namespace DatabaseSystem.Transactional.Graph
{
    public interface IGraph
    {
        /// <summary>
        /// Get the vertices of the graph
        /// </summary>
        IEnumerable<IGraphElement> Vertices { get; } 

        /// <summary>
        /// This method it is used for pushing an edge into the graph
        /// </summary>
        /// <param name="from">the element that represents the starting point of the graph</param>
        /// <param name="to">the element that represents the ending point of the graph</param>
        void InsertEdge(IGraphElement @from, IGraphElement to);

        /// <summary>
        /// Removes the edge from the graph
        /// </summary>
        /// <param name="from">the element that represents the starting point of the graph</param>
        /// <param name="to">the element that represents the ending point of the graph</param>
        void RemoveEdge(IGraphElement @from, IGraphElement to);

        /// <summary>
        /// Traverse the graph and return the nodes from the dfs
        /// <para name="startPoint">the starting point of the traversal</para>
        /// </summary>
        /// <returns>the list of elements</returns>
        IEnumerable<IGraphElement> TraverseUsingDfs(IGraphElement startPoint);

        /// <summary>
        /// This method it is used for getting the graph cycles
        /// </summary>
        /// <returns>a list of lists, each list represent a cycle</returns>
        IList<IGraphElement> GetACycle();
    }
}

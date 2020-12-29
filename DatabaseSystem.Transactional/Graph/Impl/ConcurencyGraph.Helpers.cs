using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using DatabaseSystem.Transactional.Graph.Element;

namespace DatabaseSystem.Transactional.Graph.Impl
{
    public partial class ConcurrencyGraph
    {
        private readonly ISet<int> _verticesThatBelongToACycle = new HashSet<int>();
        private readonly ISet<int> _verticesThatAreNotInAnyCycle = new HashSet<int>();
        private readonly IDictionary<int, int?> _vertexParent = new Dictionary<int, int?>();

        private IEnumerable<IGraphElement> TraverseUsingDfs(int nodeId, ISet<int> visited)
        {
            //mark the element as visited
            visited.Add(nodeId);

            if (_idToGraphElement.ContainsKey(nodeId))
            {
                yield return _idToGraphElement[nodeId];
            }

            if (!_internalGraph.ContainsKey(nodeId))
            {
                yield break;
            }

            //go to each node and get traverse the graph
            foreach (var node in _internalGraph[nodeId])
            {
                if (visited.Contains(node))
                {
                    continue;
                }

                foreach (var resultNode in TraverseUsingDfs(node, visited))
                {
                    yield return resultNode;
                }
            }
        }

        /// <summary>
        /// This method it is used for getting a cycle
        /// </summary>
        /// <param name="startPoint">the point from which the search will begin</param>
        /// <param name="parent">the parent of the node</param>
        /// <returns>a group of two elements (the cycle last viable element and its parent)</returns>
        public Tuple<int, int?> GetCycle(int startPoint, int? parent = null)
        {
            //set the parent of the starting point
            if (!_vertexParent.ContainsKey(startPoint))
            {
                _vertexParent.Add(startPoint, parent);
            }

            //check if a cycle is formed
            if (_verticesThatBelongToACycle.Contains(startPoint))
            {
                return Tuple.Create(startPoint, parent);
            }

            //add the item into the assumed cycle nodes
            _verticesThatBelongToACycle.Add(startPoint);

            //iterate the adjacency
            foreach (var vertex in _internalGraph.ContainsKey(startPoint) ? _internalGraph[startPoint] : new List<int>())
            {
                //try only the valid nodes
                if (_verticesThatAreNotInAnyCycle.Contains(vertex))
                {
                    continue;
                }

                //get the node
                var node = GetCycle(vertex, startPoint);
                if (node != null)
                {
                    return node;
                }
            }

            //the vertex does not belong to a cycle
            _verticesThatAreNotInAnyCycle.Add(startPoint);

            //add it into the vertices that does not contain cycles
            _verticesThatBelongToACycle.Remove(startPoint);

            return null;
        }

        /// <summary>
        /// This method it is used for reconstructing the cycle
        /// </summary>
        /// <param name="cycle">the object that contains info about the cycle</param>
        /// <returns>a list of elements that represents the graph</returns>
        public IList<IGraphElement> ReconstructCycle(Tuple<int, int?> cycle)
        {
            //deconstruct the cycle
            var (cyclePeek, cycleLastNode) = cycle;

            //declare a stack
            var stack = new Stack<IGraphElement>();

            //build the stack
            stack.Push(_idToGraphElement[cyclePeek]);
            stack.Push(_idToGraphElement[cycleLastNode.Value]);

            //get the other elements
            for (var node = _vertexParent[cycleLastNode.Value]; node != cyclePeek; node = _vertexParent[node.Value])
            {
                Debug.Assert(node != null, nameof(node) + " != null");
                stack.Push(_idToGraphElement[node.Value]);
            }

            return stack.ToList();
        }
    }
}

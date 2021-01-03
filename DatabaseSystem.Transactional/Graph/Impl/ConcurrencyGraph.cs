using System.Linq;
using System.Collections.Generic;
using DatabaseSystem.Transactional.Graph.Element;

namespace DatabaseSystem.Transactional.Graph.Impl
{
    public partial class ConcurrencyGraph : IGraph
    {
        private static readonly object Lock = new object();

        private readonly IDictionary<int, IList<int>> _internalGraph
            = new Dictionary<int, IList<int>>();

        private readonly IDictionary<int, IGraphElement> _idToGraphElement
            = new Dictionary<int, IGraphElement>();

        public IEnumerable<IGraphElement> Vertices
        {
            get
            {
                lock (Lock)
                {
                    return _internalGraph
                        .Values
                        .SelectMany(x => x)
                        .ToHashSet()
                        .Select(x => _idToGraphElement[x]);
                }
            }
        }

        public void InsertEdge(IGraphElement @from, IGraphElement to)
        {
            lock (Lock)
            {
                //check if we already have the transaction into the mapping dictionary
                if (!_idToGraphElement.ContainsKey(@from.Id))
                {
                    _idToGraphElement.Add(@from.Id, null);
                }

                _idToGraphElement[@from.Id] = @from;

                //check if we already have the transaction into the mapping dictionary
                if (!_idToGraphElement.ContainsKey(to.Id))
                {
                    _idToGraphElement.Add(to.Id, null);
                }

                _idToGraphElement[to.Id] = to;

                //check if the node is present into the graph list
                if (!_internalGraph.ContainsKey(@from.Id))
                {
                    _internalGraph.Add(@from.Id, new List<int>());
                }

                _internalGraph[@from.Id].Add(to.Id);
            }
        }

        public void RemoveVertex(IGraphElement vertex)
        {
            lock (Lock)
            {
                //remove the vertex from any node
                foreach (var key in _internalGraph.Keys)
                {
                    _internalGraph[key].Remove(vertex.Id);
                }

                //remove the vertex
                _internalGraph.Remove(vertex.Id);
            }
        }

        public void RemoveEdge(IGraphElement @from, IGraphElement to)
        {
            lock (Lock)
            {
                if (!_internalGraph.ContainsKey(@from.Id))
                {
                    return;
                }

                //remove the child from the graph
                _internalGraph[@from.Id].Remove(to.Id);

                //remove the empty child
                if (_internalGraph[@from.Id].Any())
                {
                    return;
                }

                _internalGraph.Remove(@from.Id);
            }

        }

        public IEnumerable<IGraphElement> TraverseUsingDfs(IGraphElement startPoint = null)
        {
            lock (Lock)
            {
                var visited = new HashSet<int>();

                if (startPoint != null)
                {
                    return TraverseUsingDfs(startPoint.Id, visited);
                }

                //traverse from any point
                var result = new List<IGraphElement>();
                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var node in Vertices.Select(x => x.Id))
                {
                    if (visited.Contains(node))
                    {
                        continue;
                    }

                    result.AddRange(TraverseUsingDfs(node, visited));
                }

                return result;
            }

        }

        public IList<IGraphElement> GetACycle()
        {
            lock (Lock)
            {
                //clear the structures
                _vertexParent.Clear();
                _verticesThatBelongToACycle.Clear();
                _verticesThatAreNotInAnyCycle.Clear();

                //get the vertex
                foreach (var vertex in Vertices.Select(x => x.Id))
                {
                    //ignore the vertexes that are visited or that does not belong to any cycle
                    if (_verticesThatAreNotInAnyCycle.Contains(vertex) || _verticesThatBelongToACycle.Contains(vertex))
                    {
                        continue;
                    }

                    //get the cycle
                    var cycle = GetCycle(vertex);
                    if (cycle == null)
                    {
                        continue;
                    }

                    return ReconstructCycle(cycle);
                }

                return new List<IGraphElement>();
            }
        }
    }
}

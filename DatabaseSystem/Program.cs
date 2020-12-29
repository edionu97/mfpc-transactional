using System;
using DatabaseSystem.Transactional.Graph.Element;
using DatabaseSystem.Transactional.Graph.Impl;

namespace DatabaseSystem
{
    public class Transaction : IGraphElement
    {
        public int Id { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var graph = new ConcurrencyGraph();

            var t1 = new Transaction { Id = 1 };
            var t2 = new Transaction { Id = 2 };
            var t3 = new Transaction { Id = 3 };
            var t4 = new Transaction { Id = 4 };
            var t5 = new Transaction { Id = 5 };
            var t6 = new Transaction { Id = 6 };
            var t7 = new Transaction { Id = 7 };

            graph.InsertEdge(t1, t2);
            graph.InsertEdge(t1, t5);
            graph.InsertEdge(t2, t3);
            graph.InsertEdge(t3, t4);
            graph.InsertEdge(t3, t7);
            graph.InsertEdge(t4, t5);
            graph.InsertEdge(t4, t7);
            graph.InsertEdge(t5, t6);
            graph.InsertEdge(t6, t7);
            graph.InsertEdge(t7, t2);

            foreach (var graphElement in graph.GetACycle())
            {
                Console.Write($"{graphElement.Id} ");
            }
            
        }
    }
}

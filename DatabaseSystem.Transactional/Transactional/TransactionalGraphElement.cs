using System.Threading;
using System.Threading.Tasks;
using DatabaseSystem.Transactional.Graph.Element;

namespace DatabaseSystem.Transactional.Transactional
{
    public class TransactionalGraphElement : IGraphElement
    {
        public int Id { get; set; }

        public Task ActiveTask { get; set; }

        public CancellationToken CancellationToken { get; set; }

        public CancellationTokenSource CancellationTokenSource { get; set; }
    }
}

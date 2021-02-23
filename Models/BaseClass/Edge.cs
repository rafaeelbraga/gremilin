using ExRam.Gremlinq.Core.GraphElements;

namespace gremlin.Models.BaseClass
{
    public class Edge : IEdge
    {
       public object Id { get; set; }
       public string Label { get; set; }
    }
}
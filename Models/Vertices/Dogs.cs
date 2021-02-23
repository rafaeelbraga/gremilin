using gremlin.Models.BaseClass;

namespace gremlin.Models.Vertices
{
    public class Dogs : Vertex
    {
        public string Name { get; set; }
        public string Age { get; set; }
        public string Race { get; set; }

        public Dogs(string Name, string Age, string Race)
        {
            this.Name = Name;
            this.Age = Age;
            this.Race = Race;
            Id = $"{Name}-{Age}";
            PartitionKey = Name;
        }
    }
}
using gremlin.Models.BaseClass;

namespace gremlin.Models.Vertices
{
    public class Person : Vertex
    {
        public string Name { get; set; }
        public string Age { get; set; }

        public Person(string Name, string Age)
        {
            this.Name = Name;
            this.Age = Age;
            Id = $"{Name}-{Age}";
            PartitionKey = Name;
        }
    }
}
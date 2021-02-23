using gremlin.Models.BaseClass;

namespace gremlin.Models.Edges
{
    public class Married : Edge 
    { 
        public Married(string Label)
        { 
            this.Label = Label;
        }
    }
}
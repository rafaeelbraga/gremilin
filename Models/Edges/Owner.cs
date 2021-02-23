using gremlin.Models.BaseClass;

namespace gremlin.Models.Edges
{
    public class Owner : Edge 
    {
        public Owner(string Label)
        {
            this.Label = Label;
        }
    }
}
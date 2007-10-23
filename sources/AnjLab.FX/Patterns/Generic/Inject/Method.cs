namespace AnjLab.FX.Patterns.Generic.Inject
{
    public class Method: Definition
    {
        private string _method;

        public string Name
        {
            get { return _method; }
            set { _method = value; }
        }

        internal override void Build(AssemblyBuilder builder)
        {
            builder.BuildFromMethodDefinition(this);
        }
    }
}

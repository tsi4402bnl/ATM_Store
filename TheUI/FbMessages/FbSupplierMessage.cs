namespace TheUI
{
    class FbSupplierMessage : FbMessage, ISupplierPropEntryFb
    {
        public FbSupplierMessage(FbEventData msg) : base(msg)
        {
            Name = "";
            Email = "";
            Parse();
        }

        public const string TABLE_NAME = "suppliers";

        protected override int Parse()
        {
            if (OPERATION == Fb_Operations.fb_add || OPERATION == Fb_Operations.fb_edit)
            {
                if (!IsParsed && ParseId(NAME) == 0) { Name = DATA; }
                if (!IsParsed && ParseId(EMAIL) == 0) { Email = DATA; }
            }
            else if (OPERATION == Fb_Operations.fb_delete)
            {
                ParseId();
            }
            return IsParsed ? 0 : -1;
        }

        public string Name { get; set; }
        public string Email { get; set; }

        private const string NAME = "Name";
        private const string EMAIL = "Email";
    }
}

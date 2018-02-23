namespace TheUI
{
    class FbCategoryMessage : FbMessage, ICategoryPropEntryFb
    {
        public FbCategoryMessage() : base(new FbEventData()) { }
        public FbCategoryMessage(FbEventData msg) : base(msg)
        {
            Name = "";
            Parse();
        }

        public override string TABLE_NAME { get { return "categories"; } }

        protected override sealed int Parse()
        {
            if (OPERATION == Fb_Operations.fb_add || OPERATION == Fb_Operations.fb_edit)
            {
                if (!IsParsed && ParseId(NAME) == 0) { Name = DATA; }
            }
            else if (OPERATION == Fb_Operations.fb_delete)
            {
                ParseId();
            }
            return IsParsed ? 0 : -1;
        }

        public string Name { get; set; }

        private const string NAME = "Name";
    }


}

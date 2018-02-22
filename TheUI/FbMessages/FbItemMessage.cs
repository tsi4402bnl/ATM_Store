using System;

namespace TheUI
{
    class FbItemMessage : FbMessage, IItemPropEntryFb
    {
        public FbItemMessage(FbEventData msg) : base(msg)
        {
            Name        = "";
            CategoryId  = "";
            Description = "";
            Units       = "";
            SupplierId  = "";
            Price       = -1.0;
            QtyPerBox   = -1;
            Image       = "";
            Parse();
        }

        public const string TABLE_NAME = "items";

        protected override sealed int Parse()
        {
            if (OPERATION == Fb_Operations.fb_add || OPERATION == Fb_Operations.fb_edit)
            {
                if (!IsParsed && ParseId(NAME)        == 0) { Name        = DATA; }
                if (!IsParsed && ParseId(CATEGORY_ID) == 0) { CategoryId  = DATA; }
                if (!IsParsed && ParseId(DESCRIPTION) == 0) { Description = DATA; }
                if (!IsParsed && ParseId(PRICE)       == 0)
                {
                    double price = -1.0;
                    if (Double.TryParse(DATA.Substring(1), out price)) Price = price;
                }
                if (!IsParsed && ParseId(QTY_PER_BOX) == 0)
                {
                    int qty = -1;
                    if (Int32.TryParse(DATA.Substring(1), out qty)) QtyPerBox = qty;
                }
                if (!IsParsed && ParseId(UNITS)       == 0) { Units      = DATA; }
                if (!IsParsed && ParseId(SUPPLIER_ID) == 0) { SupplierId = DATA; }
                if (!IsParsed && ParseId(IMAGE)       == 0) { Image      = DATA; }
            }
            else if (OPERATION == Fb_Operations.fb_delete)
            {
                ParseId();
            }
            return IsParsed ? 0 : -1;
        }

        public string Name        { get; set; }
        public string CategoryId  { get; set; }
        public string Description { get; set; }
        public double Price       { get; set; }
        public int    QtyPerBox   { get; set; }
        public string Units       { get; set; }
        public string SupplierId  { get; set; }
        public string Image       { get; set; }

        private const string NAME        = "Name";
        private const string CATEGORY_ID = "CategoryId";
        private const string DESCRIPTION = "Description";
        private const string PRICE       = "Price";
        private const string QTY_PER_BOX = "QtyPerBox";
        private const string UNITS       = "Units";
        private const string SUPPLIER_ID = "SupplierId";
        private const string IMAGE       = "Image";
    }
}

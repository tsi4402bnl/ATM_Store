using System;
using System.Collections.Generic;

namespace TheUI
{
    class FbTransactionMessage : FbMessage, ITransactionPropEntryFb
    {
        public FbTransactionMessage() : base(new FbEventData()) { }
        public FbTransactionMessage(FbEventData msg) : base(msg)
        {
            ItemId          = "";
            ItemName        = "";
            SingleItemPrice = -1.0;
            Qty             = -1;
            EpochTime       = -1.0;
            Timestamp       = new Dictionary<string, string> { { ".sv", "timestamp" } };
            Parse();
        }

        public override string TABLE_NAME { get { return "transactions"; } }

        protected override sealed int Parse()
        {
            if (OPERATION == Fb_Operations.fb_add || OPERATION == Fb_Operations.fb_edit)
            {
                if (!IsParsed && ParseId(ITEM_ID) == 0)   { ItemId = DATA; }
                if (!IsParsed && ParseId(ITEM_NAME) == 0) { ItemName = DATA; }
                if (!IsParsed && ParseId(PRICE) == 0)
                {
                    double price = -1.0;
                    if (Double.TryParse(DATA.Substring(1), out price)) SingleItemPrice = price;
                }
                if (!IsParsed && ParseId(QTY) == 0)
                {
                    int qty = -1;
                    if (Int32.TryParse(DATA.Substring(1), out qty)) Qty = qty;
                }
                if (!IsParsed && ParseId(TIME_STAMP) == 0)
                {
                    double time = -1.0;
                    if (Double.TryParse(DATA.Substring(1), out time)) EpochTime = time;
                }
                if (!IsParsed && ParseId(EPOCH_TIME) == 0) {  } // ignore this field, using timestamp field value for epochtime
            }
            else if (OPERATION == Fb_Operations.fb_delete)
            {
                ParseId();
            }
            return IsParsed ? 0 : -1;
        }

        public string ItemId          { get; set; }
        public string ItemName        { get; set; }
        public double SingleItemPrice { get; set; }
        public int    Qty             { get; set; }
        public double EpochTime       { get; set; }
        public Dictionary<string, string> Timestamp { get; }

        private const string ITEM_ID    = "ItemId";
        private const string ITEM_NAME  = "ItemName";
        private const string PRICE      = "SingleItemPrice";
        private const string QTY        = "Qty";
        private const string EPOCH_TIME = "EpochTime";
        private const string TIME_STAMP = "Timestamp";
    }
}

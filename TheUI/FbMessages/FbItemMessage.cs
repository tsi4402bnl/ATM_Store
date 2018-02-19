using System;

namespace TheUI
{
    class FbItemMessage : FbMessage
    {
        public FbItemMessage(FbEventData msg) : base(msg)
        {
            Price = -1.0;
            QtyPerBox = -1;
        }

        public override int Respond()
        {
            int ret = -1;

            if (!isParsed && Parse() != 0)
            {
                ParseFailedMessage();
            }
            else if (OperationType() == TheUI::Fb_Operations::fb_add || OperationType() == TheUI::Fb_Operations::fb_edit)
            {
                System::String ^ idCli = msclr::interop::marshal_as < System::String ^> (id);
                System::String ^ nameCli = msclr::interop::marshal_as < System::String ^> (name);
                System::String ^ catCli = msclr::interop::marshal_as < System::String ^> (categoryId);
                System::String ^ descrCli = msclr::interop::marshal_as < System::String ^> (description);
                System::String ^ unitsCli = msclr::interop::marshal_as < System::String ^> (units);
                System::String ^ supplierIdCli = msclr::interop::marshal_as < System::String ^> (supplierId);
                TheUI::ItemPropEntryFb item(nameCli, catCli, descrCli, price, qtyPerBox, unitsCli, supplierIdCli);
                ManagedCode::ManagedGlobals::w->AddProperties(idCli, % item);
                ret = 0;
            }
            else if (OperationType() == TheUI::Fb_Operations::fb_delete)
            {
                System::String ^ idCli = msclr::interop::marshal_as < System::String ^> (id);
                ManagedCode::ManagedGlobals::w->RemoveItemProperties(idCli);
                ret = 0;
            }
            else UnsupportedOperationMessage();

            return ret;
        }

        public const string TABLE_NAME = "items";

        protected override int Parse()
        {
            return 0;
        }

        private string Name        { get; set; }
        private string CategoryId  { get; set; }
        private string Description { get; set; }
        private double Price       { get; set; }
        private int    QtyPerBox   { get; set; }
        private string Units       { get; set; }
        private string SupplierId  { get; set; }

        private const string NAME        = "Name";
        private const string CATEGORY_ID = "CategoryId";
        private const string DESCRIPTION = "Description";
        private const string PRICE       = "Price";
        private const string QTY_PER_BOX = "QtyPerBox";
        private const string UNITS       = "Units";
        private const string SUPPLIER_ID = "SupplierId";
    }



}

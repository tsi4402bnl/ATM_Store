#include "FbItemMessage.h"

#include <msclr\marshal_cppstd.h>

#include "MainWindow.h"

FbItemMessage::FbItemMessage(const TheUI::FbEventData& msg)
	: FbMessage(TableName(), msg)
	, name()
	, categoryId()
	, description()
	, price(-1.0)
	, qtyPerBox(-1)
	, units()
	, supplierId()
{ }

int FbItemMessage::Respond()
{
	int ret = -1;

	if (!isParsed && Parse() != 0)
	{
		ParseFailedMessage();
	}
	else if (OperationType() == TheUI::Fb_Operations::fb_add || OperationType() == TheUI::Fb_Operations::fb_edit)
	{
		System::String^ idCli         = msclr::interop::marshal_as<System::String^>(id);
		System::String^ nameCli       = msclr::interop::marshal_as<System::String^>(name);
		System::String^ catCli        = msclr::interop::marshal_as<System::String^>(categoryId);
		System::String^ descrCli      = msclr::interop::marshal_as<System::String^>(description);
		System::String^ unitsCli      = msclr::interop::marshal_as<System::String^>(units);
		System::String^ supplierIdCli = msclr::interop::marshal_as<System::String^>(supplierId);
		TheUI::ItemPropEntryFb item(nameCli, catCli, descrCli, price, qtyPerBox, unitsCli, supplierIdCli);
		ManagedCode::ManagedGlobals::w->AddProperties(idCli, %item);
		ret = 0;
	}
	else if (OperationType() == TheUI::Fb_Operations::fb_delete)
	{
		System::String^ idCli = msclr::interop::marshal_as<System::String^>(id);
		ManagedCode::ManagedGlobals::w->RemoveItemProperties(idCli);
		ret = 0;
	}
	else UnsupportedOperationMessage();

	return ret;
}

int FbItemMessage::Parse()
{
	if (OperationType() == TheUI::Fb_Operations::fb_add || OperationType() == TheUI::Fb_Operations::fb_edit)
	{
		if (!isParsed && ParseId(NAME)		  == 0) { name		  =      Data()         ; }
		if (!isParsed && ParseId(CATEGORY_ID) == 0) { categoryId  =      Data()         ; }
		if (!isParsed && ParseId(DESCRIPTION) == 0) { description =		 Data()			; }
		if (!isParsed && ParseId(PRICE)		  == 0) { price		  = atof(Data().substr(1).c_str()); }
		if (!isParsed && ParseId(QTY_PER_BOX) == 0) { qtyPerBox	  = atoi(Data().substr(1).c_str()); }
		if (!isParsed && ParseId(UNITS)		  == 0) { units		  =      Data()         ; }
		if (!isParsed && ParseId(SUPPLIER_ID) == 0) { supplierId  =      Data()         ; }
	}
	else if (OperationType() == TheUI::Fb_Operations::fb_delete)
	{
		ParseId();
	}
	return isParsed ? 0 : -1;
}

const std::string FbItemMessage::NAME		 = "Name";
const std::string FbItemMessage::CATEGORY_ID = "CategoryId";
const std::string FbItemMessage::DESCRIPTION = "Description";
const std::string FbItemMessage::PRICE		 = "Price";
const std::string FbItemMessage::QTY_PER_BOX = "QtyPerBox";
const std::string FbItemMessage::UNITS		 = "Units";
const std::string FbItemMessage::SUPPLIER_ID = "SupplierId";

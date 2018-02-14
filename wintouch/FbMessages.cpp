#include "FbMessages.h"

#include <msclr\marshal_cppstd.h>

#include "Log.h"
#include "MainWindow.h"

FbMessage::FbMessage(const std::string& tableName, const TheUI::FbEventData& msg)
	: isParsed(false)
	, id()
	, OPERATION(msg.operation)
	, PATH(msclr::interop::marshal_as<std::string>(msg.path))
	, DATA(msclr::interop::marshal_as<std::string>(msg.data))
	, TABLE_NAME(tableName)
{ }

int FbMessage::ParseId(const std::string& propName)
{
	isParsed = false;
	std::string endStr("/" + propName);

	if (DATA.empty() || PATH.empty()) return -1;

	int endPos = PATH.rfind(endStr);
	if (endPos == PATH.size() - endStr.size())
	{
		int idPos = PATH.rfind("/", endPos - 1) + 1;
		if (idPos != std::string::npos && idPos < endPos)
		{
			id = PATH.substr(idPos, endPos - idPos);
			isParsed = true;
		}
	}

	return isParsed ? 0 : -1;
}

int FbMessage::ParseId()
{
	isParsed = false;

	if (PATH.empty()) return -1;

	int idPos = PATH.rfind("/") + 1;
	if (idPos != std::string::npos)
	{
		id = PATH.substr(idPos);
		isParsed = true;
	}

	return isParsed ? 0 : -1;
}

void FbMessage::ParseFailedMessage() const
{
	LOG("Failed To parse message! table: " + TABLE_NAME + ", Path: " + PATH + ", Data: " + DATA);
}

void FbMessage::UnsupportedOperationMessage() const
{
	LOG("Unsupported FB operation! table: " + TABLE_NAME + ", operation: " + std::to_string(static_cast<int>(OPERATION)));
}


FbItemMessage::FbItemMessage(const TheUI::FbEventData& msg)
	: FbMessage(TableName(), msg)
	, name()
	, category()
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
		//ParseFailedMessage();
	}
	else if (OperationType() == TheUI::Fb_Operations::fb_add || OperationType() == TheUI::Fb_Operations::fb_edit)
	{
		System::String^ idCli = msclr::interop::marshal_as<System::String^>(id);
		System::String^ nameCli = msclr::interop::marshal_as<System::String^>(name);
		System::String^ catCli = msclr::interop::marshal_as<System::String^>(category);
		System::String^ descrCli = msclr::interop::marshal_as<System::String^>(description);
		System::String^ unitsCli = msclr::interop::marshal_as<System::String^>(units);
		System::String^ supplierIdCli = msclr::interop::marshal_as<System::String^>(supplierId);
		TheUI::ItemPropEntryFb item(nameCli, catCli, descrCli, price, qtyPerBox, unitsCli, supplierIdCli);
		ManagedCode::ManagedGlobals::w->AddItemProperties(idCli, %item);
		ret = 0;
	}
	else if (OperationType() == TheUI::Fb_Operations::fb_delete)
	{
		System::String^ idCli = msclr::interop::marshal_as<System::String^>(id);
		ManagedCode::ManagedGlobals::w->RemoveItemProperties(idCli);
		ret = 0;
	}
	else
	{
		UnsupportedOperationMessage();
	}
	return ret;
}

int FbItemMessage::Parse()
{
	if (OperationType() == TheUI::Fb_Operations::fb_add || OperationType() == TheUI::Fb_Operations::fb_edit)
	{
		if (!isParsed && ParseId(NAME)		  == 0) { name		  =      Data()         ; }
		if (!isParsed && ParseId(CATEGORY)	  == 0) { category	  =      Data()         ; }
		if (!isParsed && ParseId(DESCRIPTION) == 0) { description =		 Data()			; }
		if (!isParsed && ParseId(PRICE)		  == 0) { price		  = atof(Data().c_str()); }
		if (!isParsed && ParseId(QTY_PER_BOX) == 0) { qtyPerBox	  = atoi(Data().c_str()); }
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
const std::string FbItemMessage::CATEGORY	 = "Category";
const std::string FbItemMessage::DESCRIPTION = "Description";
const std::string FbItemMessage::PRICE		 = "Price";
const std::string FbItemMessage::QTY_PER_BOX = "QtyPerBox";
const std::string FbItemMessage::UNITS		 = "Units";
const std::string FbItemMessage::SUPPLIER_ID = "SupplierId";

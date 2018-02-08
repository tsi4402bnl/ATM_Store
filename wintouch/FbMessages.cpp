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
	, price(-1)
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
		ManagedCode::ManagedGlobals::w->AddItemProperties(idCli, nameCli, price);
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
		if (!isParsed && ParseId(NAME ()) == 0) { name  =      Data()         ; }
		if (!isParsed && ParseId(PRICE()) == 0) { price = atoi(Data().c_str()); }
	}
	else if (OperationType() == TheUI::Fb_Operations::fb_delete)
	{
		ParseId();
	}
	return isParsed ? 0 : -1;
}
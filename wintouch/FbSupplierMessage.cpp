#include "FbSupplierMessage.h"

#include <msclr\marshal_cppstd.h>

#include "MainWindow.h"

FbSupplierMessage::FbSupplierMessage(const TheUI::FbEventData& msg)
	: FbMessage(TableName(), msg)
	, name()
	, email()
{ }

int FbSupplierMessage::Respond()
{
	int ret = -1;

	if (!isParsed && Parse() != 0)
	{
		ParseFailedMessage();
	}
	else if (OperationType() == TheUI::Fb_Operations::fb_add || OperationType() == TheUI::Fb_Operations::fb_edit)
	{
		System::String^ idCli = msclr::interop::marshal_as<System::String^>(id);
		System::String^ nameCli = msclr::interop::marshal_as<System::String^>(name);
		System::String^ emailCli = msclr::interop::marshal_as<System::String^>(email);
		TheUI::SupplierPropEntryFb item(nameCli, emailCli);
		ManagedCode::ManagedGlobals::w->AddProperties(idCli, %item);
		ret = 0;
	}
	else if (OperationType() == TheUI::Fb_Operations::fb_delete)
	{
		System::String^ idCli = msclr::interop::marshal_as<System::String^>(id);
		ManagedCode::ManagedGlobals::w->RemoveSupplierProperties(idCli);
		ret = 0;
	}
	else UnsupportedOperationMessage();

	return ret;
}

int FbSupplierMessage::Parse()
{
	if (OperationType() == TheUI::Fb_Operations::fb_add || OperationType() == TheUI::Fb_Operations::fb_edit)
	{
		if (!isParsed && ParseId(NAME)  == 0) { name = Data();  }
		if (!isParsed && ParseId(EMAIL) == 0) { email = Data(); }
	}
	else if (OperationType() == TheUI::Fb_Operations::fb_delete)
	{
		ParseId();
	}
	return isParsed ? 0 : -1;
}

const std::string FbSupplierMessage::NAME = "Name";
const std::string FbSupplierMessage::EMAIL = "Email";

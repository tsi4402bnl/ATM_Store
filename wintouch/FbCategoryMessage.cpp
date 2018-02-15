#include "FbCategoryMessage.h"

#include <msclr\marshal_cppstd.h>

#include "MainWindow.h"

FbCategoryMessage::FbCategoryMessage(const TheUI::FbEventData& msg)
	: FbMessage(TableName(), msg)
	, name()
{ }

int FbCategoryMessage::Respond()
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
		TheUI::CategoryPropEntryFb item(nameCli);
		ManagedCode::ManagedGlobals::w->AddProperties(idCli, %item);
		ret = 0;
	}
	else if (OperationType() == TheUI::Fb_Operations::fb_delete)
	{
		System::String^ idCli = msclr::interop::marshal_as<System::String^>(id);
		ManagedCode::ManagedGlobals::w->RemoveCategoryProperties(idCli);
		ret = 0;
	}
	else UnsupportedOperationMessage();

	return ret;
}

int FbCategoryMessage::Parse()
{
	if (OperationType() == TheUI::Fb_Operations::fb_add || OperationType() == TheUI::Fb_Operations::fb_edit)
	{
		if (!isParsed && ParseId(NAME)== 0) { name = Data(); }
	}
	else if (OperationType() == TheUI::Fb_Operations::fb_delete)
	{
		ParseId();
	}
	return isParsed ? 0 : -1;
}

const std::string FbCategoryMessage::NAME = "Name";

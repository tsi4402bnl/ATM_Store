#include "UiInit.h"

#include <process.h>
#include <msclr\marshal_cppstd.h>

#include "MainWindow.h"
#include "Log.h"
#include "FbItemMessage.h"
#include "FbCategoryMessage.h"

void UiInit::Init()
{
	unsigned tid; // thread ID
	HANDLE hThread; // thread handle

	hThread = (HANDLE)_beginthreadex(NULL, 0,
		UiInitThread, (void *)NULL,
		0, &tid);

	hThread = (HANDLE)_beginthreadex(NULL, 0,
		FetchFbMessagesThread, (void *)NULL,
		0, &tid);
}

unsigned __stdcall UiInit::UiInitThread(void * param)
{
	using namespace System::Windows::Controls;
	using namespace System::Windows::Controls::Primitives;

	//Button^ btnStopMacro = ManagedCode::ManagedGlobals::w->GetTestTabBtn();
	//btnStopMacro->Click += gcnew System::Windows::RoutedEventHandler(&OnTestTabBtnClick);

	return 0;
};

unsigned __stdcall UiInit::FetchFbMessagesThread(void * param)
{
	using namespace System::Windows::Controls;
	using namespace System::Windows::Controls::Primitives;

	do
	{
		while (ManagedCode::ManagedGlobals::w->IsFbMessagePending())
		{
			TheUI::FbEventData fbMessage = ManagedCode::ManagedGlobals::w->FetchNextFbMessage();
			std::string path = msclr::interop::marshal_as<std::string>(fbMessage.path);
			if (path.empty()) continue;

			if (path.substr(1, FbItemMessage::TableName().size() + 1) == FbItemMessage::TableName() + "/")
			{
				FbItemMessage(fbMessage).Respond();
			}
			else if (path.substr(1, FbCategoryMessage::TableName().size() + 1) == FbCategoryMessage::TableName() + "/")
			{
				FbCategoryMessage(fbMessage).Respond();
			}

			// other table messages here

		}
		Sleep(250);
	} while (true);
	
	return 0;
};

void UiInit::OnTestTabBtnClick(System::Object ^sender, System::Windows::RoutedEventArgs ^e)
{
	LOG("Clicked form C++");
}

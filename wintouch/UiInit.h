#pragma once
#include <string>

public class UiInit
{
public:
	static void Init();

private:
	static unsigned __stdcall UiInitThread(void * param);
	static unsigned __stdcall FetchFbMessagesThread(void * param);
	static void OnTestTabBtnClick(System::Object ^sender, System::Windows::RoutedEventArgs ^e);

};
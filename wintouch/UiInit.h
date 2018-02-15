#pragma once
#include <string>

public class UiInit
{
public:
	static void Init();

private:
	static unsigned __stdcall FetchFbMessagesThread(void * param);

};
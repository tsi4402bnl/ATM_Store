#include "Log.h"
#include "MainWindow.h"
#include <msclr\marshal_cppstd.h>

void Logger::Log(std::string msg)
{
	System::String^ cliMsg = msclr::interop::marshal_as<System::String^>(msg);
	ManagedCode::ManagedGlobals::w->Log(cliMsg);
}
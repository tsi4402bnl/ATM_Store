#include "MainWindow.h"
#include <Windows.h>
#include "UiInit.h"

namespace ManagedCode
{
	using namespace System;
	using namespace System::Windows;
	using namespace System::Windows::Interop;

	HWND GetHwnd(HWND parent, int x, int y, int width, int height) {
		HwndSource^ source = gcnew HwndSource(
			0, // class style  
			WS_VISIBLE | WS_CHILD, // style  
			0, // exstyle  
			x, y, width, height,
			"wintouch", // NAME  
			IntPtr(parent)        // parent window
		);

		ManagedGlobals::w = gcnew TheUI::MainWindow(width, height);

		InitApp();

		source->RootVisual = ManagedGlobals::w;
		source->SizeToContent = SizeToContent::WidthAndHeight;
		return (HWND)source->Handle.ToPointer();
	}

	int InitApp()
	{
		UiInit::Init();
		return 0;
	}

}
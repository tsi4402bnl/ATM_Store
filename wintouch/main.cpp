#include "MainWindow.h"
#include <Windows.h>

using namespace std;
LRESULT CALLBACK WindowFunc(HWND, UINT, WPARAM, LPARAM);

[System::STAThreadAttribute]
int WINAPI WinMain(_In_ HINSTANCE hThisInst, _In_opt_ HINSTANCE hPrevInst,
	_In_ LPSTR args, _In_ int winMode)
{
	HWND hwnd;
	MSG msg;
	WNDCLASSEX wcl;
	HACCEL hAccel;

	// Define a window class.  
	wcl.cbSize = sizeof(WNDCLASSEX);

	wcl.hInstance = hThisInst;     // handle to this instance  
	wcl.lpszClassName = "wintouch";   // window class name  
	wcl.lpfnWndProc = WindowFunc;  // window function  
	wcl.style = 0;                 // default style  

	wcl.hIcon = LoadIcon(NULL, IDI_APPLICATION); // large icon  
	wcl.hIconSm = NULL; // use small version of large icon 
	wcl.hCursor = LoadCursor(NULL, IDC_ARROW);  // cursor style 

	wcl.lpszMenuName = "wintouch"; // main menu 

	wcl.cbClsExtra = 0; // no extra memory needed 
	wcl.cbWndExtra = 0;

	// Make the window background white. 
	wcl.hbrBackground = (HBRUSH)GetStockObject(WHITE_BRUSH);

	// Register the window class.  
	if (!RegisterClassEx(&wcl)) return 0;

	/* Now that a window class has been registered, a window
	can be created. */
	hwnd = CreateWindow(
		wcl.lpszClassName, // name of window class  
		"wintouch", // title 
		WS_OVERLAPPEDWINDOW, // window style - normal  
		CW_USEDEFAULT, // X coordinate - let Windows decide  
		CW_USEDEFAULT, // Y coordinate - let Windows decide  
		500,           // width
		400,           // height
		NULL,          // no parent window  
		NULL,          // no override of class menu 
		hThisInst,     // instance handle 
		NULL           // no additional arguments  
	);
	::SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE)^WS_MAXIMIZEBOX^WS_SIZEBOX);     

	// Load the keyboard accelerators. 
	hAccel = LoadAccelerators(hThisInst, "wintouch");

	// Display the window.  
	ShowWindow(hwnd, winMode);
	UpdateWindow(hwnd);

	// Create the message loop.  
	while (GetMessage(&msg, NULL, 0, 0))
	{
		if (!TranslateAccelerator(hwnd, hAccel, &msg)) {
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
	}

	return static_cast<int>(msg.wParam);
}

LRESULT CALLBACK WindowFunc(HWND hWnd, UINT message,
	WPARAM wParam, LPARAM lParam)
{
	LRESULT ret;
	RECT rect;

	switch (message) {
	case WM_CREATE:
		GetClientRect(hWnd, &rect);
		ManagedCode::GetHwnd(hWnd, 0, 0, rect.right, rect.bottom);
		break;
	case WM_COMMAND:
		break;
	case WM_DESTROY: // terminate the program 
		PostQuitMessage(0);
		break;
	default:
		ret = DefWindowProc(hWnd, message, wParam, lParam);
		return ret;
	}

	return 0;
}

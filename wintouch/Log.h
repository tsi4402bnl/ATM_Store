#pragma once

#include <string>

#define NUMBER_AS_STR_HELPER(x) #x
#define NUMBER_AS_STR(x) NUMBER_AS_STR_HELPER(x)
#define LOG(x) Logger::Log(x + std::string(" - [ ") + __FILE__ + ": " + NUMBER_AS_STR(__LINE__) + " ]")

public class Logger
{
public:
	static void Log(std::string msg);
};
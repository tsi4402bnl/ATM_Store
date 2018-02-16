#include "FbMessage.h"

#include <msclr\marshal_cppstd.h>

#include "Log.h"

FbMessage::FbMessage(const std::string& tableName, const TheUI::FbEventData& msg)
	: isParsed(false)
	, id()
	, OPERATION(msg.operation)
	, PATH(msclr::interop::marshal_as<std::string>(msg.path))
	, DATA("/" + msclr::interop::marshal_as<std::string>(msg.data))
	, TABLE_NAME(tableName)
{ }

int FbMessage::ParseId(const std::string& propName)
{
	isParsed = false;
	std::string endStr("/" + propName);

	if (PATH.empty()) return -1;

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

void FbMessage::ParseFailedMessage(bool isEmptyDataAllowed) const
{
	if (!isEmptyDataAllowed || !DATA.empty()) // in most cases empty data is ok, do not print error then
		LOG("Failed To parse message! table: " + TABLE_NAME + ", Path: " + PATH + ", Data: " + DATA);
}

void FbMessage::UnsupportedOperationMessage() const
{
	LOG("Unsupported FB operation! table: " + TABLE_NAME + ", operation: " + std::to_string(static_cast<int>(OPERATION)));
}

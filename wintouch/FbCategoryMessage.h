#pragma once

#include "FbMessages.h"

class FbCategoryMessage : public FbMessage
{
public:
	FbCategoryMessage(const TheUI::FbEventData& msg);
	static const std::string TableName() { return "categories"; }
	int Respond();

protected:
	virtual int Parse();

private:
	static const std::string NAME;

	std::string name;
};

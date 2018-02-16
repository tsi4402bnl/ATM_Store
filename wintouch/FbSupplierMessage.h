#pragma once

#include "FbMessage.h"

class FbSupplierMessage : public FbMessage
{
public:
	FbSupplierMessage(const TheUI::FbEventData& msg);
	static const std::string TableName() { return "suppliers"; }
	int Respond();

protected:
	virtual int Parse();

private:
	static const std::string NAME;
	static const std::string EMAIL;

	std::string name;
	std::string email;
};

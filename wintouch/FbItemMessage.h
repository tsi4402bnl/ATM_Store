#pragma once

#include "FbMessage.h"

class FbItemMessage : public FbMessage
{
public:
	FbItemMessage(const TheUI::FbEventData& msg);
	static const std::string TableName() { return "items"; }
	int Respond();

protected:
	virtual int Parse();

private:
	static const std::string NAME;
	static const std::string CATEGORY_ID;
	static const std::string DESCRIPTION;
	static const std::string PRICE;
	static const std::string QTY_PER_BOX;
	static const std::string UNITS;
	static const std::string SUPPLIER_ID;

	std::string name;
	std::string	categoryId;
	std::string description;
	double		price;
	int			qtyPerBox;
	std::string	units;
	std::string	supplierId;
};

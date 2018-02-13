#pragma once

#include <string>

class FbMessage
{
public:
	FbMessage(const std::string& tableName, const TheUI::FbEventData& msg);
	virtual int Respond() = 0;

protected:
	const TheUI::Fb_Operations OperationType() const { return OPERATION; }
	const std::string Data() const { return DATA; }

	int ParseId(const std::string& propName);
	int ParseId();

	void ParseFailedMessage() const;
	void UnsupportedOperationMessage() const;

	virtual int Parse() = 0;

	int isParsed;
	std::string id;

private:
	const TheUI::Fb_Operations OPERATION;
	const std::string PATH;
	const std::string DATA;
	const std::string TABLE_NAME;
};

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
	static const std::string CATEGORY;
	static const std::string DESCRIPTION;
	static const std::string PRICE;
	static const std::string QTY_PER_BOX;
	static const std::string UNITS;
	static const std::string SUPPLIER_ID;

	std::string name;
	std::string	category;
	std::string description;
	double		price;
	int			qtyPerBox;
	std::string	units;
	std::string	supplierId;
};

#include "CDTTable.h"

void CDTTable::extractAttrValue()
{
	attrValueList.resize(attrName.size());
	for (int j = 0; j<attrName.size(); j++)
	{
		map<string, int> value;
		for (int i = 0; i<data.size(); i++)
			value[data[i][j]] = 1;

		for (auto iter = value.begin(); iter != value.end(); iter++)
			attrValueList[j].push_back(iter->first);
	}
}
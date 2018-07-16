#pragma once
#include <vector>
#include <map>
using namespace std;

class CDTTable
{
public:
	vector<string> attrName;
	vector<vector<string> > data;
	vector<vector<string> > attrValueList;

	void extractAttrValue();
};


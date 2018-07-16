#pragma once
#include <string>
#include <vector>
using namespace std;

class CNode
{
public:
	int criteriaAttrIndex;
	string attrValue;

	int treeIndex;
	bool isLeaf;
	string label;

	vector<int > children;

	CNode();
};


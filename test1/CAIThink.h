#pragma once
#include "CDTTable.h"
#include "CNode.h"
#include <iostream>

using namespace std;

class CAIThink
{
public:
	CDTTable initialTable;
	vector<CNode> tree;

	CAIThink(CDTTable table);
	string guess(vector<string> row);
	int dfs(vector<string>& row, int here);
	void run(CDTTable table, int nodeIndex);
	double getEstimatedError(double f, int N);
	pair<string, int> getMajorityLabel(CDTTable table);
	bool isLeafNode(CDTTable table);
	int getSelectedAttribute(CDTTable table);
	double getGainRatio(CDTTable table, int attrIndex);
	double getInfoD(CDTTable table);
	double getInfoAttrD(CDTTable table, int attrIndex);
	double getGain(CDTTable table, int attrIndex);
	double getSplitInfoAttrD(CDTTable table, int attrIndex);
};


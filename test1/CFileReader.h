#pragma once
#include <fstream>
#include "CDTTable.h"
#include <iostream>
#include <string>
using namespace std;

class CFileReader
{
private:
	ifstream fin;
	CDTTable table;
public:
	CFileReader(string filename);
	void Parse();
	inline CDTTable GetTable() { return table; };
};


#include "CFileReader.h"

CFileReader::CFileReader(string filename)
{
	fin.open(filename);
	if (!fin)
	{
		cout << filename << " file could not be opened\n";
		exit(0);
	}
	Parse();
}

void CFileReader::Parse()
{
	string str;
	bool isAttrName = true;
	while (!getline(fin, str).eof()) {
		vector<string> row;
		int pre = 0;
		for (int i = 0; i<str.size(); i++) {
			if (str[i] == '\t') {
				string col = str.substr(pre, i - pre);

				row.push_back(col);
				pre = i + 1;
			}
		}
		string col = str.substr(pre, str.size() - pre - 1);
		row.push_back(col);

		if (isAttrName) {
			table.attrName = row;
			isAttrName = false;
		}
		else {
			table.data.push_back(row);
		}
	}
}

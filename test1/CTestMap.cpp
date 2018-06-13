#include "CTestMap.h"

CTestMap::CTestMap(string givenMapName)
{
	mapFileName = givenMapName;

	string line;
	int stringMax;
	ifstream myfile(mapFileName);
	if (myfile.is_open())
	{
		int currentLine = 0;
		while (getline(myfile, line))
		{
			stringMax = line.size();
			for (int i = 0; i < stringMax && i < mapSize; ++i) // check which is hit first line size or map size
			{
				cout << line[i] - '0' << endl;
				mapSquares[currentLine][i] = line[i] - '0'; // convert char to int and put it into the map
			}
			++currentLine; // next line before repeating
		}
		myfile.close();
	}
	else
	{
		//TODO error report
		cout << "Error opening file" << endl;
	}
}


CTestMap::~CTestMap()
{
}

#pragma once
#include <string>
#include <fstream>	// reading map - ifstream
using namespace std;

const int mapSize = 10;

class CTestMap
{
private:
	int mapSquares[mapSize][mapSize];
	string mapFileName;
	
public:
	CTestMap(string givenMapName);
	virtual ~CTestMap();
};
#pragma once
#include <string>
#include <fstream>	// reading map - ifstream
#include <iostream>
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
	inline int GetCubeData(int x, int y) { return mapSquares[x][y]; };	// returns data in map
};
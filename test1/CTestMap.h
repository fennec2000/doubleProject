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
	pair<int, int> spawnCoords;
	pair<int, int> goalCoords;

public:
	CTestMap(string givenMapName);
	virtual ~CTestMap();
	inline int GetCubeData(int x, int y) { return mapSquares[x][y]; };	// returns data in map
	inline pair<int, int> GetSpawnPoint() { return spawnCoords; };	// returns the coordinated for the spawn
	inline pair<int, int> GetGoalPoint() { return goalCoords; };	// returns the coordinated for the spawn
};
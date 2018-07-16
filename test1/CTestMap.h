#pragma once
#include <string>
#include <fstream>		// reading map - ifstream
#include <iostream>		// cout // debug
#include <vector>		// vector
#include <deque>		// deque
#include <memory>		// unique_ptr
#include <algorithm>	// sort
using namespace std;


enum EDirrection { North, East, South, West, NumberOfDirections };

struct coords
{
	pair<int, int> location;	// x, y coordinats on the map
	int manhattanDist;			// the manhattan distance to the goal
	int runningDist;			// distance from the starting point

	coords* parent;				// previous node
};

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
	inline pair<int, int> GetSpawnPoint() { return spawnCoords; };		// returns the coordinated for the spawn
	inline pair<int, int> GetGoalPoint() { return goalCoords; };		// returns the coordinated for the spawn
	int CalcManDist(pair<int, int> Loc, pair<int, int> End);			// returns the manhattan distance
	int CalcRunDist(unique_ptr <coords>& givenPoint);					// returns the running distance
	bool Find(deque<unique_ptr<coords>>& myList, pair<int, int> Loc);	// finds a point in the list
	void SwapFirstWithCheck(deque<unique_ptr<coords>>& myList, unique_ptr <coords>& givenPoint);	// swaps two points
	vector<pair<int, int>> Pathfind(pair<int, int> startPoint, pair<int, int> endPoint);	// gives a vector of pair int, int of all points from startPoint to endPoint
};
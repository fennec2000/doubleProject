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
				//cout << line[i] - '0'; // static debugging
				mapSquares[currentLine][i] = line[i] - '0'; // convert char to int and put it into the map

				// goal and spawn checking
				if (mapSquares[currentLine][i] == 1)
				{
					spawnCoords.first = currentLine;
					spawnCoords.second = i;
				}
				if (mapSquares[currentLine][i] == 2)
				{
					goalCoords.first = currentLine;
					goalCoords.second = i;
				}
			}
			++currentLine; // next line before repeating
			//cout << endl; // static debugging
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

int CTestMap::CalcManDist(pair<int, int> Loc, pair<int, int> End)
{
	return abs(End.first - Loc.first) + abs(End.second - Loc.second);
}

int CTestMap::CalcRunDist(unique_ptr <coords>& givenPoint)
{
	// TODO: each square costs 1 for now, needs to change if the ground changes
	return givenPoint->parent->runningDist + 1;
}

bool CTestMap::Find(deque<unique_ptr<coords>>& myList, pair<int, int> Loc)
{
	for (auto it = myList.begin(); it != myList.end(); ++it)
	{
		if ((*it)->location.first == Loc.first && (*it)->location.second == Loc.second)
			return true;
	}
	return false;
}

bool CompareCoords(unique_ptr<coords>& lhs, unique_ptr<coords>& rhs)
{
	return lhs->manhattanDist + lhs->runningDist < rhs->manhattanDist + rhs->runningDist;
}

void CTestMap::SwapFirstWithCheck(deque<unique_ptr<coords>>& myList, unique_ptr <coords>& givenPoint)
{
	for (auto it = myList.begin(); it != myList.end(); ++it)
	{
		if ((*it)->location.first == givenPoint->location.first && (*it)->location.second == givenPoint->location.second &&
			CompareCoords(givenPoint, *it))
		{
			givenPoint.swap(*it);
			return;
		}
	}
}

vector<pair<int, int>> CTestMap::Pathfind(pair<int, int> startPoint, pair<int, int> endPoint)
{
	bool found = false;
	deque <unique_ptr <coords>> openList, closedList;
	unique_ptr <coords> current(new coords), tmp(new coords), goal(new coords);


	// put the start into open list
	current->location = startPoint;
	current->manhattanDist = CalcManDist(startPoint, endPoint);
	current->runningDist = 0;
	current->parent = new coords;
	current->parent = 0;
	openList.push_back(move(current));
	current.reset(new coords);

	while (!openList.empty() && !found)
	{
		// pick best option (first)
		current = move(openList.front());
		openList.pop_front();

		// is goal?
		if (current->location.first == endPoint.first && current->location.second == endPoint.second)
		{
			// goal found
			goal = move(current);
			found = true;
			break;
		}

		// check arround
		for (int i = 0; i < EDirrection::NumberOfDirections; ++i)
		{
			// reset tmp // start with a new tmp
			tmp.reset(new coords);

			// set tmp's similar variables
			tmp->location = current->location;
			tmp->parent = current.get();

			switch (i)
			{
			case EDirrection::North:
				++(tmp->location.second);
				break;
			case EDirrection::East:
				++(tmp->location.first);
				break;
			case EDirrection::South:
				--(tmp->location.second);
				break;
			case EDirrection::West:
				--(tmp->location.first);
				break;
			default:
				break;
			}

			// is valid?
			if (tmp->location.first < 0 || tmp->location.first >= mapSize ||
				tmp->location.second < 0 || tmp->location.second >= mapSize)
			{
				continue; // std::cout of bounds go to next itt
			}

			// is goal?
			if (tmp->location.first == endPoint.first && tmp->location.second == endPoint.second)
			{
				goal = move(tmp);
				found = true;
				break; // goal found
			}

			// calc running dist
			tmp->runningDist = CalcRunDist(tmp);

			// calc manhattan dist
			tmp->manhattanDist = CalcManDist(tmp->location, endPoint);

			// push to openList
			// check closed list
			if (!Find(closedList, tmp->location))
			{
				// is in open list?
				if (Find(openList, tmp->location))
				{
					// yes - it is better?
					SwapFirstWithCheck(openList, tmp);
				}
				else
				{
					// no add it
					openList.push_back(move(tmp));
				}
			}
		}
		// sort openList
		std::sort(openList.begin(), openList.end(), CompareCoords);

		// push current to closedList
		closedList.push_back(move(current));
		current.reset(new coords);
	}
	if (!found)
	{
		std::cout << "Path not found" << endl;
		return vector<pair<int, int>>();
	}
	else
	{
		vector<pair<int, int>> mPath;	// path from start to end
		// Get a list from end to start
		coords* current;

		// put the first path into mPath
		mPath.push_back(goal->location);
		current = goal->parent;

		// loop till parent = 0
		while (current->parent != 0)
		{
			mPath.push_back(current->location);
			current = current->parent;
		}

		// flip the list
		reverse(mPath.begin(), mPath.end());

		return mPath;
	}
}

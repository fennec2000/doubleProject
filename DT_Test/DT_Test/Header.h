#pragma once
#include "stdafx.h"
#include <iostream>
#include <fstream>
#include <string>
#include <vector>

using namespace std;

typedef vector<string> vs;	// vector of strings
typedef vector<vs> vvs;		// vector of vector of strings

struct node
{
	string splitString;		// string used to split node
	bool leaf;				// flag for leaf node
};
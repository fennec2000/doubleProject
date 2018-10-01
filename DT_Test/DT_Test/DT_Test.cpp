// DT_Test.cpp : Defines the entry point for the console application.
//
#include "stdafx.h"
#include "Header.h"

int main()
{
	// read input file
	ifstream inputFile;		// input file stream
	string line;			// string for each line of input file
	vvs data;				// data stored as vector of vector of strings from the input

	string inputFileName = "train.txt";		// input file name this is forced for testing
	inputFile.open(inputFileName);			// open file
	if (inputFile.is_open())				// check open file
	{
		while (getline(inputFile, line))	// read each line
		{
			vs vectorOfSingleLineOfData;
			size_t lineSize = line.size();

			while (lineSize > 0)
			{
				size_t seperation = line.find(",");	// find the next seperator between data
				vectorOfSingleLineOfData.push_back(line.substr(0, seperation));	// store data

				if (seperation == string::npos)	// if no more seperations found then break loop, no more data to get
					break;
				line.erase(0, seperation + 1);	// remove that data and its seperator(+1)
				lineSize -= seperation;	// updating the stored line size
			}

			data.push_back(vectorOfSingleLineOfData);	// add line to data
			vectorOfSingleLineOfData.clear();			// reset vs for next line
		}
	}
	else									// file did not open stop and report
	{
		cout << "Err1: " << inputFileName << " file did not open." << endl;
		return 1;
	}

	inputFile.close();	// close input file


	// build tree


	// output result

	// pause to prevent window from closing
	system("pause");
    return 0;
}


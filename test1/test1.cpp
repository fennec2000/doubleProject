// test1.cpp: A program using the TL-Engine

#include <TL-Engine.h>	// TL-Engine include file and namespace
#include "CTestMap.h"	// map
#include <iostream>
using namespace tle;

void main()
{
	// Create a 3D engine (using TLX engine here) and open a window for it
	I3DEngine* myEngine = New3DEngine( kTLX );
	myEngine->StartWindowed();

	// Add default folder for meshes and other media
	myEngine->AddMediaFolder( "C:\\ProgramData\\TL-Engine\\Media" );
	myEngine->AddMediaFolder("Media");

	/**** Set up your scene here ****/
	CTestMap* pTest = new CTestMap("Maps\\testMap1.map");
	
	// display map
	// set up
	// mesh
	IMesh* pCubeMesh = myEngine->LoadMesh("Cube.x");
	IMesh* pFloorMesh = myEngine->LoadMesh("Floor.x");

	// models
	float cubeSize = 10.0f;
	IModel* pCube[mapSize][mapSize];
	for (int i = 0; i < mapSize; ++i)
		for (int j = 0; j < mapSize; ++j)
		{
			pCube[i][j] = pCubeMesh->CreateModel(i * cubeSize, -cubeSize / 2.0f, j * cubeSize);
			int currentCubeData = pTest->GetCubeData(i, j);
			//cout << currentCubeData << endl;
			if (currentCubeData == 0)
				pCube[i][j]->SetSkin("WhiteCube.png");
		}

	IModel* pFloor = pFloorMesh->CreateModel(0.0f, -0.1f, 0.0f);

	// camera
	ICamera* myCamera = myEngine->CreateCamera(kFPS);
	myCamera->SetRotationSpeed(10.0f);

	// fps limmiter
	float frameTime;
	float frameDeltaCurrent = 0.0f;
	int targetFps = 144;
	float frameDetla = 1.0f / targetFps;

	// The main game loop, repeat until engine is stopped
	while (myEngine->IsRunning())
	{
		// get time between frames
		frameTime = myEngine->Timer();
		frameDeltaCurrent += frameTime;

		// Draw the scene
		if (frameDeltaCurrent >= frameDetla)
		{
			frameDeltaCurrent = 0.0f;
			myEngine->DrawScene();
		}

		// keybindings
		if (myEngine->KeyHit(Key_Escape))
			myEngine->Stop();

	}

	delete pTest; // clean up the map

	// Delete the 3D engine now we are finished with it
	myEngine->Delete();
}

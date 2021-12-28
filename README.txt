This project was develop using the following tools: 
- Visual Studio 2019
- Windows 10
- Docker Desktop 4.3.2
- MongoDb

Buid:
	Run build.bat or type dotnet build in console.
Test: 
	Run test.bat, a docker container running in localhost with default MongoDb port is a must.
Host:
	Run start.bat. 
	
In order to use the api (and read documentation) you could type in your browser: http://localhost/swagger/index.html or use postman.
In order to check if webhost is runing, request a get to : http://localhost/api/health/WebHost
In order to check if mongoDb is runing and the application is sucessfull connected, request a get to : http://localhost/api/health/database

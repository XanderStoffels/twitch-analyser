#!/bin/bash
cd ./Shared
dotnet restore
dotnet publish -o publish -r linux-x64

cd ../ChatBot
dotnet restore
dotnet publish -o publish -r linux-x64

cd ../Persistor
dotnet restore
dotnet publish -o publish -r linux-x64

cd ..
docker-compose up --build

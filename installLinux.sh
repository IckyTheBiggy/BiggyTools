#!/bin/bash

dotnet publish -c Release -r linux-x64
sudo cp bin/Release/net9.0/linux-x64/publish/BiggyTools /usr/bin/biggytools
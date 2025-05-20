#!/bin/bash

dotnet publish -c Release -r linux-x64 -p:PublishAot=true
sudo cp bin/Release/net9.0/linux-x64/native/BiggyTools /usr/bin/biggytools
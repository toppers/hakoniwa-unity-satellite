#!/bin/bash

if [ -d plugin/plugin-srcs ]
then
    :
else
    echo "ERROR: can not find submodule plugin"
    echo "git pull"
    echo "git submodule update --init --recursive"
    exit 1
fi

cd plugin

bash install.bash
cd ..

cp satelite/hakoniwa_path.json plugin/plugin-srcs/
cp -rp satelite/Assets/* plugin/plugin-srcs/Assets/

#! /bin/sh

project="ELB"
projectPath="$(pwd)/Empire - The Last Battle"

# echo "Attempting to build $project for Windows"
# /Applications/Unity/Unity.app/Contents/MacOS/Unity \
#   -batchmode \
#   -nographics \
#   -silent-crashes \
#   -logFile $(pwd)/unity.log \
#   -projectPath "$projectPath" \
#   -buildWindowsPlayer "$(pwd)/Build/windows/$project.exe" \
#   -quit
#
# echo "Attempting to build $project for OS X"
# /Applications/Unity/Unity.app/Contents/MacOS/Unity \
#   -batchmode \
#   -nographics \
#   -silent-crashes \
#   -logFile $(pwd)/unity.log \
#   -projectPath "$projectPath" \
#   -buildOSXUniversalPlayer "$(pwd)/Build/osx/$project.app" \
#   -quit

echo "Attempting to build $project for Web"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile \
  -projectPath "$projectPath" \
  -buildWebPlayer  "$(pwd)/Build/ELB" \
  -quit
#
# echo 'Zipping files for distribution'
# zip -r Build/ELB_OSX.zip Build/osx
# zip -r Build/ELB_Win.zip Build/windows

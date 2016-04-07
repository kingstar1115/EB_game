#! /bin/sh

echo 'Downloading from http://download.unity3d.com/download_unity/e87ab445ead0/MacEditorInstaller/Unity-5.3.2f1.pkg: '
curl -o Unity.pkg http://download.unity3d.com/download_unity/e87ab445ead0/MacEditorInstaller/Unity-5.3.2f1.pkg

echo 'Installing Unity.pkg'
sudo installer -dumplog -package Unity.pkg -target /

echo 'Removing unity'
rm Unity.pkg

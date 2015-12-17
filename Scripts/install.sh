#! /bin/sh

echo 'Downloading from http://download.unity3d.com/download_unity/3757309da7e7/MacEditorInstaller/Unity-5.2.2f1.pkg: '
curl -o Unity.pkg http://download.unity3d.com/download_unity/3757309da7e7/MacEditorInstaller/Unity-5.2.2f1.pkg

echo 'Installing Unity.pkg'
sudo installer -dumplog -package Unity.pkg -target /

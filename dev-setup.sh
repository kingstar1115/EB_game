git config --global credential.helper wincred

alasdair="Alasdair Hurst"
steven="Steven P Simmons"
ewan="Ewan Harris"
martin="Martin Dawson"
username=${!$1}

git remote add alasdair http://github.com/alasdairhurst/ELB
git remote add steven http://github.com/epicpants90/ELB
git remote add ewan http://github.com/eharris93/ELB
git remote add martin http://github.com/yoshimiii/ELB
git remote add upstream http://github.com/eharris93/ELB

git fetch --all

git config --global user.name $username

echo "Set username to: $username"
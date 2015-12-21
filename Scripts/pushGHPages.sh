#! /bin/sh

if [ "$TRAVIS_REPO_SLUG" == "eharris93/ELB" ] && [ "$TRAVIS_PULL_REQUEST" == "false" ] && [ "$TRAVIS_BRANCH" == "master" ]; then

echo "Looks good to publish to gh-page"

cp Build/ELB/ELB.html ./ELB.html
cp Build/ELB/ELB.unity3d ./ELB.unity3d

git config --global user.email "ewanharris93@gmail.com"
git config --global user.name "travis-ci"

git add .
git commit -m "Push to github pages"

git push --force --quiet "https://${GH_TOKEN}@${GH_REF}" > /dev/null 2>&1

echo "Publish done"
fi

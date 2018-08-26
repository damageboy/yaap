#!/bin/bash

set -e

COMMITER=$(git config user.email)
COMMITER_NAME=$(git config user.name)

mono ~/docfx/docfx.exe ./docs/docfx.json


SOURCE_DIR=$PWD
TEMP_REPO_DIR=$PWD/../yaap-pages.git

echo "Removing temporary doc directory $TEMP_REPO_DIR"
rm -rf $TEMP_REPO_DIR
mkdir $TEMP_REPO_DIR

# Make sure we pull latest gh-pages
git fetch

echo "Cloning the repo with the gh-pages branch"
git clone $PWD --branch gh-pages $TEMP_REPO_DIR
(cd $TEMP_REPO_DIR; git reset --hard origin/gh-pages)

#echo "Clear repo directory"
#cd $TEMP_REPO_DIR
#git rm -r *

echo "Copy documentation into the repo"
(cd $SOURCE_DIR/docs/_site/;  tar cf - *) | (cd $TEMP_REPO_DIR; tar xf -)

echo "Push the new docs to the remote branch"
(cd $TEMP_REPO_DIR; git config user.email $COMMITER; git config user.name $COMMITER_NAME; git add . -A; git commit -m "Update generated documentation"; git push origin gh-pages)
git push origin gh-pages:gh-pages

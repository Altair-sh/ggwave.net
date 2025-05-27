#!/usr/bin/bash
###############################################################################
# For some reason cmake can't override library output directory on windows,   #
# so i made this script to find libggwave and copy it.                        # 
###############################################################################

set -eo pipefail
TARGET_DIR=$1
FILE_NAME=$2
DESTINATION_DIR=$3

FILE_PATH=$(find $TARGET_DIR -name $FILE_NAME -print -quit)
if [ -z "$FILE_PATH" ]; then
    echo "Error: '$FILE_NAME' not found in '$TARGET_DIR'"
    exit 1
fi
echo "Found '$FILE_PATH'"
mkdir -p "$DESTINATION_DIR"
cp -v "$FILE_PATH" "$DESTINATION_DIR"

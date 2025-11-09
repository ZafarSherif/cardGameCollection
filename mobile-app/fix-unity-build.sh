#!/bin/bash
# Script to fix Unity's nested Build folder structure
# Run this after building Unity WebGL

cd "$(dirname "$0")/public/unity"

if [ -d "Build/Build" ]; then
  echo "Fixing Unity build structure..."

  # Move index.html to parent if it exists
  if [ -f "Build/index.html" ]; then
    mv Build/index.html .
  fi

  # Move Build/* files up one level
  mv Build/Build/* Build/

  # Remove empty nested Build folder
  rmdir Build/Build

  # Remove duplicate thumbnail.png.txt if it exists
  rm -f Build/thumbnail.png.txt

  echo "✅ Unity build structure fixed!"
  echo ""
  echo "Structure:"
  echo "public/unity/"
  echo "├── Build/"
  ls -1 Build/ | sed 's/^/│   ├── /'
  echo "└── index.html"
else
  echo "✅ Build structure is already correct!"
fi

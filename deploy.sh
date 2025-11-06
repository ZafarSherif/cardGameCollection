#!/bin/bash

echo "🎮 Deploying Unity Build to React Native..."
echo ""

# Project directories
UNITY_BUILD="unity-games/builds/webgl"
REACT_PUBLIC="mobile-app/public"

# Check if Unity build exists
if [ ! -d "$UNITY_BUILD" ]; then
    echo "❌ Error: Unity build not found at $UNITY_BUILD"
    echo "Please build the Unity project first!"
    echo ""
    echo "Steps:"
    echo "1. Open Unity project: unity-games/CardGames/"
    echo "2. File → Build Settings → WebGL"
    echo "3. Build to: unity-games/builds/webgl/"
    exit 1
fi

# Remove old Unity build from React Native
echo "📦 Removing old Unity build..."
rm -rf "$REACT_PUBLIC/unity"
rm -rf "$REACT_PUBLIC/Build"
rm -rf "$REACT_PUBLIC/TemplateData"
rm -f "$REACT_PUBLIC/index.html"

# Copy new Unity build
echo "📋 Copying new Unity build..."
cp -r "$UNITY_BUILD"/* "$REACT_PUBLIC/"

# Rename the Build folder to unity for cleaner structure (optional)
if [ -d "$REACT_PUBLIC/Build" ]; then
    mkdir -p "$REACT_PUBLIC/unity"
    mv "$REACT_PUBLIC/Build" "$REACT_PUBLIC/unity/Build"
    mv "$REACT_PUBLIC/TemplateData" "$REACT_PUBLIC/unity/TemplateData"
    mv "$REACT_PUBLIC/index.html" "$REACT_PUBLIC/unity/index.html"
fi

# Verify deployment
if [ -f "$REACT_PUBLIC/unity/index.html" ]; then
    echo ""
    echo "✅ Deploy successful!"
    echo ""
    echo "📁 Unity files copied to: $REACT_PUBLIC/unity/"
    echo ""
    echo "🌐 Next steps:"
    echo "   1. Make sure React Native dev server is running"
    echo "   2. Open browser and hard refresh (Cmd+Shift+R)"
    echo "   3. Navigate to Solitaire game"
    echo "   4. Test the updated game!"
    echo ""
else
    echo ""
    echo "❌ Deploy failed!"
    echo "Check that Unity build exists and is valid"
    exit 1
fi

#!/bin/bash

# Cross-platform publish script for CNC Predictive Maintenance Tool
# Builds self-contained single-file executables for Windows, macOS, and Linux

set -e

PROJECT_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_FILE="$PROJECT_DIR/BACKEND_CNC-IIoT-Predictive-Maintenance-Tool-/BACKEND_CNC-IIoT-Predictive-Maintenance-Tool-.csproj"
OUTPUT_DIR="$PROJECT_DIR/publish"
VERSION="1.0.0"

echo "================================================"
echo "CNC Predictive Maintenance Tool - Build Script"
echo "================================================"
echo ""

# Clean previous builds
echo "Cleaning previous builds..."
rm -rf "$OUTPUT_DIR"
mkdir -p "$OUTPUT_DIR"

# Define target runtimes
declare -a RUNTIMES=(
    "win-x64:Windows (64-bit)"
    "osx-x64:macOS (Intel)"
    "osx-arm64:macOS (Apple Silicon)"
    "linux-x64:Linux (64-bit)"
)

# Build for each platform
for RUNTIME_INFO in "${RUNTIMES[@]}"; do
    RUNTIME="${RUNTIME_INFO%%:*}"
    DESCRIPTION="${RUNTIME_INFO#*:}"

    echo ""
    echo "Building for $DESCRIPTION ($RUNTIME)..."
    echo "----------------------------------------"

    dotnet publish "$PROJECT_FILE" \
        -c Release \
        -r "$RUNTIME" \
        -o "$OUTPUT_DIR/$RUNTIME" \
        --nologo \
        -v quiet

    # Get the output file name
    if [[ "$RUNTIME" == win-* ]]; then
        EXE_NAME="CNC-Predictive-Maintenance.exe"
    else
        EXE_NAME="CNC-Predictive-Maintenance"
    fi

    # Check if build succeeded
    if [ -f "$OUTPUT_DIR/$RUNTIME/$EXE_NAME" ]; then
        SIZE=$(du -h "$OUTPUT_DIR/$RUNTIME/$EXE_NAME" | cut -f1)
        echo "✓ Built successfully: $EXE_NAME ($SIZE)"
    else
        echo "✗ Build failed for $RUNTIME"
        exit 1
    fi
done

echo ""
echo "================================================"
echo "Build Complete!"
echo "================================================"
echo ""
echo "Output files are in: $OUTPUT_DIR"
echo ""
echo "Files created:"
for RUNTIME_INFO in "${RUNTIMES[@]}"; do
    RUNTIME="${RUNTIME_INFO%%:*}"
    DESCRIPTION="${RUNTIME_INFO#*:}"
    if [[ "$RUNTIME" == win-* ]]; then
        EXE="CNC-Predictive-Maintenance.exe"
    else
        EXE="CNC-Predictive-Maintenance"
    fi
    SIZE=$(du -h "$OUTPUT_DIR/$RUNTIME/$EXE" 2>/dev/null | cut -f1 || echo "N/A")
    echo "  - $RUNTIME/$EXE ($SIZE) - $DESCRIPTION"
done
echo ""
echo "To distribute, share the single executable file for each platform."
echo "Users can run it directly - no .NET installation required!"

#!/bin/bash
#
# CNC Predictive Maintenance Tool - Installation Script
# Usage: curl -fsSL https://raw.githubusercontent.com/JonaShoukri/CNC-IIoT-Predictive-Maintenance-Tool/main/install.sh | bash
#

set -e

REPO="JonaShoukri/CNC-IIoT-Predictive-Maintenance-Tool"
APP_NAME="CNC-Predictive-Maintenance"
INSTALL_DIR="/usr/local/bin"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo ""
echo "=========================================="
echo "  CNC Predictive Maintenance Installer"
echo "=========================================="
echo ""

# Detect OS and architecture
detect_platform() {
    OS="$(uname -s)"
    ARCH="$(uname -m)"

    case "$OS" in
        Linux*)
            PLATFORM="linux-x64"
            EXT="tar.gz"
            ;;
        Darwin*)
            case "$ARCH" in
                arm64)
                    PLATFORM="osx-arm64"
                    ;;
                x86_64)
                    PLATFORM="osx-x64"
                    ;;
                *)
                    echo -e "${RED}Unsupported architecture: $ARCH${NC}"
                    exit 1
                    ;;
            esac
            EXT="tar.gz"
            ;;
        *)
            echo -e "${RED}Unsupported operating system: $OS${NC}"
            echo "For Windows, use PowerShell:"
            echo "  irm https://raw.githubusercontent.com/$REPO/main/install.ps1 | iex"
            exit 1
            ;;
    esac

    echo -e "Detected platform: ${GREEN}$PLATFORM${NC}"
}

# Get latest release version
get_latest_version() {
    echo "Fetching latest release..."
    LATEST_VERSION=$(curl -fsSL "https://api.github.com/repos/$REPO/releases/latest" | grep '"tag_name":' | sed -E 's/.*"([^"]+)".*/\1/')

    if [ -z "$LATEST_VERSION" ]; then
        echo -e "${RED}Failed to fetch latest version${NC}"
        exit 1
    fi

    echo -e "Latest version: ${GREEN}$LATEST_VERSION${NC}"
}

# Download and install
install() {
    DOWNLOAD_URL="https://github.com/$REPO/releases/download/$LATEST_VERSION/$APP_NAME-$PLATFORM.$EXT"
    TMP_DIR=$(mktemp -d)

    echo ""
    echo "Downloading from: $DOWNLOAD_URL"

    if ! curl -fsSL "$DOWNLOAD_URL" -o "$TMP_DIR/$APP_NAME.$EXT"; then
        echo -e "${RED}Download failed. Make sure a release exists.${NC}"
        rm -rf "$TMP_DIR"
        exit 1
    fi

    echo "Extracting..."
    cd "$TMP_DIR"
    tar -xzf "$APP_NAME.$EXT"

    echo "Installing to $INSTALL_DIR..."

    # Check if we need sudo
    if [ -w "$INSTALL_DIR" ]; then
        mv "$APP_NAME" "$INSTALL_DIR/"
        chmod +x "$INSTALL_DIR/$APP_NAME"
    else
        echo -e "${YELLOW}Requesting sudo access to install to $INSTALL_DIR${NC}"
        sudo mv "$APP_NAME" "$INSTALL_DIR/"
        sudo chmod +x "$INSTALL_DIR/$APP_NAME"
    fi

    # Cleanup
    rm -rf "$TMP_DIR"

    echo ""
    echo -e "${GREEN}Installation complete!${NC}"
    echo ""
    echo "Run the application with:"
    echo -e "  ${GREEN}$APP_NAME${NC}"
    echo ""
}

# Verify installation
verify() {
    if command -v "$APP_NAME" &> /dev/null; then
        echo -e "${GREEN}Verification: $APP_NAME is accessible from terminal${NC}"
    else
        echo -e "${YELLOW}Note: You may need to restart your terminal or add $INSTALL_DIR to your PATH${NC}"
    fi
}

# Main
detect_platform
get_latest_version
install
verify

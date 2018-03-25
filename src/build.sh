#!/usr/bin/env bash

##########################################################################
# This is the Cake bootstrapper script for Linux and OS X.
# This version was download from https://github.com/larzw/Cake.Paket
# It was modified to use paket (instead of NuGet) for dependency management.
# Feel free to change this file to fit your needs.
##########################################################################

# Define default arguments.
SCRIPT="build.cake"
TARGET="Default"
CONFIGURATION="Release"
VERBOSITY="verbose"
DRYRUN=
SHOW_VERSION=false
PAKET="./.paket"
CAKE="./packages/cake/Cake"
TOOLS="./packages/cake"
ADDINS="./packages/cake"
MODULES="./packages/cake"
SCRIPT_ARGUMENTS=()

# Parse arguments.
for i in "$@"; do
    case $1 in
        -s|--script) SCRIPT="$2"; shift ;;
        -t|--target) TARGET="$2"; shift ;;
        -c|--configuration) CONFIGURATION="$2"; shift ;;
        -v|--verbosity) VERBOSITY="$2"; shift ;;
        -d|--dryrun) DRYRUN="-dryrun" ;;
        --version) SHOW_VERSION=true ;;
        --paket) PAKET="$2"; shift ;;
        --cake) CAKE="$2"; shift ;;
        --tools) TOOLS="$2"; shift ;;
        --addins) ADDINS="$2"; shift ;;
        --modules) MODULES="$2"; shift ;;
        --) shift; SCRIPT_ARGUMENTS+=("$@"); break ;;
        *) SCRIPT_ARGUMENTS+=("$1") ;;
    esac
    shift
done

# Used to convert relative paths to absolute paths.
SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
function twoAbsolutePath {
    cd ~ && cd $SCRIPT_DIR
    absolutePath=$(cd $1 && pwd)
    echo $absolutePath
}

PAKET_DIR=$(twoAbsolutePath $PAKET)

# Make sure the .paket directory exits.
if [ ! -d "$PAKET_DIR" ]; then
    echo "Could not find .paket at '$PAKET_DIR'."
    exit 1
fi

# Set paket directory enviornment variable.
export PAKET=$PAKET_DIR

# If paket.exe does not exits then download it using paket.bootstrapper.exe.
PAKET_EXE=$PAKET_DIR/paket.exe
if [ ! -f "$PAKET_EXE" ]; then

    # If paket.bootstrapper.exe exits then run it.
    PAKET_BOOTSTRAPPER_EXE=$PAKET_DIR/paket.bootstrapper.exe
    if [ ! -f "$PAKET_BOOTSTRAPPER_EXE" ]; then
        echo "Could not find paket.bootstrapper.exe at '$PAKET_BOOTSTRAPPER_EXE'."
        exit 1
    fi

    # Download paket.exe.
    mono "$PAKET_BOOTSTRAPPER_EXE"

    if [ ! -f "$PAKET_EXE" ]; then
        echo "Could not find paket.exe at '$PAKET_EXE'."
        exit 1
    fi
fi

# Restore the dependencies.
mono "$PAKET_EXE" restore

# tools
if [ -d "$TOOLS" ]; then
    TOOLS_DIR=$(twoAbsolutePath $TOOLS)
    export CAKE_PATHS_TOOLS=$TOOLS_DIR
else
    echo "Could not find tools directory at '$TOOLS'."
fi

# addins
if [ -d "$ADDINS" ]; then
    ADDINS_DIR=$(twoAbsolutePath $ADDINS)
    export CAKE_PATHS_ADDINS=$ADDINS_DIR
else
    echo "Could not find addins directory at '$ADDINS'."
fi

# modules
if [ -d "$MODULES" ]; then
    MODULES_DIR=$(twoAbsolutePath $MODULES)
    export CAKE_PATHS_MODULES=$MODULES_DIR
else
    echo "Could not find modules directory at '$MODULES'."
fi

# Make sure that Cake has been installed.
CAKE_DIR=$(twoAbsolutePath $CAKE)
CAKE_EXE=$CAKE_DIR/Cake.exe
if [ ! -f "$CAKE_EXE" ]; then
    echo "Could not find Cake.exe at '$CAKE_EXE'."
    exit 1
fi

# Start Cake.
if $SHOW_VERSION; then
    exec mono "$CAKE_EXE" -version
else
    exec mono "$CAKE_EXE" $SCRIPT -verbosity=$VERBOSITY -configuration=$CONFIGURATION -target=$TARGET $DRYRUN "${SCRIPT_ARGUMENTS[@]}"
fi
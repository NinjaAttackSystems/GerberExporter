# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

GerberExporter is a .NET 9 library that converts Gerber file shapes into STL or SVG formats. Currently supports circle and rectangle elements with plans to add other shapes.

## Build and Test Commands

```bash
# Build the solution
dotnet build

# Run tests
dotnet test

# Run tests with details
dotnet test --logger "console;verbosity=detailed"

# Run a specific test
dotnet test --filter "FullyQualifiedName~TestName"

# Build in release mode
dotnet build -c Release
```

## Architecture

The codebase follows a strategy pattern for parsing Gerber files:

1. **BrokerDocument**: Central data model containing all parsed shapes
2. **GerberExporter**: Main entry point that orchestrates file parsing and shape extraction
3. **Strategy Pattern**: Used for parsing different Gerber line types
   - `GerberLineStrategyContext`: Manages parsing strategies
   - `IGerberLineStrategy`: Interface for line parsing strategies
   - Concrete strategies: `FlashCircleLine`, `DrawRectangleLine`, `GerberSelectApertureStrategy`, etc.
4. **Shape Models**: Located in `Common/Models/`, implements `IShape` interface
5. **Exporters**: `StlGenerator` and `SvgGenerator` convert BrokerDocument to target formats

## Key Components

- **ExporterState**: Maintains state during Gerber file parsing (current aperture, position, etc.)
- **Aperture System**: Models different aperture types used in Gerber files
- **Test Files**: `mobo.GTS` and `Gerber_TopSolderMaskLayer.GTS` are sample Gerber files for testing

## Testing

Uses NUnit framework. Test files are automatically copied to output directory during build.
# GerberExporter
A library to export Gerber file shapes into a more friendly format which can be converted to other formats like STL or SVG.

## Project Status
The exporter now supports multiple shape types and is actively being developed. You can input a Gerber file and export it as STL or SVG containing all the geometric elements from the Gerber file.

### Supported Features
- **Shape Types:**
  - Circles
  - Rectangles
  - Rounded Rectangles
  - Ovals (rendered as rectangles/circles based on dimensions)
  - Aperture Macros (basic support)
  
- **Export Formats:**
  - STL (3D format for PCB visualization)
  - SVG (2D vector format)

- **Technical Details:**
  - Proper coordinate scaling (6 decimal places, 1:1,000,000 scale)
  - Negative coordinate support
  - Cross-platform file path support
  - .NET 9 compatibility

### Recent Updates
- Implemented aperture macro parsing for complex shape definitions
- Fixed coordinate scaling issues (was dividing by 10,000 instead of 1,000,000)
- Fixed circle sizing (removed erroneous /2 factor)
- Fixed rounded rectangle rendering ("pac-man" issue resolved)
- Added support for detecting when rounded rectangles should be rendered as circles
- Upgraded to .NET 9

Below is a sample of the exported STL
<img width="768" alt="image" src="https://user-images.githubusercontent.com/33508/210236467-b6ded390-4efc-48c0-9ca6-93374f76c5e8.png">

## Building and Running

### Prerequisites
- .NET 9 SDK

### Build
```bash
dotnet build
```

### Run Tests
```bash
dotnet test
```

### Run Console Application
```bash
cd GerberExporterConsole
dotnet run
```

This will process the sample `mobo.GTS` file and generate `mobo_output.stl`.

## Architecture

The library uses a strategy pattern for parsing different Gerber commands:
- Aperture definition strategies (Circle, Rectangle, RoundRect, Oval, Macro)
- Flash command strategy for placing shapes
- Line drawing strategy (for traces - in development)

Each shape type implements the `IShape` interface and can generate both STL and SVG output.

namespace GerberExporter.Gerber.GerberLineStrategy;

public enum GerberParsedLineType
{
    CircleAperture = 0,
    MacroCircleAperture = 5,
    RectangleAperture = 10,

    DrawRectangle = 100,
    FlashCircle = 105,
    DrawPolygon = 110,
    DrawOval = 115,

    Move = 200,
    Comment = 205

}
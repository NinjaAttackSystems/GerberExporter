using System.Text.RegularExpressions;
using GerberExporter.GerberModels;

namespace GerberExporter.Gerber.GerberLineStrategy;

public class GerberApertureMacroStrategy : IGerberLineStrategy
{
    public bool CanHandleLine(string line)
    {
        return line.StartsWith("%AM") && !line.EndsWith("*%");
    }

    public IGerberParsedLine Handle(string line, ExporterState state)
    {
        // Parse macro definition start
        // Example: %AMRoundRect*
        var match = Regex.Match(line, @"%AM(.+)\*");
        if (match.Success)
        {
            var macroName = match.Groups[1].Value;
            state.CurrentMacroName = macroName;
            state.CurrentMacroDefinition = new List<string>();
        }
        
        return null;
    }
}

public class GerberMacroLineStrategy : IGerberLineStrategy
{
    // Need to handle this differently - we can't access state in CanHandleLine
    // So we'll make this stateful within the strategy
    private static bool InMacroDefinition = false;
    
    public bool CanHandleLine(string line)
    {
        // Check if we're starting a macro
        if (line.StartsWith("%AM") && !line.EndsWith("*%"))
        {
            InMacroDefinition = true;
            return false; // Let the macro strategy handle the start
        }
        
        // Check if we're ending a macro
        if (InMacroDefinition && line.EndsWith("*") && !line.StartsWith("%"))
        {
            var result = true;
            InMacroDefinition = false;
            return result;
        }
        
        // Handle lines within macro definition
        return InMacroDefinition && !line.StartsWith("%");
    }

    public IGerberParsedLine Handle(string line, ExporterState state)
    {
        // Only handle if we're currently in a macro definition
        if (!string.IsNullOrEmpty(state.CurrentMacroName) && state.CurrentMacroDefinition != null)
        {
            state.CurrentMacroDefinition.Add(line);
            
            // Check if this is the end of the macro
            if (line.EndsWith("*"))
            {
                // Store the complete macro
                state.Macros[state.CurrentMacroName] = state.CurrentMacroDefinition;
                state.CurrentMacroName = null;
                state.CurrentMacroDefinition = null;
            }
        }
        
        return null;
    }
}
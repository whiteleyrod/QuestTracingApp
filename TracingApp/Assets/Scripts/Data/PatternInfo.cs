using System;

namespace TracingApp.Data
{
    /// <summary>
    /// Describes a single pattern with its metadata.
    /// </summary>
    [Serializable]
    public class PatternInfo
    {
        public string Key;           // e.g., "level1_horizontal"
        public string DisplayName;   // e.g., "Horizontal Line"
        public int Complexity;       // 1-5

        // All 20 patterns
        public static readonly PatternInfo[] AllPatterns = new PatternInfo[]
        {
            // Level 1 - Straight Lines
            new PatternInfo { Key = "level1_horizontal", DisplayName = "Horizontal Line", Complexity = 1 },
            new PatternInfo { Key = "level1_vertical", DisplayName = "Vertical Line", Complexity = 1 },
            new PatternInfo { Key = "level1_diagonal", DisplayName = "Diagonal Line", Complexity = 1 },
            new PatternInfo { Key = "level1_zigzag", DisplayName = "Zigzag", Complexity = 1 },

            // Level 2 - Simple Shapes
            new PatternInfo { Key = "level2_square", DisplayName = "Square", Complexity = 2 },
            new PatternInfo { Key = "level2_triangle", DisplayName = "Triangle", Complexity = 2 },
            new PatternInfo { Key = "level2_rectangle", DisplayName = "Rectangle", Complexity = 2 },
            new PatternInfo { Key = "level2_cross", DisplayName = "Cross", Complexity = 2 },

            // Level 3 - Complex Shapes
            new PatternInfo { Key = "level3_pentagon", DisplayName = "Pentagon", Complexity = 3 },
            new PatternInfo { Key = "level3_hexagon", DisplayName = "Hexagon", Complexity = 3 },
            new PatternInfo { Key = "level3_star", DisplayName = "Star", Complexity = 3 },
            new PatternInfo { Key = "level3_arrow", DisplayName = "Arrow", Complexity = 3 },

            // Level 4 - Curves
            new PatternInfo { Key = "level4_circle", DisplayName = "Circle", Complexity = 4 },
            new PatternInfo { Key = "level4_oval", DisplayName = "Oval", Complexity = 4 },
            new PatternInfo { Key = "level4_semicircle", DisplayName = "Semicircle", Complexity = 4 },
            new PatternInfo { Key = "level4_wave", DisplayName = "Wave", Complexity = 4 },

            // Level 5 - Complex Curves
            new PatternInfo { Key = "level5_heart", DisplayName = "Heart", Complexity = 5 },
            new PatternInfo { Key = "level5_spiral", DisplayName = "Spiral", Complexity = 5 },
            new PatternInfo { Key = "level5_figure8", DisplayName = "Figure Eight", Complexity = 5 },
            new PatternInfo { Key = "level5_scurve", DisplayName = "S-Curve", Complexity = 5 },
        };
    }
}

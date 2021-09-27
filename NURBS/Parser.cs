using System.Collections.Generic;
using CGLabPlatform;

namespace nurbs
{
    public static class Parser
    {
        public static DVector4? Parse(string text)
        {
            string[] textParts = text.Split(' ');

            var parsedDoubles = new List<double>();

            foreach (var part in textParts)
            {
                if (double.TryParse(part, out double result))
                {
                    parsedDoubles.Add(result);
                }
            }

            if (parsedDoubles.Count != 4) return null;
            if (parsedDoubles[3] < 0.5) parsedDoubles[3] = 0.5;

            return new DVector4(parsedDoubles[0], parsedDoubles[1], parsedDoubles[2], parsedDoubles[3]);
        }
    }
}
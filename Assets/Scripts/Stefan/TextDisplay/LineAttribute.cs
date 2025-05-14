/*
Yarn Spinner is licensed to you under the terms found in the file LICENSE.md.
*/

namespace Dialogue
{
    public class LineAttribute
    {
        public readonly int Position;
        public readonly string Protocol;

        public LineAttribute(int position, string name)
        {
            Position = position;
            Protocol = name;
        }
    }
}

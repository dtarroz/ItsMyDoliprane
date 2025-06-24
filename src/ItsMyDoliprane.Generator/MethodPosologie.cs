using System.Collections.Generic;

namespace ItsMyDoliprane.Generator
{
    public class MethodPosologie
    {
        public int MedicationFilterHour { get; set; }
        public List<MethodRegle> Methods { get; set; }
    }

    public class MethodRegle
    {
        public string Name { get; set; }
        public string Content { get; set; }
    }
}

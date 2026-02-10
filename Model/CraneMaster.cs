using System.Diagnostics.Contracts;

namespace MESCHECKLIST.Model
{
    public class CraneMaster
    {
        public string engineNo { get; set; }
        public string model { get; set; }
        public string chassisNo {get;set;}
    }

    public class SaveHistoryCard_Make
    {
       public string EngineNo { get; set; }
        public string Make { get; set; }
        public int Id { get; set; }
        public string UpdatedBy { get; set; }
    }
}

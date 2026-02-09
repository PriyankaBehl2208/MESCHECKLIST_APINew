namespace MESCHECKLIST.Model
{
    public class MESCHECK_MODEL
    {
    }



    public class MESVALIDATE_USER
    {

        public String? UserName { get; set; }
        public String? Passward { get; set; }
    }

    public class MES_PREPDI_ENGINE_PIN

    {


        public String? Pin { get; set; }


    }
    public class MES_PREPDI_UploadImage
    {
        public IFormFile File { get; set; }

        public string? Engineno { get; set; }

        public string? MASTER_ID { get; set; }


    }
    public class MES_PREPDI_GETMAIN_LIST
    {
        public String? Department { get; set; }
        public String? Engineno { get; set; }



        public String? Modelno { get; set; }
        public String? Pin { get; set; }

        public String? Name { get; set; }



    }

    public class MES_PREPDI_UPDATEREMARKS

    {

        public string? Remarks { get; set; }

        public string? Remarks_ID { get; set; }



    }

    public class MES_PREPDI_Engine

    {

        public string? Engine_no { get; set; }





    }


    public class MESPREPDI_UPDATEREMARKS

    {

        public string? Remarks { get; set; }

        public string? Remarks_ID { get; set; }



    }


    public class MEPREPDIMODEL_GETMAIN_LIST
    {
        public String? Department { get; set; }
        public String? Engineno { get; set; }



        public String? Modelno { get; set; }
        public String? Pin { get; set; }

        public String? Name { get; set; }



    }



    public class MEPREPDIMODEL_GETMAIN_LISTInspection
    {
      
        public String? Engineno { get; set; }



        public String? Modelno { get; set; }
        public String? Pin { get; set; }

        public String? Name { get; set; }



    }


}

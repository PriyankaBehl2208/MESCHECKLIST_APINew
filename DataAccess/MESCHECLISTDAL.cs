using Microsoft.Data.SqlClient;
using System.Data;

namespace MESCHECKLIST.DataAccess
{
    public class MESCHECLISTDAL
    {

        private readonly CommonDAL _commonDAL;

        public MESCHECLISTDAL(CommonDAL commonDAL)
        {
            _commonDAL = commonDAL;
        }

      
        public async Task<DataTable> VALIDATE_USER()
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[] {


                };
                DataTable dt = await _commonDAL.ExecuteStoredProcedureAsync("Sp_Validate_UESR_DETAILS", parameters);
                //


                //DataTable dt = await _commonDAL.ExecuteStoredProcedureAsync("GET_LOGIN_LIST", parameters);
                if (dt != null && dt.Rows.Count > 0)
                {

                    if (dt.Rows[0][0].ToString() == "NA")
                    {
                        return null;

                    }
                    else
                    {
                        return dt;
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return null;
        }



    }
}

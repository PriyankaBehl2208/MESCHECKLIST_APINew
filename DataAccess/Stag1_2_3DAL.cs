using MESCHECKLIST.Model;
using Microsoft.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace MESCHECKLIST.DataAccess
{
    public class Stag1_2_3DAL
    {
        static string ORACLECON = "Data Source=(DESCRIPTION =" +
  "(ADDRESS = (PROTOCOL = TCP)(HOST = eklprod-scan.escortskubota.com)(PORT = 1527))" +
  "(CONNECT_DATA =" + "(SERVER = DEDICATED)" +
  "(SERVICE_NAME = PROD)));" +
  "User Id=barcode;Password=BaRc0d32019;";

        private readonly CommonDAL _commonDAL;

        public Stag1_2_3DAL(CommonDAL commonDAL)
        {
            _commonDAL = commonDAL;
        }

        public async Task<DataTable> GetHistoryCard(string EngineNo,string Model,string chassisNo)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[] {

                     new SqlParameter("@EngineNo", EngineNo),
                      new SqlParameter("@Model", Model),
                      new SqlParameter("@ChassisNo", chassisNo),
                };
                DataTable dt = await _commonDAL.ExecuteStoredProcedureAsync("SP_GetHistoryCard", parameters);

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

        public async Task<DataTable> GET_ENGINEVALUE(string Pin)
        {



            DataTable ENGINEDetails = new DataTable();

            try
            {


                // Step 3: Oracle validation
                using (OracleConnection con = new OracleConnection(ORACLECON))
                {
                    await con.OpenAsync();

                    using (OracleDataAdapter da = new OracleDataAdapter($"SELECT CM.ITEM_CODE CF_CODE,CM.CATEGORY,CM.SHORTNAME MODEL,CM.DESCRIPTION,CS.JOBID\r\n,TO_CHAR(XM.PLAN_DATE, 'dd-Mon-yyyy HH24:MI:SS') LOADING_DATE\r\n,XM.CREATEDBY LOADING_BY\r\n,CS.CHASISNO CHASIS_NO\r\n,(SELECT TO_CHAR(S.SCAN_DATE, 'dd-Mon-yyyy HH24:MI:SS') \r\n FROM XXES_SCAN_TIME S \r\n WHERE S.JOBID = CS.CHASISNO OR S.JOBID = CS.JOBID\r\nORDER BY S.SCAN_DATE DESC \r\n FETCH FIRST 1 ROW ONLY) AS ENGINE_MAPPING_DATE\r\n,(SELECT  S.SCANNED_BY \r\n FROM XXES_SCAN_TIME S \r\n WHERE S.JOBID = CS.CHASISNO OR S.JOBID = CS.JOBID\r\n ORDER BY S.SCAN_DATE DESC \r\n FETCH FIRST 1 ROW ONLY) ENGINE_MAPPING_BY\r\n,TO_CHAR(CS.ROLLOUTDATE, 'dd-Mon-yyyy HH24:MI:SS') ROLLOUTDATE,CS.ROLLOUTBY,CM.ENGINE_DCODE,CS.ENGINE_SRNO,'' PRODUCT_NO,\r\n(SELECT current_status FROM apps.mtl_serial_numbers \r\nwhere serial_number=CS.CHASISNO order By inventory_item_id desc fetch FIRST 1 row only) ORACLE_STATUS,\r\n(SELECT to_char(completion_date, 'dd-Mon-yyyy HH24:MI:SS') FROM apps.mtl_serial_numbers \r\nwhere serial_number=CS.CHASISNO  order By inventory_item_id desc fetch FIRST 1 row only) ORACLE_FG_DATE,\r\n(SELECT current_subinventory_code FROM apps.mtl_serial_numbers \r\nwhere serial_number=CS.CHASISNO order By inventory_item_id desc fetch FIRST 1 row only) ORACLE_SUBINVENTORY_CODE,\r\n'' OKTS_DATE,'' OKTS_BY\r\nFROM XXES_CRANE_STATUS CS INNER JOIN\r\nXXES_CRANE_MASTER CM ON CS.PLANT_CODE = CM.PLANT_CODE\r\n AND CS.FAMILY_CODE = CM.FAMILY_CODE AND CS.ITEM_CODE = CM.ITEM_CODE\r\n INNER JOIN XXES_DAILY_PLAN_TRAN DT ON CS.PLANT_CODE = DT.PLANT_CODE\r\n AND CS.FAMILY_CODE = DT.FAMILY_CODE AND CS.FCODE_ID=TO_CHAR(DT.AUTOID)\r\n INNER JOIN XXES_DAILY_PLAN_MASTER XM ON DT.PLAN_ID = XM.PLAN_ID\r\n AND DT.PLANT_CODE = XM.PLANT_CODE AND DT.PLAN_ID=XM.PLAN_ID  \r\nWHERE cs.ENGINE_SRNO='{Pin}'", con))
                    {
                        DataTable jobStatusTable = new DataTable();
                        da.Fill(ENGINEDetails);


                    }
                }

                return ENGINEDetails;
            }
            catch (Exception ex)
            {
                throw new Exception("Error verifying tractor checklist", ex);
            }
        }


        public async Task<int> SaveHistoryCard_Make(SaveHistoryCard_Make objSaveHistoryCard_Make)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[] {

                     new SqlParameter("@EngineNo", objSaveHistoryCard_Make.EngineNo),
                     new SqlParameter("@Make", objSaveHistoryCard_Make.Make),
                     new SqlParameter("@Id", objSaveHistoryCard_Make.Id),
                     new SqlParameter("@UpdatedBy", objSaveHistoryCard_Make.UpdatedBy),
                };
              return  Convert.ToInt32(await _commonDAL.ExecuteScalarStoredProcedureAsync("SP_SaveHistoryCard_Make", parameters));


            }
            catch (Exception ex)
            {

                return 0;
            }
            return 0;
        }

        public async Task<int> SaveHistoryCard_Aging(SaveHistoryCard_Make objSaveHistoryCard_Make)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[] {

                     new SqlParameter("@EngineNo", objSaveHistoryCard_Make.EngineNo),
                     new SqlParameter("@Aging", objSaveHistoryCard_Make.Make),
                     new SqlParameter("@Id", objSaveHistoryCard_Make.Id),
                     new SqlParameter("@UpdatedBy", objSaveHistoryCard_Make.UpdatedBy),
                };
                return Convert.ToInt32(await _commonDAL.ExecuteScalarStoredProcedureAsync("SP_SaveHistoryCard_Aging", parameters));


            }
            catch (Exception ex)
            {

                return 0;
            }
            return 0;
        }


        public async Task<int> SaveHistoryCard_Serial(SaveHistoryCard_Make objSaveHistoryCard_Make)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[] {

                     new SqlParameter("@EngineNo", objSaveHistoryCard_Make.EngineNo),
                     new SqlParameter("@serial", objSaveHistoryCard_Make.Make),
                     new SqlParameter("@Id", objSaveHistoryCard_Make.Id),
                     new SqlParameter("@UpdatedBy", objSaveHistoryCard_Make.UpdatedBy),
                };
                return Convert.ToInt32(await _commonDAL.ExecuteScalarStoredProcedureAsync("SP_SaveHistoryCard_serial", parameters));


            }
            catch (Exception ex)
            {

                return 0;
            }
            return 0;
        }

        public async Task<DataTable> GETMAIN_LISTStage1( String Modelno, String Pin, String Engineno, String Name)
        {
            try
            {


                SqlParameter[] parameters = new SqlParameter[]
                {
                      new SqlParameter("@Model_no", Modelno),
                      new SqlParameter("@Chassis_no", Pin),
                      new SqlParameter("@EngineNO", Engineno),
                      new SqlParameter("@Name", Name)




                };
                DataTable dt = await _commonDAL.ExecuteStoredProcedureAsync("GET_CHECKLIST_Stage1", parameters);

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

        public async Task<DataTable> REMARKS_UPDATED(String Remarks, String RemarksID)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                     new SqlParameter("@Remarks", Remarks),
                      //new SqlParameter("@Modelno", Modelno),
                      //new SqlParameter("@Pin", Pin),
                      new SqlParameter("@RemarksID", RemarksID),
                      //new SqlParameter("@Name", Name)




                };
                DataTable dt = await _commonDAL.ExecuteStoredProcedureAsync("Stage1_CHECKLIST_REMARKS", parameters);
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

        public async Task<DataTable> FINALSAVE(String EngineNO)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                     new SqlParameter("@EngineNO", EngineNO)





                };
                DataTable dt = await _commonDAL.ExecuteStoredProcedureAsync("Stage1_CHECKLIST_FINAL_SAVE", parameters);
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

﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Cynthia.Data;

namespace SurveyFeature.Data
{
    /// <summary>
    /// Author:				Rob Henry
    /// Created:			2007-08-31
    /// Last Modified:		2008-11-13
    /// 
    /// 
    /// The use and distribution terms for this software are covered by the 
    /// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
    /// which can be found in the file CPL.TXT at the root of this distribution.
    /// By using this software in any fashion, you are agreeing to be bound by 
    /// the terms of this license.
    ///
    /// You must not remove this notice, or any other, from this software.
    ///  
    /// </summary>
    public static class DBSurvey
    {
        private static string GetConnectionString()
        {
            return ConfigurationManager.AppSettings["MSSQLConnectionString"];

        }

        public static String DBPlatform()
        {
            return "MSSQL";
        }

        /// <summary>
        /// Inserts a row in the cy_Surveys table. Returns rows affected count.
        /// </summary>
        /// <param name="surveyGuid"> surveyGuid </param>
        /// <param name="siteGuid"> siteGuid </param>
        /// <param name="surveyName"> surveyName </param>
        /// <param name="creationDate"> creationDate </param>
        /// <param name="startPageText"> startPageText </param>
        /// <param name="endPageText"> endPageText </param>
        /// <returns>int</returns>
        public static int Add(
            Guid surveyGuid,
            Guid siteGuid,
            string surveyName,
            DateTime creationDate,
            string startPageText,
            string endPageText)
        {
            SqlParameterHelper sph = new SqlParameterHelper(GetConnectionString(), "cy_Survey_Insert", 6);
            sph.DefineSqlParameter("@SurveyGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, surveyGuid);
            sph.DefineSqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, siteGuid);
            sph.DefineSqlParameter("@SurveyName", SqlDbType.NVarChar, 255, ParameterDirection.Input, surveyName);
            sph.DefineSqlParameter("@CreationDate", SqlDbType.DateTime, ParameterDirection.Input, creationDate);
            sph.DefineSqlParameter("@StartPageText", SqlDbType.NText, ParameterDirection.Input, startPageText);
            sph.DefineSqlParameter("@EndPageText", SqlDbType.NText, ParameterDirection.Input, endPageText);
            int rowsAffected = sph.ExecuteNonQuery();
            return rowsAffected;
        }


        /// <summary>
        /// Updates a row in the cy_Surveys table. Returns true if row updated.
        /// </summary>
        /// <param name="surveyGuid"> surveyGuid </param>
        /// <param name="siteGuid"> siteGuid </param>
        /// <param name="surveyName"> surveyName </param>
        /// <param name="creationDate"> creationDate </param>
        /// <param name="startPageText"> startPageText </param>
        /// <param name="endPageText"> endPageText </param>
        /// <returns>bool</returns>
        public static bool Update(
            Guid surveyGuid,
            Guid siteGuid,
            string surveyName,
            DateTime creationDate,
            string startPageText,
            string endPageText)
        {
            SqlParameterHelper sph = new SqlParameterHelper(GetConnectionString(), "cy_Survey_Update", 6);
            sph.DefineSqlParameter("@SurveyGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, surveyGuid);
            sph.DefineSqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, siteGuid);
            sph.DefineSqlParameter("@SurveyName", SqlDbType.NVarChar, 255, ParameterDirection.Input, surveyName);
            sph.DefineSqlParameter("@CreationDate", SqlDbType.DateTime, ParameterDirection.Input, creationDate);
            sph.DefineSqlParameter("@StartPageText", SqlDbType.NText,  ParameterDirection.Input, startPageText);
            sph.DefineSqlParameter("@EndPageText", SqlDbType.NText, ParameterDirection.Input, endPageText);
            int rowsAffected = sph.ExecuteNonQuery();
            return (rowsAffected > 0);
        }

        /// <summary>
        /// Deletes a row from the cy_Surveys table. Returns true if row deleted.
        /// </summary>
        /// <param name="surveyGuid"> surveyGuid </param>
        /// <returns>bool</returns>
        public static void Delete(Guid surveyGuid)
        {
            SqlParameterHelper sph = new SqlParameterHelper(GetConnectionString(), "cy_Survey_Delete", 1);
            sph.DefineSqlParameter("@SurveyGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, surveyGuid);
            sph.ExecuteNonQuery();
        }

        public static bool DeleteBySite(int siteId)
        {
            SqlParameterHelper sph = new SqlParameterHelper(GetConnectionString(), "cy_Survey_DeleteBySite", 1);
            sph.DefineSqlParameter("@SiteID", SqlDbType.Int, ParameterDirection.Input, siteId);
            int rowsAffected = sph.ExecuteNonQuery();
            return (rowsAffected > -1);

        }

        /// <summary>
        /// Gets an IDataReader with one row from the cy_Surveys table.
        /// </summary>
        /// <param name="surveyGuid"> surveyGuid </param>
        public static IDataReader GetOne(
            Guid surveyGuid)
        {
            SqlParameterHelper sph = new SqlParameterHelper(GetConnectionString(), "cy_Survey_SelectOne", 1);
            sph.DefineSqlParameter("@SurveyGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, surveyGuid);
            return sph.ExecuteReader();
        }

        /// <summary>
        /// Gets a count of rows in the cy_Surveys table.
        /// </summary>
        public static int GetCount()
        {

            return Convert.ToInt32(SqlHelper.ExecuteScalar(
                GetConnectionString(),
                CommandType.StoredProcedure,
                "cy_Survey_GetCount",
                null));

        }

        /// <summary>
        /// Gets a count of responses in to a survey.
        /// </summary>
        public static int GetResponseCount(Guid surveyGuid)
        {
            SqlParameterHelper sph = new SqlParameterHelper(GetConnectionString(), "cy_Survey_GetResponseCount", 1);
            sph.DefineSqlParameter("@SurveyGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, surveyGuid);
            return Convert.ToInt32(sph.ExecuteScalar());

        }

        /// <summary>
        /// Gets an IDataReader with all rows in the cy_Surveys table.
        /// </summary>
        public static IDataReader GetAll(Guid siteGuid)
        {
            SqlParameterHelper sph = new SqlParameterHelper(GetConnectionString(), "cy_Survey_SelectAll", 1);
            sph.DefineSqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, siteGuid);
            return sph.ExecuteReader();

            

        }

        public static int PagesCount(Guid surveyGuid)
        {
            SqlParameterHelper sph = new SqlParameterHelper(GetConnectionString(), "cy_Survey_CountPages", 1);
            sph.DefineSqlParameter("@SurveyGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, surveyGuid);
            return (int)sph.ExecuteScalar();
        }

        public static void AddToModule(Guid surveyGuid, int moduleId)
        {
            SqlParameterHelper sph = new SqlParameterHelper(GetConnectionString(), "cy_Survey_AddToModule", 2);
            sph.DefineSqlParameter("@SurveyGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, surveyGuid);
            sph.DefineSqlParameter("@ModuleId", SqlDbType.Int, ParameterDirection.Input, moduleId);
            sph.ExecuteNonQuery();
        }

        public static void RemoveFromModule(Guid surveyGuid, int moduleId)
        {
            SqlParameterHelper sph = new SqlParameterHelper(GetConnectionString(), "cy_Survey_RemoveFromModule", 2);
            sph.DefineSqlParameter("@SurveyGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, surveyGuid);
            sph.DefineSqlParameter("@ModuleId", SqlDbType.Int, ParameterDirection.Input, moduleId);
            sph.ExecuteNonQuery();
        }

        public static void RemoveFromModule(int moduleId)
        {
            SqlParameterHelper sph = new SqlParameterHelper(GetConnectionString(), "cy_Survey_DeleteByModule", 1);
            sph.DefineSqlParameter("@ModuleId", SqlDbType.Int, ParameterDirection.Input, moduleId);
            sph.ExecuteNonQuery();
        }

        public static Guid GetModulesCurrentSurvey(int moduleId)
        {
            SqlParameterHelper sph = new SqlParameterHelper(GetConnectionString(), "cy_Survey_GetModulesCurrentSurvey", 1);
            sph.DefineSqlParameter("@ModuleId", SqlDbType.Int, ParameterDirection.Input, moduleId);
            Object id = sph.ExecuteScalar();

            if (id == null) return Guid.Empty;
            return (Guid)id;
        }

        public static Guid GetFirstPageGuid(Guid surveyGuid)
        {
            SqlParameterHelper sph = new SqlParameterHelper(GetConnectionString(), "cy_Survey_GetFirstPageGuid", 1);
            sph.DefineSqlParameter("@SurveyGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, surveyGuid);
            Object id = sph.ExecuteScalar();

            if (id == null) return Guid.Empty;
            return (Guid)id;
        }

        public static Guid GetNextPageGuid(Guid pageGuid)
        {
            SqlParameterHelper sph = new SqlParameterHelper(GetConnectionString(), "cy_SurveyPages_GetNextPageGuid", 1);
            sph.DefineSqlParameter("@PageGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, pageGuid);

            Object id = sph.ExecuteScalar();

            if (id == null) return Guid.Empty;
            return (Guid)id;
        }

        public static Guid GetPreviousPageGuid(Guid pageGuid)
        {
            SqlParameterHelper sph = new SqlParameterHelper(GetConnectionString(), "cy_SurveyPages_GetPreviousPageGuid", 1);
            sph.DefineSqlParameter("@PageGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, pageGuid);

            Object id = sph.ExecuteScalar();

            if (id == null) return Guid.Empty;
            return (Guid)id;
        }

        /// <summary>
        /// Gets an IDataReader from the cy_SurveyQuestionAnswers table.
        /// </summary>
        /// <param name="answerGuid"> answerGuid </param>
        public static IDataReader GetResults(Guid surveyGuid)
        {
            SqlParameterHelper sph = new SqlParameterHelper(GetConnectionString(), "cy_SurveyQuestionAnswers_SelectBySurvey", 1);
            sph.DefineSqlParameter("@SurveyGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, surveyGuid);

            return sph.ExecuteReader();
        }

        /// <summary>
        /// Gets an IDataReader with all rows in the cy_SurveyQuestionAnswers table.
        /// </summary>
        public static IDataReader GetOneResult(Guid responseGuid)
        {
            SqlParameterHelper sph = new SqlParameterHelper(GetConnectionString(), "cy_SurveyResults_Select", 1);
            sph.DefineSqlParameter("@ResponseGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, responseGuid);
            return sph.ExecuteReader();
        }

    }
}

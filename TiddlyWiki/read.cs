using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text;
using Exova.Core.Text;
using System.Globalization;

namespace Exova.GroupITAccounts.Web.Export
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class Export : IHttpHandler
    {
        private int month;
        private int year;

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public void ProcessRequest(HttpContext context)
        {
            this.GetExportParametersFromQueryString(context);

            this.ProcessInvoicesOrCreditNotes(context, invoiceSql);
            this.ProcessInvoicesOrCreditNotes(context, creditNoteSql);

            context.Response.BufferOutput = false;
            context.Response.ContentType = "text/csv";
            context.Response.AddHeader("content-disposition", "attachment; filename=\"" + this.year + "_" + this.month + "_" + "ColAccountsExport.csv");
        }

        /// <summary>
        /// Gets the export parameters from the query string.
        /// </summary>
        /// <param name="context">The context.</param>
        private void GetExportParametersFromQueryString(HttpContext context)
        {
            NameValueCollection queryStringCollection = context.Request.QueryString;

            try
            {
                this.year = int.Parse(queryStringCollection["year"]);
                this.month = int.Parse(queryStringCollection["month"]);
            }
            catch
            {
                context.Response.Redirect("~/Default.aspx", true);
            }
        }

        /// <summary>
        /// Processes the invoices or credit notes.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="sql">The SQL query to use</param>
        public void ProcessInvoicesOrCreditNotes(HttpContext context, string sql)
        {
            using (SqlConnection conn = Export.GetConnection())
            using (SqlCommand command = new SqlCommand(sql, conn))
            {
                SqlParameter paramYear = new SqlParameter("@Year", SqlDbType.Int);
                paramYear.Value = this.year;
                command.Parameters.Add(paramYear);

                SqlParameter paramMonth = new SqlParameter("@Month", SqlDbType.Int);
                paramMonth.Value = this.month;
                command.Parameters.Add(paramMonth);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!OutputDataRecordAsCsv(context.Response, reader))
                        {
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <returns></returns>
        private static SqlConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ColAccountsConnectionString"].ConnectionString);
            connection.Open();

            return connection;
        }

        /// <summary>
        /// Outputs the data record as CSV.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="record">The record.</param>
        /// <returns></returns>
        private bool OutputDataRecordAsCsv(HttpResponse response, IDataRecord record)
        {
            this.RecordToRow(record, response);

            return response.IsClientConnected;
        }

        /// <summary>
        /// Records to row.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="response">The response.</param>
        private void RecordToRow(IDataRecord record, HttpResponse response)
        {
            StringBuilder temp = new StringBuilder();

            int numFields = record.FieldCount;

            for (int i = 0; i < numFields; i++)
            {
                if (temp.Length > 0)
                {
                    temp.Append(',');
                }

                string value = record.GetValue(i).ToString();
                Type type = record.GetValue(i).GetType();

                value = value.RemoveLineBreakCharacters(" ");

                if (type == typeof(DateTime))
                {
                    value = this.ConvertDateTimeToOutputFileFormat(record, i);
                    temp.Append(value);
                }
                else if (type == typeof(decimal))
                {
                    value = this.ConvertDecimalToOutputFileFormat(record, i);
                    temp.Append(value);
                }
                else if (value.NeedsCsvQuoting())
                {
                    temp.Append('"').Append(value.Replace("\"", "\"\"")).Append('"');
                }
                else
                {
                    value = this.ConvertStringToOutputFormat(record, i);
                    temp.Append(value);
                }
            }
            temp.Append(Environment.NewLine);
            response.Write(temp.ToString());
        }

        /// <summary>
        /// Converts the string to output format.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="fieldIndex">Index of the field.</param>
        /// <returns></returns>
        private string ConvertStringToOutputFormat(IDataRecord record, int fieldIndex)
        {
            string columnName = record.GetName(fieldIndex).ToString();
            string columnValue = record.GetValue(fieldIndex).ToString();

            if (columnName == "CustomerRef" && String.IsNullOrEmpty(columnValue))
            {
                columnValue = "N/A       ";
            }
            else if (columnName == "CustomerRef" && !String.IsNullOrEmpty(columnValue))
            {
                // pad out CustomerRef with spaces - must be 10 characters in length
                int numberOfSpacesToAdd = 10 - columnValue.Length;
                StringBuilder strBuilder = new StringBuilder(10);
                strBuilder.Append(columnValue);

                for (int i = 0; i < numberOfSpacesToAdd; i++)
                {
                    strBuilder.Append(" "); 
                }

                columnValue = strBuilder.ToString();
            }

            return columnValue;
        }

        /// <summary>
        /// Converts the decimal to output file format.
        /// Export file has some special cases for decimal output...
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="fieldIndex">Index of the field.</param>
        /// <returns></returns>
        private string ConvertDecimalToOutputFileFormat(IDataRecord record, int fieldIndex)
        {
            string result = string.Empty;
            decimal decimalValue = decimal.Parse(record.GetValue(fieldIndex).ToString());
            string columnName = record.GetName(fieldIndex);

            if (columnName == "InvoiceNet2")
            {
                result = decimalValue.ToString("0000000000.00");
            }
            else
            {
                result = decimalValue.ToString("000000000000.00");
            }

            return result;
        }

        /// <summary>
        /// Converts the date time to output file format.
        /// Export file has some special cases for DateTime output...
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="fieldNumber">The field number.</param>
        /// <returns></returns>
        private string ConvertDateTimeToOutputFileFormat(IDataRecord record, int fieldIndex)
        {
            string result = string.Empty;

            DateTime dateTime = DateTime.Parse(record.GetValue(fieldIndex).ToString());
            string columnName = record.GetName(fieldIndex);

            if (columnName == "TodaysDate")
            {
                result = dateTime.ToString("dd MMM yy", CultureInfo.CurrentCulture);
            }
            else
            {
                result = dateTime.ToString("ddMMyy", CultureInfo.CurrentCulture);
            }

            return result;
        }

        private const string invoiceSql = @"SELECT [Customer].[Code] +'   ' As CustomerCode
                                            ,[Invoice].[InvoiceDate] As InvoiceDate
                                            ,[Invoice].[InvoiceDate] + [Customer].[Terms] As PaymentDate
                                            ,'04' As TransactionType
                                            ,'01' As VatCode
                                            ,[Invoice].[Net] As InvoiceNet
                                            ,([Invoice].[Net] / 100) * [Invoice].[VatRate]  As InvoiceVAT
                                            ,'000000000000.00' As DiscountAmount
                                            ,'00' As UnknownValue1
                                            ,[Invoice].[Number] As InvoiceNumber
                                            ,[Invoice].CustomerPOId + ' ' As CustomerRef
                                            ,'0002' As UnknownValue2
                                            ,'0000000' As UnknownValue3
                                            ,'01' As UnknownValue4
                                            ,'0000000001.0000' As UnknownValue5
                                            ,GETDATE()AS 'TodaysDate'
                                            ,'001'  As UnknownValue6
                                            ,'45  ' As LabNo
                                            ,'    ' As WorkAreaType
                                            ,[Invoice].[NominalCode] + '    ' As NormalAnalysisCode
                                            ,[Invoice].[Net] As InvoiceNet2
                                            FROM [ColAccounts].[dbo].[Invoice]
	                                            INNER JOIN Customer ON [Customer].[CustomerKey] = [Invoice].[CustomerKey]
                                            WHERE YEAR(Invoice.CreatedDate) = @Year
	                                            AND MONTH(Invoice.CreatedDate) = @Month";

        private const string creditNoteSql = @"SELECT [Customer].[Code] +'   ' As CustomerCode
                                                ,[CreditNote].[CreditDate] As CreditDate
                                                ,[Invoice].[InvoiceDate] As PaymentDate
                                                ,'05' As TransactionType
                                                ,'01' As VatCode
                                                ,[CreditNote].[Net] As InvoiceNet
                                                ,([CreditNote].[Net] / 100) * [CreditNote].[VatRate] As InvoiceVAT
                                                ,'000000000000.00' As DiscountAmount
                                                ,'00' As UnknownValue1
                                                ,[Invoice].[Number] As InvoiceNumber
                                                ,[Invoice].CustomerPOId + ' ' As CustomerRef
                                                ,'0002' As UnknownValue2
                                                ,'0000000' As UnknownValue3
                                                ,'01' As UnknownValue4
                                                ,'0000000001.0000' As UnknownValue5
                                                ,GETDATE()AS 'TodaysDate'
                                                ,'001'  As UnknownValue6
                                                ,'45  ' As LabNo
                                                ,'    ' As WorkAreaType
                                                ,[Invoice].[NominalCode] + '    ' As NormalAnalysisCode
                                                ,[CreditNote].[Net] As InvoiceNet2
                                                FROM [ColAccounts].[dbo].[Invoice]
                                                    INNER JOIN Customer ON [Customer].[CustomerKey] = [Invoice].[CustomerKey]
	                                                    INNER JOIN CreditNote ON [CreditNote].[InvoiceNumber] = [Invoice].[Number]
                                                WHERE YEAR(Invoice.CreatedDate) = @Year
                                                    AND MONTH(Invoice.CreatedDate) = @Month";
    }
}

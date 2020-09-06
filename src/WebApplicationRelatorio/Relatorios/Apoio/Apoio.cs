using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApplicationRelatorio.Apoio
{
    public class Apoio
    {

        protected static Queue reportQueue = new Queue();

  
        protected static ReportDocument CreateReport(Type reportClass)
        {
            object report = Activator.CreateInstance(reportClass);
            reportQueue.Enqueue(report);
            return (ReportDocument)report;
        }

        public static ReportDocument GetReport(Type reportClass)
        {
            //75 is my print job limit.
            if (reportQueue.Count > 10)
            {
                ((ReportDocument)reportQueue.Dequeue()).Close();
                ((ReportDocument)reportQueue.Dequeue()).Dispose();
                GC.Collect();
            }
            return CreateReport(reportClass);
        }
        public static int GetPrintJob()
        {
            if (reportQueue != null)
                return reportQueue.Count;
            else
                return 0;
        }

        /// <summary>
        /// Caso o relatório tenha a opção de exportar para Word, informar o quarto parâmetro na função exportarPDF_WORD.
        /// </summary>
        /// <param name="report"></param>
        /// <param name="response"></param>
        /// <param name="nome"></param>
        /// <param name="tipo"></param>
        public static void exportarPDF_WORD(ReportDocument report, HttpResponse response, string nome, string tipo = null)
        {
            response.ClearContent();
            response.ClearHeaders();

            Stream stream;
            if (tipo == "W")
            {
                response.ContentType = "application/msword";
                response.AddHeader("Content-Disposition", "inline; filename=" + nome + ".doc");
                stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.WordForWindows);
            }
            else
            {
                response.ContentType = "application/pdf";
                response.AddHeader("Content-Disposition", "inline; filename=" + nome + ".pdf");
                stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }

            byte[] buffer = new byte[stream.Length];
            BinaryReader reader = new BinaryReader(stream);
            for (long i = 0; i < stream.Length; i++)
                buffer[i] = reader.ReadByte();

            report.Close();
            report.Dispose();

            response.BinaryWrite(buffer);
            response.Flush();
            response.End();
        }

     
 
    }
}
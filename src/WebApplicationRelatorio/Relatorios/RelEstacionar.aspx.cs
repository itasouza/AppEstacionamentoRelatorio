using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using WebApplicationRelatorio.Apoio;
using WebApplicationRelatorio.Repositorio;

namespace WebApplicationRelatorio.Relatorios
{
    public partial class RelEstacionar : System.Web.UI.Page
    {
        private Contexto contexto;
        private DataSet ds;
        private StringBuilder lsSQL;
        private ReportDocument Report;
        private string logomarca = "http://localhost:44349/Relatorios/Imagens/Logo.jpg";

        protected void Page_Load(object sender, EventArgs e)
        {
            ///"DefaultConnection": "Server=DESKTOP-N8415MR\\SQLEXPRESS;Database=AppEstacionamento;User Id=sa;Password=root;"
            ///"DefaultConnection": "Server=VPSCLOUD-340\\SQLEXPRESS;Database=AppEstacionamento;User Id=sa;Password=root;"

            #region Parametros
            /*if (Page.Request.Params["prm"] != null)
            {

            }*/
            #endregion

            #region SQL

            lsSQL = new StringBuilder();
            lsSQL.AppendLine(" SELECT a.NomeCompleto as UsuarioCriacao, e.ValorHora, e.NumeroVaga, e.PlacaVeiculo,  e.DataEntrada, ");
            lsSQL.AppendLine(" e.DataSaida, e.ValorDevido as ValorPago, e.TempoDecorrido, b.NomeCompleto as UsuarioFechamento ");
            lsSQL.AppendLine(" from Estacionar e ");
            lsSQL.AppendLine(" inner join usuario a on e.UsuarioCriacaoId = a.Id ");
            lsSQL.AppendLine(" inner join usuario b on e.UsuarioAlteracaoId = b.Id ");
            lsSQL.AppendLine(" where e.Ativo = 1 ");

            #endregion

            try
            {

                using (contexto = new Contexto())
                {
                    ds = new DataSet();
                    ds = contexto.Consulta(lsSQL.ToString(), "Estacionar");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            #region CrystalReports
            Report = new ReportDocument();

            #region Load Report
            if (Session["RptEstacionar"] == null)
            {
                Application["nRequicao"] = (int)Application["nRequicao"] + 1;

                Report = Apoio.Apoio.GetReport(Report.GetType());
                Report.Load(Server.MapPath("rpt/RptEstacionar.rpt"));
                Session["RptEstacionar"] = Report;
            }
            else
            {

                Report = (ReportDocument)Session["RptEstacionar"];

                string fileName = "";
                try
                {
                    fileName = Report.FileName;
                }
                catch (Exception) { }

                if (fileName == "")
                {
                    Application["nRequicao"] = (int)Application["nRequicao"] + 1;
                    Report = Apoio.Apoio.GetReport(Report.GetType());
                    Report.Load(Server.MapPath("rpt/RptEstacionar.rpt"));
                    Session["RptEstacionar"] = Report;
                }
                else
                {

                    Application["nReaprov"] = (int)Application["nReaprov"] + 1;
                }
            }
            #endregion

            Report.SetDataSource(ds.Tables["Estacionar"]);
            Report.SetParameterValue("prmLogomarca", logomarca);
            #endregion

            #region CrystalReports PDF e Word
            //Leia o summary da função abaixo:
            Apoio.Apoio.exportarPDF_WORD(Report, Response, "RelEstacionar");
            #endregion
        }
    }

       
}
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Configuration;
using System.Web.Configuration;
using System.Web;

namespace GlobalTranslator
{
    public partial class Inicio : System.Web.UI.Page
    {
        private SqlConnection myConnection = new SqlConnection();
        private string ConnectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
        //private Boolean dbInicializada;
        //private string ConnectionString = string.Format("Data Source={0}; Initial Catalog={1}; Application Name={2}; Integrated Security=SSPI", @"ARIPAU03\SQL2017", "Idiomas", "GlobalTranslator");
        //private string ConnectionString = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) { Response.Cookies["dbInicializada"].Value = "false"; }
        }

        protected void btnSetDataBase_Click(object sender, EventArgs e)
        {
            string path = Request.ApplicationPath;

            var configuration = WebConfigurationManager.OpenWebConfiguration(path);
            var section =
              (ConnectionStringsSection)configuration.GetSection("connectionStrings");
            section.ConnectionStrings["MyConnectionString"].ConnectionString =
                string.Format("Data Source={0}; Initial Catalog={1}; Application Name={2}; Integrated Security=SSPI",
                txtServerName.Text, txtDataBaseName.Text, "GlobalTranslator"); ;
            configuration.Save();

            //Test de conexión:
            try
            {
                myConnection.ConnectionString = ConnectionString;
                myConnection.Open();
                Response.Cookies["dbInicializada"].Value = "true";
                myConnection.Close();
                lblSuccess.Text = "Datos guardados correctamente";
                lblSuccess.Visible = true;
                txtServerName.Enabled = false;
                txtDataBaseName.Enabled = false;
                btnSetDataBase.Enabled = false;
            }
            catch (SqlException)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('" + "Datos de conexión incorrectos." + "');", true);
            }
        }

        private void Abrir()
        {
            myConnection.ConnectionString = ConnectionString;
            myConnection.Open();
        }

        private void Cerrar()
        {
            if (myConnection != null && myConnection.State == ConnectionState.Open)
                myConnection.Close();
        }

        protected void btnTranslate_Click(object sender, EventArgs e)
        {
            Boolean dbInicializada = false;

            HttpCookie reqCookies = Request.Cookies["dbInicializada"];
            if (reqCookies != null) { dbInicializada = Boolean.Parse(reqCookies.Value); }

            if (dbInicializada)
            {
                Boolean datosEntradaOk = true;
                int lngToDel = 0;
                DataTable tbl_LngFrom = new DataTable();
                tbl_LngFrom = Leer(String.Format("SELECT IdIdioma FROM Idioma WHERE CodIdioma = '{0}'", txtLngFrom.Text.Trim()));
                if ((tbl_LngFrom != null) && (tbl_LngFrom.Rows.Count == 0)) { datosEntradaOk = false; }

                DataTable tbl_LngTo = new DataTable();
                tbl_LngTo = Leer(String.Format("SELECT IdIdioma FROM Idioma WHERE CodIdioma = '{0}'", txtLngTo.Text.Trim()));
                if ((tbl_LngTo != null) && (tbl_LngTo.Rows.Count == 0)) { datosEntradaOk = false; }
                else
                {
                    foreach (DataRow fila in tbl_LngTo.Rows)
                    {
                        lngToDel = int.Parse(fila["IdIdioma"].ToString());
                    }
                }
                //Para que no elimine lo mismo que quiere insertar
                if (txtLngFrom.Text == txtLngTo.Text) { datosEntradaOk = false; }

                if (datosEntradaOk)
                {
                    string delLng = String.Format("DELETE FROM Texto WHERE IdIdioma = {0}", lngToDel.ToString());
                    Escribir(delLng);

                    DataTable tbl_Words = new DataTable();
                    tbl_Words = Leer(string.Format("SELECT i.idIdioma, i.CodIdioma, i.DescripcionIdioma, t.IdFrase, t.Texto " +
                                                   "FROM Texto t JOIN Idioma i ON t.IdIdioma = i.IdIdioma " +
                                                   "WHERE i.CodIdioma = '{0}'", txtLngFrom.Text));

                    if ((tbl_Words != null) && (tbl_Words.Rows.Count > 0))
                    {
                        HttpClient client = new HttpClient();
                        int countEntries = 0;

                        foreach (DataRow fila in tbl_Words.Rows)
                        {
                            try
                            {
                                string requestStr = String.Format("?q={0}&langpair={1}|{2}", fila["Texto"].ToString(), fila["CodIdioma"].ToString(), txtLngTo.Text);

                                TranslationResponse.Rootobject tResponse = new TranslationResponse.Rootobject();
                                string jsonResp = client.GetStringAsync("https://api.mymemory.translated.net/get" + requestStr).Result;
                                tResponse = JsonConvert.DeserializeObject<TranslationResponse.Rootobject>(jsonResp);

                                if (tResponse != null)
                                {
                                    //if matches.count??
                                    string newText = tResponse.responseData.translatedText;
                                    string query = String.Format("INSERT INTO Texto (IdIdioma, IdFrase, Texto) VALUES ({0},{1},'{2}')", lngToDel.ToString(), fila["IdFrase"].ToString(), newText.Replace("&#39;", "''"));
                                    Escribir(query);
                                    countEntries++;
                                }
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                        }
                        lblResponse.Text = string.Format("Se tradujeron: {0} Textos.", countEntries.ToString());
                        lblResponse.Visible = true;
                    }
                }
                else { ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('" + "Alguno de los idiomas seleccionados no existe o está incorrecto." + "');", true); }
            }
            else { ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('" + "No ha seteado los datos de la conexión." + "');", true); }
        }

        private DataTable Leer(string Query)
        {
            DataTable TablaRet = new DataTable();
            Abrir();
            using (SqlDataAdapter myAdaptador = new SqlDataAdapter())
            {
                myAdaptador.SelectCommand = new SqlCommand();
                myAdaptador.SelectCommand.CommandText = Query;
                myAdaptador.SelectCommand.Connection = myConnection;

                try
                {
                    myAdaptador.Fill(TablaRet);
                }
                catch (Exception ex)
                {
                    string s = ex.Message;
                    TablaRet = null;
                }
            }
            Cerrar();
            return TablaRet;
        }

        public int Escribir(string Query)
        {
            int ret = 0;
            Abrir();
            using (SqlCommand myCommand = new SqlCommand())
            {
                myCommand.Connection = myConnection;
                myCommand.CommandText = Query;

                try
                {
                    ret = myCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    string s = ex.Message;
                    ret = -1;
                }
            }
            Cerrar();
            return ret;
        }
    }
}
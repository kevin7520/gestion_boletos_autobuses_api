
using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace gestion_boletos_autobuses_api.Helpers
{
    public class MetodosXLM
    {
        public static XDocument getXML<T>(T criterio)
        {
            XDocument resultado = new XDocument(new XDeclaration("1.0", "utf-8", "true"));
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using XmlWriter xmlWriter = resultado.CreateWriter();
                serializer.Serialize(xmlWriter, criterio);
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: ", ex);
                return null;
            }
        }
        public static async Task<DataSet> ejecutaBase(string cadenaConexion, string nombreProcedimiento, string transaccion = null, string dataXml = null)
        {
            DataSet dsResultado = new DataSet();
            SqlConnection cnn = new SqlConnection(cadenaConexion);
            try
            {
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter adt = new SqlDataAdapter();
                cmd.CommandText = nombreProcedimiento;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = cnn;
                cmd.CommandTimeout = 120;
                cmd.Parameters.Add("@idTransaccion", SqlDbType.VarChar).Value = transaccion;
                if (dataXml != null)
                {
                    cmd.Parameters.Add("@iXML", SqlDbType.Xml).Value = dataXml.ToString();
                }
                await cnn.OpenAsync().ConfigureAwait(false);
                adt = new SqlDataAdapter(cmd);
                adt.Fill(dsResultado);
                cmd.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error SQL: " + e.Message);
                cnn.Close();
            }
            finally
            {
                if (cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }
            }
            Console.WriteLine("Cadena:" + cadenaConexion + "\n- SP:" + nombreProcedimiento + " - TR:" + transaccion + " - XML:" + dataXml);
            return dsResultado;
        }
    }
}


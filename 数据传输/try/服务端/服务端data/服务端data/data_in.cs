using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;


namespace CSharpConnectMySQL
{
    public class MySqlHelper
    {
        private static string connstr = "server=127.0.0.1;database=trydata;username=root;password=123456;";


        //更新
        public static string xyz_enter(string location_data,string x_data, string y_data,string z_data)
        {
            if (sql_LocationDataQuery(location_data))
            {
                if (sql_xyzDataUpdate(location_data, x_data, y_data, z_data)) return "录入成功";
                else return "录入错误";
            }
            else return "不存在";
        }
      
        //插入

        public static string sql_xyzDatainsert(string location_data, string x_data, string y_data, string z_data)

        {
            if (sql_LocationDataQuery(location_data))
            {
                return "目标位置已存在";
            }
            else
            {
                MySqlConnection conn = new MySqlConnection(connstr);
                conn.Open();

                String sql = "INSERT INTO data (location, x_data, y_data,z_data) VALUES ('" + location_data + "','" + x_data + "' , '" + y_data + "','" + z_data + "');";//插入

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0) //如果能查到，说明数据插入成功
                {

                    conn.Close();
                    return "插入成功";

                }
                else
                {
                    conn.Close();
                    return "插入失败";

                }
            }
        }

        //location查询
        private static bool sql_LocationDataQuery(string location_data)
        {
            MySqlConnection conn = new MySqlConnection(connstr);

            conn.Open();
            String sql = "select location from data where location=" + location_data + ";";//SQL语句实现表数据的读取

            MySqlCommand cmd = new MySqlCommand(sql, conn);

            MySqlDataReader sqlDataReader = cmd.ExecuteReader();
            if (sqlDataReader.HasRows) //如果能查到，说明该用户存在
            {
                sqlDataReader.Close();
                conn.Close();
                return true;

            }
            else
            {
                conn.Close();
                return false;

            }
        }

        //更新xyz数据
        private static bool sql_xyzDataUpdate(string location_data, string x_data, string y_data, string z_data)
        {
            MySqlConnection conn = new MySqlConnection(connstr);

            conn.Open();


            String sql = "UPDATE data SET x_data = '"+x_data+ "', y_data = '"+y_data+ "', z_data = '"+z_data+"'  WHERE location = '"+location_data+"';";//SQL语句实现表数据的读取

            MySqlCommand cmd = new MySqlCommand(sql, conn);

            int rowsAffected = cmd.ExecuteNonQuery();
            if (rowsAffected>0) //如果能查到，说明数据存在
            { 
               
                conn.Close();
                return true;

            }
            else
            {
                conn.Close();
                return false;

            }
        }

        //查询
        public static string sql_XYZDataQuery(string Query_passworddata)
        {
            
            MySqlConnection conn = new MySqlConnection(connstr);

            conn.Open();


            String xyz_data = "select x_data,y_data,z_data from data where location='" + Query_passworddata + "';";//SQL语句实现表数据的读取
            

            MySqlCommand cmd = new MySqlCommand(xyz_data, conn);

            MySqlDataReader sqlDataReader = cmd.ExecuteReader();
            
            if (sqlDataReader.HasRows) //如果能查到，说明该密码正确存在
            {
                sqlDataReader.Read();
                string X_data = (string)sqlDataReader["x_data"];
                string Y_data = (string)sqlDataReader["y_data"];
                string Z_data = (string)sqlDataReader["z_data"];
                sqlDataReader.Close();
                conn.Close();
                xyz_data = X_data + "," + Y_data + "," + Z_data; 
                return xyz_data;

                
            }
            else
            {
                conn.Close();
                return "error";

            }
        }







        #region 执行查询语句，返回MySqlDataReader

        /// <summary>
        /// 执行查询语句，返回MySqlDataReader
        /// </summary>
        /// <param name="sqlString"></param>
        /// <returns></returns>
        public static MySqlDataReader ExecuteReader(string sqlString)
        {
            MySqlConnection connection = new MySqlConnection(connstr);
            MySqlCommand cmd = new MySqlCommand(sqlString, connection);
            MySqlDataReader myReader = null;
            try
            {
                connection.Open();
                myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return myReader;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                connection.Close();
                throw new Exception(e.Message);
            }
            finally
            {
                if (myReader == null)
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
        #endregion

        #region 执行带参数的查询语句，返回 MySqlDataReader

        /// <summary>
        /// 执行带参数的查询语句，返回MySqlDataReader
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static MySqlDataReader ExecuteReader(string sqlString, params MySqlParameter[] cmdParms)
        {
            MySqlConnection connection = new MySqlConnection(connstr);
            MySqlCommand cmd = new MySqlCommand();
            MySqlDataReader myReader = null;
            try
            {
                PrepareCommand(cmd, connection, null, sqlString, cmdParms);
                myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return myReader;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                connection.Close();
                throw new Exception(e.Message);
            }
            finally
            {
                if (myReader == null)
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
        #endregion

        #region 执行sql语句,返回执行行数

        /// <summary>
        /// 执行sql语句,返回执行行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int ExecuteSql(string sql)
        {
            using (MySqlConnection conn = new MySqlConnection(connstr))
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    try
                    {
                        conn.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        conn.Close();
                        //throw e;
                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        cmd.Dispose();
                        conn.Close();
                    }
                }
            }

            return -1;
        }
        #endregion

        #region 执行带参数的sql语句，并返回执行行数

        /// <summary>
        /// 执行带参数的sql语句，并返回执行行数
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static int ExecuteSql(string sqlString, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connstr))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, sqlString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException E)
                    {
                        throw new Exception(E.Message);
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }
        #endregion

        #region 执行查询语句，返回DataSet

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(string sql)
        {
            using (MySqlConnection conn = new MySqlConnection(connstr))
            {
                DataSet ds = new DataSet();
                try
                {
                    conn.Open();
                    MySqlDataAdapter DataAdapter = new MySqlDataAdapter(sql, conn);
                    DataAdapter.Fill(ds);
                }
                catch (Exception ex)
                {
                    //throw ex;
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
                return ds;
            }
        }
        #endregion

        #region 执行带参数的查询语句，返回DataSet

        /// <summary>
        /// 执行带参数的查询语句，返回DataSet
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(string sqlString, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connstr))
            {
                MySqlCommand cmd = new MySqlCommand();
                PrepareCommand(cmd, connection, null, sqlString, cmdParms);
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                    return ds;
                }
            }
        }
        #endregion

        #region 执行带参数的sql语句，并返回 object

        /// <summary>
        /// 执行带参数的sql语句，并返回object
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static object GetSingle(string sqlString, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connstr))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, sqlString, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// 执行存储过程,返回数据集
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>DataSet</returns>
        public static DataSet RunProcedureForDataSet(string storedProcName, IDataParameter[] parameters)
        {
            using (MySqlConnection connection = new MySqlConnection(connstr))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                MySqlDataAdapter sqlDA = new MySqlDataAdapter();
                sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDA.Fill(dataSet);
                connection.Close();
                return dataSet;
            }
        }

        /// <summary>
        /// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand</returns>
        private static MySqlCommand BuildQueryCommand(MySqlConnection connection, string storedProcName,
            IDataParameter[] parameters)
        {
            MySqlCommand command = new MySqlCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (MySqlParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
            return command;
        }

        #region 装载MySqlCommand对象

        /// <summary>
        /// 装载MySqlCommand对象
        /// </summary>
        private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, string cmdText,
            MySqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
            {
                cmd.Transaction = trans;
            }
            cmd.CommandType = CommandType.Text; //cmdType;
            if (cmdParms != null)
            {
                foreach (MySqlParameter parm in cmdParms)
                {
                    cmd.Parameters.Add(parm);
                }
            }
        }
        #endregion

    }
}
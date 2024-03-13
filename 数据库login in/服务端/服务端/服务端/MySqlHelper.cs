using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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


        //登陆
        public static string Login_in_Click(string username, string password)
        {
            bool namebool = sql_nameDataQuery(username);
            bool passwordbool = sql_passwordDataQuery(password);
            if (namebool)
            {
                if(passwordbool) return "登陆成功";
                else return "密码错误";
            }
            else return "账号未注册";
        }
       /* public static string Login_in_Click(string username, string password)
        {
            MySqlConnection conn = new MySqlConnection(connstr);
            conn.Open();
            String sql = "select username,password from login_try where username=" + username + " and password='" + password + "';";//SQL语句实现表数据的读取
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader sqlDataReader = cmd.ExecuteReader();
            if (sqlDataReader.HasRows) //如果能查到，说明该用户密码存在
            {//MessageBox.Show("登陆成功");
                sqlDataReader.Close();
                conn.Close();
                return "登陆成功";
            }
            else
            {
               // MessageBox.Show("账号或密码错误或未注册");
                conn.Close();
                return "账号或密码错误或未注册";

            }

        }//注册*/

        public static  string Login_up_Click(string username, string password)

        {
            //判断账号重复插入
            bool namebool = sql_nameDataQuery(username);
            if (namebool)  return "账号已注册";
            else 
            {
                MySqlConnection conn = new MySqlConnection(connstr);

                conn.Open();

                String sql = "INSERT INTO login_try(username,password) VALUES(" + username + ",'" + password + "');"; //没有判断重复插入

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.ExecuteNonQuery();

                //MessageBox.Show("注册成功");

                conn.Close();
                return "注册成功";
            }
        }


        private static bool sql_nameDataQuery( string Query_namedata)
        {
            MySqlConnection conn = new MySqlConnection(connstr);

            conn.Open();


            String sql = "select username,password from login_try where username=" + Query_namedata + ";";//SQL语句实现表数据的读取

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


        private static bool sql_passwordDataQuery(string Query_passworddata)
        {
            MySqlConnection conn = new MySqlConnection(connstr);

            conn.Open();


            String sql = "select username,password from login_try where password='" + Query_passworddata + "';";//SQL语句实现表数据的读取

            MySqlCommand cmd = new MySqlCommand(sql, conn);

            MySqlDataReader sqlDataReader = cmd.ExecuteReader();
            if (sqlDataReader.HasRows) //如果能查到，说明该密码正确存在
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
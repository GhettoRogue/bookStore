using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace clientChat
{
    public partial class AdminUsersForm : Form
    {
        SqlConnection conn;
        string cs = null;
        DataSet set = null;
        SqlCommandBuilder cmdb = null;
        SqlDataAdapter da = null;
        string sql = null;
        LoginForm loginForm = null;

        public AdminUsersForm(LoginForm lg)
        {
            InitializeComponent();
            conn = new SqlConnection();
            cs = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            conn.ConnectionString = cs;

            loginForm = lg;

            sql = "select * from [dbo].[Managers]";
            FillDataGridView();
        }
        private void FillDataGridView()
        {
            try
            {
                conn = new SqlConnection(cs);
                set = new DataSet();
                da = new SqlDataAdapter(sql, conn);
                DGVUsers.DataSource = null;
                cmdb = new SqlCommandBuilder(da);
                da.Fill(set);
                DGVUsers.DataSource = set.Tables[0];
                sql = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void AdminUsersForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            loginForm.Close();
        }

        private void buttonSaveUsers_Click(object sender, EventArgs e)
        {
            try
            {
                da.Update(set);
                MessageBox.Show("Обновление успешно");

                sql = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }
    }
}

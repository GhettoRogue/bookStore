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
    public partial class AdminForm : Form
    {
        SqlConnection conn;
        string cs = null;
        DataSet set = null;
        SqlCommandBuilder cmdb = null;
        SqlDataAdapter da = null;
        string sql = null;
        LoginForm loginForm = null;

        public AdminForm(LoginForm lg)
        {
            InitializeComponent();
            conn = new SqlConnection();
            cs = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            conn.ConnectionString = cs;

            loginForm = lg;

            sql = "select * from [dbo].[Books]";
            FillDataGridView();
        }
        private void FillDataGridView()
        {
            try
            {
                conn = new SqlConnection(cs);
                set = new DataSet();
                da = new SqlDataAdapter(sql, conn);
                DGVBooks.DataSource = null;
                cmdb = new SqlCommandBuilder(da);
                da.Fill(set);
                DGVBooks.DataSource = set.Tables[0];
                sql = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void buttonSaveBooks_Click(object sender, EventArgs e)
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

        private void AdminForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            loginForm.Close();
        }
    }
}

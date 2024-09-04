using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace clientChat
{
    public partial class bookStoreForm : Form
    {
        SqlConnection BSConn;
        string cs = null;
        DataSet set = null;
        SqlCommandBuilder cmdb = null;
        SqlDataAdapter da = null;
        string sql = null;
        LoginForm loginForm = null;

        //скругление формы
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
           int nLeftRect,
           int nTopRect,
           int nRightRect,
           int nBottomRect,
           int nWidthEllipse,
           int nHeightEllipse
        );

        public bookStoreForm(LoginForm lg)
        {
            InitializeComponent();
            
            //скругление формы
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            BSConn = new SqlConnection();
            cs = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            BSConn.ConnectionString = cs;
            loginForm = lg;

            sql = "select * from [dbo].[Books]";
            FillDataGridView();
        }
        private void books_Click(object sender, EventArgs e)
        {
            sql = "select * from [dbo].[Books]";
            FillDataGridView();
        }
        private void FillDataGridView()
        {
            try
            {
                BSConn = new SqlConnection(cs);
                set = new DataSet();
                da = new SqlDataAdapter(sql, BSConn);
                dataGridView1.DataSource = null;
                cmdb = new SqlCommandBuilder(da);
                da.Fill(set);
                dataGridView1.DataSource = set.Tables[0];
                sql = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }
        private void listNewBooks()
        {
            sql = "select * from[dbo].[Books] where[Date] > (select dateadd(year, -1, (select getdate())))";
            FillDataGridView();
        }
        private void listTop3Sales()
        {
            sql = @"select top(3) [Name],[Author],[Publisher],sum([Quantity]) as [Sales]
                    from [dbo].[Books] as B, [dbo].[Sales] as S
                    where B.[Id] = S.[BookID]
                    group by [Name],[Author],[Publisher]
                    order by [Sales] desc";
            FillDataGridView();
        }
        private void listTopAuthors()
        {
            sql = @"select top(3) [Author],sum([Quantity]) as [Sales]
                    from [dbo].[Books] as B, [dbo].[Sales] as S
                    where B.[Id] = S.[BookID]
                    group by [Author]
                    order by [Sales] desc";
            FillDataGridView();
        }
        private void GenreDay()
        {
            sql = @"select top(3) T.[Genre], sum([Quantity]) as [Sales]
                    from (select [Genre],[DateOfSale],[Quantity]
		                from [dbo].[Books] as B, [dbo].[Sales] as S
		                where B.[Id] = S.[BookID] and datediff(day, [DateOfSale], (select getdate())) = 0) as T
                    group by [Genre]
                    order by [Sales] desc";
            FillDataGridView();
        }
        private void GenreWeek()
        {
            sql = @"select top(3) T.[Genre], sum([Quantity]) as [Sales]
                    from (select [Genre],[DateOfSale],[Quantity]
		                from [dbo].[Books] as B, [dbo].[Sales] as S
		                where B.[Id] = S.[BookID] and datediff(day, [DateOfSale], (select getdate())) <= 7) as T
                    group by [Genre]
                    order by [Sales] desc";
            FillDataGridView();
        }
        private void GenreMonth()
        {
            sql = @"select top(3) T.[Genre], sum([Quantity]) as [Sales]
                    from (select [Genre],[DateOfSale],[Quantity]
		                from [dbo].[Books] as B, [dbo].[Sales] as S
		                where B.[Id] = S.[BookID] and datediff(month, [DateOfSale], (select getdate())) = 0) as T
                    group by [Genre]
                    order by [Sales] desc";
            FillDataGridView();
        }
        private void GenreYear()
        {
            sql = @"select top(3) T.[Genre], sum([Quantity]) as [Sales]
                    from (select [Genre],[DateOfSale],[Quantity]
		                from [dbo].[Books] as B, [dbo].[Sales] as S
		                where B.[Id] = S.[BookID] and datediff(year, [DateOfSale], (select getdate())) = 0) as T
                    group by [Genre]
                    order by [Sales] desc";
            FillDataGridView();
        }

        private void showFiltr_Click(object sender, EventArgs e)
        {
            if (FiltrComboBox.Texts == "Список новинок") 
            {
                listNewBooks();
            }
            else if (FiltrComboBox.Texts == "Список самых продаваемых книг")
            {
                listTop3Sales();
            }
            else if(FiltrComboBox.Texts == "Популярные авторы")
            {
                listTopAuthors();
            }
            else if (FiltrComboBox.Texts == "Самый популярный жанр за день")
            {
                GenreDay();
            }
            else if (FiltrComboBox.Texts == "Самый популярный жанр за неделю")
            {
                GenreWeek();
            }
            else if (FiltrComboBox.Texts == "Самый популярный жанр за месяц")
            {
                GenreMonth();
            }
            else if (FiltrComboBox.Texts == "Самый популярный жанр за год")
            {
                GenreYear();
            }
        }

        private void find_Click(object sender, EventArgs e)
        {
            if (comboBoxFindBy.Texts == "автору")
            {
                string param = textParam.Text;
                sql = "SELECT * FROM [dbo].[Books] WHERE [Author] = N'" + param + "'";
                FillDataGridView();
            }
            else if (comboBoxFindBy.Texts == "жанру")
            {
                string param = textParam.Text;
                sql = "SELECT * FROM [dbo].[Books] WHERE [Genre] = N'" + param + "'";
                FillDataGridView();
            }
            else if (comboBoxFindBy.Texts == "имени")
            {
                string param = textParam.Text;
                sql = "SELECT * FROM [dbo].[Books] WHERE [Name] = N'" + param + "'";
                FillDataGridView();
            }
        }

        private void sell_Click(object sender, EventArgs e)
        {
            try
            {
                BSConn.Open();
                SqlCommand command = new SqlCommand("sp_sellBook", BSConn);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter idParam = new SqlParameter
                {
                    ParameterName = "@id",
                    Value = dataGridView1.CurrentCell.Value.ToString()
                };
                command.Parameters.Add(idParam);

                SqlParameter qParam = new SqlParameter
                {
                    ParameterName = "@quantity",
                    Value = textBox2.Text
                };
                command.Parameters.Add(qParam);
                command.ExecuteNonQuery();
                BSConn.Close();
                MessageBox.Show("Продано");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void bookStoreForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            loginForm.Close();
        }

        private void sales_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            this.dataGridView1.RowsDefaultCellStyle.BackColor = Color.FromArgb(189, 224, 254);
            this.dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            
        }

        private void customButton1_Click(object sender, EventArgs e)
        {

        }

        private void close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void minimized_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x84:
                    base.WndProc(ref m);
                    if ((int)m.Result == 0x1)
                        m.Result = (IntPtr)0x2;
                    return;
            }

            base.WndProc(ref m);
        }
    }
}

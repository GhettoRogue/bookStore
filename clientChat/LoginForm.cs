using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Configuration;
using System.Threading;

namespace clientChat
{
    public partial class LoginForm : Form
    {
        SqlConnection LFConn;
        string cs = null;
        int connectCount = 0;
        ChatForm chatForm;
        bookStoreForm BSForm;
        AdminForm adminForm;
        AdminUsersForm adminUsersForm;

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

        public LoginForm()
        {
            InitializeComponent();
            LFConn = new SqlConnection();
            cs = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            LFConn.ConnectionString = cs;

            //скругление формы
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        private void buttonEnter_Click(object sender, EventArgs e)
        {
            if (isValid())
            {
                try
                {
                    string query = "select * from [dbo].[Managers] where [Login] = N'"
                        + login.Text.Trim() + "' and [Password] = N'"
                        + password.Text.Trim() + "'";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, LFConn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    
                    if (dt.Rows.Count == 1)
                    {
                        int lvl = dt.Rows[0].Field<int>("AccessLevel");

                        connectCount++;
                        string name = dt.Rows[0][2].ToString();
                        if (lvl == 2)
                        {
                            if (connectCount == 1)
                            {
                                chatForm = new ChatForm(name, this);
                                BSForm = new bookStoreForm(this);
                            }
                            else chatForm.ChangeUser(name);

                            this.Hide();

                            chatForm.Show();
                            BSForm.Show();
                        }
                        else if (lvl == 1) 
                        {
                            if(connectCount == 1)
                            {
                                chatForm = new ChatForm(name, this);
                                adminForm = new AdminForm(this);
                                adminUsersForm = new AdminUsersForm(this);
                            }
                            else chatForm.ChangeUser(name);

                            this.Hide();

                            chatForm.Show();
                            adminForm.Show();
                            adminUsersForm.Show();
                        }

                        login.Clear();
                        password.Clear();
                    }
                    else MessageBox.Show("Ошибка. Проверьте корректность введнных даных");
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка входа!");
                    connectCount--;
                }
            }
        }
        private bool isValid()
        {
            if (login.Text.TrimStart() == string.Empty)
            {
                MessageBox.Show("Введите логин");
                return false;
            }
            else if (password.Text.TrimStart() == string.Empty)
            {
                MessageBox.Show("Введите пароль");
                return false;
            }
            return true;
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

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

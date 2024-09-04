using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace clientChat
{
    public partial class ChatForm : Form, ChatService.IChatServiceCallback
    {
        bool IsConnected = false;
        ChatService.ChatServiceClient Client;
        int ID;
        LoginForm loginForm;
        public ChatForm(string name, LoginForm lg)
        {
            InitializeComponent();
            
            loginForm = lg;

            UserBox.Text = name;
            UserBox.Enabled = false;
            ConnectUser();
        }
        public void ChangeUser(string name)
        {
            UserBox.Text = name;
            ConnectUser();
        }
        void ConnectUser()
        {
            if (!IsConnected)
            {
                Client = new ChatService.ChatServiceClient(new System.ServiceModel.InstanceContext(this));

                ID = Client.Connect(UserBox.Text.ToString());

                //UserBox.Enabled = false;
                //buttonEnter.Text = "Отключиться";
                IsConnected = true;
            }
        }
        void DisconnectUser()
        {
            if (IsConnected)
            {
                Client.Disconnect(ID);
                
                //UserBox.Enabled = true;
                //buttonEnter.Text = "Подключиться";
                IsConnected = false;

                this.Hide();
                loginForm.Show();
            }
        }
        private void buttonEnter_Click(object sender, EventArgs e)
        {
            if(IsConnected) DisconnectUser();
            else ConnectUser();
        }

        public void MessageCallback(string msg)
        {
            chatBox.Items.Add(msg);
        }
        private void ChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisconnectUser();
            loginForm.Close();
        }

        private void msgBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if(Client != null)
                {
                    Client.SendMessage(msgBox.Text.ToString(), ID);
                    msgBox.Clear();
                }
            }
        }
    }
}

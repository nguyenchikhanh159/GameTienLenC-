using HienBai;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Player
{
    public partial class WaitingRoom : Form
    {
        public WaitingRoom()
        {
            InitializeComponent();

        }
        Socket sk_vaoPhong;
        Socket sk_port;
        public static IPEndPoint ipe;
        public static string room;
        public static string betMoney;
        public static int backup_port=0;
        static public Socket sk_status;
        static public Socket sk_request;
        static public Socket sk_card;
        static public Socket sk_activating;
        static public Socket sk_signalDel;
        static public Socket sk_receiveBroadcast;
        static public Socket sk_signalOtherPlayerPlaying;
        static public Socket sk_Rank;
        static public Socket sk_money;

        public void WaitingRoom_Load(object sender, EventArgs e)
        {
            cmbMoney.Items.Add(100);
            cmbMoney.Items.Add(200);
            cmbMoney.Items.Add(500);
            cmbMoney.SelectedItem = 100;
            btnCreateRoom.Enabled = false;
            btnFindRoom.Enabled = false;
           
        }

        public void btnFindRoom_Click(object sender, EventArgs e)
        {
            string money = cmbMoney.SelectedItem.ToString();
            sk_vaoPhong.Send(Encoding.Unicode.GetBytes("Find" + "_" + money));
            VaoPhong();
        }
        public void CreateRoom(object sender, EventArgs e)
        {
            string money = cmbMoney.SelectedItem.ToString();
            sk_vaoPhong.Send(Encoding.Unicode.GetBytes("Create" + "_" + money));
            VaoPhong();
        }
     
       
        public void ConnectToServer(string ip,int port)
        {
            try
            {
                ipe = new IPEndPoint(IPAddress.Parse(ip), port);
                sk_vaoPhong = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                sk_vaoPhong.Connect(ipe);

                sk_port = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                sk_port.Connect(ipe);

                sk_status = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                sk_status.Connect(ipe);

                sk_request = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                sk_request.Connect(ipe);

                sk_card = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                sk_card.Connect(ipe);

                sk_activating = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                sk_activating.Connect(ipe);

                sk_signalDel = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                sk_signalDel.Connect(ipe);

                sk_receiveBroadcast = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                sk_receiveBroadcast.Connect(ipe);

                sk_signalOtherPlayerPlaying = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                sk_signalOtherPlayerPlaying.Connect(ipe);

                sk_Rank = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                sk_Rank.Connect(ipe);

                sk_money = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                sk_money.Connect(ipe);


                Thread checkconnect = new Thread(SocketConnected);
                checkconnect.IsBackground = true;
                checkconnect.Start(sk_vaoPhong);
                
                
                Thread p = new Thread(ListenPort);
                p.IsBackground = true;
                p.Start();

            }
            catch
            {
                ConnectToServer(txtIP.Text, 14000);
                //MessageBox.Show("Fail Connect");
            }
        }

        public void ListenPort()
        {
                byte[] buff = new byte[1024];
                int rec = sk_port.Receive(buff);
                string sig = "";
                for (int i = 0; i < rec; i += 2)
                {
                    sig += Convert.ToChar(buff[i]).ToString();
                }
                backup_port = Convert.ToInt32(sig);
            
           
        }

        public void SocketConnected(object obj)
        {
            Thread.Sleep(1000);
            while (true)
            { 
            Socket s = obj as Socket;
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            if (part1 && part2)
              {
                if (backup_port != 0)
                {
                        ConnectToServer(txtIP.Text, backup_port);
                        break;
                }
             }
           }
        }
        public void VaoPhong()
        {
            byte[] buff = new byte[1024];
            int rec = sk_vaoPhong.Receive(buff);
            string sig = "";
            for (int i = 0; i < rec; i += 2)
            {
               sig += Convert.ToChar(buff[i]).ToString();
            }
            if (sig == "Not found")
            {
                MessageBox.Show(sig);
            }
            else if(sig.Contains("Create"))
            {
                string[] temp = sig.Split('_');
                room = temp[1];
                betMoney = temp[2];
                frmRoom frm = new frmRoom();
                this.Hide();
                frm.Show();
            }
            else 
            {
                string[] temp = sig.Split('_');
                room = temp[1];
                betMoney = temp[2];
                frmRoom frm = new frmRoom();
                this.Hide();
                frm.Show();
            }
           
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            if (txtIP.Text != "")
            {
                ConnectToServer(txtIP.Text, 13000);
                btnConnect.Enabled = false;
                btnCreateRoom.Enabled = true;
                btnFindRoom.Enabled = true;
            }
            else
            {
                MessageBox.Show("Nhập IP!!!");
            }
            
            
            
            
        }

        private void btnCreaRoom_MouseMove(object sender, MouseEventArgs e)
        {
            btnCreateRoom.BackColor = Color.BurlyWood;
        }

        private void btnCreateRoom_MouseLeave(object sender, EventArgs e)
        {
            btnCreateRoom.BackColor = Color.White;
        }

        private void btnConnect_MouseMove(object sender, MouseEventArgs e)
        {
            btnConnect.BackColor= Color.BurlyWood;
        }

        private void btnConnect_MouseLeave(object sender, EventArgs e)
        {
            btnConnect.BackColor = Color.White;
        }

        private void btnFindRoom_MouseMove(object sender, MouseEventArgs e)
        {
            btnFindRoom.BackColor= Color.BurlyWood;
        }

        private void btnFindRoom_MouseLeave(object sender, EventArgs e)
        {
            btnFindRoom.BackColor = Color.White;
        }
    }
}

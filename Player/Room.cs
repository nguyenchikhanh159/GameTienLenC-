

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TienLen;
using Newtonsoft.Json;
using System.Xml.Serialization;
using Player;
//using Newtonsoft.Json;


namespace HienBai
{
    public partial class frmRoom : Form
    {

        #region properties
        public static List<Control> ctrl = new List<Control>();
        public static List<Control> ctrl1 = new List<Control>();// chứa bài từ sv broadcast về
        public static List<Control> ctrl2 = new List<Control>();// chứa các lable 
        public static List<ClsRank> RankInfo = new List<ClsRank>();
        public static ClsHandCards hand = new ClsHandCards();
        public static ClsRules rule = new ClsRules();
        public string[] filename = new string[13];
        static string signal = "";
        static bool flag = true;// kiểm tra lần đầu kết nối
        private bool _exiting;

        #endregion
        #region Support Method
        private void ShowCardOfBroadcast(List<ClsCard> broadcast_card)// Show bài vừa đánh
        {
            for (int index = 0; index < ctrl1.Count && index < broadcast_card.Count; index++)
            {
                string name = broadcast_card[index].Value + "_" + broadcast_card[index].Character;
                Bitmap temp = new Bitmap(Application.StartupPath + "\\Resources\\" + name + ".jpg");
                PictureBox pb = ctrl1[index] as PictureBox;

                pb.SizeMode = PictureBoxSizeMode.AutoSize;

                pb.Image = temp;
            }
        }
        private void ShowImageBackGround()
        {
            for (int index = 0; index < ctrl.Count; index++)
            {
                Bitmap temp = new Bitmap(Application.StartupPath + "\\Resources\\" + "BG4" + ".png");
                PictureBox pb = ctrl[index] as PictureBox;

                pb.SizeMode = PictureBoxSizeMode.AutoSize;

                pb.Image = temp;
            }
        }
        private void ClearCardBroadCast()//Xóa bài trên bàn
        {

            for (int index = 0; index < ctrl1.Count; index++)
            {

                PictureBox pb = ctrl1[index] as PictureBox;
                pb.Invoke(new MethodInvoker(delegate ()
                {
                    pb.Image = null;
                }));

            }
        }
        private void ClearHandCard()// xóa các quân bài trên tay
        {
            for (int index = 0; index < ctrl.Count; index++)
            {
                PictureBox pb = ctrl[index] as PictureBox;
                pb.Invoke(new MethodInvoker(delegate ()
                {
                    pb.Image = null;
                }));
            }
        }

        public void ShowCards(ClsHandCards clc)//show bài vừa nhận đc
        {
            for (int index = 0; index < ctrl.Count; index++)
            {
                // int index = (i + 13 - clc.numOfRemainCards);
                if (clc.arrCard[index].Played == false)
                {
                    ctrl[index].Visible = true;
                    string name = clc.arrCard[index].Value + "_" + clc.arrCard[index].Character;
                    Bitmap temp = new Bitmap(Application.StartupPath + "\\Resources\\" + name + ".jpg");

                    PictureBox pb = ctrl[index] as PictureBox;
                    pb.Invoke(new MethodInvoker(delegate ()
                    {
                        pb.SizeMode = PictureBoxSizeMode.AutoSize;

                        pb.Image = temp;
                    }));


                }
            }
        }
        public static void Swap(ref string lhs, ref string rhs)
        {
            string temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
        public void sortFileName(string[] filename)
        {
            for (int i = 0; i < filename.Length - 1; i++)
            {
                for (int j = i + 1; j < filename.Length; j++)
                {
                    int v1 = Convert.ToInt32(filename[i].Substring(0, filename[i].LastIndexOf('_')));
                    int v2 = Convert.ToInt32(filename[j].Substring(0, filename[j].LastIndexOf('_')));
                    if (v1 > v2)
                    {
                        Swap(ref filename[i], ref filename[j]);
                    }
                    else if (v1 == v2)
                    {

                        int c1 = Convert.ToInt32(filename[i].Substring(filename[i].LastIndexOf('_') + 1));
                        int c2 = Convert.ToInt32(filename[j].Substring(filename[j].LastIndexOf('_') + 1));
                        if (c1 > c2)
                        {
                            Swap(ref filename[i], ref filename[j]);
                        }
                    }
                }
            }
        }
        public static List<ClsRank> Deserialize_Rank(byte[] buff)// giải mã rank
        {
            MemoryStream stream = new MemoryStream(buff);
            XmlSerializer deserializer = new XmlSerializer(typeof(List<ClsRank>));
            List<ClsRank> c = (List<ClsRank>)deserializer.Deserialize(stream);
            return c;
        }
        public static List<ClsCard> Deserialize(byte[] buff)// giải mã bài
        {
            MemoryStream stream = new MemoryStream(buff);
            XmlSerializer deserializer = new XmlSerializer(typeof(List<ClsCard>));
            List<ClsCard> c = (List<ClsCard>)deserializer.Deserialize(stream);
            return c;
        }
        public static byte[] Serialize(List<ClsCard> c)//mã hóa bài
        {        
            MemoryStream stream = new MemoryStream();
            XmlSerializer xmlForamt = new XmlSerializer(typeof(List<ClsCard>));
            xmlForamt.Serialize(stream, c);
            byte[] buff = new byte[1024];
            buff = stream.ToArray();
            stream.Close();
            return buff;

        }
        public bool IsContain(List<ClsCard> arr, ClsCard t)//kiểm tra sự tồn tại của 1 quân bài
        {
            foreach (ClsCard c in arr)
            {
                if (t.Value == c.Value && t.Character == c.Character)
                    return true;
            }
            return false;
        }
        public void AddImgName(List<ClsCard> clc)
        {
            for (int i = 0; i < clc.Count; i++)
            {
                string name = clc[i].Value + "_" + clc[i].Character;
                filename[i] = name;

            }
        }
        #endregion
        #region Listen
        public void ListenToServer()//Kết nối với server
        {
            try
            {

                Thread s = new Thread(ListenToStatus);
                s.IsBackground = true;
                s.Start();

                Thread c = new Thread(ListenToCard);
                c.IsBackground = true;
                c.Start();
                Thread a = new Thread(ListenToActivating);
                a.IsBackground = true;
                a.Start();
                Thread b = new Thread(ListenToReciveBroadcast);
                b.IsBackground = true;
                b.Start();
                Thread o = new Thread(ListenToSigalOtherPlayerPlaying);
                o.IsBackground = true;
                o.Start();
                Thread r = new Thread(ListenToRank);
                r.IsBackground = true;
                r.Start();

                Thread m = new Thread(ListenToMoney);
                m.IsBackground = true;
                m.Start();

            }
            catch (Exception)
            {

            }
        }
        private void ListenToMoney()// Lắng nghe tiền phạt or thưởng
        {
            while (true)
            {
                byte[] buff = new byte[1024];
                int rec = WaitingRoom.sk_money.Receive(buff);
                string sig = "";
                for (int i = 0; i < rec; i += 2)
                {
                    sig += Convert.ToChar(buff[i]).ToString();
                }
                hand.money += Convert.ToInt32(sig);
                lblMoney.Invoke(new MethodInvoker(delegate ()
                {
                    lblMoney.Text = sig;
                }));
                             
            }
        }
        private void ListenToActivating()//Lắng nghe lượt đánh
        {
           while(true)
            {
                byte[] buff = new byte[1024];
                int rec=WaitingRoom.sk_activating.Receive(buff);
                string sig = "";
                for(int i=0;i<rec;i+=2)
                {
                    sig += Convert.ToChar(buff[i]).ToString();
                }
               if(sig=="Active")
                {
                    btnPlay.Invoke(new MethodInvoker(delegate ()
                    {
                        btnPlay.Enabled = true;
                    }));
                    btnIgnore.Invoke(new MethodInvoker(delegate ()
                    {
                        btnIgnore.Enabled = true;
                    }));
                    
                    hand.Activated = true;
                }
                else
                {
                    btnPlay.Invoke(new MethodInvoker(delegate ()
                    {
                        btnPlay.Enabled = true;
                    }));
                    hand.Activated = true;
                    btnIgnore.Invoke(new MethodInvoker(delegate ()
                    {
                        btnIgnore.Enabled = false;
                    }));
                }
            }
        }
        public void ListenToReciveBroadcast()// lắng nghe bài vừa đánh
        {
            while (true)
            {
                try
                {                 
                    byte[] buff = new byte[1024];
                    int rcv = WaitingRoom.sk_receiveBroadcast.Receive(buff);                   
                    List<ClsCard> broadcast_card = new List<ClsCard>();
                    broadcast_card = Deserialize(buff);
                    ClearCardBroadCast();
                    ShowCardOfBroadcast(broadcast_card);
                }
                catch
                {
                   // MessageBox.Show("fail");
                    break;
                }
            }

        }
        public void ListenToSignalDel()//lắng nghe tín hiệu xóa
        {
            byte[] buff = new byte[1024];
            int rcv = WaitingRoom.sk_signalDel.Receive(buff);
            signal = "";
            if (rcv != 0)
            {
                for (int i = 0; i < rcv; i += 2)

                    signal += Convert.ToChar(buff[i]).ToString();
            }

        }
        private void ListenToStatus()// lắng nghe số người chơi tring phòng
        {
            int old_index = 0;
            while (true)
            {
                try
                {
                    byte[] buff = new byte[1024];
                    int rcv = WaitingRoom.sk_status.Receive(buff);
                    if (rcv != 0)
                    {
                        lblNumberOfPlayer.Invoke(new MethodInvoker(delegate ()
                        {
                            lblNumberOfPlayer.ResetText();
                            for (int i = 0; i < rcv; i += 2)
                            {
                                lblNumberOfPlayer.Text += Convert.ToChar(buff[i]).ToString();
                            }
                        }));
                       
                        
                        if (!flag)
                        {                           
                            int index = Convert.ToInt32(lblNumberOfPlayer.Text);
                            Label templb = ctrl2[(index-old_index-1)%3] as Label;
                            ctrl2[(index - old_index - 1) % 3].Text = "Player:" + index.ToString() ;
                            templb.ForeColor = System.Drawing.Color.Yellow;
                                                       
                        }
                        else
                        {
                            flag = false;
                            old_index = Convert.ToInt32(lblNumberOfPlayer.Text);
                            int index = Convert.ToInt32(lblNumberOfPlayer.Text) - 1;                          
                            for(int i=2;i>=0;i--)
                            {
                                if (index != 0)
                                {
                                    Label templb = ctrl2[i] as Label;
                                    ctrl2[i].Text = "Player:"+index.ToString();
                                    templb.ForeColor = System.Drawing.Color.Yellow;
                                    index--;
                                }
                                else
                                    break;
                            }
                                
                            
                        }
                    }
                }
                catch
                {
                    break;
                }

            }
        }
        private void ListenToCard()// lắng nghe bài đc chia
        {

            while (true)
            {
                try
                {
                    lvRank.Invoke(new MethodInvoker(delegate ()
                    {
                        if (lvRank.Visible == true)
                        {
                            lvRank.Visible = false;
                            for (int i = lvRank.Items.Count - 1; i >= 0; i--)
                            {
                                lvRank.Items[i].Remove();
                            }
                        }
                    }));
                   
                    byte[] buff = new byte[1024];
                    WaitingRoom.sk_card.Receive(buff);
                    Array.Clear(filename, 0, 13);
                    ClearCardBroadCast();
                    hand.arrCard = Deserialize(buff);
                    AddImgName(hand.arrCard);
                    ClearHandCard();              
                    ShowCards(hand);
                   
            

                }
                catch
                {
                    break;
                }
            }

        }
        private void ListenToSigalOtherPlayerPlaying()// lắng nghe ai đang có quyền đánh
        {
            while (true)
            {
                try
                {
                    string temp = "";
                    byte[] buff = new byte[1024];
                    int rec= WaitingRoom.sk_signalOtherPlayerPlaying.Receive(buff);
                    for (int i = 0; i < rec; i += 2)
                    {
                        temp+= Convert.ToChar(buff[i]).ToString();
                    }
                    if (temp != "")
                    {
                        foreach (Control item in ctrl2)
                        {
                            Label lb = item as Label;
                            lb.ForeColor = System.Drawing.Color.Yellow;
                        }
                        int index = Convert.ToInt32(temp);
                        foreach (Control item in ctrl2)
                        {
                            Label lb = item as Label;
                            if (lb.Text == "Player:" + index.ToString())
                                lb.ForeColor = System.Drawing.Color.Blue;
                            else
                            {
                                continue;
                            }
                        }
                    }

                }
                catch
                {
                    break;
                }

            }
        }
        public void ListenToRank()// lắng nghe bảng xếp hạng
        {
            while (true)
            {
                try
                {
                    byte[] buff = new byte[1024];
                    int rec = WaitingRoom.sk_Rank.Receive(buff);
                    RankInfo = Deserialize_Rank(buff);
                    if (rec != 0)
                    {

                        foreach (ClsRank item in RankInfo)
                        {
                            ListViewItem temp;
                            string[] arr = new string[2];
                            arr[0] = item.name;
                            arr[1] = item.rank.ToString();
                            temp = new ListViewItem(arr);
                            lvRank.Items.Add(temp);
                        }
                        lvRank.Visible = true;
                    }
                }
                catch
                {
                    break;
                }
            }
        }
        #endregion
       
        public frmRoom()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            lblNumberRoom.Text += WaitingRoom.room;
            lblBetMoney.Text += WaitingRoom.betMoney;
            lblNumberOfPlayer.Text = "0";
            lblMoney.Text = hand.money.ToString();
            ListenToServer();
            //CheckForIllegalCrossThreadCalls = false;
            ctrl.Add(pictureBox1);
            ctrl.Add(pictureBox2);
            ctrl.Add(pictureBox3);
            ctrl.Add(pictureBox4);
            ctrl.Add(pictureBox5);
            ctrl.Add(pictureBox6);
            ctrl.Add(pictureBox7);
            ctrl.Add(pictureBox8);
            ctrl.Add(pictureBox9);
            ctrl.Add(pictureBox10);
            ctrl.Add(pictureBox11);
            ctrl.Add(pictureBox12);
            ctrl.Add(pictureBox13);

            ctrl1.Add(pictureBox14);
            ctrl1.Add(pictureBox15);
            ctrl1.Add(pictureBox16);
            ctrl1.Add(pictureBox17);
            ctrl1.Add(pictureBox18);
            ctrl1.Add(pictureBox19);
            ctrl1.Add(pictureBox20);
            ctrl1.Add(pictureBox21);
            ctrl1.Add(pictureBox22);
            ctrl1.Add(pictureBox23);
            ctrl1.Add(pictureBox24);
            ctrl1.Add(pictureBox25);
            ctrl1.Add(pictureBox26);

            ctrl2.Add(lbl1);
            ctrl2.Add(lbl2);
            ctrl2.Add(lbl3);

            ShowImageBackGround();
         
            btnPlay.Enabled = false;
            btnIgnore.Enabled = false;

            lvRank.Visible = false;
            lvRank.Columns.Add("Tên", 150 / 2);
            lvRank.Columns.Add("Hạng", 150 / 2);
            //Tien.Text=Tien.Text+ WaitingRoom.cmbMoney.SelectedItem.ToString();

        }


        #region Event
      
        private void pictureBox11_Click(object sender, EventArgs e)
        {

            PictureBox pb = sender as PictureBox;
            if (pb.Location.Y == 581)
            {
                int x = pb.Location.X;
                int y = pb.Location.Y;
                pb.Location = new System.Drawing.Point(x, y - 26);

            }
            else
            {
                int x = pb.Location.X;
                int y = pb.Location.Y;
                pb.Location = new System.Drawing.Point(x, y + 26);


            }
        }
        private void btnShow_Click(object sender, EventArgs e)
        {
            WaitingRoom.sk_request.Send(Encoding.Unicode.GetBytes("Deal"));
            btnShow.Enabled = false;
        }// show bài
        private void btnSort_Click(object sender, EventArgs e)//xếp bài
        {
            hand.arrCard.Sort();
            ShowCards(hand);
            sortFileName(filename);
        }

     
        private void btnPlay_Click_1(object sender, EventArgs e)//đánh bài
        {
            List<int> IndexOfSel = new List<int>();
            for (int i = 0; i < ctrl.Count; i++)
            {
                if (hand.arrCard[i].Played == false && ctrl[i].Location.Y != 581)
                {

                    PictureBox temp_pic = ctrl[i] as PictureBox;

                    string nameImage = filename[i];

                    string[] temp = nameImage.Split('_');
                    ClsCard c = new ClsCard();
                    c.Value = Convert.ToInt32(temp[0]);
                    c.Character = Convert.ToInt32(temp[1]);
                    hand.arrSelCards.Add(c);
                    IndexOfSel.Add(i);
                }

            }
            bool check = rule.IsValid(hand.arrSelCards);
            if (check == true)
            {

                byte[] buf = new byte[1024];
                buf = Serialize(hand.arrSelCards);
                WaitingRoom.sk_card.Send(buf);
                ListenToSignalDel();
                if (signal == "OK")
                {
                    foreach (int item in IndexOfSel)
                    {

                        PictureBox temp_pic = ctrl[item] as PictureBox;
                        temp_pic.Visible = false;
                        int x = ctrl[item].Location.X;
                        int y = ctrl[item].Location.Y;
                        ctrl[item].Location = new System.Drawing.Point(x, y + 26);
                        hand.arrCard[item].Played = true;
                    }
                    hand.numOfSelectedCards = hand.arrSelCards.Count;
                    hand.numOfRemainCards -= hand.numOfSelectedCards;
                    ShowCards(hand);
                    //gửi socket xong -> xóa arrsel

                    hand.arrSelCards.RemoveRange(0, hand.arrSelCards.Count);
                    hand.Activated = false;
                    btnPlay.Enabled = false;
                    btnIgnore.Enabled = false;


                }
                else
                {
                    MessageBox.Show("Ahihi! bạn đã chơi ngu rồi :))","Trừ tiền nhé");
                    foreach (PictureBox item in ctrl)
                    {
                        if (item.Location.Y != 581)
                        {
                            int x = item.Location.X;
                            int y = item.Location.Y;
                            item.Location = new System.Drawing.Point(x, y + 26);
                            hand.arrSelCards.RemoveRange(0, hand.arrSelCards.Count);

                        }
                    }
                    ShowCards(hand);
                }
            }
            else
            {
                foreach (PictureBox item in ctrl)
                {
                    if (item.Location.Y != 581)
                    {
                        int x = item.Location.X;
                        int y = item.Location.Y;
                        item.Location = new System.Drawing.Point(x, y + 26);
                        hand.arrSelCards.RemoveRange(0, hand.arrSelCards.Count);

                    }
                }
                ShowCards(hand);

            }

        }

        private void btnIgnore_Click_1(object sender, EventArgs e)//bỏ lượt
        {
            try
            {
                WaitingRoom.sk_activating.Send(Encoding.Unicode.GetBytes("Ignore"));
                hand.Activated = false;
                hand.Status = false;
                btnPlay.Enabled = false;
                btnIgnore.Enabled = false;
            }
            catch
            {

            }
        }

        private void pictureBox27_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmRoom_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_exiting && MessageBox.Show("Are you sure want to exit?",
                      "My First Application",
                       MessageBoxButtons.OKCancel,
                       MessageBoxIcon.Information) == DialogResult.OK)
            {
                _exiting = true;
                // this.Close(); // you don't need that, it's already closing
                Environment.Exit(1);
            }
        }

		private void lbl2_Click(object sender, EventArgs e)
		{

		}

        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
            button1.BackColor = Color.BurlyWood;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackColor = Color.Khaki;
        }

        private void btnPlay_MouseMove(object sender, MouseEventArgs e)
        {
            btnPlay.BackColor = Color.BurlyWood;
        }

        private void btnPlay_MouseLeave(object sender, EventArgs e)
        {
            btnPlay.BackColor = Color.Khaki;
        }

        private void btnIgnore_MouseMove(object sender, MouseEventArgs e)
        {
            btnIgnore.BackColor=Color.BurlyWood;
        }

        private void btnIgnore_MouseLeave(object sender, EventArgs e)
        {
            btnIgnore.BackColor = Color.Khaki;
        }

        private void btnShow_MouseMove(object sender, MouseEventArgs e)
        {
            btnShow.BackColor = Color.BurlyWood;
        }

        private void btnShow_MouseLeave(object sender, EventArgs e)
        {
            btnShow.BackColor= Color.Khaki;
        }
    }
    #endregion


}

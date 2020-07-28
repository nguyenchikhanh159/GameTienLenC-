
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TienLen;

namespace Server
{
    class Program
    {
        #region properties
        static object thislock = new object();
        static TcpListener server;
        Random rd = new Random();
        static List<Socket> socket_cards = new List<Socket>();// để nhận và gửi bài
        static List<Socket> socket_status = new List<Socket>();//để cập nhật số người chơi
        static List<Socket> socket_request = new List<Socket>();// để nhận tín hiệu yêu cầu chơi
        static List<Socket> socket_activatingPlayer = new List<Socket>();//lắng nghe tín hiệu bỏ lượt và kích hoạt người chơi
        static List<Socket> socket_signalDel = new List<Socket>();
        static List<Socket> socket_broadcastCard = new List<Socket>();
        static List<Socket> socket_broadcastStatusPlaying = new List<Socket>();
        static List<Socket> socket_Rank = new List<Socket>();
        static List<Socket> socket_money = new List<Socket>();
        static List<Socket> socket_port = new List<Socket>();
        static List<ClsHandCards> client = new List<ClsHandCards>();
        static ClsRules rule = new ClsRules();
        static IPEndPoint ipe;
        static string _myIp;
        static List<phong> dsPhong = new List<phong>();
        static int numberOfRoom = 0;
        static List<Socket> socket_vaophong = new List<Socket>();
        static int num = -1;
        static int dem = 0; //đếm số lượng người sẵn sàng
        #endregion
        #region   Accept Socket
        public static void Accept_Socket_Port(int num)
        {
            int status = -1;
            Socket sk = SetUpANewConnection(ref status);
            if (status != -1)
            {
                socket_port.Add(sk);
                socket_port[num].Send(Encoding.Unicode.GetBytes("14000"));
            }
        }
        public static void Accept_Socket_VaoPhong(int num)
        {
            int status = -1;
            Socket sk = SetUpANewConnection(ref status);
            if (status != -1)
            {
                socket_vaophong.Add(sk);
                Console.WriteLine("Chap nhan ket noi:" + sk.RemoteEndPoint.ToString());
                client.Add(new ClsHandCards());
                //num = client.Count - 1;
            }
            Thread join = new Thread(vaoPhong);
            join.Start(num);
        }
        public static void Accept_Socket_Status()
        {
            int status = -1;
            Socket sk = SetUpANewConnection(ref status);
            if (status != -1)
            {
                socket_status.Add(sk);

            }

        }
        public static void Accept_Socket_Cards()
        {
            int status = -1;
            Socket sk = SetUpANewConnection(ref status);
            if (status != -1)
            {
                socket_cards.Add(sk);
            }

        }
        public static void Accept_Socket_Request()
        {
            int status = -1;
            Socket sk = SetUpANewConnection(ref status);
            if (status != -1)
            {
                socket_request.Add(sk);
            }
        }
        public static void Accept_Socket_Activating()
        {
            int status = -1;
            Socket sk = SetUpANewConnection(ref status);
            if (status != -1)
            {
                socket_activatingPlayer.Add(sk);
            }

        }
        public static void Accept_Socket_SignalDel()
        {
            int status = -1;
            Socket sk = SetUpANewConnection(ref status);
            if (status != -1)
            {
                socket_signalDel.Add(sk);
            }

        }
        public static void Accept_Socket_BroadCast()
        {
            int status = -1;
            Socket sk = SetUpANewConnection(ref status);
            if (status != -1)
            {
                socket_broadcastCard.Add(sk);
            }
        }
        public static void Accept_Socket_BroadCastStatusPlaying()
        {
            int status = -1;
            Socket sk = SetUpANewConnection(ref status);
            if (status != -1)
            {
                socket_broadcastStatusPlaying.Add(sk);
            }
        }
        public static void Accept_Socket_Rank()
        {
            int status = -1;
            Socket sk = SetUpANewConnection(ref status);
            if (status != -1)
            {
                socket_Rank.Add(sk);
            }
        }
        public static void Accept_Socket_Money()
        {
            int status = -1;
            Socket sk = SetUpANewConnection(ref status);
            if (status != -1)
            {
                socket_money.Add(sk);

            }

        }
        #endregion
        #region Support Method
        public static Socket SetUpANewConnection(ref int status)
        {
            Socket socket = server.AcceptSocket();
            status = 1;
            return socket;
        }//tạo kết nối mới
        public static void GetIP()// Lấy IP
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress diachi in host.AddressList)
            {
                if (diachi.AddressFamily.ToString() == "InterNetwork")
                {
                    _myIp = diachi.ToString();
                }
            }
            ipe = new IPEndPoint(IPAddress.Parse(_myIp), 13000);
            server = new TcpListener(ipe);
            Console.WriteLine(_myIp);
        }
        public static byte[] Serialize_Rank(List<ClsRank> r)//mã hóa rank
        {
            MemoryStream stream = new MemoryStream();
            XmlSerializer xmlForamt = new XmlSerializer(typeof(List<ClsRank>));
            xmlForamt.Serialize(stream, r);
            byte[] buff = new byte[1024];
            buff = stream.ToArray();
            stream.Close();
            return buff;
        }
        public static List<ClsCard> Deserialize(byte[] buff)//giải mã bài
        {
            MemoryStream stream = new MemoryStream(buff);
            XmlSerializer deserializer = new XmlSerializer(typeof(List<ClsCard>));
            List<ClsCard> c = (List<ClsCard>)deserializer.Deserialize(stream);
            return c;
        }
        public static byte[] Serialize(List<ClsCard> c)// mã hóa bài
        {
            MemoryStream stream = new MemoryStream();
            XmlSerializer xmlForamt = new XmlSerializer(typeof(List<ClsCard>));
            xmlForamt.Serialize(stream, c);
            byte[] buff = new byte[1024];
            buff = stream.ToArray();
            stream.Close();
            return buff;

        }
        public static void FindNextPlayer(int index_next, int pos)//gửi tính hiệu ai có quyền đánh,xác định vòng mới hay vòng cũ
        {
            BroadcastStatusPlaying(index_next, client[pos].room);//
            if (index_next == dsPhong[client[pos].room].managerOfTheGame.justPlayer)
            {
                dsPhong[client[pos].room].managerOfTheGame.temp.RemoveRange(0, dsPhong[client[pos].room].managerOfTheGame.temp.Count);
                for (int i = 0; i < dsPhong[client[pos].room].indexOfClient.Count; i++)
                {
                    if (client[dsPhong[client[pos].room].indexOfClient[i]].Rank == 0)
                    {
                        client[dsPhong[client[pos].room].indexOfClient[i]].Status = true;
                    }
                }
                socket_activatingPlayer[dsPhong[client[pos].room].indexOfClient[index_next]].Send(Encoding.Unicode.GetBytes("Active New"));//vòng mới
            }
            else
            {
                socket_activatingPlayer[dsPhong[client[pos].room].indexOfClient[index_next]].Send(Encoding.Unicode.GetBytes("Active"));
            }
        }
        #endregion
        #region Method
        public static void StartServer()//Khởi động server
        {
            GetIP();
            string ip = _myIp;
            int port = 13000;
            server = new TcpListener(IPAddress.Parse(_myIp), port);
            server.Start();
            Thread t = new Thread(ServeClients);
            t.Start();
        }
        public static void ServeClients()//Phục vụ nhiều client
        {
            for (int i = 0; i < 200; i++)
            {
                Thread serAclient = new Thread(ServeAClient);
                serAclient.Start();
            }
        }
        public static void ServeAClient()//Phục vụ 1 client
        {

            try
            {
                //int num = -1;
                //int numberOfClient_temp = .numOfPlayers;//lấy số người chơi hiện tại
                lock (thislock)
                {
                    Accept_Socket_VaoPhong(num + 1);
                    Accept_Socket_Port(num + 1);
                    Accept_Socket_Status();
                    Accept_Socket_Request();
                    Accept_Socket_Cards();
                    Accept_Socket_Activating();
                    Accept_Socket_SignalDel();
                    Accept_Socket_BroadCast();
                    Accept_Socket_BroadCastStatusPlaying();
                    Accept_Socket_Rank();
                    Accept_Socket_Money();
                    //.numOfPlayers++;
                    //.client.Add(new ClsHandCards());
                    // client.Add(new ClsHandCards());
                    num = client.Count - 1;
                }
                // .client.Add(new ClsHandCards());
                //if (numberOfClient_temp != .numOfPlayers)//nếu có sự thay đổi số người chơi

                //    foreach (Socket item in socket_status)//gửi về tất tả các client đang kết nối
                //        item.Send(Encoding.Unicode.GetBytes(.numOfPlayers.ToString()));




                Thread r = new Thread(ListenRequest);
                r.Start(num);
                Thread c = new Thread(ListenCard);
                c.Start(num);
                Thread a = new Thread(ListenActivating);
                a.Start(num);


            }
            catch
            {

            }
        }
        public static void vaoPhong(object obj)// cho 1 client vào phòng
        {
            int pos = (Int32)obj;
            while (true)
            {
                try
                {
                    byte[] buff = new byte[1024];
                    int rcv = socket_vaophong[pos].Receive(buff);
                    string sig = "";
                    for (int i = 0; i < rcv; i += 2)
                    {
                        sig += Convert.ToChar(buff[i]).ToString();
                    }
                    if (sig.Contains("Create"))
                    {

                        string[] t = sig.Split('_');

                        phong temp = new phong();
                        numberOfRoom++;
                        temp.betMoney = Convert.ToInt32(t[1]);
                        temp.soPhong = numberOfRoom - 1;
                        temp.indexOfClient.Add(pos);
                        temp.managerOfTheGame.clientOfRoom.Add(client[pos]);
                        temp.managerOfTheGame.numOfPlayers++;
                        client[pos].room = temp.soPhong;
                        dsPhong.Add(temp);
                        socket_vaophong[pos].Send(Encoding.Unicode.GetBytes("Create" + "_" + (temp.soPhong + 1).ToString() + "_" + temp.betMoney.ToString()));
                        Thread.Sleep(1000);
                        socket_status[pos].Send(Encoding.Unicode.GetBytes(temp.indexOfClient.Count.ToString()));
                    }
                    else
                    {
                        string[] t = sig.Split('_');
                        bool check = false;
                        foreach (phong item in dsPhong)
                        {
                            if (item.indexOfClient.Count < 4 && Convert.ToInt32(t[1]) == item.betMoney)
                            {
                                client[pos].room = item.soPhong;
                                item.indexOfClient.Add(pos);
                                item.managerOfTheGame.clientOfRoom.Add(client[pos]);
                                item.managerOfTheGame.numOfPlayers++;
                                socket_vaophong[pos].Send(Encoding.Unicode.GetBytes("Find" + "_" + (item.soPhong + 1).ToString() + "_" + item.betMoney.ToString()));
                                Thread.Sleep(1000);
                                foreach (int x in item.indexOfClient)
                                    socket_status[x].Send(Encoding.Unicode.GetBytes(item.indexOfClient.Count.ToString()));
                                break;
                            }

                        }
                        if (check == false)
                            socket_vaophong[pos].Send(Encoding.Unicode.GetBytes("Not found"));
                    }
                }
                catch
                {

                    break;
                }

            }
        }
        public static void ListenCard(object obj)//Lắng nghe bài từ client
        {
            int pos = (Int32)obj;
            while (true)
            {
                try
                {
                    byte[] buff = new byte[1024];
                    int k = socket_cards[pos].Receive(buff);
                    if (k != 0)
                    {
                        client[pos].arrSelCards = Deserialize(buff);
                    }

                    if (dsPhong[client[pos].room].flag3bich)
                    {
                        client[pos].arrSelCards.Sort();
                        if (client[pos].arrSelCards[0].Value == 3 && client[pos].arrSelCards[0].Character == 1)
                        {
                            dsPhong[client[pos].room].flag3bich = false;
                            socket_signalDel[pos].Send(Encoding.Unicode.GetBytes("OK"));
                            dsPhong[client[pos].room].managerOfTheGame.justPlayer = dsPhong[client[pos].room].indexOfClient.IndexOf(pos);
                            client[pos].Activated = false;
                            client[pos].numOfRemainCards -= client[pos].arrSelCards.Count;
                            // thêm số phòng để gửi broadcast các client trong phòng
                            BroadcastResult(client[pos].arrSelCards, client[pos].room);
                            dsPhong[client[pos].room].managerOfTheGame.temp = client[pos].arrSelCards;
                            dsPhong[client[pos].room].managerOfTheGame.justPlayer = dsPhong[client[pos].room].indexOfClient.IndexOf(pos);
                            int index_next = dsPhong[client[pos].room].managerOfTheGame.NextPlayer() % 4;
                            FindNextPlayer(index_next, pos);
                        }
                        else
                        {
                            socket_signalDel[pos].Send(Encoding.Unicode.GetBytes("Dont't del"));
                            client[pos].money -= 100;
                            socket_money[pos].Send(Encoding.Unicode.GetBytes(client[pos].money.ToString()));
                        }
                    }
                    else if (dsPhong[client[pos].room].managerOfTheGame.temp.Count == 0 || dsPhong[client[pos].room].rule.
                        IsWin(dsPhong[client[pos].room].managerOfTheGame.temp, client[pos].arrSelCards) != 0)
                    {
                        //.temp = .client[pos].arrSelCards;
                        //thay thế mấy chỗ gán temp như ở trên thành như ở dưới
                        if (dsPhong[client[pos].room].managerOfTheGame.temp.Count != 0 && dsPhong[client[pos].room].rule.
                        IsWin(dsPhong[client[pos].room].managerOfTheGame.temp, client[pos].arrSelCards) == 2)
                        {
                            client[pos].money += dsPhong[client[pos].room].betMoney;
                            socket_money[pos].Send(Encoding.Unicode.GetBytes(client[pos].money.ToString()));
                            client[dsPhong[client[pos].room].managerOfTheGame.justPlayer].money -= dsPhong[client[pos].room].betMoney;
                            socket_money[dsPhong[client[pos].room].managerOfTheGame.justPlayer].Send(Encoding.Unicode.GetBytes(client[dsPhong[client[pos].room].managerOfTheGame.justPlayer].money.ToString()));

                        }
                        dsPhong[client[pos].room].managerOfTheGame.temp = client[pos].arrSelCards;
                        dsPhong[client[pos].room].managerOfTheGame.justPlayer = dsPhong[client[pos].room].indexOfClient.IndexOf(pos);
                        BroadcastResult(client[pos].arrSelCards, client[pos].room);
                        socket_signalDel[pos].Send(Encoding.Unicode.GetBytes("OK"));
                        client[pos].Activated = false;
                        client[pos].numOfRemainCards -= client[pos].arrSelCards.Count;

                        if (client[pos].numOfRemainCards == 0)
                        {
                            client[pos].Status = false;
                            dsPhong[client[pos].room].tempRank++;
                            client[pos].Rank = dsPhong[client[pos].room].tempRank;
                            dsPhong[client[pos].room].listRank.Add(new ClsRank("Player " + (dsPhong[client[pos].room].indexOfClient.IndexOf(pos) + 1).ToString(), client[pos].Rank));
                            dsPhong[client[pos].room].managerOfTheGame.dem++;
                            if (dsPhong[client[pos].room].managerOfTheGame.dem == 3)
                            {
                                dsPhong[client[pos].room].managerOfTheGame.temp.RemoveRange(0, dsPhong[client[pos].room].managerOfTheGame.temp.Count);
                                foreach (int item in dsPhong[client[pos].room].indexOfClient)
                                {
                                    socket_Rank[item].Send(Serialize_Rank(dsPhong[client[pos].room].listRank));
                                }
                                dsPhong[client[pos].room].listRank.RemoveRange(0, dsPhong[client[pos].room].listRank.Count);


                                foreach (int item in dsPhong[client[pos].room].indexOfClient)
                                {
                                    if (client[item].Rank == 1)
                                    {
                                        client[item].money += dsPhong[client[pos].room].betMoney * 2;
                                        socket_money[item].Send(Encoding.Unicode.GetBytes(client[item].money.ToString()));
                                    }
                                    else if (client[item].Rank == 3 || client[item].Rank == 0)
                                    {
                                        client[item].money -= dsPhong[client[pos].room].betMoney;
                                        socket_money[item].Send(Encoding.Unicode.GetBytes(client[item].money.ToString()));
                                    }
                                }
                                Thread.Sleep(3000);
                                int temp_anTrang = StartNewGame(client[pos].room);
                                while (temp_anTrang == 1)
                                {
                                    temp_anTrang = StartNewGame(client[pos].room);
                                }
                            }
                            else
                            {
                                int index_next = dsPhong[client[pos].room].managerOfTheGame.NextPlayer() % 4;
                                FindNextPlayer(index_next, pos);
                            }
                        }
                        else
                        {
                            int index_next = dsPhong[client[pos].room].managerOfTheGame.NextPlayer() % 4;
                            FindNextPlayer(index_next, pos);
                        }
                    }
                    else
                    {
                        socket_signalDel[pos].Send(Encoding.Unicode.GetBytes("Dont't del"));
                        client[pos].money -= 100;
                        socket_money[pos].Send(Encoding.Unicode.GetBytes(client[pos].money.ToString()));
                    }
                }
                catch
                {
                    Console.WriteLine("Cham dut ket noi:" + socket_status[pos].RemoteEndPoint.ToString());
                    socket_cards[pos].Close();
                    socket_cards.RemoveAt(pos);

                    socket_status[pos].Close();
                    socket_status.RemoveAt(pos);

                    dsPhong[client[pos].room].indexOfClient.Remove(pos);

                    if (socket_status.Count != 0)
                        foreach (int item in dsPhong[client[pos].room].indexOfClient)
                        {
                            socket_status[pos].Send(Encoding.Unicode.GetBytes(dsPhong[client[pos].room].indexOfClient.Count.ToString()));
                        }
                    break;
                }
            }
        }
        public static int StartNewGame(int room)//Bắt đầu ván chơi mới
        {
            dsPhong[room].managerOfTheGame.Clear();
            dsPhong[room].managerOfTheGame.Deal();
            dsPhong[room].managerOfTheGame.dem = 0;
            dsPhong[room].tempRank = 0;

            foreach (int i in dsPhong[room].indexOfClient)
            {
                if (rule.isAnTrang(client[i].arrCard))
                {
                    client[i].money += dsPhong[room].betMoney;
                    socket_money[i].Send(Encoding.Unicode.GetBytes(client[i].money.ToString()));
                    foreach (int j in dsPhong[room].indexOfClient)
                        if (j != i)
                        {
                            client[j].money -= dsPhong[room].betMoney;
                            socket_money[j].Send(Encoding.Unicode.GetBytes(client[j].money.ToString()));
                        }
                    return 1;
                }

            }

            for (int i = 0; i < dsPhong[room].indexOfClient.Count; i++)
            {
                client[dsPhong[room].indexOfClient[i]].Status = true;
            }
            for (int i = 0; i < dsPhong[room].indexOfClient.Count; i++)
            {
                byte[] buff_temp = new byte[1024];
                buff_temp = Serialize(client[dsPhong[room].indexOfClient[i]].arrCard);
                socket_cards[dsPhong[room].indexOfClient[i]].Send(buff_temp);

            }
            if (dsPhong[room].flag3bich)
                for (int j = 0; j < dsPhong[room].indexOfClient.Count; j++)
                {
                    if (dsPhong[room].managerOfTheGame.IsContain(client[dsPhong[room].indexOfClient[j]].arrCard, new ClsCard(3, 1)))
                    {
                        socket_activatingPlayer[dsPhong[room].indexOfClient[j]].Send(Encoding.Unicode.GetBytes("Active New"));
                    }
                }
            else
            {
                for (int j = 0; j < dsPhong[room].indexOfClient.Count; j++)
                {

                    if (client[dsPhong[room].indexOfClient[j]].Rank == 1)
                    {
                        socket_activatingPlayer[dsPhong[room].indexOfClient[j]].Send(Encoding.Unicode.GetBytes("Active New"));
                        BroadcastStatusPlaying(dsPhong[room].indexOfClient[j], room);
                    }

                    client[dsPhong[room].indexOfClient[j]].Rank = 0;
                }
                //foreach (ClsHandCards item in .client)
                //{
                //    item.Rank = 0;
                //}
            }
            return 0;
        }
        public static void ListenRequest(object obj)//lắng nghe yêu cầu chia bài
        {
            int pos = (Int32)obj;

            while (true)
            {
                try
                {
                    byte[] buff = new byte[1024];
                    int k = socket_request[pos].Receive(buff);
                    dem++;
                    if (dem == 4)
                    {
                        if (dsPhong[client[pos].room].indexOfClient.Count == 4)
                        {
                            int temp_anTrang = StartNewGame(client[pos].room);
                            while (temp_anTrang == 1)
                            {
                                temp_anTrang = StartNewGame(client[pos].room);
                            }
                        }
                    }
                }
                catch
                {
                    socket_request[pos].Close();
                    socket_request.RemoveAt(pos);
                    break;
                }
            }


        }
        public static void BroadcastResult(List<ClsCard> c, int room)//gửi bài vừa đánh đến người chơi trong phòng
        {
            foreach (int item in dsPhong[room].indexOfClient)
            {
                socket_broadcastCard[item].Send(Serialize(c));
            }

        }
        public static void BroadcastStatusPlaying(int index, int room)//gửi trạng thái ai đang đánh
        {
            foreach (int item in dsPhong[room].indexOfClient)
            {
                socket_broadcastStatusPlaying[item].Send(Encoding.Unicode.GetBytes((index + 1).ToString()));
            }

        }
        public static void ListenActivating(object obj)// lắng nghe tín hiệu bỏ lượt
        {
            int pos = (Int32)obj;
            while (true)
            {
                try
                {
                    byte[] buff = new byte[1024];
                    int rcv = socket_activatingPlayer[pos].Receive(buff);//nhận tín hiệu bỏ lượt
                    if (rcv != 0)
                    {
                        client[pos].Status = false;
                        int index_next = dsPhong[client[pos].room].managerOfTheGame.NextPlayer() % 4;
                        FindNextPlayer(index_next, pos);

                    }
                }
                catch
                {
                    if (socket_activatingPlayer.Count != 0)
                    {
                        socket_activatingPlayer[pos].Close();
                        socket_activatingPlayer.RemoveAt(pos);
                    }
                    break;
                }

            }


        }
        #endregion
        static void Main(string[] args)
        {
            thislock = new object();

            StartServer();
        }
    }
}

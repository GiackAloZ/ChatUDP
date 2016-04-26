using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

//Librerie per UDP e SOCKET
using System.Net;
using System.Net.Sockets;
using System.Windows.Threading;

namespace ChatUDP_Aloisi
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Campo di classe che identifica il socket
        private Socket _socket = null;

        //Il dispatcherTimer controllo lo scorrere del tempo
        private DispatcherTimer _dTimer = null;

        //Costante intera che identfica il numero della porta che vogliamo utilizzare per il nostro socket
        private const int LOCAL_PORT_NUMBER = 11000;

        //Costante intera che identfica il numero della porta che vogliamo utilizzare per il socket remoto
        private const int REMOTE_PORT_NUMBER = 11000;

        //Costante double che identifica il periodo del tick del timer (millisecondi)
        private const double PERIOD_TIMER = 250;

        public MainWindow()
        {
            InitializeComponent();

            //Inizializziamo il socket
            //AddressFamily.InterNetwork = specifichiamo di usare IPv4
            //SocketType.Dgram = usiamo un buffer di tipo datagram
            //ProtocolType.Udp = usiamo il protocollo UDP
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            //Creiamo una variabile con il nostro ip
            IPAddress localAddress = IPAddress.Any; //Si riferisce a TUTTE le schede di rete del nostro pc

            //Creiamo un IPEndPoint che è in pratica un socket
            IPEndPoint localEndPoint = new IPEndPoint(localAddress, LOCAL_PORT_NUMBER);

            //Associamo il socket all'endpoint
            _socket.Bind(localEndPoint);

            //Creiamo il dispatcherTmer
            _dTimer = new DispatcherTimer();

            //Definiamo l'intervallo di tempo che fungerà da timer
            _dTimer.Interval = TimeSpan.FromMilliseconds(PERIOD_TIMER);
            //Definiamo l'evento che dovrà essere triggerato
            _dTimer.Tick += new EventHandler(update_dTimer);
            //Facciamo partire il timer
            _dTimer.Start();
        }

        private void update_dTimer(object sender, EventArgs e)
        {
            //Variabile per il conteggio dei byte in arrivo inizializzata a i byte che arrivano per il socket
            int nBytes = _socket.Available;

            //Verifichiamo se è arrivato qualcosa
            if(nBytes > 0)
            {
                //Creiamo un buffer di byte della grandezza letta
                byte[] buffer = new byte[nBytes];

                //Inizializziamo l'endpoint remoto a null
                EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

                //Riceviamo dal buffer e ci salviamo il remoteEndPoint
                nBytes = _socket.ReceiveFrom(buffer, ref remoteEndPoint);

                //Leggiamo il mittente
                string from = ((IPEndPoint)remoteEndPoint).Address.ToString();
                //Leggiamo il messaggio
                string messagge = Encoding.UTF8.GetString(buffer, 0, nBytes);

                //Aggiungiamo il messaggio con il mittente
                txtChat.Text += from + " : " + messagge + "\n";
                txtChat.ScrollToEnd();
            }
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            //Leggiamo dalla textbox l'indirizzo IP a cui inviare il messaggio e creiamo un IPAddress
            IPAddress remoteAddress = IPAddress.Parse(txtTo.Text);

            //Creiamo l'IPEndPoint remoto
            IPEndPoint remoteEndPoint = new IPEndPoint(remoteAddress, REMOTE_PORT_NUMBER);

            //Creiamo il buffer da inviare utilizzando il contenuto della textbox contenente il messaggio da inviare
            byte[] message = Encoding.UTF8.GetBytes(txtMex.Text);

            //Inviamo il messaggio
            _socket.SendTo(message, remoteEndPoint);
        }
    }
}

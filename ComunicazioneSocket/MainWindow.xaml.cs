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
//aggiunta
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ComunicazioneSocket
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// <summary>
    /// Dennis Forlesi 
    /// 4^L
    /// 17-05-2021
    /// Progetto socket
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            IPEndPoint localendpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 56000);//il Localendpoint inizializza a 127.0.0.1 l'indirizzo IP con porta di tipologia Dynamic or private port 56000

            Thread t1 = new Thread(new ParameterizedThreadStart(SocketReceive));//thread che dovra ricevere i dati usando il metodo  

            t1.Start(localendpoint);//avvia thread


        }
        /// <summary>Metodo per ricevere i dati del socket</summary>
        /// questo metodo utilizzera il protocollo UDP
        /// <param name="sourceEndPoint"></param>
        public async/*Utilizzo async per non far partire tutto insieme */ void SocketReceive(object sourceEndPoint)
        {
            //inserice come parametro il localendpoint che era stato impostato prima 
            IPEndPoint sourceEP = (IPEndPoint)sourceEndPoint;
            //usa il protocollo UDP
            Socket t = new Socket(sourceEP.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            //
            t.Bind(sourceEP);
            //Creazione di byte 
            Byte[] byteRicevuti = new byte[256];
            string message = "";
            //inizializzo i byte a 0
            int bytes = 0;
            //facio iniziare la task
            await Task.Run(() =>
            {
                while (true)
                {
                    if (t.Available > 0)//in caso il thread  available abbia un valore maggiore di 0 viene eseguitoil contenuto del if
                    {

                        message = "";
                        //avro dati in ricezione
                        bytes = t.Receive(byteRicevuti, byteRicevuti.Length, 0);
                        //eseguo la decodifica del messagio
                        message = message + Encoding.ASCII.GetString(byteRicevuti, 0, bytes);
                        //mostro in messagio nella lable
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            lblRicezione.Content = message;//
                        }));
                    }


                }

            });
        }
        /// <summary>bottone invia </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            //ricevo ed imposto lindirizo ip del destinatario e mi preparo per linvio della risposta
            IPAddress ipDest = IPAddress.Parse(txtIpAdd.Text);
            int portDest = int.Parse(txtDestPort.Text);
            //imposto un ip remoto
            IPEndPoint remoteEndPoint = new IPEndPoint(ipDest, portDest);
            //utilizzo il protocollo udp per inviare i dati
            Socket s = new Socket(ipDest.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            //il messagio da inviare deve essere codificato per poter essere inviato
            Byte[] byteInviati = Encoding.ASCII.GetBytes(txtMsg.Text);
            //invio risposta
            s.SendTo(byteInviati, remoteEndPoint);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WebSocket4Net;

namespace websocket_example_window
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        //[要変更]ここにWebsocket エコーサーバのURLをセットします。
        private string serverURL = "";

        private WebSocket websocket;
        private bool is_connected = false;
        public MainWindow()
        {
            InitializeComponent();
            if (serverURL == "")
            {
                textBox1.Text = "[エラー]";
                textBox2.Text = "[エラー]\n";
                textBox2.Text += "　サーバのURLを講義担当者から聞いてserverURL変数にセットしてください\n";
            }
            else
            {
                websocket = new WebSocket(serverURL);
                websocket.Closed += new EventHandler(websocket_Closed);
                websocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(websocket_Error);
                websocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(websocket_MessageReceived);
                websocket.Opened += new EventHandler(websocket_Opened);
                textBox2.Text += "[使い方]\n";
                textBox2.Text += "　上のテキストボックスに入力しenterか送信ボタンでサーバへ送信されます。\n";
                textBox2.Text += "　サーバは得たデータの先頭に'Server said'と付けて、そのまま返します。\n";
                textBox2.Text += "　サーバから帰って来た文字列がこのテキストボックスに表示されます。\n";
                textBox2.Text += "　発言の先頭に「all:」を付けて送信すると、サーバに接続している全員に送られます。\n";
                textBox2.Text += "[init]\n";
                textBox1.Focus();
                websocket.Open();
            }
        }
        private void websocket_Opened(object sender, EventArgs e)
        {
            is_connected = true;
            //以下のブロックはスレッドセーフにGUIを扱うおまじない
            this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                //ここにGUI関連の処理を書く。
                button1.IsEnabled = true;
                textBox2.Text += "[open]\n";
            }));

        }

        private void websocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            //以下のブロックはスレッドセーフにGUIを扱うおまじない
            this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                //ここにGUI関連の処理を書く。
                textBox2.Text += "[error] " + e.Exception.Message + "\n";
                button1.IsEnabled = false;
            }));
        }

        private void websocket_Closed(object sender, EventArgs e)
        {
            //以下のブロックはスレッドセーフにGUIを扱うおまじない
            this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                //ここにGUI関連の処理を書く。
                textBox2.Text += "[closed]\n";
                button1.IsEnabled = false;
            }));
        }

        private void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            //以下のブロックはスレッドセーフにGUIを扱うおまじない
            this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                //ここにGUI関連の処理を書く。
                textBox2.Text += e.Message + "\n";
                textBox2.ScrollToEnd();
            }));
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            sendMessage();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                sendMessage();
            }
        }
        private void sendMessage()
        {
            if (is_connected)
            {
                //以下のブロックはスレッドセーフにGUIを扱うおまじない
                this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                {
                    //ここにGUI関連の処理を書く。
                    websocket.Send(textBox1.Text);
                    textBox2.Text += "I said \"" + textBox1.Text + "\"\n";
                    textBox1.Text = "";
                }));
            }
        }
    }
}

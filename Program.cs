using System.Text;

namespace GsmGadgetry
{
    using System.IO.Ports;
    using Microsoft.SPOT;
    using Micro.Hardware;
    using Micro.Hardware.Gsm;

    public class Program
    {
        public static void Main()
        {
            Debug.EnableGCMessages(false);

            using (var client = new Sim900Client(Serial.COM1))
            {
                client.MessageDataReceived += MessageReceived;
                client.PowerOnReceived += PowerOnReceived;
                client.PhoneFunctionalityReceived += PhoneFunctionalityReceived;
                client.EnterPinReceived += EnterPinReceived;
                client.CallReadyReceived += CallReadyReceived;
                client.GprsStateReceived += GprsStateReceived;
                client.DataHeaderStateReceived += DataHeaderStateReceived;
                client.TextEncodingCharacterSetReceived += TextEncodingCharacterSetReceived;
                client.SmsMessageFormatReceived += SmsMessageFormatReceived;
                client.PowerOffReceived += ClientPowerOffReceived;

                // Send SMS using Text Mode
                //client.SendSmsMessage("Hello world.", "27xxxxxxxxx"/*MSISDN*/);

                // Send SMS using PDU Mode
                //client.SendSmsMessage("Hello world.", "27xxxxxxxxx"/*MSISDN*/, false, false);

                // Send Data using GPRS
                client.StartDataConnection("TCP", "www.google.co.za", 80);
                client.SendData(Encoding.UTF8.GetBytes("GET / HTTP/1.1\nHost: www.google.co.za\n\n"));
                //do something smarter here, but in the meantime stare at the debug output window to find inspiration.
                client.SuspendMessageProcessing();
                client.DataReceived += DataReceived;

                while (true) { }
            }
        }

        static void DataReceived(object sender, DataReceivedEventArgs e)
        {
            Debug.Print(e.Data);
        }

        static void MessageReceived(object sender, DataReceivedEventArgs e)
        {
            Debug.Print("<Message>" + e.Data + "</Message>");
        }

        static void PowerOnReceived(object sender, EventArgs e)
        {
            Debug.Print("Power On");
        }

        static void PhoneFunctionalityReceived(object sender, DataReceivedEventArgs e)
        {
            Debug.Print("Phone Functionality: " + e.Data);
        }

        static void EnterPinReceived(object sender, DataReceivedEventArgs e)
        {
            Debug.Print("Enter Pin: " + e.Data);
        }

        static void CallReadyReceived(object sender, EventArgs e)
        {
            Debug.Print("Call Ready");
        }

        static void GprsStateReceived(object sender, DataReceivedEventArgs e)
        {
            Debug.Print("GPRS State: " + e.Data);
        }

        static void DataHeaderStateReceived(object sender, DataReceivedEventArgs e)
        {
            Debug.Print("Data Header State: " + e.Data);
        }

        static void TextEncodingCharacterSetReceived(object sender, DataReceivedEventArgs e)
        {
            Debug.Print("Text Encoding Character Set: " + e.Data);
        }

        static void SmsMessageFormatReceived(object sender, DataReceivedEventArgs e)
        {
            Debug.Print("SMS Message Format: " + e.Data);
        }

        static void ClientPowerOffReceived(object sender, EventArgs e)
        {
            Debug.Print("Power Off");
        }
    }
}
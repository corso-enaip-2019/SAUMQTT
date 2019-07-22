using System;
using System.Net;
using System.Text;
using System.Timers;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Mqtt
{
    class Program
    {
        static void Main(string[] args)
        {

            Timer publishTimer = new Timer();
            Timer unsubTimer = new Timer();
            MqttClient client;
            string clientId;

            client = new MqttClient("test.mosquitto.org");

            // register to message received
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

            //connecting to the broker
            clientId = Guid.NewGuid().ToString();
            client.Connect(clientId);

            Subscribe(client);

            publishTimer.Interval = 2000;
            publishTimer.Elapsed += new ElapsedEventHandler((sender, e) => Publish(sender, e, client));
            publishTimer.Start();

            unsubTimer.Interval = 10000;
            unsubTimer.Elapsed += new ElapsedEventHandler((sender, e) => Unsubscribe(sender, e, client));
            unsubTimer.Start();
            
        }

        static void Subscribe(MqttClient client)
        {
            // subscribe to the topic "/testing" with QoS 2
            client.Subscribe(new string[] { "/testing" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

        private static void Unsubscribe(object source, ElapsedEventArgs e, MqttClient client)
        {
            client.Unsubscribe(new string[] { "/testing" });
        }

        static void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var message = Encoding.Default.GetString(e.Message);
            Console.WriteLine("Message received: " + message);
        }

        private static void Publish(object source, ElapsedEventArgs e, MqttClient client)
        {
            // publish a message on "/testing" topic with QoS 2
            client.Publish("/testing", Encoding.UTF8.GetBytes("test"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        }
    }
}

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
        const string MQTT_BROKER = "test.mosquitto.org";
        const string TOPIC_1 = "/testing";
        const string TOPIC_2 = "/test2";

        static void Main(string[] args)
        {

            Timer publishTimer = new Timer();
            Timer unsubTimer = new Timer();


            MqttClient publishingClient = new MqttClient(MQTT_BROKER);
            MqttClient subscribingClient = new MqttClient(MQTT_BROKER);

            string pubClientId;
            string subClientId;


            // register to message received
            subscribingClient.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

            //connecting to the broker for the subscriber
            subClientId = Guid.NewGuid().ToString();
            subscribingClient.Connect(subClientId);

            //connecting to the broker for the publisher
            pubClientId = Guid.NewGuid().ToString();
            publishingClient.Connect(pubClientId);

            Subscribe(subscribingClient);

            publishTimer.Interval = 2000;
            publishTimer.Elapsed += new ElapsedEventHandler((sender, e) => Publish(sender, e, publishingClient));
            publishTimer.Start();

            unsubTimer.Interval = 10000;
            unsubTimer.Elapsed += new ElapsedEventHandler((sender, e) => Unsubscribe(sender, e, subscribingClient));
            unsubTimer.Start();
            
        }

        static void Subscribe(MqttClient client)
        {
            // subscribe to the topic "/testing" with QoS 2
            client.Subscribe(new string[] { TOPIC_1 }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

        private static void Unsubscribe(object source, ElapsedEventArgs e, MqttClient client)
        {
            client.Unsubscribe(new string[] { TOPIC_1 });
        }

        static void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var message = Encoding.Default.GetString(e.Message);
            Console.WriteLine("Message received: " + message);
        }

        private static void Publish(object source, ElapsedEventArgs e, MqttClient client)
        {
            // publish a message on "/testing" topic with QoS 2

            client.Publish(TOPIC_1, Encoding.UTF8.GetBytes("test on topic 1"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            client.Publish(TOPIC_2, Encoding.UTF8.GetBytes("test on topic 2"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            client.Publish(TOPIC_1, Encoding.UTF8.GetBytes("Hello World"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        }
    }
}

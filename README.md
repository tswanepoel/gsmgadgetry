# GSM Gadgetry
The GSM Gadgetry project aims to demonstrate simple integration with the SIM900 GSM/GPRS modem using the .NET Gadgeteer platform and the .NET Micro Framework.

The example program shows using Sim900Client to send a text message via SMS, which is based on SIM900 AT Command Manual v1.03. At the time, the program was run on a FEZ Panda II.

**SMS (PDU Mode)**
```C#
using (var client = new Sim900Client(Serial.COM1))
{
    client.SendSmsMessage("Hello, world!", "27xxxxxxxxx"/*MSISDN*/, false, false);
}
```

> Only a handful of commands are actually available in this initial commit. Although, you may also expect to find some attempt at using GPRS commands for TCP/IP data, which would naturally be the next step to hopefully make this project a little more exciting...

**TCP/IP Data**
```C#
using (var client = new Sim900Client(Serial.COM1))
{
    client.StartDataConnection("TCP", "www.google.co.za", 80);
    client.SendData(Encoding.UTF8.GetBytes("GET / HTTP/1.1\nHost: www.google.co.za\n\n"));
    ...
}
```

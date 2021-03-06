﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapProcessor;

namespace PcapProcessorTest
{
    [TestClass]
    public class PcapProcessorTest
    {
        public string TcpFivePacketsFilePath { get; set; }
        public string HttpSmallFilePath { get; set; }


        public PcapProcessorTest()
        {
            this.TcpFivePacketsFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"Test Files\Tcp - 5 Packets.pcap");
            this.HttpSmallFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"Test Files\HTTP - Small File.pcap");
        }

        [TestMethod]
        public void PcapProcessor_ReadTcpPackets_ReadSuccess()
        {
            // Arrange.
            var recievedPackets = new List<PcapProcessor.TcpPacket>();
            var processor = new PcapProcessor.Processor();

            processor.TcpPacketArived +=
                (object sender, TcpPacketArivedEventArgs e) => recievedPackets.Add(e.Packet);

            // Act.
            processor.ProcessPcap(this.TcpFivePacketsFilePath);

            // Assert.
            Assert.AreEqual(5, recievedPackets.Count);
        }

        [TestMethod]
        public void PcapProcessor_BuildTcpSession_BuildSuccess()
        {
            // Arrange.
            var recievedSessions = new List<TcpSession>();
            var processor = new Processor();
            processor.BuildTcpSessions = true;
            processor.TcpSessionArived +=
                (object sender, TcpSessionArivedEventArgs e) => recievedSessions.Add(e.TcpSession);

            // Act.
            processor.ProcessPcap(this.HttpSmallFilePath);
            string firstSessionText = Encoding.UTF8.GetString(recievedSessions[0].Data);

            // Assert.
            Assert.AreEqual(18843, recievedSessions[0].Data.Length);
            StringAssert.StartsWith(firstSessionText, @"GET /download.html HTTP/1.1");
        }

        [TestMethod]
        public void PcapProcessor_ReadTcpPacketsMultipleFiles_ReadSuccess()
        {
            // Arrange.
            var recievedPackets = new List<PcapProcessor.TcpPacket>();
            var processor = new PcapProcessor.Processor();

            processor.TcpPacketArived +=
                (object sender, TcpPacketArivedEventArgs e) => recievedPackets.Add(e.Packet);

            // Act.
            processor.ProcessPcaps( new List<string>() {
                this.HttpSmallFilePath,
                this.TcpFivePacketsFilePath });

            // Assert.
            Assert.AreEqual(46, recievedPackets.Count);
        }
    }
}

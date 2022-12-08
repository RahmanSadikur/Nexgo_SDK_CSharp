﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nexgo.Com.APIx4._5;

namespace Nexgo.client
{
    public partial class Form1 : Form
    {
        private  Config config;
        SerialPort _serialPort;
        public string receievedData = "";

        // delegate is used to write to a UI control from a non-UI thread
        private delegate void SetTextDeleg(string text);
        public Form1()
        {
            InitializeComponent();
            _serialPort = new SerialPort();
            string portName = "COM6";
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
            string[] comPorts = SerialPort.GetPortNames();
            if (comPorts.Any())
            {
                var pname = comPorts.FirstOrDefault(a => a.Contains("COM6"));
                if (!string.IsNullOrEmpty(pname))
                {
                    _serialPort.PortName = pname;
                }
            }
            _serialPort.PortName = portName; //Com Port Name                
            _serialPort.BaudRate = 115200; //COM Port 
            _serialPort.Handshake = System.IO.Ports.Handshake.None;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;
            
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
            try
            {
                _serialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            this.config = new Config(_serialPort);

        }

        private void ConvertBtn_Click(object sender, EventArgs e)
        {
            
           
            this.config.ConvertStringIntoMesg(dataFieldTextBox.Text.ToString());
            if (config.isError)
            {
                MessageBox.Show(config.errorMessage);
                return;
            }
            this.stxlbl.Text = config.stx;
            this.lenLbl.Text = config.len;
            this.typeLbl.Text = config.type;
            this.dataLbl.Text = config.DataHexFormat;
            this.etxLbl.Text = config.etx;
            this.lrcLbl.Text = config.lrc;
            outputTextBox.Text = config.FinalhexString;
        }



        private void ProcessBtn_Click(object sender, EventArgs e)
        {

            this.config.SendMessageInit(amountTextBox.Text.ToString(), invoiceTextBox.Text.ToString());
            if (config.isError)
            {
                MessageBox.Show(config.errorMessage);
                return;
            }
            this.stxlbl.Text = config.stx;
            this.lenLbl.Text = config.len;
            this.typeLbl.Text = config.type;
            this.dataLbl.Text = config.DataHexFormat;
            this.etxLbl.Text = config.etx;
            this.lrcLbl.Text = config.lrc;
            outputTextBox.Text = config.FinalhexString;
            dataStringrtxb.Text = config.dataString;
        }

       
        void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(500);
           
            try
            {
                receievedData = _serialPort.ReadExisting();
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
            
            this.BeginInvoke(new SetTextDeleg(si_DataReceived), new object[] { receievedData });
            
        }

        private void si_DataReceived(string data)
        {
            recievedoutputrtxb.Text = receievedData.Trim();
            Console.WriteLine(receievedData);
        }

        private void confirmbtn_Click(object sender, EventArgs e)
        {
           
            config.SendingDataToPos();
            
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OopFactory.X12.Parsing.Model;
using OopFactory.X12.Hipaa.Claims;
using OopFactory.X12.Parsing;
using System.IO;

namespace EDIApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public void EdiMessage()
        {
            Interchange message= new Interchange(DateTime.Now, 1, true);
            message.InterchangeSenderIdQualifier = "ZZ";
            message.InterchangeSenderId = "9012345720000";
            message.InterchangeReceiverIdQualifier = "ZZ";
            message.InterchangeReceiverId = "9088877320000";
            message.SetElement(12, "00501");

            var group = message.AddFunctionGroup("HC", DateTime.Now, 999999);
            group.ApplicationSendersCode = "901234572000";
            group.ApplicationReceiversCode = "908887732000";
            group.VersionIdentifierCode = "005010X222";

            var transaction = group.AddTransaction("837", "0034");
            var bhtSegment = transaction.AddSegment("BHT");

            var submitterLoop = transaction.AddLoop("NM1*41"); // 'Submitter Identifer Code
            submitterLoop.SetElement(2, "2");// 'Non-Person Entity
            submitterLoop.SetElement(3, "My Submitter");// 'Organization Name

            var perSegment = submitterLoop.AddSegment("PER");
            perSegment.SetElement(1, "IC");// 'Information Contact Function Code/
            perSegment.SetElement(2, "My Contact");// 'Name
            perSegment.SetElement(3, "TE");// 'Telephone Qualifier
            perSegment.SetElement(4, "18005555555");// 'Communication Number

            var providerHLoop = transaction.AddHLoop("1", "20", true);// 'Information Source
            providerHLoop.AddSegment("PRV");// 'Speciality Segment
            var providerNameLoop = providerHLoop.AddLoop("NM1*85");
            providerNameLoop.SetElement(2, "1");// 'Person Entity
            providerNameLoop.SetElement(3, "Doe");// 'Last Name
            providerNameLoop.SetElement(4, "John");// 'First Name


            var x12 = message.SerializeToX12(true);

            System.Diagnostics.Trace.Write(x12);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EdiMessage();
            Claims();
        }

        private void Claims()
        {
            var claim = new Claim();
            var claimDocument = new ClaimDocument();
            claimDocument.Claims = new List<Claim>() { claim };

            claim.Subscriber = new ClaimMember() { Name = new OopFactory.X12.Hipaa.Common.EntityName() { FirstName = "RO" } };
            var xml = claim.Serialize();

            var dir = Directory.GetCurrentDirectory();
            var stream = new StreamReader(@"..\..\..\..\TestData\sampleClaim.xml");
            var xmlText = stream.ReadToEnd();

            X12Parser parser = new X12Parser();
            var x12 = parser.TransformToX12(xmlText);

        /*   Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("OopFactory.X12.Hipaa.Tests.Unit.Claims.TestData.SKInstitutionalClaim5010.xml");

            TextReader tr = new StreamReader(stream);
            string xml2 = tr.ReadToEnd();

            //Now convert the XML to X12
            X12Parser parser = new X12Parser();
            string myX12 = parser.TransformToX12(xml2);
            */
        }
    }
}


using DropDownListApplication.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using ClosedXML.Excel;
using System.Data;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Graph;
using Microsoft.Office.Interop.Excel;
using Worksheet = Microsoft.Office.Interop.Excel.Worksheet;
using Workbook = DocumentFormat.OpenXml.Spreadsheet.Workbook;
using System.Configuration;
using System.Linq;

namespace DropDownListApplication.Controllers
{
    public class GridListController : Controller
    {






        [HttpGet]
        public ActionResult GridView()
        {

            return View();
        }

        public ActionResult Export(string value, string key)
        {
            /*i = 0;
            var walue[i] = value;
            Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();

            Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);

            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

            app.Visible = true;


            Worksheet = Workbook.Sheets["Sheet1"];
            worksheet = Workbook.ActiveSheet;

            worksheet.Name = "Exported from gridview";

            for (int i = 1; i < value[].Count + 1; i++)
            {
                worksheet.Cells[1, i] = dataGridView1.Columns[i - 1].HeaderText;
            }

            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    worksheet.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                }
            }

            workbook.SaveAs("c:\\output.xls", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            app.Quit();*/
            /*DataTable products = new DataTable("Grid");
            products.Columns.Add("Key", typeof(string));
            products.Columns.Add("value", typeof(string));

            products.Rows.Add(key, value);
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(products);
                using (MemoryStream stream = new MemoryStream())  
                {
                    *//*wb.SaveAs(stream);*//*
                    stream.Seek(0, SeekOrigin.Begin);

                    return File(stream.ToString(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Report.xlsx");
                }

            }
            return RedirectToAction("GridView");*/
            return View();
        }

        //send the dropdownlists values 
        string keyvalue = System.Configuration.ConfigurationManager.AppSettings["XmlReaderNode"];

        public ActionResult NodeObject()
        {

            if (Request.Files.Count > 0)
            {
                HttpFileCollectionBase files = Request.Files;
                HttpPostedFileBase file = files[0];
                try
                {
                    string fname;
                    fname = file.FileName;
                    string extension = Path.GetExtension(file.FileName);

                    string[] allowExtention = { ".xml" };

                    if (extension == allowExtention[0])
                    {
                        fname = Path.Combine(Server.MapPath("~/Uploads/"), fname);
                        file.SaveAs(fname);
                        XmlDataDocument xmldoc = new XmlDataDocument();
                        List<NodeObject> Nodelist = new List<NodeObject>();
                        
                        using (FileStream fs = new FileStream(fname, FileMode.Open, FileAccess.Read))
                        {
                            xmldoc.Load(fs);
                            bool haschildnode = false;
                            string str = null;
                            haschildnode = xmldoc.HasChildNodes;
                            if (haschildnode)
                            {
                                XmlElement elm = xmldoc.DocumentElement;
                                XmlNodeList nodelist = elm.ChildNodes;
                                for (int j = 0; j <= nodelist.Count - 1; j++)
                                {

                                    NodeObject oNodelist = new NodeObject();
                                    str = nodelist[j].Name;
                                    oNodelist.code = str;
                                    string content = nodelist[j].FirstChild == null ? null : nodelist[j].FirstChild.ToString();
                                    if (content != "System.Xml.XmlText" || content == null)
                                    {
                                        oNodelist.value = null;
                                    }
                                    else
                                    {
                                        oNodelist.value = nodelist[j].InnerText;
                                    }
                                    Nodelist.Add(oNodelist);
                                }
                                List<NodeObject> nodeListNew= new List<NodeObject>();
                                List<string> keylist = new List<string>();
                                if (!string.IsNullOrEmpty(keyvalue))
                                {

                                    string[] keyvaluelist = keyvalue.Split(new char[] {','});
                                    if (keyvaluelist.Length > 0)
                                    {
                                        
                                        foreach(var key in keyvaluelist)
                                        {
                                            keylist.Add(key.Trim());
                                        }
                                    }
                                }
                                if (nodelist != null&& keylist != null)
                                {
                                    foreach(var key in Nodelist)
                                    {
                                        if (keylist.Contains(key.code))
                                        {
                                            nodeListNew.Add(key);
                                        }
                                    }
                                    Nodelist = nodeListNew;
                                }


                            }
                           
                            return Json( Nodelist, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {

                    }
                }
                catch (Exception)
                {

                }
                finally
                {

                }
            }
            return null;
        }


        //Read the xml value and send first dropdownlist 
        public ActionResult GetFirstChileNodeByName(string tagName, string uploadFileName)
        {
            string firstNode = tagName;

            List<NodeObject> cboSecondNode = new List<NodeObject>();


            XmlNodeList store = GetXmlNodeList(firstNode, uploadFileName);
            for (int i = 0; i <= store.Count - 1; i++)
            {
                NodeObject oNodelist = new NodeObject();
                string str = store[i].Name;
                oNodelist.code = str;
                string content = store[i].FirstChild == null ? null : store[i].FirstChild.ToString();
                if (content != "System.Xml.XmlText" || content == null)
                {
                    oNodelist.value = null;
                }
                else
                {
                    oNodelist.value = store[i].InnerText;
                }
                cboSecondNode.Add(oNodelist);
            }
            return Json(cboSecondNode, JsonRequestBehavior.AllowGet);
        }

        //Read NodeList to send the dropdownlist
        public XmlNodeList GetXmlNodeList(string getNode, string UploadedFileName)
        {
            XmlDataDocument xmldoc = new XmlDataDocument();
            XmlNodeList nodelist = null;            string filename = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/" + UploadedFileName);
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                xmldoc.Load(fs);
                bool haschildnode = false;
                haschildnode = xmldoc.HasChildNodes;
                if (haschildnode)
                {
                    string getValue = xmldoc.ChildNodes[1].Name;
                    var firstRoot = "//" + getValue + getNode;
                    XmlNodeList xmlNodebyname = xmldoc.SelectNodes(firstRoot);

                    for (int i = 0; i < xmlNodebyname.Count; i++)
                    {
                        nodelist = xmlNodebyname[i].ChildNodes;
                    }
                    return nodelist;

                }
            }

            return null;
        }
    }
}



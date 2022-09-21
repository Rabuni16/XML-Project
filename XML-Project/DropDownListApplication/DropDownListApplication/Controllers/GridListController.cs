
using DropDownListApplication.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Text;
using Microsoft.VisualStudio.Services.Client.AccountManagement.Logging;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using NLog;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using System.Linq;

namespace DropDownListApplication.Controllers
{
    public class GridListController : Controller
    {

        private static Logger logger = LogManager.GetLogger("myAppLoggerRules");


        //main view page actionresult
        [HttpGet]
        public ActionResult GridView()
        {
            return View();
        }
        //get key and value class
        public class Thing
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        [HttpPost]
        public FileResult Export(List<Thing> exportExcel, string fileName)
        {
            try
            {
                string folder = Server.MapPath(string.Format("~/Files/"));
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                var sb = new StringBuilder();
                string csvPath = Server.MapPath("~/Files/");
                /*var basePath = AppDomain.CurrentDomain.BaseDirectory;*/
                var header = "";
                var info = typeof(Thing).GetProperties();
                var finalPath = Path.Combine(csvPath, fileName + ".csv");
                var line = "";
                foreach (var obj in exportExcel)
                {
                    sb = new StringBuilder();
                    TextWriter sw = new StreamWriter(finalPath, true);
                    foreach (var prop in info)
                    {
                        line += prop.GetValue(obj, null) + "; ";
                    }
                    line = line.Substring(0, line.Length - 2);
                    sb.AppendLine(line);
                    sw.Write(sb.ToString());
                    sw.Close();
                }
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=Export.csv");
                Response.Charset = "";
                Response.ContentType = "application/octet-stream";
                Response.Output.Write(sb);
                Response.Flush();
                Response.End();
                return File(new UTF8Encoding().GetBytes(sb.ToString()), "text/csv", "export.csv");

            }
            catch (Exception)
            {

            }
            return null;
        }


        //send the dropdownlists values 
        string keyvalue = System.Configuration.ConfigurationManager.AppSettings["XmlReaderNode"];


        public ActionResult NodeObjects()
        {

            try
            {

                if (Request.Files.Count > 0)
                {
                    HttpFileCollectionBase files = Request.Files;
                    HttpPostedFileBase file = files[0];

                    string fname;
                    fname = file.FileName;
                    string extension = Path.GetExtension(file.FileName);

                    string[] allowExtention = { ".xml" };

                    if (extension == allowExtention[0])
                    {
                        string folder = Server.MapPath(string.Format("~/Uploads/"));
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }

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
                                List<NodeObject> nodeListNew = new List<NodeObject>();
                                List<string> keylist = new List<string>();
                                if (!string.IsNullOrEmpty(keyvalue))
                                {

                                    string[] keyvaluelist = keyvalue.Split(new char[] { ',' });
                                    if (keyvaluelist.Length > 0)
                                    {

                                        foreach (var key in keyvaluelist)
                                        {

                                            keylist.Add(key.Trim());
                                        }
                                    }
                                }

                                if (nodelist != null && keylist != null)
                                {
                                    foreach (var key in Nodelist)
                                    {
                                        if (keylist.Contains(key.code))
                                        {
                                            nodeListNew.Add(key);
                                        }
                                    }

                                    Nodelist = nodeListNew;
                                }
                            }

                            return Json(Nodelist, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception)
            {

            }
            return null;
        }


        //Read the xml value and send first dropdownlist 
        public ActionResult GetFirstChileNodeByName(string tagName, string uploadFileName,int nodeindex)
        {
            try
            {
                string firstNode = tagName;

                List<NodeObject> cboSecondNode = new List<NodeObject>();

                XmlNodeList store = GetXmlNodeList(firstNode, uploadFileName,  nodeindex);

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
            catch (Exception)
            {

            }
            return null;
        }

        //Read NodeList to send the dropdownlist
        public XmlNodeList GetXmlNodeList(string getNode, string UploadedFileName , int nodeindex)
        {

            try
            {
                XmlDataDocument xmldoc = new XmlDataDocument();
                XmlNodeList nodelist = null;
                string filename = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/" + UploadedFileName);
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

                        if (xmlNodebyname.Count == 1)
                        {
                            for (int i = 0; i < xmlNodebyname.Count; i++)
                            {
                                nodelist = xmlNodebyname[i].ChildNodes;
                            }
                            
                        }
                        /*  else
                          {
                              for (int i = 0; i < nodeindex; i++)
                              {
                                  int j = nodeindex;*/
                        else
                        {
                            nodelist = xmlNodebyname[nodeindex - 1].ChildNodes;
                            /*          }*/
                            
                        }
                        return nodelist;
                        /*}*/
                    }
                }
            }
            catch (Exception)
            {

            }
            return null;               
        }
    }
}




using DropDownListApplication.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;



namespace DropDownListApplication.Controllers
{
    public class GridListController : Controller
    {
       /* private readonly IConfiguration _configuration;

        public GridListController(IConfiguration configuration)
        {
            _configuration = configuration;
        }*/


        [HttpGet]
        public ActionResult GridView()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GridView(HttpPostedFileBase ImageFile)
        {
            
            return View();
        }

        //Get file into select the path

        [HttpPost]
        public ActionResult GetFile()
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                   
                    HttpFileCollectionBase files = Request.Files;
                    XmlNodeList firstNodelist=null;
                    for (int i = 0; i < files.Count; i++)
                    {
                       

                        HttpPostedFileBase file = files[i];
                        string fname;

                        
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }
                        
                        
                        fname = Path.Combine(Server.MapPath("~/XmlFile/"), fname);
                        file.SaveAs(fname);
                        
                    }

                    
                    return Json(NodeObject(), JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        //send the dropdownlists values 
        public ActionResult NodeObject()
        {

            XmlDataDocument xmldoc = new XmlDataDocument();
            string filename = System.Web.Hosting.HostingEnvironment.MapPath("~/XmlFile/Dummy.Xml");
            List<NodeObject> Nodelist = new List<NodeObject>();
            try
            {
                bool haschildnode = false;
                string str = null;
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                xmldoc.Load(fs);
                haschildnode = xmldoc.HasChildNodes;
                if (haschildnode)
                {
                    XmlElement elm = xmldoc.DocumentElement;
                    XmlNodeList nodelist = elm.ChildNodes;


                    for (int i = 0; i <= nodelist.Count - 1; i++)
                    {
                        NodeObject oNodelist = new NodeObject();
                        str = nodelist[i].Name;
                        oNodelist.code = str;
                        string content = nodelist[i].FirstChild == null ? null : nodelist[i].FirstChild.ToString();
                        if (content != "System.Xml.XmlText" || content == null)
                        {
                            oNodelist.value = null;
                        }
                        else
                        {
                            oNodelist.value = nodelist[i].InnerText;
                        }
                        Nodelist.Add(oNodelist);
                    }
                    return Json(Nodelist, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {

            }
            return null;
        }

        
        

       //Read the xml value and send first dropdownlist 
        public ActionResult GetFirstChileNodeByName(string tagName)
        {
            string firstNode = tagName;
            XmlNodeList store =GetXmlNodeList(firstNode);
            List<NodeObject> cboSecondNode = new List<NodeObject>();
            for (int i = 0; i <= store.Count - 1; i++)
            {
                NodeObject oNodelist = new NodeObject();
                string str = store[i].Name;
                oNodelist.code = str;
                string content = store[i].FirstChild==null?null:store[i].FirstChild.ToString();
                if(content != "System.Xml.XmlText" || content==null)
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
        public XmlNodeList GetXmlNodeList(string getNode)
        {
            XmlDataDocument xmldoc = new XmlDataDocument();
            XmlNodeList nodelist = null;
            string filename = System.Web.Hosting.HostingEnvironment.MapPath("~/XmlFile/Dummy.Xml");
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            xmldoc.Load(fs);
            /*XmlNodeList xmlNodebyname = xmldoc.SelectNodes("//" + getNode);*/
            XmlNodeList xmlNodebyname = xmldoc.GetElementsByTagName(getNode);

            for (int i = 0; i < xmlNodebyname.Count; i++)
            {
                nodelist = xmlNodebyname[i].ChildNodes;
                
            }
            return nodelist;
        }

        
       
    }


}
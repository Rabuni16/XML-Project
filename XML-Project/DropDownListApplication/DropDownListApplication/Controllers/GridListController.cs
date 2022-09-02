
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Xml;



namespace DropDownListApplication.Controllers
{
    public class GridListController : Controller
    {
        [HttpGet]
        public ActionResult GridView()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GridView(HttpPostedFileBase ImageFile)
        {

            XmlDataDocument xmldoc = new XmlDataDocument();
            string filename=Server.MapPath("~/XmlFile/Dummy.xml");
            /* string filename = ImageFile.FileName;*/
            List<NodeObject> Nodelist = new List<NodeObject>();
            try
            {
                XmlNodeList xmlnode;
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
                        Nodelist.Add(oNodelist);

                    }
                }
            }
            catch (Exception )
            {

            } 

            return View(Nodelist);
        }
        
        
        
    }
}
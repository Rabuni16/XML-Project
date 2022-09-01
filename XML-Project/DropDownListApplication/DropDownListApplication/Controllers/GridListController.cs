
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;



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
        public ActionResult GridView(HttpPostedFileBase GetXmlFile, string store)
        {
            string path = Server.MapPath("~/XmlFile/");
            String[] allowExtention = { ".xml" };
            string extension = Path.GetExtension(GetXmlFile.FileName);
            if (extension == allowExtention[0])
            {
                try
                {
                    var xml = XDocument.Load(Server.MapPath("~/XmlFile/Dummy.xml"));
                    Dictionary<string, string> nodelist = new Dictionary<string, string>();
                    nodelist.Add("Form", "Form");
                    foreach (var element in xml.Root.Elements())
                    {
                        nodelist.Add(element.Name.ToString(), element.Name.ToString());

                    }
                    Dictionary<string, string> node2list = new Dictionary<string, string>();
                    foreach (var firstparent in nodelist)
                    {
                        foreach (var element in xml.Root.Descendants(firstparent.Key))
                        {
                            node2list.Add(firstparent.Key, element.Name.ToString());

                        }
                    }
                    Dictionary<string, string> node3list = new Dictionary<string, string>();
                    foreach (var firstparent in nodelist)
                    {
                        foreach (var secondparent in node2list)
                        {
                            foreach (var element in xml.Root.Elements(secondparent.Key))
                            {
                                node3list.Add(secondparent.Key, element.Name.ToString());
                            }
                        }
                    }

                    Dictionary<string, string> node4list = new Dictionary<string, string>();
                    foreach (var firstparent in nodelist)
                    {
                        foreach (var secondparent in node2list)
                        {
                            foreach (var thirdparent in node3list)
                            {
                                foreach (var element in xml.Root.Elements(thirdparent.Key))
                                {
                                    node4list.Add(thirdparent.Key, element.Name.ToString());
                                }
                            }
                        }
                    }


                    GetXmlFile.SaveAs(path + GetXmlFile.FileName);
                    ViewBag.ErrorMessage = "File  uploaded Successfully!!";
                    store = path + GetXmlFile.FileName;
                    GetXmlFile.SaveAs(path + GetXmlFile.FileName);
                }
                catch
                {
                    ViewBag.ErrorMessage = "File Cannot be uploaded";
                }
            }
            else
            {
                ViewBag.ErrorMessage = "File not allow to upload";
            }
            return View();



        }
    }
}